# Architecture Notes

## Overview

The repository is organized around a pipeline that transforms PDF input into
intermediate Markdown artifacts and final structured CSV output.

## Main Components

- `Program.cs`
  Entry point, backend selection, model selection, and mode dispatch.
- `Workspace.cs`
  Shared runtime context holding backend, model, and directory layout.
- `chat/`
  Provider-neutral chat interfaces plus GitHub Copilot and Ollama
  implementations.
- `core/`
  PDF extraction, condensation, technology summarization, and classification.
- `helpers/`
  Numeric verification, normalization, benchmarking, and validation support.
- `format_output/`
  Output record definitions and CSV/Markdown formatting helpers.
- `console/`
  Interactive command handling for terminal mode.
- `web/`
  Local browser-based UI that exposes the same workflow.

## Design Choices Relevant to JOSS

- Backend abstraction
  The extraction workflow is insulated from the model provider by an
  `IChatClient` and `IChatSession` abstraction, allowing the same pipeline to run
  with Copilot or Ollama.

- Cached condensation
  PDF condensation is stored as reusable Markdown to reduce repeated token costs
  and improve inspectability of the intermediate representation.

- Intermediate artifacts
  Technology lists and summaries are stored as human-readable files so that a
  researcher can inspect or correct them before CSV classification.

- Validation outputs
  Separate validation reports make the workflow more auditable than a single
  opaque end-to-end extraction step.

## Directory Semantics

- `01_input/`
  Runtime inputs and cached intermediates tied to a source PDF.
- `02_output/`
  Generated summaries, classifications, and validation artifacts.

This structure is intentionally workflow-oriented because the software is used as
an interactive research pipeline rather than a library-only package.
