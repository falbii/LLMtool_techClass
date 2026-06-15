using System.Globalization;
using System.Text;

namespace TechClassificationApp;

public static class CommandHandlers
{
    // Diagnostic for issue #2 (LLM condensation is lossy). Measures, with NO LLM call, how many
    // numbers in the raw PDF survive into the cached condensed text. Raw extraction is deterministic
    // iText; the condensed side is read from the existing cache only — never generated here, so the
    // audit can never trigger a model call. Run 'auto-summarize' first to produce the cache.
    // Returns false when the audit could not run (missing PDF/cache or an unexpected error),
    // so non-console callers (the web API) can report failure instead of silent success.
    public static async Task<bool> HandleCondenseCheckAsync(Workspace ws, string pdfFile)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
        {
            ConsoleEx.Error("❌ PDF not found or invalid path.");
            return false;
        }

        var cachePath = PdfCondenser.GetCachePath(pdfFile, ws.CacheDir);
        if (!File.Exists(cachePath))
        {
            ConsoleEx.Warn("⚠️  No cached condensed file found — run 'auto-summarize' first.");
            ConsoleEx.Warn($"    Expected at: {cachePath}");
            ConsoleEx.Dim("    (The audit reads the existing cache only; it never calls the model.)");
            return false;
        }

        ConsoleEx.Info("🔬 Auditing condensation fidelity (raw PDF vs cached condensed) — no LLM used...\n");

