using System.Text;
using GitHub.Copilot.SDK;
using static TechClassificationApp.TechClassifierHelpers;

namespace TechClassificationApp;

// Produces a condensed Markdown version of a PDF and caches it under cache/<name>.condensed.md.
// The condensed file preserves all numbers/units/tables but strips prose, cutting the token cost
// of every downstream operation that would otherwise re-send the full PDF text.
public static class PdfCondenser
{
    private const string CacheSuffix = ".condensed.md";

    public static string GetCachePath(string pdfFile, string cacheDirectory)
        => Path.Combine(cacheDirectory, $"{Path.GetFileNameWithoutExtension(pdfFile)}{CacheSuffix}");

    // Returns the condensed text for a PDF, generating and caching it on first use.
    // Reuses the cache unless the source PDF is newer than the cached file.
    public static async Task<string> GetCondensedTextAsync(Workspace ws, string pdfFile)
    {
        Directory.CreateDirectory(ws.CacheDir);
        var cachePath = GetCachePath(pdfFile, ws.CacheDir);

        if (IsCacheValid(pdfFile, cachePath))
        {
            ConsoleEx.Dim($"   ♻️  Using cached condensed PDF: {Path.GetFileName(cachePath)}");
            return await File.ReadAllTextAsync(cachePath, Encoding.UTF8);
        }

        var rawText = await PdfExtractor.ExtractTextAsync(pdfFile);
        var chunks = PdfExtractor.SplitIntoChunks(rawText);

        ConsoleEx.Info($"   🗜️  Condensing PDF (one-time, {chunks.Count} part(s)) to save tokens...");

        var template = LoadPromptTemplate("condense_pdf.md");
        var condensed = new StringBuilder();

        for (int i = 0; i < chunks.Count; i++)
        {
            var prompt = template.Replace("{{PDF_CONTENT}}", chunks[i]);

            // Fresh session per chunk: avoids context accumulation that would grow each prompt.
            await using var session = await Sessions.NewAsync(ws.Client, ws.Model);

            var part = await Program.RunWithSpinnerAsync(
                $"   Condensing part {i + 1}/{chunks.Count}",
                () => Program.SendMessageAndCollectResponseSilentAsync(session, prompt));

            condensed.AppendLine(part.Trim());
            condensed.AppendLine();
        }

        var result = condensed.ToString().TrimEnd();

        var header = $"<!-- condensed from {Path.GetFileName(pdfFile)} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} -->\n\n";
        await File.WriteAllTextAsync(cachePath, header + result, Encoding.UTF8);

        var rawKb = Encoding.UTF8.GetByteCount(rawText) / 1024;
        var condensedKb = Encoding.UTF8.GetByteCount(result) / 1024;
        var pct = rawKb > 0 ? 100 - (condensedKb * 100 / rawKb) : 0;

        ConsoleEx.Success($"   ✓ Condensed {rawKb} KB → {condensedKb} KB ({pct}% smaller)");
        ConsoleEx.Success($"   📁 Cached at: {cachePath}");

        return result;
    }

    private static bool IsCacheValid(string pdfFile, string cachePath)
    {
        if (!File.Exists(cachePath))
            return false;

        // Stale if the source PDF was modified after the cache was written.
        return File.GetLastWriteTimeUtc(pdfFile) <= File.GetLastWriteTimeUtc(cachePath);
    }
}
