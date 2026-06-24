using System.Diagnostics;
using System.Text;

namespace TechClassificationApp;

// Multi-model benchmark. Runs the EXACT same pipeline as the normal auto-classify flow
// (condense → find technologies → summarize → classify), but:
//   1. one model finds all technologies in the paper,
//   2. the user picks exactly three of them,
//   3. every available model then summarizes + classifies those same three technologies,
// so the models' outputs are directly comparable on an identical, focused task.
//
// Results land in ws.BenchmarkDir (3_output/3_benchmark):
//   • benchmark_summary_<pdf>_<date>.md         — each model's per-technology summary
//   • benchmark_classification_<pdf>_<date>.csv — each model's structured rows (Model column prefixed)
//   • benchmark_overview_<pdf>_<date>.csv       — per-model timing / counts / status comparison
//
// Selecting the three technologies is interactive. The console drives that at the keyboard via
// RunAsync; the web UI calls the two public steps (FindTechnologiesAsync, then RunOnSelectionAsync)
// directly, doing the picking in the browser.
public static class Benchmark
{
    public const int SelectionCount = 3;

    // Console entry point: select PDF → find technologies → pick three at the keyboard → run.
    // Returns false when the benchmark could not run at all (no PDF, no models, cancelled selection);
    // per-model errors are recorded in the overview and still count as a run.
    public static async Task<bool> RunAsync(Workspace ws, string? pdfPath = null)
    {
        if (string.IsNullOrWhiteSpace(pdfPath))
        {
            ConsoleEx.Info("🏁 Benchmark — select the PDF to run all models on:");
            pdfPath = await CommandHandlers.HandleListPdfsAsync(ws.PdfDir);
            if (string.IsNullOrWhiteSpace(pdfPath))
            {
                ConsoleEx.Warn("❌ No PDF selected — benchmark cancelled.");
                return false;
            }
        }

        if (!File.Exists(pdfPath))
        {
            ConsoleEx.Error($"❌ Benchmark PDF not found: {pdfPath}");
            return false;
        }

        var allTechs = await FindTechnologiesAsync(ws, pdfPath);
        if (allTechs == null)
            return false;
        if (allTechs.Count < SelectionCount)
        {
            ConsoleEx.Warn($"⚠️  Need at least {SelectionCount} technologies to benchmark, but found {allTechs.Count}.");
            return false;
        }

        var selected = PromptForTechnologies(allTechs);
        if (selected == null)
        {
            ConsoleEx.Warn("❌ No technologies selected — benchmark cancelled.");
            return false;
        }

        return await RunOnSelectionAsync(ws, pdfPath, selected) != null;
    }

    // Step 1 (shared by console + web): condense the PDF and ask one model for every technology in
    // the paper. Returns the list, or null on failure (already logged through ConsoleEx).
    public static async Task<List<string>?> FindTechnologiesAsync(Workspace ws, string pdfPath)
    {
        if (!File.Exists(pdfPath))
        {
            ConsoleEx.Error($"❌ Benchmark PDF not found: {pdfPath}");
            return null;
        }

        List<string> chunks;
        try
        {
            chunks = await Program.RunWithSpinnerAsync(" Condensing PDF", async () =>
            {
                var pdfText = await PdfCondenser.GetCondensedTextAsync(ws, pdfPath);
                return PdfExtractor.SplitIntoChunks(pdfText);
            });
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Could not prepare PDF text: {ex.Message}");
            return null;
        }

        // Reuse the SAME frozen technology list the summarize pipeline caches (<name>_technologies.md),
        // so the benchmark runs on exactly the rows summarize would produce and re-runs are reproducible.
        // Falls back to scanning the paper — and caches the result the same way — when no list exists yet.
        var cached = await PdfCondenser.TryReadTechListAsync(pdfPath, ws.CacheDir);
        if (cached is { Count: > 0 })
        {
            ConsoleEx.Dim($"   ♻️  Using cached technology list ({cached.Count})");
            return cached;
        }

        ConsoleEx.Warn($"   1. Finding technologies (model: {ws.Model})...");
        try
        {
            await using var session = await Sessions.NewAsync(ws.Client, ws.Model);
            var findPrompt = TechnologySummarizer.BuildFindTechnologiesPrompt(chunks);
            var namesResponse = await Program.RunWithSpinnerAsync("   Scanning PDF",
                () => Program.SendMessageAndCollectResponseSilentAsync(session, findPrompt));
            var allTechs = TechnologySummarizer.ParseTechnologyNames(namesResponse);
            if (allTechs.Count > 0)
                await PdfCondenser.WriteTechListAsync(pdfPath, ws.CacheDir, allTechs);
            ConsoleEx.Success($"   Found {allTechs.Count} technologies.");
            return allTechs;
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Could not find technologies: {ex.Message}");
            return null;
        }
    }

