# Architecture Notes

## Overview

The repository is organized around a pipeline that transforms PDF input into
intermediate Markdown artifacts and final structured CSV output.

## Main Components

- `techclass/console/cli.py`
  Entry point, backend/model selection, and interactive command loop.
- `techclass/chat/backends.py`
  Provider-neutral chat interfaces plus GitHub Copilot SDK and Ollama adapters.
- `techclass/core/pipeline.py`
  Cached condensation, technology summarization, classification, and checks.
- `techclass/core/pdf.py`
  PDF text extraction, chunking, and document prompt assembly.
- `techclass/helpers/classifier.py`, `techclass/helpers/verifier.py`
  Deterministic normalization, validation, merging, and numeric verification.
- `techclass/format_output/`
  Data models plus Markdown and CSV formatting.
- `techclass/helpers/benchmark.py`
  Cross-model benchmark orchestration and report generation.
- `techclass/web/server.py`
  FastAPI host for the local browser interface.
- `web/wwwroot/`
  Framework-free browser assets served by the Python host.

## Design Choices Relevant to JOSS

- Backend abstraction
  The extraction workflow is insulated from the model provider by an
  `ChatClient` and `ChatSession` abstraction, allowing the same pipeline to run
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
