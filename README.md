# Open-source LLM Tool for Technical Data Extraction and Classification

This repository contains a .NET application for extracting structured technical
data from PDF documents with large language models. The software condenses a PDF
into cached Markdown, identifies technologies described in the document,
summarizes those technologies, and produces structured CSV output for downstream
analysis.

The project supports two execution backends:

- GitHub Copilot via `GitHub.Copilot.SDK`
- Local Ollama models via `--local` or `--ollama`

## Statement of Need

Researchers and analysts often need to turn semi-structured technical reports
into structured datasets that can be compared across technologies, years, and
sources. That work is usually manual, difficult to reproduce, and slow to audit.

This software supports research workflows where the source material is a PDF and
the desired result is a structured, reviewable dataset. It combines:

- cached PDF-to-Markdown condensation for token-efficient reuse
- technology-level extraction and summarization
- structured CSV classification
- verification reports for numeric fidelity and classification checks
- comparative model benchmarking across the same document subset

## Current JOSS Preparation Status

This `joss-prep` branch is being prepared for Journal of Open Source Software
(JOSS) submission. The repository now includes:

- reviewer-oriented documentation
- contribution and citation metadata
- a JOSS paper scaffold
- a GitHub Actions build workflow
- an OSI-approved MIT license

## Installation

### Prerequisites

- .NET 10 SDK
- One runtime backend:
  - GitHub Copilot CLI with an authenticated Copilot account, or
  - Ollama with at least one local model pulled

### Install .NET 10

```powershell
winget install Microsoft.DotNet.SDK.10
```

On macOS or Linux, use the platform-specific instructions from Microsoft:
https://learn.microsoft.com/dotnet/core/install/

### Restore dependencies

```bash
dotnet restore
```

## Quick Start

### Default backend: GitHub Copilot

```bash
npm install -g @github/copilot
copilot
dotnet run
```

### Local backend: Ollama

```bash
ollama serve
ollama pull llama3.2
dotnet run -- --local
```

### Web interface

```bash
dotnet run -- --web
dotnet run -- --local --web
```

The web UI runs locally and exposes the same core pipeline as the console mode.

## Example Workflow

1. Add or select a PDF from `01_input/11_pdf_to_analyze/`
2. Run `/extraction`
3. Inspect the generated summary in `02_output/21_tech_summary_md/`
4. Inspect the classified CSV in `02_output/22_tech_classification_csv/`
5. Review validation reports in `02_output/23_validation/`

The included `Allgoewer_2024` example files document one end-to-end run and are
kept as a reviewer-friendly reproducible example.

## Commands

| Command | Description |
| --- | --- |
| `/list` | List PDFs in `01_input/11_pdf_to_analyze/` and select one |
| `/upload <path>` | Copy a PDF into the input folder and load it |
| `/current` | Show the currently selected PDF |
| `/extraction` | Run `condense -> summarize -> classify` |
| `/condense` | Condense the PDF to cached Markdown |
| `/summarize` | Extract technology summaries to Markdown |
| `/classify` | Convert summary Markdown into structured CSV |
| `/condense-check` | Compare numeric data in the PDF and condensed Markdown |
| `/batch-analyze <question>` | Ask one question across all PDFs |
| `/benchmark` | Compare models on selected technologies from one PDF |
| `/help` | Show command help |

## Repository Layout

```text
CopilotSDK_techClass/
|- chat/                          backend abstraction and implementations
|- console/                       console command handlers and output helpers
|- core/                          PDF extraction, condensation, summary, classification
|- docs/                          reviewer and architecture documentation
|- examples/                      pointers to sample input and output material
|- format_output/                 output formats and technology record model
|- helpers/                       validation, parsing, and benchmark utilities
|- prompt/                        prompt templates
|- web/                           local web interface
|- 01_input/                      sample and runtime input folders
|- 02_output/                     sample and runtime output folders
|- .github/workflows/ci.yml       build workflow
|- CITATION.cff                   citation metadata
|- CONTRIBUTING.md                contribution guide
|- paper.md                       JOSS paper draft
|- paper.bib                      JOSS bibliography
```

## Documentation

- Reviewer guide: [docs/reviewer-guide.md](docs/reviewer-guide.md)
- Architecture notes: [docs/architecture.md](docs/architecture.md)
- Example materials: [examples/README.md](examples/README.md)
- Ollama backend notes: [chat/README.md](chat/README.md)

## Reproducibility and Limitations

- PDF condensation is lossy and should be checked with `/condense-check`.
- Scanned PDFs require OCR preprocessing because extraction uses the PDF text
  layer.
- Local Ollama runs default to deterministic settings where possible, but model
  outputs should still be reviewed by a human.
- GitHub Copilot does not expose the same deterministic controls as Ollama.
- A reviewer can objectively inspect example inputs, outputs, and validation
  reports even when backend output varies.

## Citation

If you use this software in research, please cite the repository metadata in
`CITATION.cff` and, once published, the JOSS paper associated with this
repository.
