using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using GitHub.Copilot.SDK;
using static TechClassificationApp.TechClassifierHelpers;

namespace TechClassificationApp;

public static class TechnologySummarizer
{
    // --- Pure prompt-building helpers ---

    public static string BuildFindTechnologiesPrompt(List<string> chunks)
    {
        var template = LoadPromptTemplate("find_technologies.md");
        return template.Replace("{{PDF_CONTENT}}", BuildPdfContentSection(chunks));
    }

    public static string BuildBatchDetailedExtractionPrompt(List<string> chunks, List<string> technologyNames)
    {
        var template = LoadPromptTemplate("batch_detailed_extraction.md");

        var technologyList = new StringBuilder();
        for (int i = 0; i < technologyNames.Count; i++)
            technologyList.AppendLine($"{i + 1}. {technologyNames[i]}");

        return template
            .Replace("{{TECHNOLOGY_COUNT}}", technologyNames.Count.ToString(CultureInfo.InvariantCulture))
            .Replace("{{TECHNOLOGY_LIST}}", technologyList.ToString().TrimEnd())
            .Replace("{{PDF_CONTENT}}", BuildPdfContentSection(chunks));
    }

    public static List<string> ParseTechnologyNames(string response)
    {
        var names = new List<string>();
        if (string.IsNullOrWhiteSpace(response))
            return names;

        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith('#') || line.Length < 3)
                continue;

            var lower = line.ToLower();
            if ((lower.Contains("technology") || lower.Contains("list")) &&
                (lower.StartsWith("technology") || lower.StartsWith("list") || lower.EndsWith(':')))
                continue;

            var cleaned = Regex.Replace(line, @"^\d+\.\s*", "");
            cleaned = Regex.Replace(cleaned, @"^[-*•]\s*", "").Trim();

