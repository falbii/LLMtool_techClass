# Smart Document Analysis Tool - Copilot SDK

A comprehensive PDF analysis and technology classification tool powered by GitHub Copilot SDK for .NET.

Analyze PDFs, extract insights, and automatically classify technologies with AI-driven intelligence.

---

## Quick Start (2 Minutes)

### Prerequisites
- **GitHub Copilot CLI** installed: `winget install GitHub.Copilot` (Windows) or `brew install copilot-cli` (macOS)
- **Authenticated** with Copilot: Run `copilot /login`
- **.NET 10.0+** installed

### Setup
```bash
cd CopilotSDK_techClass
dotnet restore
dotnet run
```

---

## Features Overview

This application provides **6 core features**:

1. **Interactive Chat** - Real-time conversation with Copilot
2. **PDF Upload & Management** - Manage, list, and select PDFs
3. **PDF Analysis** - Ask questions about PDF content with context injection
4. **Batch Analysis** - Analyze multiple PDFs at once with a single question
5. **Auto-Summarize** - Extract comprehensive technology summaries from a PDF into a TXT file
6. **Auto-Classify** - Convert technology summaries into a structured CSV

---

## Feature 1: Interactive Chat

Start a conversation with Copilot right in the terminal:

```
You: What is machine learning?
Copilot: Machine learning is a subset of artificial intelligence...

You: Can you give me a Python example?
Copilot: Here's a simple machine learning example:
```

Type `exit` or `quit` to exit the program.

---

## Feature 2: PDF Upload & Management

### Available Commands

| Command | Description |
|---------|-------------|
| `list` | List all PDFs in `pdf_to_analyze/` with file sizes |
| `current` | Show which PDF is currently loaded |
| `upload <path>` | Upload a PDF from your computer to analyze |
| `analyze <file>` | Load and analyze a specific PDF (by name or file number) |
| `auto-summarize` | Extract technology summaries from PDF into a TXT file |
| `auto-classify` | Convert TXT summaries into a structured CSV |
| `batch-analyze <q>` | Analyze all PDFs with a question |
| `commands` or `help` | Display all available commands |

### Example Workflow

```
You: list
📁 Available PDFs:
   1. research_paper.pdf (245 KB)
   2. technical_manual.pdf (512 KB)

You: analyze 1
✓ Loaded: research_paper.pdf

You: current
📄 Current PDF: research_paper.pdf

You: upload C:\Documents\new_document.pdf
✓ Loaded: new_document.pdf
```

---

## Feature 3: PDF Analysis

Ask specific questions about loaded PDFs. The system automatically extracts the text and provides contextual answers.

### How It Works

1. **Text Extraction** - Extracts all text from the PDF with table detection
2. **Smart Chunking** - Splits large PDFs into 30KB chunks to manage token limits
3. **Context Injection** - Combines your question with PDF content
4. **AI Analysis** - Copilot analyzes and responds with insights

### Example Questions

```
You: What are the main findings?
Copilot: The document identifies three key findings...

You: Summarize in 3 bullet points
Copilot: • Finding 1
          • Finding 2
          • Finding 3

You: Extract all numerical data
Copilot: The following numerical data appears in the document...

You: What technologies are mentioned?
Copilot: The document discusses the following technologies...
```

---

## Feature 4: Batch Analysis

Analyze **all PDFs** in the folder with a single question:

```
You: batch-analyze What are the key challenges mentioned across all documents?
📊 Batch analyzing 3 PDF(s)...
Copilot: [Provides analysis combining insights from all PDFs]
```

### Use Cases for Batch Analysis
- Compare findings across multiple reports
- Find common themes in document sets
- Extract patterns from document collections
- Conduct systematic literature reviews

---

## Feature 5: Auto-Summarize

Extract comprehensive, structured technology summaries from a PDF into a TXT file.

### Command

```
You: auto-summarize
```

### How It Works

1. **Stage 1 – Find Technologies**: Scans the entire PDF and identifies all unique technology names (no year/region duplicates)
2. **Stage 2 – Extract Summaries**: For each technology, extracts all data from the PDF organised by year

### TXT Output Structure

Each technology gets its own section with year sub-sections:

```
═══ TECHNOLOGY 1: Alkaline water electrolysis ═══

--- Year: 2020 (current/baseline) ---
Efficiency: 63–70% (LHV)
CAPEX: 500–1400 EUR/kW
Lifetime: 80,000 hours
TRL: 9
...

--- Year: 2035 (near future) ---
Efficiency: 65–72% (LHV)
CAPEX: 400–800 EUR/kW
...

--- Year: 2050 (long-term) ---
Efficiency: 70–80% (LHV)
...

───────────────────────────────────────────────────────────────

═══ TECHNOLOGY 2: Proton exchange membrane electrolysis ═══
...
```

