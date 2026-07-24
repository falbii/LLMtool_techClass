---
title: 'Open-source LLM Tool for Technical Data Extraction and Classification'
tags:
  - PDF extraction
  - large language models
  - information extraction
  - technology classification
authors:
  - name: Francesco Albisetti
    affiliation: 1
  - name: Barton Yi-Chung Chen
    affiliation: 1
affiliations:
  - name: Empa, Swiss Federal Laboratories for Materials Science and Technology, UES Laboratory, MES Team, Dübendorf, Switzerland
    index: 1
date: 17 July 2026
bibliography: paper.bib
---

# Summary

This open-source LLM tool for technical data extraction and classification is a .NET application for transforming technical PDF documents into structured, reviewable datasets. The software condenses a PDF into Markdown first, identifies the main technologies discussed in the source file, and produces two primary outputs: a technology-level summary and, in a subsequent step, a structured CSV dataset. The same pipeline can run using commercial models on the GitHub Copilot platform or local models via Ollama, allowing users to choose between cloud-connected and fully offline local deployments. By combining document parsing, model-driven extraction, and explicit validation steps, the tool makes technical evidence extraction reproducible, auditable, and reusable across repeated research workflows.

# Statement of Need

Researchers working with technical reports in energy-system studies and related literature often need to extract comparable quantitative values from various PDF sources. Manual extraction is labor-intensive, difficult to reproduce, and hard to audit. Existing general-purpose LLM interfaces help with ad hoc question answering, but they do not automatically preserve an inspectable pipeline of intermediate artifacts and structured outputs tailored to research data extraction workflows.

This software addresses that gap by providing a reproducible pipeline from PDF to condensed Markdown, technology summaries, and classification CSVs, backed by explicit validation reports. It is designed for researchers who require not only generated end-results, but also reviewable intermediate artifacts that can be audited, corrected, and reused. Intended users include energy-system analysts, technology modelers, and research-data practitioners who compare quantitative claims across technical literature and require a transparent, verifiable record of how data was extracted and structured.

# State of the Field

The broader software ecosystem offers general document-conversion tools such as Docling [@docling], application frameworks such as LangChain [@langchain] and LlamaIndex [@llamaindex], and custom extraction scripts. While these tools provide components for parsing documents, retrieval, and model orchestration, adopting them still requires building a domain-specific workflow for technology discovery, technology-level summaries, fixed classification schemas, numeric-fidelity checks, and benchmark evaluation artifacts.

More specialized extraction systems have also emerged. Göpfert et al. developed Quinex, a domain-specific framework for quantitative information extraction operating in four stages: quantity detection, contextual anchoring, multi-turn QA for property/temporal scope, normalization/unit linking, and multi-label classification [@gopfert2026quinex]. Their design highlights the value of structuring extraction around explicit numerical anchors and standardized unit representations. Similarly, Odobesku et al. introduced NanoMiner, an agent-based multimodal pipeline combining a ReAct orchestrator, a YOLO visual extractor, GPT-4o reasoning, and domain-specific named-entity recognition [@Odobesku2025]. 

While these systems demonstrate the power of task-specific prompting and multimodal orchestration, they remain domain-specific or narrower in workflow design. This software complements existing work by providing an end-to-end framework focused on structured and reusable classification outputs for energy and industrial technology literature, with  reviewable intermediate artifacts and deterministic numeric validation.

# Software Design

The software is organized around a sequential pipeline that separates provider access, workflow orchestration, and output formatting. A provider-neutral chat interface allows identical extraction stages to run with GitHub Copilot or local Ollama models [@copilot_sdk; @ollama]. This reduces coupling to a single vendor and supports both connected and local workflows. The application targets .NET [@dotnet] and uses iText 7 for PDF text extraction [@itext], making the workflow suitable for both interactive exploration and batch processing.

Intermediate files are stored in dedicated directories. In the first step, the condensed Markdown generated with `/condense` reduces repeated token usage while preserving a reviewable representation of the source text. Next, `/summarize` generates a technology list and a summary Markdown file that can be inspected or manually edited prior to classification. Finally, `/classify` extracts key parameters for each technology and saves a structured CSV file. Validation reports provide an audit layer for verifying whether numeric content survived condensation and classification.

