using System.Text;
using System.Text.RegularExpressions;
using static TechClassificationApp.TechClassifierHelpers;

namespace TechClassificationApp;

// Produces a condensed Markdown version of a PDF and caches it as
// 01_input/12_condensed_md/<name>_condensed.md (ws.CacheDir).
// The condensed file preserves all numbers/units/tables but strips prose, cutting the token cost
// of every downstream operation that would otherwise re-send the full PDF text.
public static class PdfCondenser
{
    private const string CacheSuffix = "_condensed.md";
    private const string TechListSuffix = "_technology_list.md";

    public static string GetCachePath(string pdfFile, string cacheDirectory)
        => Path.Combine(cacheDirectory, $"{Path.GetFileNameWithoutExtension(pdfFile)}{CacheSuffix}");

    public static string GetTechListPath(string pdfFile, string techListDirectory)
        => Path.Combine(techListDirectory, $"{Path.GetFileNameWithoutExtension(pdfFile)}{TechListSuffix}");

    // A replacement upload can preserve an older source timestamp, so timestamp-based cache
    // validation alone cannot reliably detect that the file's contents changed. Remove both
    // artifacts derived from that filename before the replacement is used.
    public static void InvalidateCachedArtifacts(string pdfFile, string cacheDirectory, string techListDirectory)
    {
        File.Delete(GetCachePath(pdfFile, cacheDirectory));
        File.Delete(GetTechListPath(pdfFile, techListDirectory));
    }

    // The discovered technology list is cached (in ws.TechListDir) so re-runs reuse the
    // SAME enumeration instead of re-asking the model — which returns a slightly different list
    // each time, making the downstream row set non-deterministic. The file is plain text, one
    // name per line, and human-editable: edit it to control exactly which rows are produced.
    // Returns null (caller re-scans) when absent, empty, or older than the condensed text it
    // was derived from.
    public static async Task<List<string>?> TryReadTechListAsync(Workspace ws, string pdfFile)
    {
        var listPath = GetTechListPath(pdfFile, ws.TechListDir);
        var condensedPath = GetCachePath(pdfFile, ws.CacheDir);
        if (!File.Exists(listPath) || !File.Exists(condensedPath))
            return null;

        // Stale if the condensed text changed after the list was written.
        if (File.GetLastWriteTimeUtc(condensedPath) > File.GetLastWriteTimeUtc(listPath))
            return null;

        var lines = await File.ReadAllLinesAsync(listPath, Encoding.UTF8);
        var names = lines
            .Select(l => l.Trim())
            .Where(l => l.Length > 0 && !l.StartsWith("<!--", StringComparison.Ordinal) && !l.StartsWith('#'))
            .ToList();
        return names.Count > 0 ? names : null;
    }

    public static async Task WriteTechListAsync(Workspace ws, string pdfFile, IReadOnlyList<string> names)
    {
        var listPath = GetTechListPath(pdfFile, ws.TechListDir);
        var sb = new StringBuilder();
        sb.AppendLine($"<!-- technologies found in {Path.GetFileName(pdfFile)} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} -->");
        sb.AppendLine("<!-- one technology per line; edit to control the rows produced downstream, then re-run -->");
        sb.AppendLine();
        foreach (var name in names)
            sb.AppendLine(name);
        await File.WriteAllTextAsync(listPath, sb.ToString(), Encoding.UTF8);
    }

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

            condensed.AppendLine(StripTableMarkers(part.Trim()));
            condensed.AppendLine();
        }

        var result = condensed.ToString().TrimEnd();

        var header = $"<!-- condensed from {Path.GetFileName(pdfFile)} by {ws.Model} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} -->\n\n";
        await File.WriteAllTextAsync(cachePath, header + result, Encoding.UTF8);

        var rawKb = Encoding.UTF8.GetByteCount(rawText) / 1024;
        var condensedKb = Encoding.UTF8.GetByteCount(result) / 1024;
        var pct = rawKb > 0 ? 100 - (condensedKb * 100 / rawKb) : 0;

        ConsoleEx.Success($"   ✓ Condensed {rawKb} KB → {condensedKb} KB ({pct}% smaller)");
        ConsoleEx.Success($"   📁 Cached at: {cachePath}");

        return result;
    }

    // The raw-extraction table markers are hints for the model, not content, but models tend to
    // echo them back (often as piles of empty pairs at the end of the output) even when told not
    // to — so any that survive condensation are removed deterministically.
    private static string StripTableMarkers(string text)
    {
        text = Regex.Replace(text, @"\[(?:TABLE REGION|TECHNOLOGY LIST TABLE|END TABLE)[^\]\r\n]*\]", "");
        // Collapse the gaps the removed markers leave behind.
        text = Regex.Replace(text, @"[ \t]+(\r?\n)", "$1");
        text = Regex.Replace(text, @"(\r?\n){3,}", "$1$1");
        return text.Trim();
    }

    private static bool IsCacheValid(string pdfFile, string cachePath)
    {
        if (!File.Exists(cachePath))
            return false;

        // Stale if the source PDF was modified after the cache was written.
        return File.GetLastWriteTimeUtc(pdfFile) <= File.GetLastWriteTimeUtc(cachePath);
    }
}