        try
        {
            var rawText = await PdfExtractor.ExtractTextAsync(pdfFile);
            var condensedText = await File.ReadAllTextAsync(cachePath, Encoding.UTF8);

            var rawDistinct = CondensedVerifier.ExtractNumbers(rawText).Distinct().ToList();
            var condensedIndex = CondensedVerifier.ExtractNumbers(condensedText);
            var missing = rawDistinct.Where(v => !CondensedVerifier.Contains(condensedIndex, v)).ToList();

            // "Data-like" numbers: non-integers (0.65, 63.5) or magnitude >= 100 (years, capex, ...).
            // Filters out list indices and other trivial small integers that aren't real data points.
            static bool IsSignificant(double v) => v % 1 != 0 || Math.Abs(v) >= 100;
            var sigDistinct = rawDistinct.Where(IsSignificant).ToList();
            var sigMissing = missing.Where(IsSignificant).ToList();

            int total = rawDistinct.Count, covered = total - missing.Count;
            int sigTotal = sigDistinct.Count, sigCovered = sigTotal - sigMissing.Count;
            double pct = total == 0 ? 100 : 100.0 * covered / total;
            double sigPct = sigTotal == 0 ? 100 : 100.0 * sigCovered / sigTotal;

            ConsoleEx.Info($"   Raw distinct numbers:        {total}");
            ConsoleEx.Info($"   Survived in condensed:       {covered} ({pct:0.#}%)");
            Console.WriteLine();
            ConsoleEx.Info($"   Data-like numbers (≥100 or decimal): {sigTotal}");
            var sigLine = $"   ...of which survived:        {sigCovered} ({sigPct:0.#}%)";
            if (sigPct >= 99) ConsoleEx.Success(sigLine); else ConsoleEx.Warn(sigLine);
            ConsoleEx.Warn($"   Data-like numbers MISSING:   {sigMissing.Count}");

            if (sigMissing.Count > 0)
            {
                Console.WriteLine();
                ConsoleEx.Warn("   ⚠️  Data-like numbers in the raw PDF but NOT in the condensed text:");
                foreach (var v in sigMissing.OrderBy(x => x).Take(40))
                    ConsoleEx.Dim($"     • {v.ToString("0.########", CultureInfo.InvariantCulture)}");
                if (sigMissing.Count > 40)
                    ConsoleEx.Dim($"     ...and {sigMissing.Count - 40} more");
            }

            var report = new StringBuilder();
            report.AppendLine($"Condensation fidelity audit — {Path.GetFileName(pdfFile)}");
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Raw distinct numbers: {total} | survived: {covered} ({pct:0.#}%)");
            report.AppendLine($"Data-like numbers: {sigTotal} | survived: {sigCovered} ({sigPct:0.#}%) | missing: {sigMissing.Count}");
            report.AppendLine();
            report.AppendLine("Data-like numbers present in raw PDF but absent from condensed text:");
            foreach (var v in sigMissing.OrderBy(x => x))
                report.AppendLine($"  • {v.ToString("0.########", CultureInfo.InvariantCulture)}");
            var reportPath = Path.Combine(ws.CheckDir, $"{Path.GetFileNameWithoutExtension(pdfFile)}_condense_check.txt");
            await File.WriteAllTextAsync(reportPath, report.ToString(), Encoding.UTF8);

            Console.WriteLine();
            ConsoleEx.Info($"   📁 Full report: {reportPath}");
            ConsoleEx.Dim("   Note: some 'missing' values can be PDF-extraction artifacts (split/garbled spacing),");
            ConsoleEx.Dim("   not true condensation loss — scan the list to judge how many are real data drops.");
            return true;
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Condense audit failed: {ex.Message}");
            return false;
        }
    }

    public static async Task<string?> HandleListPdfsAsync(string pdfInputDirectory)
    {
        if (!Directory.Exists(pdfInputDirectory))
        {
            Console.WriteLine("📁 PDF folder not found or not accessible.");
            return null;
        }

        var pdfs = Directory.GetFiles(pdfInputDirectory, "*.pdf")
            .OrderBy(f => Path.GetFileName(f))
            .ToArray();

        if (pdfs.Length == 0)
        {
            Console.WriteLine("📁 No PDFs found in 1_pdf_to_analyze folder.");
            return null;
        }

        ConsoleEx.Info("📁 Available PDFs:");

        for (int i = 0; i < pdfs.Length; i++)
        {
            var sizeKb = new FileInfo(pdfs[i]).Length / 1024;
            Console.WriteLine($"   {i + 1}. {Path.GetFileName(pdfs[i])} ({sizeKb} KB)");
        }

        Console.WriteLine();
        Console.Write("Enter number or filename to analyze, or press Enter to cancel: ");
        var choice = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(choice))
            return null;

        var selected = ResolvePdfSelection(choice, pdfInputDirectory);
        if (selected != null)
            return selected;

        ConsoleEx.Warn("No valid selection made.");
        return null;
    }

    public static void HandleCurrentCommand(string? selectedPdfPath)
    {
        if (selectedPdfPath != null)
            Console.WriteLine($"📄 Current PDF: {Path.GetFileName(selectedPdfPath)}");
        else
            Console.WriteLine("❌ No PDF loaded. Use 'upload' or 'list' to select one.");
    }

    public static void HandleCommandsCommand()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("⚡ Available Commands:");
        Console.ResetColor();
        Console.WriteLine("  '/commands' or '/help'  - Display all available commands");
        Console.WriteLine("  '/exit' or '/quit'      - Exit the program");
        Console.WriteLine("  '/upload <path>'        - Upload a PDF to analyze (or drop PDFs in ./1_pdf_to_analyze/)");
        Console.WriteLine("  '/list'                 - List available PDFs and choose one to analyze");
        Console.WriteLine("  '/current'              - Show current PDF");
        Console.WriteLine("  '/auto-summarize'       - Extract technology summaries to Markdown");
        Console.WriteLine("  '/auto-classify' (beta) - Classify technologies and export CSV");
        Console.WriteLine("  '/batch-analyze <q>'    - Analyze all PDFs with a question");
        Console.WriteLine("  '/benchmark'            - Compare all models on the Allgoewer paper");
        Console.WriteLine("  '/condense-check'       - Check the quality of md condensed");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("💡 Tips:");
        Console.ResetColor();
        Console.WriteLine("  • Or just ask a question normally for AI analysis");
        Console.WriteLine("  • Commands also work without the leading '/'");
        if (Directory.Exists("./1_pdf_to_analyze"))
            Console.WriteLine("  • Drop PDFs in the ./1_pdf_to_analyze/ folder for quick access");
        Console.WriteLine();
    }

    public static async Task<string?> HandleUploadPdfAsync(string sourceFile, string pdfInputDirectory)
    {
        if (!File.Exists(sourceFile))
        {
            ConsoleEx.Error("❌ Source file not found.");
            return null;
        }

        try
        {
            var destFile = Path.Combine(pdfInputDirectory, Path.GetFileName(sourceFile));

            await Program.RunWithSpinnerAsync($" Uploading {Path.GetFileName(sourceFile)}", async () =>
            {
                await Task.Run(() => File.Copy(sourceFile, destFile, true));
            });

            ConsoleEx.Success($"Loaded: {Path.GetFileName(sourceFile)}");
            return destFile;
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Error uploading PDF: {ex.Message}");
            return null;
        }
    }

    public static string? ResolvePdfSelection(string input, string pdfInputDirectory)
    {
        if (!Directory.Exists(pdfInputDirectory) || string.IsNullOrEmpty(input))
        {
            ConsoleEx.Error("❌ Invalid input or folder.");
            return null;
        }

        var pdfFiles = Directory.GetFiles(pdfInputDirectory, "*.pdf")
            .OrderBy(f => Path.GetFileName(f))
            .ToArray();

        if (pdfFiles.Length == 0)
        {
            ConsoleEx.Error("❌ No PDFs found in 1_pdf_to_analyze folder.");
            return null;
        }

        string? selectedFile = null;

        if (int.TryParse(input, out int listNumber) && listNumber >= 1 && listNumber <= pdfFiles.Length)
        {
            selectedFile = pdfFiles[listNumber - 1];
        }
        else
        {
            selectedFile = pdfFiles.FirstOrDefault(f =>
                Path.GetFileName(f).Equals(input, StringComparison.OrdinalIgnoreCase));

            selectedFile ??= pdfFiles.FirstOrDefault(f =>
                Path.GetFileNameWithoutExtension(f).Equals(input, StringComparison.OrdinalIgnoreCase));
        }

        if (selectedFile != null && File.Exists(selectedFile))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded: {Path.GetFileName(selectedFile)}");
            Console.ResetColor();
            return selectedFile;
        }

        ConsoleEx.Error($"❌ PDF '{input}' not found. Use 'list' to see available PDFs.");
        return null;
    }

