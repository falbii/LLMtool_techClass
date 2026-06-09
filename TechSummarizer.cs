using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using GitHub.Copilot.SDK;
using static TechClassificationApp.TechClassifierUtils;

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

    public static List<string> ParseBatchedExtractionResponse(string response, int expectedCount)
    {
        var details = new List<string>();
        if (string.IsNullOrWhiteSpace(response))
        {
            for (int i = 0; i < expectedCount; i++)
                details.Add($"No data found for technology {i + 1}");
            return details;
        }

        var sections = Regex.Split(response, @"===\s*TECHNOLOGY\s+\d+:.*?===", RegexOptions.IgnoreCase);
        for (int i = 1; i < sections.Length; i++)
        {
            var section = sections[i].Trim();
            if (!string.IsNullOrWhiteSpace(section))
                details.Add(section);
        }

        if (details.Count < expectedCount)
        {
            details.Clear();
            var lines = response.Split('\n');
            var linesPerTech = Math.Max(1, lines.Length / expectedCount);
            for (int i = 0; i < expectedCount; i++)
            {
                var start = i * linesPerTech;
                var count = (i == expectedCount - 1) ? (lines.Length - start) : linesPerTech;
                details.Add(string.Join('\n', lines.Skip(start).Take(count)).Trim());
            }
        }

        return details;
    }

    // --- Full summarization pipeline ---

    public static async Task<string?> RunAsync(
        CopilotClient client, string model, string pdfFile, string outputDirectory)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ PDF not found or invalid path.");
            Console.ResetColor();
            return null;
        }

        try { Directory.CreateDirectory(outputDirectory); }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Cannot create output folder: {ex.Message}");
            Console.ResetColor();
            return null;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📝 Summarising technologies from PDF...\n");
        Console.ResetColor();

        try
        {
            var pdfText = await PdfExtractor.ExtractTextAsync(pdfFile);
            var chunks = PdfExtractor.SplitIntoChunks(pdfText);
            var txtPath = Path.Combine(outputDirectory,
                $"{Path.GetFileNameWithoutExtension(pdfFile)}.txt");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   1. Finding technologies...");
            Console.ResetColor();

            string namesResponse;
            await using (var stage1Session = await client.CreateSessionAsync(new SessionConfig
            {
                Model = model,
                Streaming = true,
                OnPermissionRequest = PermissionHandler.ApproveAll
            }))
            {
                var findNamesPrompt = BuildFindTechnologiesPrompt(chunks);
                namesResponse = await Program.RunWithSpinnerAsync("   Scanning PDF",
                    () => Program.SendMessageAndCollectResponseSilentAsync(stage1Session, findNamesPrompt));
            }

            var technologyNames = ParseTechnologyNames(namesResponse);
            if (technologyNames.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  No technologies found in PDF.");
                Console.ResetColor();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   Found {technologyNames.Count} technologies:");
            Console.ResetColor();
            foreach (var name in technologyNames)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"     • {name}");
                Console.ResetColor();
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   2. Generating detailed summaries...\n");
            Console.ResetColor();

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
                    await using var batchSession = await client.CreateSessionAsync(new SessionConfig
                    {
                        Model = model,
                        Streaming = true,
                        OnPermissionRequest = PermissionHandler.ApproveAll
                    });

                    var response = await SendBatchAsync(batchSession, chunks, batchTechs);

                    AppendBatchResults(response, batchTechs, batchCount, technologyDetails);
                    await WriteProgressAsync(txtPath, pdfFile, technologyNames, technologyDetails);
                }
                catch (Exception ex)
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ {ex.Message}");
                    Console.ResetColor();
                    for (int i = 0; i < batchCount; i++)
                        technologyDetails.Add($"Extraction failed for {batchTechs[i]}: {ex.Message}");

                    await WriteProgressAsync(txtPath, pdfFile, technologyNames, technologyDetails);
                }
            }

            Console.WriteLine();

            while (technologyDetails.Count < technologyNames.Count)
                technologyDetails.Add($"ERROR: Missing data for {technologyNames[technologyDetails.Count]}");

            await WriteProgressAsync(txtPath, pdfFile, technologyNames, technologyDetails);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"   📁 Saved to: {txtPath}");
            Console.WriteLine($"   ✓ {technologyNames.Count} technologies extracted and summarised");
            Console.WriteLine();
            Console.WriteLine($"✅ Summarisation complete!");
            Console.ResetColor();

            return txtPath;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Auto-summarize failed: {ex.Message}");
            Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠️ Empty response");
            Console.ResetColor();

            for (int i = 0; i < batchCount; i++)
                technologyDetails.Add($"No data found for {batchTechs[i]}");

            return;
        }

        var parsed = ParseBatchedExtractionResponse(response, batchCount);
        while (parsed.Count < batchCount)
            parsed.Add($"Incomplete data for {batchTechs[parsed.Count]}");

        if (parsed.Count > batchCount)
            parsed = [..parsed.Take(batchCount)];

        technologyDetails.AddRange(parsed);
    }

    private static async Task WriteProgressAsync(
        string txtPath, string sourcePdf,
        List<string> technologyNames, List<string> technologyDetails)
    {
        var txtContent = new StringBuilder();
        txtContent.AppendLine("═══════════════════════════════════════════════════════════════");
        txtContent.AppendLine($"Technology Extraction Data - {Path.GetFileName(sourcePdf)}");
        txtContent.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        txtContent.AppendLine($"Total Technologies: {technologyNames.Count}");
        txtContent.AppendLine($"Completed Technologies: {technologyDetails.Count}");
        txtContent.AppendLine("═══════════════════════════════════════════════════════════════");
        txtContent.AppendLine();

        for (int i = 0; i < technologyDetails.Count && i < technologyNames.Count; i++)
        {
            txtContent.AppendLine($"═══ TECHNOLOGY {i + 1}: {technologyNames[i]} ═══");
            txtContent.AppendLine();
            txtContent.AppendLine(technologyDetails[i]);
            txtContent.AppendLine();
            txtContent.AppendLine("───────────────────────────────────────────────────────────────");
            txtContent.AppendLine();
        }

        await File.WriteAllTextAsync(txtPath, txtContent.ToString(), Encoding.UTF8);
    }
}