    // Step 2 (shared by console + web): run the real summarize → classify pipeline for the chosen
    // technologies across every available model, writing the three benchmark output files. Returns
    // the main previewable output path (classification CSV, else overview), or null on total failure.
    public static async Task<string?> RunOnSelectionAsync(
        Workspace ws, string pdfPath, IReadOnlyList<string> selected)
    {
        if (selected == null || selected.Count == 0)
        {
            ConsoleEx.Warn("❌ No technologies selected — benchmark cancelled.");
            return null;
        }
        if (!File.Exists(pdfPath))
        {
            ConsoleEx.Error($"❌ Benchmark PDF not found: {pdfPath}");
            return null;
        }

        var pdfName = Path.GetFileName(pdfPath);
        var baseName = Path.GetFileNameWithoutExtension(pdfPath);
        var date = DateTime.Now.ToString("dd-MM-yyyy");

        // Re-condense (cheap — the cache is already warm from the find step) and split into chunks,
        // exactly as the normal summarize pipeline does.
        List<string> chunks;
        try
        {
            var pdfText = await PdfCondenser.GetCondensedTextAsync(ws, pdfPath);
            chunks = PdfExtractor.SplitIntoChunks(pdfText);
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Could not prepare PDF text: {ex.Message}");
            return null;
        }

        var models = await ws.Client.ListModelsAsync();
        if (models == null || models.Count == 0)
        {
            ConsoleEx.Error("❌ No models available.");
            return null;
        }

        Console.WriteLine();
        ConsoleEx.Info($"🏁 Benchmarking {models.Count} models on {selected.Count} technologies from: {pdfName}");
        foreach (var t in selected)
            ConsoleEx.Dim($"     • {t}");
        Console.WriteLine();

        // ref_year is the source's publication year — one value per paper, stamped on every row.
        var refYear = TechnologyClassifier.ExtractSourceYear(ws, pdfPath);

        // --- Stage 2: summarize the selected technologies with each model ---
        var selectedList = selected.ToList();
        ConsoleEx.Warn("   2. Summarizing the selected technologies with each model...");
        var summaries = new List<(string Model, List<(string Name, string Content)> Sections, int Words, long Ms, string Status)>();
        foreach (var m in models)
        {
            ConsoleEx.Dim($"   ▶ {m.Id}…");
            try
            {
                await using var session = await Sessions.NewAsync(ws.Client, m.Id);
                var prompt = TechnologySummarizer.BuildSummaryTechnologyPrompt(chunks, selectedList);
                var sw = Stopwatch.StartNew();
                var response = await Program.SendMessageAndCollectResponseSilentAsync(session, prompt);
                sw.Stop();

                // Same parser the normal pipeline uses: one section per selected technology, keyed by
                // the "=== TECHNOLOGY N ===" header the model emits (missing ones get a marker).
                var details = TechnologySummarizer.ParseBatchedExtractionResponse(response, selected.Count);
                var sections = selected.Select((name, i) => (Name: name, Content: details[i])).ToList();

                var words = sections.Sum(s => s.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
                summaries.Add((m.Id, sections, words, sw.ElapsedMilliseconds, "OK"));
                ConsoleEx.Success($"   ✓ {m.Id,-35}  {words,5} words  {sw.ElapsedMilliseconds,6} ms");
            }
            catch (Exception ex)
            {
                summaries.Add((m.Id, new List<(string, string)>(), 0, 0, $"ERROR: {ex.Message}"));
                ConsoleEx.Error($"   ❌ {m.Id,-35}  {ex.Message}");
            }
        }

        var summaryPath = Path.Combine(ws.BenchmarkDir, $"benchmark_summary_{baseName}_{date}.md");
        await WriteSummaryMarkdownAsync(summaryPath, pdfName, selected, summaries);

        // --- Stage 3: classify each model's summary ---
        Console.WriteLine();
        ConsoleEx.Warn("   3. Classifying each model's summary...");
        var classified = new Dictionary<string, (List<TechnologyRecord> Rows, long Ms, string Status)>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var s in summaries.Where(x => x.Status == "OK" && x.Sections.Count > 0))
        {
            ConsoleEx.Dim($"   ▶ {s.Model}…");
            try
            {
                var sw = Stopwatch.StartNew();
                var rows = await ClassifySectionsAsync(ws, s.Model, s.Sections, refYear);
                sw.Stop();

                classified[s.Model] = (rows, sw.ElapsedMilliseconds, rows.Count > 0 ? "OK" : "No valid data");
                ConsoleEx.Success($"   ✓ {s.Model,-35}  {rows.Count,3} rows  {sw.ElapsedMilliseconds,6} ms");
            }
            catch (Exception ex)
            {
                classified[s.Model] = (new List<TechnologyRecord>(), 0, $"ERROR: {ex.Message}");
                ConsoleEx.Error($"   ❌ {s.Model,-35}  {ex.Message}");
            }
        }

        string? classificationPath = null;
        if (classified.Values.Any(c => c.Rows.Count > 0))
        {
            classificationPath = Path.Combine(ws.BenchmarkDir, $"benchmark_classification_{baseName}_{date}.csv");
            await WriteClassificationCsvAsync(classificationPath, summaries, classified);
        }

        // --- Overview: per-model comparison ---
        var overviewPath = Path.Combine(ws.BenchmarkDir, $"benchmark_overview_{baseName}_{date}.csv");
        var overview = new StringBuilder();
        overview.AppendLine("Model,SummaryWords,SummaryMs,SummaryStatus,ClassifiedRows,ClassifyMs,ClassifyStatus");
        foreach (var s in summaries)
        {
            var hasClassify = classified.TryGetValue(s.Model, out var c);
            var rows = hasClassify ? c.Rows.Count.ToString() : "N/A";
            var classifyMs = hasClassify ? c.Ms.ToString() : "N/A";
            var classifyStatus = hasClassify ? c.Status : "skipped";
            overview.AppendLine(string.Join(',',
                Csv(s.Model), s.Words, s.Ms, Csv(s.Status), rows, classifyMs, Csv(classifyStatus)));
        }
        await File.WriteAllTextAsync(overviewPath, overview.ToString(), Encoding.UTF8);

        Console.WriteLine();
        ConsoleEx.Info($"💾 Summary        → {summaryPath}");
        if (classificationPath != null)
            ConsoleEx.Info($"💾 Classification → {classificationPath}");
        ConsoleEx.Info($"💾 Overview       → {overviewPath}");
        Console.WriteLine();

        // Hand back the most useful file to preview: the merged classification CSV when any model
        // produced rows, otherwise the per-model overview.
        return classificationPath ?? overviewPath;
    }