    // PdfIncluded tells the caller whether the document text actually made it into the prompt,
    // so it can track what context the session has already received.
    public static async Task<(string Prompt, bool PdfIncluded)> BuildPromptWithPdfContextAsync(
        Workspace ws, string pdfFile, string userQuestion)
    {
        try
        {
            var pdfText = await PdfCondenser.GetCondensedTextAsync(ws, pdfFile);
            var chunks = PdfExtractor.SplitIntoChunks(pdfText);
            return (PdfExtractor.BuildSingleDocumentPrompt(chunks, userQuestion), true);
        }
        catch (Exception ex)
        {
            ConsoleEx.Warn($"⚠️  Could not extract PDF text: {ex.Message}. Sending question without PDF context.");
            return (userQuestion, false);
        }
    }

    public static async Task HandleBatchAnalyzeAsync(
        Workspace ws, IChatSession session, string question)
    {
        if (!Directory.Exists(ws.PdfDir))
        {
            ConsoleEx.Error("❌ PDF folder not found.");
            return;
        }

        var pdfFiles = Directory.GetFiles(ws.PdfDir, "*.pdf");
        if (pdfFiles.Length == 0)
        {
            ConsoleEx.Warn("❌ No PDF files found in 1_pdf_to_analyze folder.");
            return;
        }

        ConsoleEx.Info($"📊 Batch analyzing {pdfFiles.Length} PDF(s)...\n");

        try
        {
            var pdfChunks = new Dictionary<string, List<string>>();
            foreach (var pdfFile in pdfFiles)
            {
                var pdfText = await PdfCondenser.GetCondensedTextAsync(ws, pdfFile);
                pdfChunks[pdfFile] = PdfExtractor.SplitIntoChunks(pdfText);
            }

            var prompt = PdfExtractor.BuildMultiDocumentPrompt(pdfChunks, question);
            await Program.SendMessageWithSpinnerAsync(session, prompt);
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Error extracting PDFs: {ex.Message}");
        }
    }
}
