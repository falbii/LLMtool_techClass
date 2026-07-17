# Reviewer Guide

This guide is written for repository reviewers and JOSS reviewers who want a
fast path to installing and exercising the software locally.

## What the Software Does

The application converts a technical PDF into a sequence of reviewable artifacts:

1. condensed Markdown
2. technology list
3. technology summary Markdown
4. structured classification CSV
5. validation reports

## Local Installation

### Requirements

- .NET 10 SDK
- One backend:
  - GitHub Copilot CLI with an authenticated account, or
  - Ollama running locally with at least one pulled model

### Install and restore

```bash
dotnet restore
```

## Automated Tests

Run the backend-independent tests without Copilot credentials or an Ollama
server:

```bash
dotnet test TechClass.sln --configuration Release
```

The tests exercise deterministic JSON parsing, record conversion, identifier
generation, record merging, and numeric verification. GitHub Actions runs the
same suite on every pull request and on pushes to `main` and `joss-prep`.

## Minimal Functional Check

1. Start the program with `dotnet run`
2. Select a backend model
3. Run `/list`
4. Select `Allgoewer_2024.pdf`
5. Run `/extraction`
6. Confirm that output files are written under:
   - `01_input/12_condensed_md/`
   - `01_input/13_technology_list_md/`
   - `02_output/21_tech_summary_md/`
   - `02_output/22_tech_classification_csv/`
7. Run `/condense-check`
8. Confirm that validation reports are written under `02_output/23_validation/`

## What to Inspect

- Whether the pipeline completes without crashing
- Whether generated files are created in the documented directories
- Whether the CSV structure matches the intended classification output
- Whether validation reports make the extraction auditable

## Sample Materials

The repository includes the `Allgoewer_2024` example to support manual review of
the workflow and generated outputs.

The functional check complements the automated tests because the LLM-backed
pipeline itself depends on the selected external model and runtime backend.