    // Lists the found technologies and reads the user's pick of exactly SelectionCount of them.
    // Accepts comma/space-separated 1-based numbers (e.g. "1, 4, 7"). Re-prompts on bad input;
    // returns null only when the user presses Enter to cancel.
    private static List<string>? PromptForTechnologies(List<string> allTechs)
    {
        Console.WriteLine();
        ConsoleEx.Info($"🔬 Found {allTechs.Count} technologies — pick exactly {SelectionCount} to benchmark:");
        for (int i = 0; i < allTechs.Count; i++)
            Console.WriteLine($"   {i + 1}. {allTechs[i]}");

        while (true)
        {
            Console.WriteLine();
            Console.Write($"Enter {SelectionCount} numbers separated by commas (or press Enter to cancel): ");
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
                return null;

            var indices = input
                .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(p => int.TryParse(p, out var n) ? n : -1)
                .ToList();

            if (indices.Any(n => n < 1 || n > allTechs.Count))
            {
                ConsoleEx.Warn($"   ⚠️  Please use numbers between 1 and {allTechs.Count}.");
                continue;
            }

            var distinct = indices.Distinct().ToList();
            if (distinct.Count != SelectionCount)
            {
                ConsoleEx.Warn($"   ⚠️  Please pick exactly {SelectionCount} distinct technologies (you gave {distinct.Count}).");
                continue;
            }

            return distinct.Select(n => allTechs[n - 1]).ToList();
        }
    }

