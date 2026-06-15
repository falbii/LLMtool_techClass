using System.Diagnostics;
using System.Text;
using static TechClassificationApp.TechClassifierHelpers;

namespace TechClassificationApp;

// Multi-model benchmark. Mirrors the normal workflow (condense → summarize → classify) but runs it
// across every available model and writes the results side-by-side, all into ws.BenchmarkDir
// (3_output/3_benchmark):
//   • benchmark_summary_<pdf>_<date>.md         — each model's technology summary (from prompt/benchmark.md)
//   • benchmark_classification_<pdf>_<date>.csv — each model's structured rows (Model column prefixed)
//   • benchmark_overview_<pdf>_<date>.csv       — per-model timing / counts / status comparison
//
// The summary step uses the single benchmark question in prompt/benchmark.md rather than the full
// find→summarize pipeline: every model answers the same prompt so their outputs are comparable.
public static class Benchmark
{
    // The benchmark question lives in prompt/benchmark.md so it can be tuned without recompiling.
    private static string LoadBenchmarkPrompt() => LoadPromptTemplate("benchmark.md").Trim();

    // Returns false when the benchmark could not run at all (no PDF, no models);
    // per-model errors are recorded in the overview and still count as a run.
    //
    // pdfPath: the document to benchmark all models on. When null (console default), the user is
    // prompted to pick one from ws.PdfDir; callers that already have a selection (the web API)
    // pass it in so no interactive console prompt is triggered.
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

        var pdfName = Path.GetFileName(pdfPath);
        var baseName = Path.GetFileNameWithoutExtension(pdfPath);
        var date = DateTime.Now.ToString("dd-MM-yyyy");

        // Build the summary prompt once: condenses the PDF (cache shared by every model) and wraps
        // the benchmark question with the condensed document. All models receive the same prompt.
        var benchmarkPrompt = LoadBenchmarkPrompt();
        string summaryPrompt = string.Empty;
        try
        {
            await Program.RunWithSpinnerAsync(" Condensing & building benchmark prompt", async () =>
            {
                (summaryPrompt, _) = await CommandHandlers.BuildPromptWithPdfContextAsync(ws, pdfPath!, benchmarkPrompt);
            });
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Could not prepare PDF text: {ex.Message}");
            return false;
        }

        var models = await ws.Client.ListModelsAsync();
        if (models == null || models.Count == 0)
        {
            ConsoleEx.Error("❌ No models available.");
            return false;
        }

        Console.WriteLine();
        ConsoleEx.Info($"🏁 Benchmarking {models.Count} models on: {pdfName}");
        Console.WriteLine();

        // ref_year is the source's publication year — one value per paper, stamped on every row.
        var refYear = TechnologyClassifier.ExtractSourceYear(ws, pdfPath!);

        // --- Stage 1: summarize with each model (same benchmark prompt) ---
        ConsoleEx.Warn("   1. Summarizing with each model...");
        var summaries = new List<(string Model, string Summary, int Words, long Ms, string Status)>();
        foreach (var m in models)
        {
            ConsoleEx.Dim($"   ▶ {m.Id}…");
            try
            {
                await using var session = await Sessions.NewAsync(ws.Client, m.Id);
                var sw = Stopwatch.StartNew();
                var summary = await Program.SendMessageAndCollectResponseSilentAsync(session, summaryPrompt);
                sw.Stop();

                var words = summary.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                summaries.Add((m.Id, summary, words, sw.ElapsedMilliseconds, "OK"));
                ConsoleEx.Success($"   ✓ {m.Id,-35}  {words,5} words  {sw.ElapsedMilliseconds,6} ms");
            }
            catch (Exception ex)
            {
                summaries.Add((m.Id, string.Empty, 0, 0, $"ERROR: {ex.Message}"));
                ConsoleEx.Error($"   ❌ {m.Id,-35}  {ex.Message}");
            }
        }

        var summaryPath = Path.Combine(ws.BenchmarkDir, $"benchmark_summary_{baseName}_{date}.md");
        await WriteSummaryMarkdownAsync(summaryPath, pdfName, benchmarkPrompt, summaries);

        // --- Stage 2: classify each model's summary ---
        Console.WriteLine();
        ConsoleEx.Warn("   2. Classifying each model's summary...");
        var classified = new Dictionary<string, (List<TechnologyRecord> Rows, long Ms, string Status)>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var s in summaries.Where(x => x.Status == "OK" && !string.IsNullOrWhiteSpace(x.Summary)))
        {
            ConsoleEx.Dim($"   ▶ {s.Model}…");
            try
            {
                // The model's whole summary is handed to the classifier as one section; it extracts
                // one row per technology, exactly as the normal classify step does.
                var sections = new List<(string Name, string Content)> { ("Benchmark Summary", s.Summary) };
                var sw = Stopwatch.StartNew();
                var rows = await ClassifySectionsAsync(ws, s.Model, sections, refYear);
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
        return true;
    }

    // Classifies one model's summary into merged records, using the same prompt and post-processing
    // as the normal auto-classify pipeline (one retry on invalid JSON).
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
        string path, string pdfName, string benchmarkPrompt,
        List<(string Model, string Summary, int Words, long Ms, string Status)> summaries)
    {
        var md = new StringBuilder();
        md.AppendLine($"# Benchmark Summaries — {pdfName}");
        md.AppendLine();
        md.AppendLine($"- **Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        md.AppendLine($"- **Models:** {summaries.Count}");
        md.AppendLine();
        md.AppendLine("**Prompt:**");
        md.AppendLine();
        md.AppendLine($"> {benchmarkPrompt.Replace("\n", "\n> ")}");
        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();

        foreach (var s in summaries)
        {
            md.AppendLine($"# MODEL: {s.Model}");
            md.AppendLine();
            md.AppendLine($"- **Status:** {s.Status}  |  **Words:** {s.Words}  |  **Duration:** {s.Ms} ms");
            md.AppendLine();
            md.AppendLine(s.Status == "OK" ? s.Summary : $"_(no summary — {s.Status})_");
            md.AppendLine();
            md.AppendLine("---");
            md.AppendLine();
        }

        await File.WriteAllTextAsync(path, md.ToString(), Encoding.UTF8);
    }

    // Combines every model's rows into one CSV with a leading Model column, reusing TechnologyCsv
    // so the column layout stays identical to the normal classification output.
    private static async Task WriteClassificationCsvAsync(
        string path,
        List<(string Model, string Summary, int Words, long Ms, string Status)> summaries,
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