### Key Features

- **Year-organised**: Data for different time horizons is grouped under year sub-sections within each technology
- **Comprehensive**: Captures all inputs/outputs, costs, efficiencies, TRL, and process details
- **Human-readable**: The TXT serves as a standalone reference and as input for `auto-classify`
- **Batched extraction**: Technologies are processed in batches of 10 to manage token limits

---

## Feature 6: Auto-Classify

Convert technology summaries (from the TXT file) into a structured **CSV file**.

### Command

```
You: auto-classify
```

> **Prerequisite**: You must run `auto-summarize` first. If the TXT file doesn't exist, the tool will prompt you:
> `⚠️ Please run 'auto-summarize' first to extract technology summaries.`

### How It Works

1. **Reads the TXT** produced by `auto-summarize`
2. **Parses technology sections** from the `═══ TECHNOLOGY N: Name ═══` headers
3. **Converts each batch** to structured JSON via Copilot (with automatic retry on failure)
4. **Validates and merges** rows, then writes to CSV

### CSV Output Structure

The generated CSV includes:

| Field | Description |
|-------|-------------|
| `Datapaper Tech ID` | Unique technology identifier |
| `description` | Brief technology description |
| `summary` | Comprehensive technology summary |
| `ProcessType` | Type of process or unit operation |
| `main_sector` | Industry sector (Energy, Chemicals, etc.) |
| `main_category` | Technology category (Electrolysis, DAC, etc.) |
| `category_spec` | Specific subcategory (Alkaline, PEM, etc.) |
| `tech_type` | Full technology name |
| `reference_unit_size` | Capacity or scale metric |
| `trl_(1-9)` | Technology Readiness Level |
| `cost_base_year` | Year the cost data applies to |
| `capex_one_time_eur` | Capital expenditure in EUR |
| `opex_*` | Operating expenditure metrics |
| `overall_efficiency` | Efficiency percentage |
| `carriers_in` | Input carriers (H2, CO2, etc.) |
| `carriers_out` | Output carriers |

### Example Output

```
You: current
📄 Current PDF: energy_technologies_2024.pdf

You: auto-summarize
📝 Summarising technologies from PDF...
   1. Finding technologies...
   Found 35 technologies
   2. Extracting detailed summaries...
   Batch 1/4 (technologies 1-10)... ✓
   Batch 2/4 (technologies 11-20)... ✓
   Batch 3/4 (technologies 21-30)... ✓
   Batch 4/4 (technologies 31-35)... ✓
✅ Summarisation complete!
   📁 Saved to: output/energy_technologies_2024.txt
   ✓ 35 technologies extracted

You: auto-classify
📋 Classifying from TXT summary and writing to CSV...
   Found 35 technology sections in TXT
   Converting summaries to structured data...
   Batch 1/4 (technologies 1-10)... ✓ (28 rows)
   Batch 2/4 (technologies 11-20)... ✓ (25 rows)
   Batch 3/4 (technologies 21-30)... ✓ (30 rows)
   Batch 4/4 (technologies 31-35)... ✓ (12 rows)

   📁 Saved to: output/energy_technologies_2024_classification.csv
   ✓ 95 rows exported
✅ Classification complete!
```

### Key Features

- **Two-step workflow**: Summarize first (human-reviewable TXT), then classify (structured CSV)
- **TXT as source of truth**: Classification reads from the curated TXT, not the raw PDF — better accuracy
- **Time Horizon Handling**: Technologies with data for multiple years produce separate CSV rows:
  ```
  ALK_ELY_2030    (with 2030-specific data)
  ALK_ELY_2050    (with 2050-specific data)
  ```
- **Merge logic**: Same technology + same year → merged; same technology + different year → separate rows
- **Automatic retry**: Failed JSON batches are retried once before skipping
- **Data Validation**: Filters out incomplete records, shows parsing notes

---

## Complete Workflow Examples

### Example 1: Simple Analysis

```
You: upload C:\Documents\research.pdf
✓ Loaded: research.pdf

You: What are the main conclusions?
Copilot: Based on the PDF, the main conclusions are:
1. Conclusion A
2. Conclusion B
3. Conclusion C
```

### Example 2: Technology Classification

```
You: list
📁 Available PDFs:
   1. hydrogen_production_2024.pdf

You: analyze hydrogen_production_2024.pdf
✓ Loaded: hydrogen_production_2024.pdf

You: auto-summarize
📝 Summarising technologies from PDF...
✅ Summarisation complete!
   📁 Saved to: output/hydrogen_production_2024.txt
   ✓ 23 technologies extracted

You: auto-classify
📋 Classifying from TXT summary and writing to CSV...
✅ Classification complete!
   📁 Saved to: output/hydrogen_production_2024_classification.csv
   ✓ 58 rows exported

You: What technologies had the highest TRL mentioned?
Copilot: The following technologies had the highest TRL values...
```

