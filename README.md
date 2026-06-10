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
dotnet run
```

On launch, the tool checks Copilot is available, lists models (with reasoning support), and asks you to pick one.

---

## Commands

| Command | Description |
|---------|-------------|
| `/list` | List PDFs in `./1_pdf_to_analyze/` and select one |
| `/upload <path>` | Copy a PDF into `./1_pdf_to_analyze/` and load it |
| `/current` | Show the currently loaded PDF |
| `/auto-summarize` | Extract technology summaries to a Markdown file |
| `/auto-classify` | Convert the summary MD into a structured CSV |
| `/batch-analyze <q>` | Ask one question across all PDFs |
| `/benchmark` | Run all models on the same PDF and compare results |
| `/commands` / `/help` | Show all commands |
| `/exit` / `/quit` | Exit |

Commands also work without the leading `/`, but an unknown `/command` reports an error instead of being sent to the model. Any other input is sent to Copilot as a question. If a PDF is loaded, its (condensed) text is injected as context on the first question about it; follow-up questions reuse the session context instead of re-sending the document.

---

## Core Workflow: PDF → MD → CSV

```
1. /list or /upload     → select a PDF
2. /auto-summarize      → 3_output/1_md_summary/document.md   (review/edit freely)
3. /auto-classify       → 3_output/2_csv_classification/document_classification.csv
```

**auto-summarize** runs in two stages:
1. Scans the PDF to find all unique technology names
2. Extracts detailed data per technology, organized by year


**auto-classify** reads the summary MD (not the raw PDF) and converts each technology into one or more CSV rows — one row per year/time horizon. Legacy `.txt` summaries are still read as a fallback.

#### Token-saving condensation cache

The first time any operation needs a PDF, the tool condenses it once into a compact
`2_md_condensed_pdf/<name>.condensed.md` — preserving every number, unit, table, and technology name
but stripping prose. All later operations (auto-summarize, batch-analyze, Q&A, benchmark)
read this cached `.md` instead of re-sending the full PDF, cutting token usage substantially.

The cache is reused automatically and regenerated only when the source PDF changes.
Because extraction is lossy compression, review the `.md` if a number looks off in the CSV.

---

## Benchmark

```
You: /benchmark
```

Runs every available Copilot model on the same standard PDF (`Allgoewer_2024.pdf`, must be in `./1_pdf_to_analyze/`), then auto-classifies each response. Saves to `./3_output/` (`.txt` responses under `1_md_summary/`, `.csv` files under `2_csv_classification/`):

- `benchmark_<timestamp>.csv` — latency, word count, classified rows per model
- `benchmark_<timestamp>.txt` — full raw responses
- `benchmark_<timestamp>_classification.csv` — combined classified rows with a `Model` column

---

## File Structure

```
CopilotSDK_techClass/
├── Program.cs              entry point, model selection, session management
├── Workspace.cs            app-wide context (client, model, directory layout)
├── CommandHandlers.cs      CLI command dispatch and benchmark
├── PdfExtractor.cs         text extraction and chunking
├── PdfCondenser.cs         one-time PDF→condensed-MD caching for token savings
├── TechSummarizer.cs       auto-summarize logic
├── TechClassifier.cs       auto-classify pipeline (batch → JSON → validate → merge)
├── TechnologyRecord.cs     the record data to classify
├── TechnologyCsv.cs        cassification CSV read/write
├── TechnologyMd.cs         summary Markdown read/write
├── GroundingVerifier.cs    deterministic numeric grounding check
├── helpers/
│   ├── AppHelpers.cs       session factory + console output helpers
│   └── TechClassifierHelpers.cs  shared parsing/formatting utilities
├── prompt/                 prompt template markdown files
├── 1_pdf_to_analyze/       input PDFs (auto-created)
├── 2_md_condensed_pdf/     condensed .md cache (auto-created, regenerable)
└── 3_output/
    ├── 1_md_summary/       auto-summarize Markdown output
    └── 2_csv_classification/  auto-classify CSV output
```

---

## Notes

- **Large PDFs** are automatically split into 30 KB chunks to stay within token limits.
- **auto-classify requires auto-summarize first** — the summary MD file is the source of truth.
- **Scanned PDFs** have limited support (text-layer only; use OCR pre-processing for image PDFs).
- **Timeout** is 15 minutes per AI call.
- The summary MD file is human-readable and editable — fix errors there before classifying.

---

## CSV Output Records

`tech_id`, `description`, `summary`, `process_type`, `main_sector`, `main_category`, `category_spec`, `tech_type`, `reference_unit_size`, `trl`, `year` (of the data point), `ref_year` (source publication year, auto-filled per paper), `capex`, `opex` (fixed + variable), `efficiency`, `carriers_in`, `carriers_out`

---

## Version History

- **v0.4** — Token-saving condensation PDFs; new folders structure (`1_pdf_to_analyze`, `2_md_condensed_pdf`, `3_output/{1_txt_summary,2_csv_classification}`); helpers files moved to `helpers/`;
- **v0.3** — Code refactored into dedicated modules; prompt templates externalized to `prompt/`; bug fixes, dead code removed
- **v0.2** — Working prototype: model selection at startup; `benchmark` command (multi-model comparison + auto-classification)
- **v0.1** — Two-step workflow: `auto-summarize` (PDF→TXT) + `auto-classify` (TXT→CSV); batch retry logic; improved prompts
- **v0.0** — Initial template with chat, PDF analysis, batch analysis, and basic auto-classification
