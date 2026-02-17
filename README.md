# How to Use This Program

A complete guide to using the PDF Analysis and Upload features with the GitHub Copilot SDK for .NET.

---

## Quick Start (2 Minutes)

### Prerequisites
- GitHub Copilot CLI installed: `winget install GitHub.Copilot` (Windows) or `brew install copilot-cli` (macOS)
- Authenticated with Copilot: Run `copilot /login`
- .NET 8.0+ installed

### Setup
```bash
cd CopilotSDK_techClass
dotnet restore
dotnet run
```

---

## Features Overview
This program provides three main features:

1. **Interactive Chat** - Ask questions and get responses from Copilot
2. **PDF Upload** - Add PDFs to analyze
3. **PDF Analysis** - Ask questions about your PDFs in context

---

## Feature 1: Interactive Chat

Once the program starts, you can chat with Copilot just like any conversational AI:

```
You: What is machine learning?
Copilot: [Provides detailed explanation]

You: Can you give me a Python example?
Copilot: [Provides Python code example]
```

Type `exit` or `quit` to quit the program.

---

## Feature 2: PDF Upload & Management

### Upload a PDF

```
You: upload C:\path\to\document.pdf
```

The PDF is copied to the `pdf_to_analyze/` folder and automatically loaded.

### Available Commands

| Command | Description |
|---------|-------------|
| `list` | Show all PDFs in the `pdf_to_analyze/` folder with file sizes |
| `current` | Show which PDF is currently loaded |
| `upload <path>` | Copy a PDF to the `pdf_to_analyze/` folder and load it |
| `load <filename>` | Load a specific PDF from the folder |
| `exit` or `quit` | Exit the program |

### Example Workflow

```
You: list
📁 Available PDFs:
   1. report.pdf (245 KB)
   2. manual.pdf (512 KB)

You: load report.pdf
✓ Loaded: report.pdf

You: current
📄 Current PDF: report.pdf
```

---

## Feature 3: PDF Analysis

Once you have a PDF loaded, ask questions about it and Copilot will analyze it in context.

### How It Works

1. **Text Extraction** - Extracts all text from the PDF
2. **Smart Chunking** - Splits large PDFs into 30KB chunks to stay within token limits
3. **Context Injection** - Your question is combined with the PDF content
4. **Analysis** - Copilot analyzes the PDF in the context of your question

### Example Questions

```
You: What are the main findings?
Copilot: [Analyzes and responds with findings]

You: Summarize this document in 3 bullet points
Copilot: [Provides summary]

You: Extract all dates and names mentioned
Copilot: [Lists extracted information]

You: Check for grammatical errors
Copilot: [Reviews and provides feedback]
```

---

## Complete Workflow Example

```
You: list
📁 Available PDFs:
   [none yet]

You: upload C:\Documents\research_paper.pdf
✓ Loaded: research_paper.pdf (125 KB)

You: What are the key conclusions?
Copilot: Based on the research paper, the key conclusions are:
1. [Conclusion 1]
2. [Conclusion 2]
3. [Conclusion 3]

You: current
📄 Current PDF: research_paper.pdf

You: load another_document.pdf
✓ Loaded: another_document.pdf

You: Summarize this one
Copilot: [Provides summary of the new document]

You: exit
```

---

## Use Cases

### Document Summarization
```
You: Summarize this PDF in 5 bullet points
```

### Content Analysis
```
You: What are the main topics discussed in this document?
```

### Data Extraction
```
You: Extract all dates, prices, and contact information
```

### Quality Review
```
You: Check for grammar, clarity, and completeness
```

### Technical Documentation Review
```
You: Explain the key technical concepts in this manual
```

---

## Best Practices

### Large PDF Files
- The program automatically handles large PDFs by splitting them into chunks
- If you get an error, try uploading a smaller PDF first
- If analysis seems incomplete, ask follow-up questions

### File Organization
- PDFs are stored in `./pdf_to_analyze/` folder (created automatically)
- Use `list` to see all available PDFs
- Use `load` to switch between PDFs without restarting

### Working with Multiple PDFs
- Switch between PDFs using `load <filename>`
- Each PDF can be analyzed independently
- No need to restart the program when switching files

### Improving Results
- Ask specific questions rather than vague ones
- Break complex analyses into multiple questions
- For long documents, ask about specific sections

---

## Supported PDF Types

- ✅ **Text-based PDFs** - Fully supported (search + copy enabled)
- ⚠️ **Scanned/Image PDFs** - Partially supported (will extract OCR'd text if available)
- ❌ **Password-protected PDFs** - Not supported (requires password handling)

For scanned PDFs, consider using OCR tools like Tesseract before uploading.

---

## Troubleshooting

### "Copilot CLI not found"
- **Install:** `winget install GitHub.Copilot`
- **Authenticate:** Run `copilot /login`
- **Verify:** Run `copilot /version`

### "No text extracted from PDF"
- PDF may be image-based (scanned document)
- Try opening the PDF in your reader to confirm it has searchable text
- Consider using OCR first

### "Token limit exceeded"
- The program automatically chunks PDFs to prevent this
- If it still happens, try analyzing smaller sections
- Use a model with higher token limits (via model selector)

### "File not found" when loading
- Use `list` to see available PDFs
- Ensure the PDF is in the `pdf_to_analyze/` folder
- Use just the filename with `load`, not the full path
- Example: `load document.pdf` (not `load C:\path\to\document.pdf`)

### "Session timeout"
- Copilot may disconnect after inactivity
- Re-create the session by typing `exit` and restarting the program
- If the issue persists, check your Copilot CLI authentication

---

## Technical Details

### How PDF Chunking Works

For a 50-page PDF (500KB text):
1. Extracted as one large string
2. Split into chunks of ~30KB each = 16-17 chunks
3. Each chunk is processed separately to stay within token limits
4. Results are combined for analysis

### File Structure

```
TestApp/
├── Program.cs          (Main program)
├── PdfAnalyzer.cs      (PDF analysis logic)
├── Commands.cs         (Command handlers)
├── pdf_to_analyze/     (PDF storage folder - auto-created)
└── HOW_TO_USE.md       (This file)
```

---

## Tips & Tricks

💡 **Organize Your PDFs** - Keep PDFs in the `pdf_to_analyze/` folder for easy access

💡 **Use Specific Prompts** - "Analyze the financial data" works better than "tell me about this PDF"

💡 **Ask Follow-ups** - After getting a response, ask clarifying questions to dig deeper

💡 **Check Current PDF** - Use `current` if you forget which PDF is loaded

💡 **Batch Analyze** - Switch between PDFs quickly with `load` to compare documents

---

## Next Steps

1. ✅ Start the program and chat with Copilot
2. ✅ Upload a sample PDF using `upload <path>`
3. ✅ Ask questions about the PDF content
4. ✅ Experiment with different questions and document types
5. ✅ Try switching between multiple PDFs

---

## Getting Help

If you encounter issues:
1. Check the **Troubleshooting** section above
2. Verify Copilot CLI is installed and authenticated
3. Try with a smaller, simple PDF first
4. Check that your PDF is not corrupted or password-protected

Enjoy using the program! 🎉
