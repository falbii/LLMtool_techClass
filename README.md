# Smart Document Analysis Tool — Copilot SDK

PDF analysis and technology classification powered by the GitHub Copilot SDK for .NET.

---

## Quick Start

**Prerequisites:** .NET 10+, GitHub Copilot CLI installed and authenticated.

```bash
dotnet restore
dotnet run
```

On launch, the tool checks Copilot is available, lists models (with reasoning support), and asks you to pick one.

---

## Commands

| Command | Description |
|---------|-------------|
| `list` | List PDFs in `./1_pdf_to_analyze/` and select one |
| `upload <path>` | Copy a PDF into `./1_pdf_to_analyze/` and load it |
| `current` | Show the currently loaded PDF |
| `auto-summarize` | Extract technology summaries to a TXT file |
| `auto-classify` | Convert the TXT into a structured CSV |
| `batch-analyze <q>` | Ask one question across all PDFs |
| `benchmark` | Run all models on the same PDF and compare results |
| `commands` / `help` | Show all commands |
| `exit` / `quit` | Exit |

Any other input is sent to Copilot as a question. If a PDF is loaded, its text is automatically injected as context.

---

## Core Workflow: PDF → TXT → CSV

```
1. list / upload        → select a PDF
2. auto-summarize       → 3_output/1_txt_summary/document.txt   (review/edit freely)
3. auto-classify        → 3_output/2_csv_classification/document_classification.csv
```

**auto-summarize** runs in two stages:
1. Scans the PDF to find all unique technology names
2. Extracts detailed data per technology, organized by year

### Token-saving condensation cache

The first time any operation needs a PDF, the tool condenses it once into a compact
`2_md_condensed_pdf/<name>.condensed.md` — preserving every number, unit, table, and technology name
but stripping prose. All later operations (auto-summarize, batch-analyze, Q&A, benchmark)
read this cached `.md` instead of re-sending the full PDF, cutting token usage substantially.

The cache is reused automatically and regenerated only when the source PDF changes.
Because extraction is lossy compression, review the `.md` if a number looks off in the CSV.

**auto-classify** reads the TXT (not the raw PDF) and converts each technology into one or more CSV rows — one row per year/time horizon.

---

## Benchmark

```
You: benchmark
```

Runs every available Copilot model on the same standard PDF (`Allgoewer_2024.pdf`, must be in `./1_pdf_to_analyze/`), then auto-classifies each response. Saves to `./3_output/` (`.txt` responses under `1_txt_summary/`, `.csv` files under `2_csv_classification/`):

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
├── TechnologyTxt.cs        summary TXT read/write
├── GroundingVerifier.cs    deterministic numeric grounding check
├── helpers/
│   ├── AppHelpers.cs       session factory + console output helpers
│   └── TechClassifierHelpers.cs  shared parsing/formatting utilities
├── prompt/                 prompt template markdown files
├── 1_pdf_to_analyze/       input PDFs (auto-created)
├── 2_md_condensed_pdf/     condensed .md cache (auto-created, regenerable)
└── 3_output/
    ├── 1_txt_summary/      auto-summarize TXT output
    └── 2_csv_classification/  auto-classify CSV output
```

---

## Notes

- **Large PDFs** are automatically split into 30 KB chunks to stay within token limits.
- **auto-classify requires auto-summarize first** — the TXT file is the source of truth.
- **Scanned PDFs** have limited support (text-layer only; use OCR pre-processing for image PDFs).
- **Timeout** is 15 minutes per AI call.
- The TXT file is human-readable and editable — fix errors there before classifying.

---

## CSV Output Fields

`Datapaper Tech ID`, `description`, `summary`, `ProcessType`, `main_sector`, `main_category`, `category_spec`, `tech_type`, `reference_unit_size`, `trl_(1-9)`, `cost_base_year`, `capex_one_time_eur`, `opex_*`, `overall_efficiency`, `carriers_in`, `carriers_out`

---

## Version History

- **v0.4** — Token-saving condensation PDFs; new folders structure (`1_pdf_to_analyze`, `2_md_condensed_pdf`, `3_output/{1_txt_summary,2_csv_classification}`); helpers files moved to `helpers/`;
- **v0.3** — Code refactored into dedicated modules; prompt templates externalized to `prompt/`; bug fixes, dead code removed
- **v0.2** — Working prototype: model selection at startup; `benchmark` command (multi-model comparison + auto-classification)
- **v0.1** — Two-step workflow: `auto-summarize` (PDF→TXT) + `auto-classify` (TXT→CSV); batch retry logic; improved prompts
- **v0.0** — Initial template with chat, PDF analysis, batch analysis, and basic auto-classification
