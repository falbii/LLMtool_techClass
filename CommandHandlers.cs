using System.Text;
using GitHub.Copilot.SDK;
using Refractored.GitHub.Copilot.SDK.Helpers;

namespace TechClassificationApp;

public static class CommandHandlers
{
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
            Console.WriteLine("📁 No PDFs found in pdf_to_analyze folder.");
            return null;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📁 Available PDFs:");
        Console.ResetColor();

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

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("No valid selection made.");
        Console.ResetColor();
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
        Console.WriteLine("  'commands' or 'help'   - Display all available commands");
        Console.WriteLine("  'exit' or 'quit'       - Exit the program");
        Console.WriteLine("  'upload <path>'        - Upload a PDF to analyze (or drop PDFs in ./pdf_to_analyze/)");
        Console.WriteLine("  'list'                 - List available PDFs and choose one to analyze");
        Console.WriteLine("  'current'              - Show current PDF");
        Console.WriteLine("  'auto-summarize'       - Extract technology summaries to TXT");
        Console.WriteLine("  'auto-classify' (beta) - Classify technologies and export CSV");
        Console.WriteLine("  'batch-analyze <q>'    - Analyze all PDFs with a question");
        Console.WriteLine("  'benchmark'            - Compare all models on the Allgoewer paper");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("💡 Tips:");
        Console.ResetColor();
        Console.WriteLine("  • Or just ask a question normally for AI analysis");
        if (Directory.Exists("./pdf_to_analyze"))
            Console.WriteLine("  • Drop PDFs in the ./pdf_to_analyze/ folder for quick access");
        Console.WriteLine();
    }

    public static async Task<string?> HandleUploadPdfAsync(string sourceFile, string pdfInputDirectory)
    {
        if (!File.Exists(sourceFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Source file not found.");
            Console.ResetColor();
            return null;
        }

        try
        {
            var destFile = Path.Combine(pdfInputDirectory, Path.GetFileName(sourceFile));

            await Program.RunWithSpinnerAsync($" Uploading {Path.GetFileName(sourceFile)}", async () =>
            {
                await Task.Run(() => File.Copy(sourceFile, destFile, true));
            });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded: {Path.GetFileName(sourceFile)}");
            Console.ResetColor();
            return destFile;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error uploading PDF: {ex.Message}");
            Console.ResetColor();
            return null;
        }
    }

    public static string? ResolvePdfSelection(string input, string pdfInputDirectory)
    {
        if (!Directory.Exists(pdfInputDirectory) || string.IsNullOrEmpty(input))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Invalid input or folder.");
            Console.ResetColor();
            return null;
        }

        var pdfFiles = Directory.GetFiles(pdfInputDirectory, "*.pdf")
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

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ PDF '{input}' not found. Use 'list' to see available PDFs.");
        Console.ResetColor();
        return null;
    }

    public static async Task<string> BuildPromptWithPdfContextAsync(string pdfFile, string userQuestion)
    {
        try
        {
            var pdfText = await PdfExtractor.ExtractTextAsync(pdfFile);
            var chunks = PdfExtractor.SplitIntoChunks(pdfText);
            return PdfExtractor.BuildSingleDocumentPrompt(chunks, userQuestion);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Could not extract PDF text: {ex.Message}. Sending question without PDF context.");
            Console.ResetColor();
            return userQuestion;
        }
    }

    public static async Task HandleBatchAnalyzeAsync(CopilotSession session, string pdfInputDirectory, string question)
    {
        if (!Directory.Exists(pdfInputDirectory))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ PDF folder not found.");
            Console.ResetColor();
            return;
        }

        var pdfFiles = Directory.GetFiles(pdfInputDirectory, "*.pdf");
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
                var pdfText = await PdfExtractor.ExtractTextAsync(pdfFile);
                pdfChunks[pdfFile] = PdfExtractor.SplitIntoChunks(pdfText);
            }

            var prompt = PdfExtractor.BuildMultiDocumentPrompt(pdfChunks, question);
            await Program.SendMessageWithSpinnerAsync(session, prompt);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error extracting PDFs: {ex.Message}");
            Console.ResetColor();
        }
    }


    private const string BenchmarkPdfName = "Allgoewer_2024.pdf";

    private const string BenchmarkPrompt =
        "Based on this paper, list the main low-carbon hydrogen production technologies discussed. " +
        "For each technology provide: (1) Technology Readiness Level (TRL) and year, " +
        "(2) production cost range (in USD/kg H2), (3) key efficiency metric, and (4) extra: if possible, provide CAPEX, OPEX, input and output ratios, and lifetime. " +
        "Be concise and use a structured format.";

    public static async Task HandleBenchmarkAsync(
        CopilotClient client, string pdfInputDirectory, string outputDirectory)
    {
        var pdfPath = Path.Combine(pdfInputDirectory, BenchmarkPdfName);
        if (!File.Exists(pdfPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Benchmark PDF not found. Place it at: {pdfPath}");
            Console.ResetColor();
            return;
        }

        string fullPrompt = string.Empty;
        await Program.RunWithSpinnerAsync(" Extracting PDF text", async () =>
        {
            fullPrompt = await BuildPromptWithPdfContextAsync(pdfPath, BenchmarkPrompt);
        });

        var modelsWithInfo = await client.ListModelsAsync();
        if (modelsWithInfo == null || modelsWithInfo.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ No models available.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"🏁 Benchmarking {modelsWithInfo.Count} models on: {BenchmarkPdfName}");
        Console.ResetColor();
        Console.WriteLine();

        var results = new List<(string Model, long LatencyMs, int WordCount, string Response, string Status)>();

        foreach (var modelInfo in modelsWithInfo)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"   ▶ {modelInfo.Id,-35}");
            Console.ResetColor();

            CopilotSession? benchSession = null;
            try
            {
                benchSession = await client.CreateSessionAsync(new SessionConfig
                {
                    Model = modelInfo.Id,
                    Streaming = true,
                    OnPermissionRequest = PermissionHandler.ApproveAll
                });

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var response = await Program.SendMessageAndCollectResponseSilentAsync(benchSession, fullPrompt);
                sw.Stop();

                var words = response.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                results.Add((modelInfo.Id, sw.ElapsedMilliseconds, words, response, "OK"));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {sw.ElapsedMilliseconds,6} ms  {words,5} words");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                results.Add((modelInfo.Id, 0, 0, string.Empty, $"ERROR: {ex.Message}"));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ❌ {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                if (benchSession != null)
                    await benchSession.DisposeAsync();
            }
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("📊 Benchmark Results:");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"   {"Model",-35} {"Latency (ms)",13}  {"Words",6}  Status");
        Console.WriteLine($"   {"─────────────────────────────────",35} {"─────────────",13}  {"─────",6}  ──────");
        Console.ResetColor();

        foreach (var (model, latencyMs, wordCount, _, status) in results)
            Console.WriteLine($"   {model,-35} {latencyMs,13}  {wordCount,6}  {status}");

        Console.ResetColor();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("🔬 Auto-classifying each model's response...");
        Console.ResetColor();
        Console.WriteLine();

        var classifyResults = new Dictionary<string, (int RowCount, string Status)>(StringComparer.OrdinalIgnoreCase);
        var allClassifiedRows = new List<(string Model, List<TechnologyRecord> Rows)>();

        foreach (var r in results.Where(r => r.Status == "OK"))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"   ▶ {r.Model,-35}");
            Console.ResetColor();

            try
            {
                var sections = new List<(string Name, string Content)>
                {
                    ("Benchmark Response", r.Response)
                };

                await using var classifySession = await client.CreateSessionAsync(new SessionConfig
                {
                    Model = r.Model,
                    Streaming = true,
                    OnPermissionRequest = PermissionHandler.ApproveAll
                });

                var classifyPrompt = TechnologyClassifier.BuildClassificationFromSummaryPrompt(sections);
                var jsonResponse = await Program.SendMessageAndCollectResponseSilentAsync(classifySession, classifyPrompt);
                var json = TechnologyClassifier.ExtractJson(jsonResponse);

                if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith('[') || !json.TrimEnd().EndsWith(']'))
                {
                    jsonResponse = await Program.SendMessageAndCollectResponseSilentAsync(classifySession, classifyPrompt);
                    json = TechnologyClassifier.ExtractJson(jsonResponse);
                }

                if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith('[') || !json.TrimEnd().EndsWith(']'))
                {
                    classifyResults[r.Model] = (0, "No valid JSON");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  ⚠️ No valid JSON returned");
                    Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {merged.Count,3} rows");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                classifyResults[r.Model] = (0, $"ERROR: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ❌ {ex.Message}");
                Console.ResetColor();
            }
        }

        string? classificationCsvPath = null;
        if (allClassifiedRows.Count > 0)
        {
            classificationCsvPath = Path.Combine(outputDirectory, $"benchmark_{timestamp}_classification.csv");
            var combinedCsv = new StringBuilder();
            bool headerWritten = false;

            foreach (var (model, rows) in allClassifiedRows)
            {
                var tempPath = Path.GetTempFileName();
                try
                {
                    TechnologyClassificationCsv.WriteCsv(tempPath, rows);
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("📋 Classification Comparison:");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"   {"Model",-35}  {"Rows",5}  Classification Status");
            Console.WriteLine($"   {"─────────────────────────────────",35}  {"─────",5}  ────────────────────");
            Console.ResetColor();
            foreach (var kvp in classifyResults)
            {
                var rowsStr = kvp.Value.Status == "OK" ? kvp.Value.RowCount.ToString() : "-";
                Console.WriteLine($"   {kvp.Key,-35}  {rowsStr,5}  {kvp.Value.Status}");
            }
            Console.WriteLine();
        }

        var csvPath = Path.Combine(outputDirectory, $"benchmark_{timestamp}.csv");
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

        var txtPath = Path.Combine(outputDirectory, $"benchmark_{timestamp}.txt");
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

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"💾 Results saved → {csvPath}");
        Console.WriteLine($"💾 Responses saved → {txtPath}");
        if (classificationCsvPath != null)
            Console.WriteLine($"💾 Classification saved → {classificationCsvPath}");
        Console.ResetColor();
        Console.WriteLine();
    }
}