            if (!string.IsNullOrWhiteSpace(cleaned) && Regex.IsMatch(cleaned, @"[a-zA-Z]{2,}"))
                names.Add(cleaned);
        }

        return names;
    }

    // Placeholder written for any technology the model didn't return a parseable section for.
    // Carries no numbers, so it parses to an empty record downstream rather than fabricated data.
    public const string MissingSectionMarker =
        "[NO PARSEABLE DATA — extraction marker missing for this technology]";

    private static readonly Regex BatchSectionHeaderPattern = new(
        @"===\s*TECHNOLOGY\s+(?<num>\d+)\s*:.*?===",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Splits a batch response into one section per technology, keyed by the technology NUMBER the
    // model declares in each "=== TECHNOLOGY N: ... ===" header — not by positional order. A skipped
    // or out-of-order technology therefore lands in the correct slot (or stays empty) instead of
    // shifting every later one by a position. Technologies with no parseable section get an explicit
    // missing-data marker; we never back-fill by slicing the text into equal line ranges, which
    // silently misattributes one technology's numbers to another.
    public static List<string> ParseBatchedExtractionResponse(string response, int expectedCount)
    {
        var byIndex = new string?[expectedCount];

        if (!string.IsNullOrWhiteSpace(response))
        {
            var matches = BatchSectionHeaderPattern.Matches(response);
            for (int i = 0; i < matches.Count; i++)
            {
                var contentStart = matches[i].Index + matches[i].Length;
                var contentEnd = (i + 1 < matches.Count) ? matches[i + 1].Index : response.Length;
                var content = response[contentStart..contentEnd].Trim();

                // Header number is 1-based; ignore anything outside the expected range.
                if (int.TryParse(matches[i].Groups["num"].Value, out var techNum)
                    && techNum >= 1 && techNum <= expectedCount
                    && !string.IsNullOrWhiteSpace(content))
                {
                    // First write wins, so a duplicated number can't clobber an earlier good section.
                    byIndex[techNum - 1] ??= content;
                }
            }
        }

        var details = new List<string>(expectedCount);
        for (int i = 0; i < expectedCount; i++)
            details.Add(byIndex[i] ?? MissingSectionMarker);
        return details;
    }

    // --- Full summarization pipeline ---

    public static async Task<string?> RunAsync(Workspace ws, string pdfFile)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
        {
            ConsoleEx.Error("❌ PDF not found or invalid path.");
            return null;
        }

        try { Directory.CreateDirectory(ws.MdDir); }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Cannot create output folder: {ex.Message}");
            return null;
        }

        ConsoleEx.Info("📝 Summarising technologies from PDF...\n");

        var startedAt = DateTime.Now;

        try
        {
            var pdfText = await PdfCondenser.GetCondensedTextAsync(ws, pdfFile);
            var chunks = PdfExtractor.SplitIntoChunks(pdfText);
            var mdPath = Path.Combine(ws.MdDir,
                $"{Path.GetFileNameWithoutExtension(pdfFile)}.md");

            ConsoleEx.Warn("   1. Finding technologies...");

            string namesResponse;
            await using (var stage1Session = await Sessions.NewAsync(ws.Client, ws.Model))
            {
                var findNamesPrompt = BuildFindTechnologiesPrompt(chunks);
                namesResponse = await Program.RunWithSpinnerAsync("   Scanning PDF",
                    () => Program.SendMessageAndCollectResponseSilentAsync(stage1Session, findNamesPrompt));
            }

            var technologyNames = ParseTechnologyNames(namesResponse);
            if (technologyNames.Count == 0)
            {
                ConsoleEx.Warn("⚠️  No technologies found in PDF.");
                return null;
            }

            ConsoleEx.Success($"   Found {technologyNames.Count} technologies:");
            foreach (var name in technologyNames)
            {
                ConsoleEx.Dim($"     • {name}");
            }
            Console.WriteLine();

            ConsoleEx.Warn("   2. Generating detailed summaries...\n");

            var technologyDetails = new List<string>();
            const int batchSize = 10;
            int totalBatches = (int)Math.Ceiling((double)technologyNames.Count / batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                // Rate-limit between batches to avoid hitting Copilot request quotas.
                if (batchIndex > 0)
                    await Task.Delay(3000);

                var batchStart = batchIndex * batchSize;
                var batchCount = Math.Min(batchSize, technologyNames.Count - batchStart);
                var batchTechs = technologyNames.Skip(batchStart).Take(batchCount).ToList();

                Console.WriteLine($"   Batch {batchIndex + 1}/{totalBatches} (technologies {batchStart + 1}-{batchStart + batchCount})");

                try
                {
                    // Fresh session per batch: accumulated context from prior batches would grow the prompt
                    // on every iteration and could bias the model toward prior extractions.
                    await using var batchSession = await Sessions.NewAsync(ws.Client, ws.Model);

                    var response = await SendBatchAsync(batchSession, chunks, batchTechs);

                    AppendBatchResults(response, batchTechs, batchCount, technologyDetails);
                    await TechnologyMd.WriteAsync(mdPath, pdfFile, ws.Model, startedAt, null, technologyNames, technologyDetails);
                }
                catch (Exception ex)
                {
                    ConsoleEx.Error($"✗ {ex.Message}");
                    for (int i = 0; i < batchCount; i++)
                        technologyDetails.Add($"Extraction failed for {batchTechs[i]}: {ex.Message}");

                    await TechnologyMd.WriteAsync(mdPath, pdfFile, ws.Model, startedAt, null, technologyNames, technologyDetails);
                }
            }

            Console.WriteLine();

            while (technologyDetails.Count < technologyNames.Count)
                technologyDetails.Add($"ERROR: Missing data for {technologyNames[technologyDetails.Count]}");

            await TechnologyMd.WriteAsync(mdPath, pdfFile, ws.Model, startedAt, DateTime.Now, technologyNames, technologyDetails);

            Console.WriteLine();
            ConsoleEx.Success($"   📁 Saved to: {mdPath}");
            ConsoleEx.Success($"   ✓ {technologyNames.Count} technologies extracted and summarised");
            Console.WriteLine();
            ConsoleEx.Success("✅ Summarisation complete!");

            return mdPath;
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Auto-summarize failed: {ex.Message}");
            return null;
        }
    }

    private static async Task<string> SendBatchAsync(
        CopilotSession session, List<string> chunks, List<string> batchTechs)
    {
        var prompt = BuildBatchDetailedExtractionPrompt(chunks, batchTechs);
        return await Program.SendMessageAndStreamToConsoleAsync(session, prompt);
    }

    private static void AppendBatchResults(
        string response, List<string> batchTechs, int batchCount, List<string> technologyDetails)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            ConsoleEx.Warn("⚠️ Empty response");

            for (int i = 0; i < batchCount; i++)
                technologyDetails.Add($"No data found for {batchTechs[i]}");

            return;
        }

        // Returns exactly batchCount entries, in technology order, with missing ones explicitly
        // marked — so no reconciliation against the count is needed here.
        var parsed = ParseBatchedExtractionResponse(response, batchCount);
        technologyDetails.AddRange(parsed);
    }
}
