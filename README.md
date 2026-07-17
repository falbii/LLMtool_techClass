# Open-source LLM Tool for Technical Data Extraction and Classification

Extract structured technical data from PDFs with either **GitHub Copilot** or
**local Ollama models**. The tool condenses a PDF, identifies and summarizes
technologies, then writes validated CSV classifications. It also provides
interactive document Q&A, deterministic verification reports, and multi-model
benchmarks.

## Statement of Need

Researchers and analysts often need to turn semi-structured technical reports
into structured datasets that can be compared across technologies, years, and
sources. Manual extraction is labor-intensive, difficult to reproduce, and slow
to audit. General-purpose LLM interfaces can assist with individual questions,
but they do not preserve a complete, inspectable extraction pipeline.

This software provides a reproducible workflow from PDF input to condensed
Markdown, technology-level summaries, structured classification CSVs, and
validation reports. Intermediate artifacts remain human-readable and editable,
which allows generated content to be reviewed before it becomes structured
research data.

## Quick Start

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

### 2. Restore project dependencies

```bash
dotnet restore
```

### 3. Choose an LLM backend and run

Both backends use the same model picker, commands, PDF workflow, output formats,
and web interface.

#### GitHub Copilot (default)

Requires Node.js 22+, the GitHub Copilot CLI, and an authenticated GitHub account
with an active Copilot subscription.

```bash
npm install -g @github/copilot
copilot            # launch once, then run /login to sign in

dotnet run                     # console mode
dotnet run -- --web            # web UI at http://localhost:5179
```

#### Local Ollama models