### Example 3: Batch Comparison

```
You: list
📁 Available PDFs:
   1. report_2024.pdf
   2. report_2025.pdf
   3. forecast_2026.pdf

You: batch-analyze What are the key differences in technology focus between these reports?
📊 Batch analyzing 3 PDF(s)...
Copilot: [Comparative analysis across all three documents]
```

---

## Use Cases

### Document Summarization
```
You: Provide a comprehensive summary of this entire PDF
```

### Technology Intelligence
```
You: What are all the emerging technologies mentioned?
You: auto-summarize
You: auto-classify
```

### Data Extraction
```
You: Extract all cost figures, capacities, and efficiency values
```

### Cross-Document Analysis
```
You: batch-analyze How do these documents compare on technology maturity levels?
```

### Technical Review
```
You: Identify any unclear or outdated technical information
```

---

## Best Practices

### Large PDF Files
- ✅ Program automatically handles large PDFs via chunking
- ✅ For PDFs >100MB, consider splitting into sections first
- ✅ If analysis seems incomplete, ask follow-up questions

### Technology Classification
- ✅ Run `auto-summarize` first, review the TXT, then run `auto-classify`
- ✅ The TXT file is human-readable — review and edit it before classifying if needed
- ✅ Review CSV output to identify extraction confidence
- ✅ Incorrect TRL values can be manually corrected in the CSV
- ✅ Time-specific variants (2030 vs 2050) are automatically separated into rows
- ✅ Re-running `auto-classify` merges new results with the existing CSV

### File Organization
- ✅ PDFs stored in `./pdf_to_analyze/` (auto-created)
- ✅ TXT summaries and CSV exports in `./output/` (auto-created)
- ✅ Use `list` to discover available PDFs
- ✅ Use `analyze` to quickly select PDFs

### Working with Multiple Documents
- ✅ Switch between PDFs using `analyze <filename>`
- ✅ Use `batch-analyze` for cross-document insights
- ✅ No need to restart when switching files
- ✅ Use `current` to verify which PDF is active

### Improving Results
- ✅ Ask **specific questions** rather than vague ones
- ✅ Break complex analysis into **multiple questions**
- ✅ Use follow-up questions to dig **deeper**
- ✅ Provide **context** in your questions

---

## Supported PDF Types

| Type | Support | Notes |
|------|---------|-------|
| **Text-based PDFs** | ✅ Full | Search and copy enabled |
| **Scanned/Image PDFs** | ⚠️ Limited | OCR text extraction only |
| **Password-protected** | ❌ None | Requires password entry |
| **Form PDFs** | ⚠️ Partial | Text-based forms work best |

For scanned PDFs, consider OCR tools like **Tesseract** before uploading.

---

## Troubleshooting

### "Copilot CLI not found"
```bash
# Windows
winget install GitHub.Copilot

# macOS
brew install copilot-cli

# Then authenticate
copilot /login

# Verify
copilot /version
```

### "Cannot extract PDF text"
- ✅ PDF may be image-based or corrupted
- ✅ Open PDF in reader to verify searchable text exists
- ✅ Try uploading a smaller test PDF first
- ✅ Consider using OCR for scanned documents

### "No technologies found in auto-summarize"
- ✅ PDF may not contain technology descriptions
- ✅ Technology keywords might use different terminology
- ✅ Ask Copilot: "What technologies are mentioned?" for verification

### "Please run 'auto-summarize' first"
- ✅ `auto-classify` requires a TXT file produced by `auto-summarize`
- ✅ Run `auto-summarize` on the currently loaded PDF first
- ✅ The TXT file is expected at `output/<pdfname>.txt`

### "CSV file not created"
- ✅ Check that `output/` folder can be created
- ✅ Verify disk space is available
- ✅ Ensure file permissions allow writing to directory
- ✅ Output folder is created automatically if missing

### "Session timeout - service not responding"
- ✅ Maximum wait time is **15 minutes** per request
- ✅ Exit and restart the program to create new session
- ✅ Check internet connection
- ✅ Verify Copilot CLI authentication is current

### "Token limit exceeded"
- ✅ Program automatically chunks PDFs to prevent this
- ✅ Try selecting a specific section to analyze
- ✅ Use models with higher token limits (Claude Sonnet 4)
- ✅ Break large analyses into multiple smaller questions

### "File not found when loading"
- ✅ Use `list` to see exactly what PDFs are available
- ✅ Ensure PDF is in `pdf_to_analyze/` folder
- ✅ Use just the filename: `analyze document.pdf` (not full path)
- ✅ Check for special characters in filename