    // Classifies one model's summary into merged records, using the same prompt and post-processing
    // as the normal auto-classify pipeline (one retry on invalid JSON). Each technology is its own
    // section, so the classifier is told the correct technology count and emits one row per technology.
    private static async Task<List<TechnologyRecord>> ClassifySectionsAsync(
        Workspace ws, string model, List<(string Name, string Content)> sections, int? refYear)
    {
        await using var session = await Sessions.NewAsync(ws.Client, model);
        var prompt = TechnologyClassifier.BuildClassificationFromSummaryPrompt(sections);

        var json = TechnologyClassifier.ExtractJson(
            await Program.SendMessageAndCollectResponseSilentAsync(session, prompt));
        if (!TechnologyClassifier.IsValidJsonArray(json))
            json = TechnologyClassifier.ExtractJson(
                await Program.SendMessageAndCollectResponseSilentAsync(session, prompt));
        if (!TechnologyClassifier.IsValidJsonArray(json))
            return new List<TechnologyRecord>();

        var rows = TechnologyClassifier.ParseRowsFromJson(json);
        var records = new List<TechnologyRecord>();
        var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            var record = TechnologyClassifier.ParseRecord(row, out _);
            // Apply the same deterministic efficiency normalization the main pipeline uses, so
            // benchmark CSVs report efficiency on the same 0-1 scale (notes discarded here).
            _ = TechnologyValidator.NormalizeAndValidate(record);
            if (string.IsNullOrWhiteSpace(record.DatapaperTechId))
                record.DatapaperTechId = TechnologyClassifier.GenerateTechId(record, usedIds);
            usedIds.Add(record.DatapaperTechId);
            if (refYear.HasValue)
                record.RefYear = refYear;
            records.Add(record);
        }

        var meaningful = records.Where(TechnologyClassifier.HasMeaningfulData).ToList();
        return TechnologyClassifier.MergeByTechnologyAndYear(meaningful);
    }

    private static async Task WriteSummaryMarkdownAsync(
        string path, string pdfName, IReadOnlyList<string> selected,
        List<(string Model, List<(string Name, string Content)> Sections, int Words, long Ms, string Status)> summaries)
    {
        var md = new StringBuilder();
        md.AppendLine($"# Benchmark Summaries — {pdfName}");
        md.AppendLine();
        md.AppendLine($"- **Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        md.AppendLine($"- **Models:** {summaries.Count}");
        md.AppendLine($"- **Technologies:** {string.Join("; ", selected)}");
        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();

        foreach (var s in summaries)
        {
            md.AppendLine($"# MODEL: {s.Model}");
            md.AppendLine();
            md.AppendLine($"- **Status:** {s.Status}  |  **Words:** {s.Words}  |  **Duration:** {s.Ms} ms");
            md.AppendLine();

            if (s.Status != "OK")
            {
                md.AppendLine($"_(no summary — {s.Status})_");
            }
            else
            {
                for (int i = 0; i < s.Sections.Count; i++)
                {
                    md.AppendLine($"## TECHNOLOGY {i + 1}: {s.Sections[i].Name}");
                    md.AppendLine();
                    md.AppendLine(s.Sections[i].Content);
                    md.AppendLine();
                }
            }

            md.AppendLine("---");
            md.AppendLine();
        }

        await File.WriteAllTextAsync(path, md.ToString(), Encoding.UTF8);
    }

    // Combines every model's rows into one CSV with a leading Model column, reusing TechnologyCsv
    // so the column layout stays identical to the normal classification output.
    private static async Task WriteClassificationCsvAsync(
        string path,
        List<(string Model, List<(string Name, string Content)> Sections, int Words, long Ms, string Status)> summaries,
        Dictionary<string, (List<TechnologyRecord> Rows, long Ms, string Status)> classified)
    {
        var combined = new StringBuilder();
        bool headerWritten = false;

        // Preserve the model order from the summary stage.
        foreach (var s in summaries)
        {
            if (!classified.TryGetValue(s.Model, out var c) || c.Rows.Count == 0)
                continue;

            var tempPath = Path.GetTempFileName();
            try
            {
                TechnologyCsv.WriteCsv(tempPath, c.Rows);
                var lines = await File.ReadAllLinesAsync(tempPath, Encoding.UTF8);

                if (!headerWritten)
                {
                    combined.AppendLine("Model," + lines[0]);
                    headerWritten = true;
                }

                foreach (var line in lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)))
                    combined.AppendLine($"{Csv(s.Model)},{line}");
            }
            finally
            {
                File.Delete(tempPath);
            }
        }

        await File.WriteAllTextAsync(path, combined.ToString(), Encoding.UTF8);
    }

    // Minimal CSV-field escaping for the columns this file writes itself (model ids, status text).
    private static string Csv(string value) =>
        value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
