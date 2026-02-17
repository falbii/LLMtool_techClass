using GitHub.Copilot.SDK;
using PdfAnalysisApp;

namespace PdfAnalysisApp;

public static class Commands
{
    /// <summary>
    /// Displays all available PDFs in the folder.
    /// </summary>
    public static async Task HandleListPdfsAsync(string pdfFolder)
    {
        var pdfs = Directory.GetFiles(pdfFolder, "*.pdf");
        if (pdfs.Length == 0)
        {
            Console.WriteLine("📁 No PDFs found in pdf_to_analyze folder.");
            return;
        }
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📁 Available PDFs:");
        Console.ResetColor();
        for (int i = 0; i < pdfs.Length; i++)
        {
            var fileInfo = new FileInfo(pdfs[i]);
            var sizeKb = fileInfo.Length / 1024;
            Console.WriteLine($"   {i + 1}. {Path.GetFileName(pdfs[i])} ({sizeKb} KB)");
        }
    }

    /// <summary>
    /// Displays the current PDF file being analyzed.
    /// </summary>
    public static void HandleCurrentCommand(string? currentPdfFile)
    {
        if (currentPdfFile != null)
            Console.WriteLine($"📄 Current PDF: {Path.GetFileName(currentPdfFile)}");
        else
            Console.WriteLine("❌ No PDF loaded. Use 'upload' or 'analyze' to select one.");
    }

    /// <summary>
    /// Uploads a PDF file to the pdf_to_analyze folder.
    /// </summary>
    public static async Task<string?> HandleUploadPdfAsync(string sourceFile, string pdfFolder)
    {
        try
        {
            await Program.RunWithSpinnerAsync($" Uploading {Path.GetFileName(sourceFile)}", async () =>
            {
                string destFile = Path.Combine(pdfFolder, Path.GetFileName(sourceFile));
                await Task.Run(() => File.Copy(sourceFile, destFile, true));
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded: {Path.GetFileName(sourceFile)}");
            Console.ResetColor();
            return Path.Combine(pdfFolder, Path.GetFileName(sourceFile));
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error uploading PDF: {ex.Message}");
            Console.ResetColor();
            return null;
        }
    }

    /// <summary>
    /// Loads a PDF file for analysis.
    /// Accepts: full filename, filename without extension, or list number.
    /// </summary>
    public static string? HandleAnalyzeCommand(string input, string pdfFolder)
    {
        var pdfFiles = Directory.GetFiles(pdfFolder, "*.pdf")
            .OrderBy(f => Path.GetFileName(f))
            .ToArray();

        if (pdfFiles.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ No PDFs found in pdf_to_analyze folder.");
            Console.ResetColor();
            return null;
        }

        string? selectedFile = null;

        // Try to parse as number (1-indexed list position)
        if (int.TryParse(input, out int listNumber) && listNumber >= 1 && listNumber <= pdfFiles.Length)
        {
            selectedFile = pdfFiles[listNumber - 1];
        }
        else
        {
            // Try exact match with .pdf extension
            selectedFile = pdfFiles.FirstOrDefault(f => 
                Path.GetFileName(f).Equals(input, StringComparison.OrdinalIgnoreCase));

            // If not found, try matching without extension
            if (selectedFile == null)
            {
                selectedFile = pdfFiles.FirstOrDefault(f => 
                    Path.GetFileNameWithoutExtension(f).Equals(input, StringComparison.OrdinalIgnoreCase));
            }
        }

        if (selectedFile != null && File.Exists(selectedFile))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded: {Path.GetFileName(selectedFile)}");
            Console.ResetColor();
            return selectedFile;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ PDF '{input}' not found. Use 'list' to see available PDFs.");
            Console.ResetColor();
            return null;
        }
    }

    /// <summary>
    /// Prepares a message with PDF context for analysis.
    /// </summary>
    public static async Task<string> PrepareMessageWithPdfContextAsync(string pdfFile, string userQuestion)
    {
        try
        {
            var pdfText = await PdfAnalyzer.ExtractTextFromPdfAsync(pdfFile);
            var chunks = PdfAnalyzer.ChunkPdfContent(pdfText);
            
            var prompt = PdfAnalyzer.BuildAnalysisPrompt(chunks, userQuestion);
            return prompt;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Could not extract PDF text: {ex.Message}. Sending question without PDF context.");
            Console.ResetColor();
            return userQuestion;
        }
    }

    /// <summary>
    /// Handles batch analysis of all PDFs in the folder.
    /// </summary>
    public static async Task HandleBatchAnalyzeAsync(CopilotSession session, string pdfFolder, string question)
    {
        var pdfFiles = Directory.GetFiles(pdfFolder, "*.pdf");
        if (pdfFiles.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("❌ No PDF files found in pdf_to_analyze folder.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"📊 Batch analyzing {pdfFiles.Length} PDF(s)...\n");
        Console.ResetColor();

        try
        {
            var pdfChunks = new Dictionary<string, List<string>>();
            
            foreach (var pdfFile in pdfFiles)
            {
                var pdfText = await PdfAnalyzer.ExtractTextFromPdfAsync(pdfFile);
                var chunks = PdfAnalyzer.ChunkPdfContent(pdfText);
                pdfChunks[pdfFile] = chunks;
            }

            var prompt = PdfAnalyzer.BuildBatchAnalysisPrompt(pdfChunks, question);
            await Program.SendMessageWithSpinnerAsync(session, prompt);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error extracting PDFs: {ex.Message}");
            Console.ResetColor();
        }
    }
}