---

## Technical Details

### PDF Chunking Algorithm

For large PDFs, text is intelligently split:

```
50-page PDF (500KB text)
    ↓
Split into 30KB chunks
    ↓
16-17 chunks created
    ↓
Each chunk analyzed separately
    ↓
Results combined for complete analysis
```

### Auto-Classification Process

```
PDF Input
    ↓
auto-summarize:
  Stage 1 – Find unique technology names
    ↓
  Stage 2 – Extract detailed summaries (batched, by year)
    ↓
  TXT Output (human-reviewable)
    ↓
auto-classify:
  Read TXT → Parse technology sections
    ↓
  Convert each batch to structured JSON via Copilot
    ↓
  Validate, merge by technology+year, de-duplicate
    ↓
  CSV Export
```

### File Structure

```
CopilotSDK_techClass/
├── Program.cs                    (Main entry point & interactive loop)
├── Commands.cs                   (Command handlers)
├── PdfAnalyzer.cs               (PDF extraction & chunking)
├── TechnologyClassification.cs   (Data model, classifier, prompt builders, CSV I/O)
├── TestApp.csproj               (Project file)
├── README.md                    (This file)
├── pdf_to_analyze/              (PDF storage – auto-created)
│   └── your_documents.pdf
└── output/                      (TXT summaries & CSV exports – auto-created)
    ├── document.txt
    └── document_classification.csv
```

---

## Error Handling & Improvements

The application includes **robust error handling**:

- ✅ **Input Validation** - All user inputs validated before processing
- ✅ **File System Safety** - Checks for file existence, permissions, disk space
- ✅ **Service Timeouts** - 5-minute timeout on AI service calls prevents hangs
- ✅ **Resource Cleanup** - Proper disposal of all resources and connections
- ✅ **PDF Error Recovery** - Single-page failures don't crash entire extraction
- ✅ **Data Validation** - Minimum field requirements prevent incomplete records
- ✅ **User-Friendly Errors** - Clear, actionable error messages

---

## Performance Considerations

| Operation | Typical Duration |
|-----------|------------------|
| PDF Upload | < 1 second |
| Text Extraction (20 pages) | 2-5 seconds |
| Single Question Analysis | 5-15 seconds |
| Auto-Summarize (35 techs) | 2-5 minutes |
| Auto-Classify (35 techs) | 1-3 minutes |
| Batch Analysis (3 PDFs) | 20-45 seconds |

---

## Tips & Tricks

💡 **Two-step workflow** - Run `auto-summarize` first, review the TXT, then `auto-classify` for best results

💡 **Edit the TXT** - You can manually edit the TXT file before classifying to fix errors or add data

💡 **Smart Commands** - Use `batch-analyze` to compare findings across multiple documents

💡 **Specific Prompts** - "Extract all aluminum production technologies" is better than "tell me about technologies"

💡 **Follow-ups** - Ask clarifying questions to dive deeper into specific topics

💡 **CSV Workflows** - Export classifications and filter in Excel for further analysis

💡 **Folder Organization** - Keep related PDFs in same folder for batch analysis

💡 **Cost Tracking** - CSV exports include capex/opex data for technology cost analysis

---

## Next Steps

1. ✅ Install GitHub Copilot CLI and authenticate
2. ✅ Start the program with `dotnet run`
3. ✅ Try interactive chat first
4. ✅ Upload a sample PDF using `upload <path>`
5. ✅ Ask questions about the PDF
6. ✅ Try `auto-summarize` on a technical document
7. ✅ Review the TXT output, then run `auto-classify`
8. ✅ Experiment with `batch-analyze` on multiple PDFs

---

## Limitations & Known Issues

- **Large PDFs** (>100MB) may take time to process
- **Scanned PDFs** require OCR pre-processing
- **Password-protected PDFs** not supported
- **Classification accuracy** depends on technology naming conventions in PDF
- **Token limits** may affect analysis of extremely large documents

---

## Support & Issues

If you encounter issues:

1. ✅ Check **Troubleshooting** section above
2. ✅ Verify Copilot CLI is installed: `copilot /version`
3. ✅ Confirm authentication: `copilot /login`
4. ✅ Test with a simple, small PDF first
5. ✅ Check that PDF is not corrupted or empty

---

## License

This tool is provided as-is for use with GitHub Copilot SDK.

---

## Version History

- **v0.1** - First working version, Two-step workflow: `auto-summarize` (PDF→TXT) + `auto-classify` (TXT→CSV), improved prompts, batch retry logic
- **v0.0** - Created first template and commands with auto-classification, batch analysis, improved error handling
- **----** - Initial release with chat and PDF analysis features

---

Enjoy using the Smart Document Analysis Tool! 🎉