Install [Ollama](https://ollama.com), start its server, and pull at least one
model:

```bash
ollama serve
ollama pull llama3.2

dotnet run -- --local          # console mode with Ollama
dotnet run -- --local --web    # web UI with Ollama
```

`--ollama` is an alias for `--local`. Ollama mode does not require the Copilot
CLI or a GitHub sign-in. See [chat/README.md](chat/README.md) for local-backend
settings such as `OLLAMA_HOST`, `OLLAMA_NUM_CTX`, `OLLAMA_TEMPERATURE`, and
`OLLAMA_SEED`.

### Run automated tests

The deterministic test suite does not require an LLM backend or network access:

```bash
dotnet test TechClass.sln
```

## Use the Tool

At startup, select a model and then select or upload a PDF. Use console commands
in console mode, or the corresponding buttons in web mode. The web UI supports
the main pipeline, its checks, benchmarks, file selection/upload, and document
Q&A.

### Commands

| Command | Description |
|---------|-------------|
| `/list` | List PDFs in `01_input/11_pdf_to_analyze/` and select one |
| `/upload <path>` | Copy a PDF into `01_input/11_pdf_to_analyze/` and load it |
| `/current` | Show the currently selected PDF |
| `/extraction` | Run the complete pipeline: condense → summarize → classify |
| `/condense` | Condense the PDF to cached Markdown |
| `/summarize` | Extract technology summaries to Markdown |
| `/classify` | Convert the summary Markdown into a structured CSV |
| `/condense-check` | Check how well numeric data survived PDF condensation |
| `/batch-analyze <question>` | Ask one question across all PDFs |
| `/benchmark` | Compare all available models on selected technologies from one PDF |
| `/commands` / `/help` | Show all commands |
| `/exit` / `/quit` | Exit the console application |

Commands also work without the leading `/`. An unknown `/command` is reported as
an error instead of being sent to the model. Other input is treated as a
question for the selected backend. For a selected PDF, condensed text is added
on the first question; follow-up questions reuse the session context.

## Extraction Workflow: PDF → Markdown → CSV

For the usual end-to-end workflow, select a PDF and run:

```text
/extraction
```

`/extraction` runs the dependent stages in sequence and stops if a stage fails:

```text
condense → summarize → classify
```

The final result is saved to:

```text
02_output/22_tech_classification_csv/<pdf-name>_classification.csv
```

Run individual stages when you want to inspect or edit the intermediate summary
before classification:

```text
1. /condense   → 01_input/12_condensed_md/<pdf-name>_condensed.md
2. /summarize  → 02_output/21_tech_summary_md/<pdf-name>_summary.md
3. /classify   → 02_output/22_tech_classification_csv/<pdf-name>_classification.csv
```

**Condense** creates a compact cached Markdown version of the PDF. **Summarize**
finds technology names and produces detailed per-technology summaries.
**Classify** converts the summary Markdown into one or more rows per technology,
including separate rows for distinct years or time horizons.

### Token-saving cache and review points

The condensed cache at `01_input/12_condensed_md/<name>_condensed.md` is reused
while its source PDF is unchanged. Replacing an uploaded PDF invalidates the
condensed cache and its derived technology list, so the next workflow uses the
replacement document.

The technology list is frozen at
`01_input/13_technology_list_md/<name>_technology_list.md`. Edit it to control
the technologies processed on later runs, or delete it to scan the PDF again.
The summary Markdown is also human-readable and editable; correct it before
running `/classify` when needed.

Because condensation is lossy, use `/condense-check` to compare numeric values
in the raw PDF and condensed Markdown. Classification also writes a numeric
verification report to
`02_output/23_validation/classification_csv_check/`.

## Included Example

The tracked `Allgoewer_2024` files provide one end-to-end example, from the
source PDF through generated outputs and validation artifacts. The name refers
to the example article by Leo Allgoewer and co-authors; those article authors
are not authors of this software. See [examples/README.md](examples/README.md)
for the file list and source citation.

## Benchmark

Run `/benchmark`, select a PDF, then choose exactly three technologies. The tool
summarizes and classifies those same technologies with every model available
from the selected backend.

Results are written to `02_output/23_validation/benchmark/`:

- `<pdf-name>_<provider>_benchmark_summary_<yyyy-MM-dd>.md` — summaries from each model
- `<pdf-name>_<provider>_benchmark_classification_<yyyy-MM-dd>.csv` — classified rows with a leading `Model` column
- `<pdf-name>_<provider>_benchmark_overview_<yyyy-MM-dd>.csv` — timing, word count, row count, and status per model

`<provider>` is `copilot` or `ollama`, matching the active backend.

## File Structure

```text
LLMtool_techClass/
├── Program.cs                    entry point, backend selection, model selection
├── Workspace.cs                  app-wide client, model, and directory context
├── core/                         PDF extraction, condensation, summary, classification
├── format_output/                technology data model and CSV/Markdown formats
├── chat/                         GitHub Copilot and local Ollama backends
├── console/                      console commands and output helpers
├── helpers/                      validation, parsing, benchmark, and verification utilities
├── web/                          local web UI and its server
├── prompt/                       LLM prompt templates
├── docs/                         architecture notes and reviewer guide
├── examples/                     example dataset documentation
├── tests/                        backend-independent automated tests
├── 01_input/
│   ├── 11_pdf_to_analyze/        input PDFs
│   ├── 12_condensed_md/          regenerable condensed Markdown cache
│   └── 13_technology_list_md/    editable frozen technology lists
└── 02_output/
    ├── 21_tech_summary_md/       technology summary Markdown
    ├── 22_tech_classification_csv/ classified CSV output
    └── 23_validation/
        ├── benchmark/            benchmark files
        ├── condensed_md_check/   condensation fidelity reports
        └── classification_csv_check/ classification numeric verification reports
```

## Documentation and Community

- [Reviewer guide](docs/reviewer-guide.md)
- [Architecture notes](docs/architecture.md)
- [Example materials](examples/README.md)
- [Ollama backend notes](chat/README.md)
- [Contribution and support guidelines](CONTRIBUTING.md)

## Notes and Limitations

- Large PDFs are split into approximately 30 KB chunks before LLM processing.
- Scanned PDFs require OCR preprocessing because extraction uses the PDF text layer.
- Each LLM call has a 15-minute timeout.
- Classification requires a summary Markdown file. `/extraction` and `/summarize` create it automatically.
- Local Ollama runs default to `temperature=0` and a fixed seed for more reproducible results. Copilot does not expose equivalent sampling controls, so values can vary between runs.
- Reproducibility means repeatable output under the same conditions; it does not guarantee that extracted values are correct. Review generated summaries and validation reports.

## CSV Output Records

The CSV includes identifiers, descriptions, classification hierarchy, year and
location, reference-unit data, maturity and efficiency, input/output carriers
and ratios, cost and lifetime values, source publication year, summary, and the
model that produced the row when applicable.

## Citation and License

If you use this software in research, cite it using [CITATION.cff](CITATION.cff)
and, once published, the associated JOSS paper. The software is distributed
under the [MIT License](LICENSE).

## Version History

- **v1.1** — Renamed the project to **Open-source LLM Tool for Technical Data Extraction and Classification**; documented GitHub Copilot and local Ollama backends; added the `/extraction` full workflow and its web UI action.
- **v1.0** — Data folders restructured into `01_input/` (`11_pdf_to_analyze`, `12_condensed_md`, `13_technology_list_md`) and `02_output/` (`21_tech_summary_md`, `22_tech_classification_csv`, `23_validation/{benchmark, condensed_md_check, classification_csv_check}`); classify verification reports get their own folder; benchmark files named `<pdf>_<provider>_benchmark_*_<yyyy-MM-dd>`; technology lists renamed `<name>_technology_list.md`.
- **v0.6** — Source reorganized by role: `core/`, `format_output/`, `chat/`, `console/`, and `helpers/`; project renamed `TestApp` → `TechClass`.
- **v0.5 and earlier** — Prototype and workflow iterations, including PDF condensation, summary/classification stages, prompt templates, and benchmark support.
