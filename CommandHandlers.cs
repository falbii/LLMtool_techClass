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

            var rawDistinct = GroundingVerifier.ExtractNumbers(rawText).Distinct().ToList();
            var condensedIndex = GroundingVerifier.ExtractNumbers(condensedText);
            var missing = rawDistinct.Where(v => !GroundingVerifier.Contains(condensedIndex, v)).ToList();

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
            var reportPath = Path.Combine(ws.CsvDir, $"{Path.GetFileNameWithoutExtension(pdfFile)}_condense_check.txt");
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


    private const string BenchmarkPdfName = "Allgoewer_2024.pdf";

    private const string BenchmarkPrompt =
        "Based on this paper, list the main low-carbon hydrogen production technologies discussed. " +
        "For each technology provide: (1) Technology Readiness Level (TRL) and year, " +
        "(2) production cost range (in USD/kg H2), (3) key efficiency metric, and (4) extra: if possible, provide CAPEX, OPEX, input and output ratios, and lifetime. " +
        "Be concise and use a structured format.";

    // Returns false when the benchmark could not run at all (missing PDF, no models);
    // per-model errors are reported in the results table and still count as a run.
    public static async Task<bool> HandleBenchmarkAsync(Workspace ws)
    {
        var pdfPath = Path.Combine(ws.PdfDir, BenchmarkPdfName);
        if (!File.Exists(pdfPath))
        {
            ConsoleEx.Error($"❌ Benchmark PDF not found. Place it at: {pdfPath}");
            return false;
        }

        string fullPrompt = string.Empty;
        await Program.RunWithSpinnerAsync(" Extracting PDF text", async () =>
        {
            (fullPrompt, _) = await BuildPromptWithPdfContextAsync(ws, pdfPath, BenchmarkPrompt);
        });

        var modelsWithInfo = await ws.Client.ListModelsAsync();
        if (modelsWithInfo == null || modelsWithInfo.Count == 0)
        {
            ConsoleEx.Error("❌ No models available.");
            return false;
        }

        Console.WriteLine();
        ConsoleEx.Info($"🏁 Benchmarking {modelsWithInfo.Count} models on: {BenchmarkPdfName}");
        Console.WriteLine();

        var results = new List<(string Model, long LatencyMs, int WordCount, string Response, string Status)>();

        foreach (var modelInfo in modelsWithInfo)
        {
            ConsoleEx.Dim($"   ▶ {modelInfo.Id}…");

            IChatSession? benchSession = null;
            try
            {
                benchSession = await Sessions.NewAsync(ws.Client, modelInfo.Id);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var response = await Program.SendMessageAndCollectResponseSilentAsync(benchSession, fullPrompt);
                sw.Stop();

                var words = response.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                results.Add((modelInfo.Id, sw.ElapsedMilliseconds, words, response, "OK"));

                ConsoleEx.Success($"   ✓ {modelInfo.Id,-35}  {sw.ElapsedMilliseconds,6} ms  {words,5} words");
            }
            catch (Exception ex)
            {
                results.Add((modelInfo.Id, 0, 0, string.Empty, $"ERROR: {ex.Message}"));
                ConsoleEx.Error($"   ❌ {modelInfo.Id,-35}  {ex.Message}");
            }
            finally
            {
                if (benchSession != null)
                    await benchSession.DisposeAsync();
            }
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        Console.WriteLine();
        ConsoleEx.Warn("📊 Benchmark Results:");
        ConsoleEx.Dim($"   {"Model",-35} {"Latency (ms)",13}  {"Words",6}  Status");
        ConsoleEx.Dim($"   {"─────────────────────────────────",35} {"─────────────",13}  {"─────",6}  ──────");

        foreach (var (model, latencyMs, wordCount, _, status) in results)
            ConsoleEx.Plain($"   {model,-35} {latencyMs,13}  {wordCount,6}  {status}");

        Console.WriteLine();
        ConsoleEx.Info("🔬 Auto-classifying each model's response...");
        Console.WriteLine();

        var classifyResults = new Dictionary<string, (int RowCount, string Status)>(StringComparer.OrdinalIgnoreCase);
        var allClassifiedRows = new List<(string Model, List<TechnologyRecord> Rows)>();

        foreach (var r in results.Where(r => r.Status == "OK"))
        {
            ConsoleEx.Dim($"   ▶ {r.Model}…");

            try
            {
                var sections = new List<(string Name, string Content)>
                {
                    ("Benchmark Response", r.Response)
                };

                await using var classifySession = await Sessions.NewAsync(ws.Client, r.Model);

                var classifyPrompt = TechnologyClassifier.BuildClassificationFromSummaryPrompt(sections);
                var jsonResponse = await Program.SendMessageAndCollectResponseSilentAsync(classifySession, classifyPrompt);
                var json = TechnologyClassifier.ExtractJson(jsonResponse);

                if (!TechnologyClassifier.IsValidJsonArray(json))
                {
                    jsonResponse = await Program.SendMessageAndCollectResponseSilentAsync(classifySession, classifyPrompt);
                    json = TechnologyClassifier.ExtractJson(jsonResponse);
                }

                if (!TechnologyClassifier.IsValidJsonArray(json))
                {
                    classifyResults[r.Model] = (0, "No valid JSON");
                    ConsoleEx.Warn($"   ⚠️ {r.Model,-35}  no valid JSON returned");
                    continue;
                }

                var rows = TechnologyClassifier.ParseRowsFromJson(json);
                var classifications = new List<TechnologyRecord>();
                var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var row in rows)
                {
                    var classification = TechnologyClassifier.ParseRecord(row, out _);
                    if (string.IsNullOrWhiteSpace(classification.DatapaperTechId))
                        classification.DatapaperTechId = TechnologyClassifier.GenerateTechId(classification, usedIds);
                    usedIds.Add(classification.DatapaperTechId);
                    classifications.Add(classification);
                }

                var meaningful = classifications.Where(TechnologyClassifier.HasMeaningfulData).ToList();
                var merged = TechnologyClassifier.MergeByTechnologyAndYear(meaningful);

                allClassifiedRows.Add((r.Model, merged));
                classifyResults[r.Model] = (merged.Count, "OK");
                ConsoleEx.Success($"   ✓ {r.Model,-35}  {merged.Count,3} rows");
            }
            catch (Exception ex)
            {
                classifyResults[r.Model] = (0, $"ERROR: {ex.Message}");
                ConsoleEx.Error($"   ❌ {r.Model,-35}  {ex.Message}");
            }
        }

        string? classificationCsvPath = null;
        if (allClassifiedRows.Count > 0)
        {
            classificationCsvPath = Path.Combine(ws.CsvDir, $"benchmark_{timestamp}_classification.csv");
            var combinedCsv = new StringBuilder();
            bool headerWritten = false;

            foreach (var (model, rows) in allClassifiedRows)
            {
                var tempPath = Path.GetTempFileName();
                try
                {
                    TechnologyCsv.WriteCsv(tempPath, rows);
                    var lines = await File.ReadAllLinesAsync(tempPath, Encoding.UTF8);

                    if (!headerWritten)
                    {
                        combinedCsv.AppendLine("Model," + lines[0]);
                        headerWritten = true;
                    }

                    var escapedModel = model.Contains(',') || model.Contains('"')
                        ? $"\"{model.Replace("\"", "\"\"\"")}\"" : model;

                    foreach (var line in lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)))
                        combinedCsv.AppendLine($"{escapedModel},{line}");
                }
                finally
                {
                    File.Delete(tempPath);
                }
            }

            await File.WriteAllTextAsync(classificationCsvPath, combinedCsv.ToString(), Encoding.UTF8);
        }

        if (classifyResults.Count > 0)
        {
            Console.WriteLine();
            ConsoleEx.Warn("📋 Classification Comparison:");
            ConsoleEx.Dim($"   {"Model",-35}  {"Rows",5}  Classification Status");
            ConsoleEx.Dim($"   {"─────────────────────────────────",35}  {"─────",5}  ────────────────────");
            foreach (var kvp in classifyResults)
            {
                var rowsStr = kvp.Value.Status == "OK" ? kvp.Value.RowCount.ToString() : "-";
                ConsoleEx.Plain($"   {kvp.Key,-35}  {rowsStr,5}  {kvp.Value.Status}");
            }
            Console.WriteLine();
        }

        var csvPath = Path.Combine(ws.CsvDir, $"benchmark_{timestamp}.csv");
        var csv = new StringBuilder();
        csv.AppendLine("Model,LatencyMs,WordCount,ClassifiedRows,Status");
        foreach (var (model, latencyMs, wordCount, _, status) in results)
        {
            var classifiedRows = classifyResults.TryGetValue(model, out var cr) && cr.Status == "OK"
                ? cr.RowCount.ToString()
                : "N/A";
            csv.AppendLine($"{model},{latencyMs},{wordCount},{classifiedRows},{status}");
        }
        await File.WriteAllTextAsync(csvPath, csv.ToString(), Encoding.UTF8);

        var txtPath = Path.Combine(ws.MdDir, $"benchmark_{timestamp}.txt");
        var txt = new StringBuilder();
        txt.AppendLine("═══════════════════════════════════════════════════════════════");
        txt.AppendLine($"Benchmark Responses - {BenchmarkPdfName}");
        txt.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        txt.AppendLine($"Prompt: {BenchmarkPrompt}");
        txt.AppendLine("═══════════════════════════════════════════════════════════════");
        txt.AppendLine();
        foreach (var r in results)
        {
            txt.AppendLine($"═══ MODEL: {r.Model} ═══");
            txt.AppendLine($"Status: {r.Status}  |  Latency: {r.LatencyMs} ms  |  Words: {r.WordCount}");
            txt.AppendLine();
            txt.AppendLine(r.Status == "OK" ? r.Response : $"(no response — {r.Status})");
            txt.AppendLine();
            txt.AppendLine("───────────────────────────────────────────────────────────────");
            txt.AppendLine();
        }
        await File.WriteAllTextAsync(txtPath, txt.ToString(), Encoding.UTF8);

        ConsoleEx.Info($"💾 Results saved → {csvPath}");
        ConsoleEx.Info($"💾 Responses saved → {txtPath}");
        if (classificationCsvPath != null)
            ConsoleEx.Info($"💾 Classification saved → {classificationCsvPath}");
        Console.WriteLine();
        return true;
    }
}
