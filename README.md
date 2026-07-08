# Smart Document Analysis Tool — Copilot SDK

PDF analysis and technology classification powered by the GitHub Copilot SDK for .NET.

---

## Quick Start

**Prerequisites:** the .NET 10+ SDK and the GitHub Copilot CLI (authenticated, with an active Copilot subscription).

### 1. Install the .NET 10 SDK

```powershell
# Windows (winget)
winget install Microsoft.DotNet.SDK.10
```

```bash
# macOS (Homebrew)
brew install dotnet-sdk
# Linux: https://learn.microsoft.com/dotnet/core/install/linux
```

### 2. Install and authenticate the GitHub Copilot CLI

The CLI requires Node.js 22+ (`winget install OpenJS.NodeJS` on Windows, or https://nodejs.org).

```bash
npm install -g @github/copilot
copilot            # launch once, then run /login to sign in to GitHub
```

### 3. Restore dependencies and run

```bash
dotnet restore     # pulls GitHub.Copilot.SDK, itext7, and the Copilot CLI binary
dotnet run                     # console mode (GitHub Copilot)
dotnet run -- --web            # browser UI at http://localhost:5179
dotnet run -- --local          # local Ollama models instead of Copilot (see chat/README.md)
```

On launch, the tool connects to the chat backend, lists models (with reasoning support), and asks you to pick one.

---

## Commands

| Command | Description |
|---------|-------------|
| `/list` | List PDFs in `./01_input/11_pdf_to_analyze/` and select one |
| `/upload <path>` | Copy a PDF into `./01_input/11_pdf_to_analyze/` and load it |
| `/current` | Show the currently loaded PDF |
| `/condense` | Condense the PDF to its cached Markdown (first step of the pipeline) |
| `/summarize` | Extract technology summaries to a Markdown file |
| `/classify` | Convert the summary MD into a structured CSV |
| `/batch-analyze <q>` | Ask one question across all PDFs |
| `/benchmark` | Run all models on the same PDF and compare results |
| `/commands` / `/help` | Show all commands |
| `/exit` / `/quit` | Exit |

Commands also work without the leading `/`, but an unknown `/command` reports an error instead of being sent to the model. Any other input is sent to Copilot as a question. If a PDF is loaded, its (condensed) text is injected as context on the first question about it; follow-up questions reuse the session context instead of re-sending the document.

---

## Core Workflow: PDF → MD → CSV

```
1. /list or /upload     → select a PDF
2. /condense            → 01_input/12_condensed_md/document_condensed.md   (optional; runs automatically otherwise)
3. /summarize           → 02_output/21_tech_summary_md/document_summary.md   (review/edit freely)
4. /classify            → 02_output/22_tech_classification_csv/document_classification.csv
```

**condense** compresses the PDF once into a cached `.md` (see below). It runs automatically the first time any step needs the PDF, so this command is only needed to do it up front.

**summarize** runs in two stages:
1. Scans the PDF to find all unique technology names
2. Extracts detailed data per technology, organized by year


**classify** reads the summary MD (not the raw PDF) and converts each technology into one or more CSV rows — one row per year/time horizon. Legacy `.txt` summaries are still read as a fallback.

#### Token-saving condensation cache

The first time any operation needs a PDF, the tool condenses it once into a compact
`01_input/12_condensed_md/<name>_condensed.md` — preserving every number, unit, table, and technology name
but stripping prose. All later operations (summarize, batch-analyze, Q&A, benchmark)
read this cached `.md` instead of re-sending the full PDF, cutting token usage substantially.

The cache is reused automatically and regenerated only when the source PDF changes.
Because extraction is lossy compression, review the `.md` if a number looks off in the CSV.

---

## Benchmark

```
You: /benchmark
```

Runs every available Copilot model on the same standard PDF (`Allgoewer_2024.pdf`, must be in `./01_input/11_pdf_to_analyze/`), then auto-classifies each response. Saves to `./02_output/23_validation/benchmark/`:

- `<pdfname>_<provider>_benchmark_summary_<yyyy-MM-dd>.md` — each model's per-technology summary
- `<pdfname>_<provider>_benchmark_classification_<yyyy-MM-dd>.csv` — combined classified rows with a `Model` column
- `<pdfname>_<provider>_benchmark_overview_<yyyy-MM-dd>.csv` — latency, word count, classified rows per model

where `<provider>` is `copilot` or `ollama`, matching the backend the run used.

---

## File Structure

```
CopilotSDK_techClass/
├── Program.cs              entry point, model selection, session management
├── Workspace.cs            app-wide context (client, model, directory layout)
├── core/                   the main pipeline
│   ├── PdfExtractor.cs     text extraction and chunking
│   ├── PdfCondenser.cs     one-time PDF→condensed-MD caching for token savings
│   ├── TechSummarizer.cs   summarize logic
│   └── TechClassifier.cs   classify pipeline (batch → JSON → validate → merge)
├── format_output/          data model + its output formats
│   ├── TechnologyRecord.cs the record data to classify
│   ├── TechnologyCsv.cs    classification CSV read/write
│   └── TechnologyMd.cs     summary Markdown read/write
├── chat/                   provider-neutral LLM backends (see chat/README.md)
│   ├── ChatBackend.cs      IChatClient / IChatSession contracts
│   ├── CopilotChatBackend.cs  GitHub Copilot backend (default)
│   ├── OllamaChatBackend.cs   local Ollama backend (--local)
│   └── Sessions.cs         session factory
├── console/                console-mode UI
│   ├── CommandHandlers.cs  CLI command dispatch
│   └── ConsoleEx.cs        color-coded console output helpers
├── helpers/                utilities + supporting features
│   ├── Benchmark.cs        multi-model benchmark
│   ├── CondensedVerifier.cs   deterministic numeric verification check
│   ├── TechClassifierHelpers.cs  shared parsing/formatting utilities
│   ├── TechnologyValidator.cs    post-parse record validation
│   └── NumberNormalizer.cs       numeric normalization
├── web/                    web UI (WebServer.cs + wwwroot/)
├── prompt/                 prompt template markdown files
├── 01_input/
│   ├── 11_pdf_to_analyze/    input PDFs (auto-created)
│   ├── 12_condensed_md/      condensed .md cache (auto-created, regenerable)
│   └── 13_technology_list_md/ frozen per-PDF technology lists (editable)
└── 02_output/
    ├── 21_tech_summary_md/          summarize Markdown output
    ├── 22_tech_classification_csv/  classify CSV output
    └── 23_validation/
        ├── benchmark/                 benchmark summary/overview + per-model classification CSV
        ├── condensed_md_check/        condense-check fidelity reports (<name>_check_condensed_with_pdf.txt)
        └── classification_csv_check/  classify numeric verification reports (<name>_check_classification_with_pdf.txt)
```

---

## Notes

- **Large PDFs** are automatically split into 30 KB chunks to stay within token limits.
- **classify requires summarize first** — the summary MD file is the source of truth.
- **Scanned PDFs** have limited support (text-layer only; use OCR pre-processing for image PDFs).
- **Timeout** is 15 minutes per AI call.
- The summary MD file is human-readable and editable — fix errors there before classifying.
- **Reproducible output:** the discovered technology list is frozen to
  `01_input/13_technology_list_md/<name>_technology_list.md` (one name per line, editable) so re-runs keep the
  same rows; delete it to re-scan. Bit-identical *cell values* require the local Ollama backend
  (`dotnet run -- --local`), which runs at `temperature=0` with a fixed `seed` (override via
  `OLLAMA_TEMPERATURE` / `OLLAMA_SEED`). The hosted Copilot backend exposes no sampling controls,
  so its values still vary slightly run-to-run — determinism here means *repeatable*, not *correct*.

---

## CSV Output Records

`tech_id`, `description`, `summary`, `process_type`, `main_sector`, `main_category`, `category_spec`, `tech_type`, `reference_unit_size`, `trl`, `year` (of the data point), `ref_year` (source publication year, auto-filled per paper), `capex`, `opex` (fixed + variable), `efficiency`, `carriers_in`, `carriers_out`

---

## Version History

- **v1.0** — Data folders restructured into `01_input/` (`11_pdf_to_analyze`, `12_condensed_md`, `13_technology_list_md`) and `02_output/` (`21_tech_summary_md`, `22_tech_classification_csv`, `23_validation/{benchmark, condensed_md_check, classification_csv_check}`); classify verification reports get their own folder; benchmark files named `<pdf>_<provider>_benchmark_*_<yyyy-MM-dd>` (copilot/ollama); technology lists renamed `<name>_technology_list.md`; comments and docs refreshed
- **v0.6** — Source reorganized by role: `core/` (extract, condense, summarize, classify), `format_output/` (record + CSV/MD formats), `chat/` (provider backends), `console/`, `helpers/`; project renamed `TestApp` → `TechClass`
- **v0.5** — Output folders reorganized: benchmark files now go to `3_output/3_benchmark/`, and condense-check + grounding-verifier reports to `3_output/4_condensed_check/`
- **v0.4** — Token-saving condensation PDFs; new folders structure (`1_pdf_to_analyze`, `2_md_condensed_pdf`, `3_output/{1_txt_summary,2_csv_classification}`); helpers files moved to `helpers/`;
- **v0.3** — Code refactored into dedicated modules; prompt templates externalized to `prompt/`; bug fixes, dead code removed
- **v0.2** — Working prototype: model selection at startup; `benchmark` command (multi-model comparison + auto-classification)
- **v0.1** — Two-step workflow: `auto-summarize` (PDF→TXT) + `auto-classify` (TXT→CSV); batch retry logic; improved prompts
- **v0.0** — Initial template with chat, PDF analysis, batch analysis, and basic auto-classification