This staged approach requires more files and orchestration than a single-turn LLM call, but failures can be localized and corrected without rerunning the entire pipeline. Caching the condensed representation reduces model context usage. A stable CSV schema separates probabilistic extraction from deterministic downstream processing. Provider abstraction permits local execution for sensitive documents, though users remain responsible for evaluating model licensing and privacy conditions. Automated unit tests cover deterministic parsing, merging, identifier generation, and numeric-verification components.

# Methods

The tool is implemented in C# and targets .NET 10.0. The architecture comprises four principal components: AI integration, PDF processing, domain summarization/classification, and output formatting.

The AI integration layer uses the GitHub Copilot SDK for .NET and Ollama, providing programmatic access to GitHub Copilot language models via CLI (requiring a Copilot license) or direct access to local Ollama models. At startup, the tool initializes the chosen backend using `CopilotChatClient` or `OllamaChatClient` and prompts the user to select an available model. All AI inference calls flow through this session to preserve conversational context, except when the PDF context is swapped, at which point the session resets to avoid context contamination.

A key design choice is the use of streaming responses to provide low-latency interaction. The built-in `/benchmark` command allows side-by-side evaluation by running all available models on a reference document and logging outputs to disk.

The core pipeline is divided into four main classes: `PdfExtractor`, `PdfCondenser`, `TechSummarizer`, and `TechClassifier`.

PDF text extraction is performed using iText 7. The `PdfExtractor` class iterates over PDF pages, annotating extracted text with explicit page boundary markers (`[PAGE N]` / `[END OF PAGE]`) to preserve spatial context. To handle tables, the tool analyzes each line for tabular indicators and wraps identified sections in `[TABLE]` / `[END TABLE]` tags. This annotation approach improves the model's ability to associate numerical values with corresponding column headers. For large PDFs, extracted text is divided into chunks of 30,000 characters with a three-line overlap.

When a PDF is first loaded, `PdfCondenser` processes the raw text into a compact Markdown summary to lower downstream token consumption. The output is saved and reused as the working reference. The `/condense-check` command offers a text comparison between the original PDF and the Markdown file to flag potential fidelity issues.

`TechSummarizer` and `TechClassifier` form the core extraction logic. `TechSummarizer` creates a structured Markdown summary of all technologies mentioned in the document. `TechClassifier` enforces a domain schema capturing key parameters required for quantitative technology assessments:

* **Identity parameters:** Unique technology identifier, process type, unit operation, short description, and summary.
* **Classification parameters:** Main sector (e.g., Energy, Industry), main category (e.g., Electrolysis, Carbon Capture), subcategory (e.g., AEC, SOEC), and specific technology variant.
* **Technical parameters:** Reference size, unit, year of development, geographic location, currency, TRL (1–9), and efficiency.
* **Energy and mass flow:** Input/output carriers, main input/output, conversion ratios, and units.
* **Techno-economic parameters:** Plant capacity, lifetime (years), CAPEX, and fixed OPEX.

## Project Structure

The project directory is structured as follows:

```text
techClass/
    |__ 01_input/
    |   |__ 11_pdf_to_analyze/         input PDFs
    |   |__ 12_condensed_md/
    |   |__ 13_technology_list_md/
    |__ 02_output/
    |   |__ 21_tech_summary_md/        technology summaries
    |   |__ 22_tech_classification_csv/ technology classifications
    |   |__ 23_validation/
    |__ core/                          main pipeline logic
    |   |__ PdfExtractor.cs
    |   |__ PdfCondenser.cs
    |   |__ TechSummarizer.cs
    |   |__ TechClassifier.cs
    |__ format_output/
    |__ chat/
    |__ console/
    |__ helpers/
    |__ prompt/                        prompt templates
    |__ Program.cs                     entry point and session management
    |__ Workspace.cs                   context layout
    |__ TechClass.csproj
    |__ TechClass.sln
```
  
## Automated extraction pipeline

