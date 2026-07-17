---
title: "Open-source LLM Tool for Technical Data Extraction and Classification"
tags:
  - PDF extraction
  - large language models
  - information extraction
  - technology classification
authors:
  - name: Francesco Albisetti
    affiliation: 1
affiliations:
  - name: Empa, Swiss Federal Laboratories for Materials Science and Technology, UES Laboratory, MES Team, Dübendorf, Switzerland
    index: 1
date: 17 July 2026
bibliography: paper.bib
---

# Summary

Open-source LLM Tool for Technical Data Extraction and Classification is a .NET
application for transforming technical PDF documents into structured,
reviewable datasets. The software condenses a PDF into Markdown, identifies
technologies discussed in the source, produces technology-level summaries, and
converts those summaries into structured CSV output. The same pipeline can run
with either GitHub Copilot or local Ollama models, allowing users to choose
between a managed backend and a local deployment.

# Statement of need

Researchers working with technical reports, energy system studies, and related
grey literature often need to extract comparable values from semi-structured PDF
documents. Manual extraction is labor-intensive, difficult to reproduce, and
hard to audit. Existing general-purpose LLM interfaces can help with ad hoc
question answering, but they do not automatically preserve an inspectable
pipeline of intermediate artifacts and structured outputs tailored to research
data extraction workflows.

This software addresses that gap by providing a reproducible pipeline from PDF
to condensed Markdown, technology summaries, classification CSVs, and explicit
validation reports. It is designed for researchers who need not only generated
results, but also intermediate artifacts that can be reviewed, corrected, and
reused across repeated analyses. The intended users are energy-system analysts,
technology assessors, and research-data practitioners who compare quantitative
technology claims across reports. The command-line and local web interfaces
expose the same workflow, allowing exploratory use without replacing the file-
based artifacts required for review and downstream analysis.

# State of the field

The broader ecosystem offers document-conversion tools such as Docling
[@docling], general LLM application frameworks such as LangChain [@langchain]
and LlamaIndex [@llamaindex], and custom extraction notebooks. These projects
provide broad components for parsing documents, retrieval, and model
orchestration, but adopting one would still require a domain workflow for
technology discovery, technology-level summaries, a fixed classification
schema, numeric-fidelity checks, and comparable benchmark artifacts.

The contribution of this project is not a generic PDF reader or a thin wrapper
around a single API. Instead, it combines provider-agnostic LLM interaction,
inspectable intermediate files, numeric validation, and benchmark reporting in a
workflow shaped around research extraction tasks. This design makes it better
suited to repeated technical-document analysis than ad hoc prompting alone.
The project is therefore maintained as a focused application rather than a
contribution to a general orchestration framework: its scholarly contribution
is the end-to-end, human-reviewable workflow and its research-specific output
contract, while PDF parsing and model execution remain delegated to existing
libraries and services.

# Software design

The software is organized around a pipeline that separates provider access,
workflow orchestration, and output formatting. A provider-neutral chat interface
allows the same extraction stages to run with GitHub Copilot or local Ollama
models [@copilot_sdk; @ollama]. This reduces coupling to a specific vendor and
supports both connected and local workflows. The application targets .NET
[@dotnet] and uses iText for PDF text extraction [@itext].

Intermediate files are stored intentionally. Condensed Markdown reduces repeated
token usage while preserving a reviewable representation of the source text.
Technology lists and summary Markdown files can be inspected or edited before
classification, which supports the practical reality that LLM-assisted
extraction is not fully automatic. Validation reports provide an audit layer for
checking whether key numeric content survived condensation and classification.

This design trades a single opaque model call for several explicit stages. The
staged approach requires more files and orchestration, but failures can be
localized and corrected without rerunning the entire analysis. Caching the
condensed representation reduces repeated model context, while invalidating it
when the source PDF changes avoids silently reusing stale text. A stable CSV
schema separates probabilistic extraction from deterministic downstream data
processing. Provider abstraction also permits local execution for sensitive
documents, although users remain responsible for evaluating the privacy and
licensing conditions of their selected model. Automated tests cover the
deterministic parsing, merging, identifier generation, and numeric-verification
components; model-dependent stages are complemented by documented functional
checks and tracked example artifacts.

# Research impact statement

The software has been developed and used to extract and classify technologies
from semi-structured technical literature. The repository demonstrates this
research workflow on the openly licensed article by Allgoewer et al.
[@allgoewer2024]. It includes the source document, intermediate Markdown,
technology list, structured classification output, numeric validation reports,
and multi-model benchmark artifacts. These materials make the demonstrated use
reproducible and allow reviewers to inspect where information changes between
pipeline stages. The same schema and staged workflow can be reused for other
technical documents without changing the core application.

The software, documentation, tests, and reproducible example are publicly
available in the project repository [@techclass].

# AI usage disclosure

GitHub Copilot and Anthropic Claude were used for code generation and
refactoring; GitHub Copilot also assisted with documentation and paper drafting.
The specific model versions used across earlier development sessions were not
recorded. The author reviewed, edited, and validated all AI-assisted outputs and
remained responsible for problem framing, architectural decisions, repository
structure, and the correctness of the submitted content.

# Acknowledgements

The design of this pipeline was inspired by the openly shared work of James
Montemagno ([github.com/jamesmontemagno](https://github.com/jamesmontemagno)),
particularly the
[podcast-metadata-generator](https://github.com/jamesmontemagno/podcast-metadata-generator) project. 

EMPA, UES lab ....... TO DO


# Conflict of interest

The author declares no conflicts of interest.

# References
