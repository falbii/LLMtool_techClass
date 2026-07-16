---
title: "Open-source LLM Tool for Technical Data Extraction and Classification"
tags:
  - PDF extraction
  - large language models
  - information extraction
  - technology classification
authors:
  - name: Felix Allgoewer
    orcid: 0000-0000-0000-0000
    affiliation: 1
affiliations:
  - name: Add institutional affiliation
    index: 1
date: 2026-07-16
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
reused across repeated analyses.

# State of the field

The broader ecosystem offers multiple ways to work with PDF documents and large
language models, including general-purpose chat interfaces, custom extraction
scripts, and workflow-specific data processing notebooks. However, many of these
approaches are either manual, opaque, or tightly bound to one model provider.

The contribution of this project is not a generic PDF reader or a thin wrapper
around a single API. Instead, it combines provider-agnostic LLM interaction,
inspectable intermediate files, numeric validation, and benchmark reporting in a
workflow shaped around research extraction tasks. This design makes it better
suited to repeated technical-document analysis than ad hoc prompting alone.

This section should be expanded before submission with explicit comparisons to
the most relevant tools in the target research domain and a clear build-versus-
contribute justification.

# Software design

The software is organized around a pipeline that separates provider access,
workflow orchestration, and output formatting. A provider-neutral chat interface
allows the same extraction stages to run with GitHub Copilot or local Ollama
models. This reduces coupling to a specific vendor and supports both connected
and local workflows.

Intermediate files are stored intentionally. Condensed Markdown reduces repeated
token usage while preserving a reviewable representation of the source text.
Technology lists and summary Markdown files can be inspected or edited before
classification, which supports the practical reality that LLM-assisted
extraction is not fully automatic. Validation reports provide an audit layer for
checking whether key numeric content survived condensation and classification.

# Research impact statement

The software is intended for research workflows that transform semi-structured
technical documents into structured comparison datasets. In its current form,
the repository already demonstrates a complete end-to-end workflow on a public
example document and provides benchmark and validation artifacts that support
reproducible review.

Before JOSS submission, this section should be strengthened with concrete
evidence of research use, such as publications, internal research workflows,
adoption by collaborators, or documented comparative analyses produced with the
software.

# AI usage disclosure

Generative AI tools were used during the development and preparation of this
software repository, including assistance with refactoring, documentation
drafting, and JOSS-preparation scaffolding. Human authors reviewed, edited, and
validated the resulting materials and remained responsible for architectural
decisions, repository structure, and the correctness of the submitted content.

# Acknowledgements

Add funding, institutional, and collaborator acknowledgements here.

# References