The core scientific contribution of the tool lies in its three-stage automated extraction pipeline, accessible via the `/condense`, `/summarize`, and `/classify` commands.

In the first phase, before the summarization pipeline, the tool sends the full PDF to the LLM for the purpose of creating a condensed version of it to reduce token consumption. In a second phase, the already condensed PDF content (saved as a Markdown file) is sent to the language model with a prompt designed to generate a unique list of all the main distinct technologies present and analyzed in the document. The prompt explicitly instructs the model to treat different years and geographic scopes of the same technology as a single entry, suppressing the redundancy that would otherwise arise. The result is a clean list of N unique technology identifiers that defines the scope of the subsequent extraction.

With the technology list established, the tool iterates over the technologies in batches and, with a detailed prompt that instructs the model to extract all quantitative and qualitative technical and economic parameters for each technology, groups all the data by technology and year in subsections. The model provides the user with a detailed summary, constructed in clear sections.

The result of this stage is a human-readable Markdown file saved to the output directory folder, 02_output/21_tech_summary_md. This file serves as a dual purpose: it is a standalone reference document that a researcher can read and manually verify, and it is also the input file for the subsequent classification stage.

In the third step, the `/classify` command reads the Markdown file produced by /summarize, parses it into N individual technology sections using the list of technologies extracted before, and submits batches of sections to the language model with a prompt requesting a JSON output file conforming to the TechnologyRecord schema. Each JSON response is parsed, validated against the schema using TechClassifier.TryClassify, and merged into a running collection. Parsing failures trigger an automatic retry mechanism, which resubmits the failed batch with corrective instructions.

Once all technologies have been processed, the collection is serialized to a CSV file using the column names defined by the domain schema. The CSV is structured to be directly importable into an energy-system database, model-parameterization workflows, or spreadsheet tools and is saved to 02_output/22_tech_classification_csv.

## Interactive interface and supplementary features

Beyond the core automated extraction pipeline, the tool provides a rich interactive terminal environment that also supports exploratory analysis workflows.

The tool's main loop for in-context questions implements a text-based command dispatcher supporting free-form chat. With any input that does not match a specific command keyword, the text is forwarded to the Copilot or Ollama session as a natural-language query, enabling the user to ask open-ended questions about the loaded document (for example, “What are the main findings?” or “Extract all numerical data about this technology”).

Additionally, the tool includes an intrinsic benchmark of all available models offered by GitHub Copilot and downloaded open-source local models. With the `/benchmark` command, the program runs the full extraction pipeline on a reference document for three requested technologies. By using every available model in sequence and recording outputs to timestamped files in the output folder (02_output/23_validation), this command provides an empirical basis for model selection decisions.

## Output records

All output files are written to the output directory, with filenames derived from the source PDF stem (for example, energy_report_2024_summary.md and energy_report_2024_classification.csv). Benchmark runs are instead timestamped (for example, benchmark_summary_energy_report_2024_dd-mm-yyyy.csv). The directory structure is created automatically at startup if it does not exist.

# Research Impact Statement

This software was developed and applied to extract structured technology parameters from semi-structured technical literature. The repository includes a complete example based on the open-access article by Allgoewer et al. [@allgoewer2024], containing the source document, intermediate Markdown artifacts, generated summaries, classification CSVs, and validation logs. The same pipeline and domain schema can be adapted to other technical literature without modifying the underlying application architecture.

The software, documentation, unit tests, and reference dataset are publicly available in the project repository [@techclass].

# AI Usage Disclosure

GitHub Copilot and Anthropic Claude were used for code generation, refactoring, documentation, and drafting manuscript text. The specific model versions used across earlier development sessions were part of the GitHub Copilot suite and local Ollama models. The authors reviewed, edited, and validated all AI-assisted outputs and take full responsibility for the methodology, repository integrity, and final paper content.

# Acknowledgements

The design of this pipeline was inspired by open-source work by James Montemagno, particularly the `podcast-metadata-generator` project. The authors acknowledge support from Empa, Swiss Federal Laboratories for Materials Science and Technology (UES Laboratory, MES Team).

# Conflict of Interest

The authors declare no conflicts of interest.

# References
