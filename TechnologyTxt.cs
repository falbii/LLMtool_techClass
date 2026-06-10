using System.Text;
using System.Text.RegularExpressions;

namespace TechClassificationApp;

// Owns the human-readable technology summary .txt format — the file that auto-summarize writes and
// auto-classify reads back. Keeping the writer and the reader together here guarantees they agree on
// the header/separator markers: previously the write lived in TechnologySummarizer and the parse in
// TechnologyClassifier, so a format change in one could silently break the other.
public static class TechnologyTxt
{
    private const string DoubleRule = "═══════════════════════════════════════════════════════════════";
    private const string SectionRule = "───────────────────────────────────────────────────────────────";

    // Matches each "═══ TECHNOLOGY N: <name> ═══" header written by WriteAsync.
    private static readonly Regex SectionHeaderPattern = new(
        @"═{3,}\s*TECHNOLOGY\s+\d+:\s*(?<name>.+?)\s*═{3,}",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Writes the full summary file. Called incrementally after each batch so progress survives a
    // mid-run failure; technologyDetails may be shorter than technologyNames while still in progress.
    public static async Task WriteAsync(
        string txtPath, string sourcePdf,
        List<string> technologyNames, List<string> technologyDetails)
    {
        var txtContent = new StringBuilder();
        txtContent.AppendLine(DoubleRule);
        txtContent.AppendLine($"Technology Extraction Data - {Path.GetFileName(sourcePdf)}");
        txtContent.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        txtContent.AppendLine($"Total Technologies: {technologyNames.Count}");
        txtContent.AppendLine($"Completed Technologies: {technologyDetails.Count}");
        txtContent.AppendLine(DoubleRule);
        txtContent.AppendLine();

        for (int i = 0; i < technologyDetails.Count && i < technologyNames.Count; i++)
        {
            txtContent.AppendLine($"═══ TECHNOLOGY {i + 1}: {technologyNames[i]} ═══");
            txtContent.AppendLine();
            txtContent.AppendLine(technologyDetails[i]);
            txtContent.AppendLine();
            txtContent.AppendLine(SectionRule);
            txtContent.AppendLine();
        }

        await File.WriteAllTextAsync(txtPath, txtContent.ToString(), Encoding.UTF8);
    }

    // Reads the summary file and splits it into one (name, content) pair per technology section.
    // Returns an empty list if the file is missing or contains no sections.
    public static List<(string Name, string Content)> ReadSections(string txtPath)
    {
        var sections = new List<(string Name, string Content)>();
        if (!File.Exists(txtPath))
            return sections;

        var txtContent = File.ReadAllText(txtPath, Encoding.UTF8);
        if (string.IsNullOrWhiteSpace(txtContent))
            return sections;

        var matches = SectionHeaderPattern.Matches(txtContent);
        for (int i = 0; i < matches.Count; i++)
        {
            string name = matches[i].Groups["name"].Value.Trim();
            var contentStart = matches[i].Index + matches[i].Length;
            var contentEnd = (i + 1 < matches.Count) ? matches[i + 1].Index : txtContent.Length;

            var content = txtContent[contentStart..contentEnd].Trim();

            // Trim the trailing separator rule that WriteAsync appends after each section.
            var separatorIdx = content.LastIndexOf("───", StringComparison.Ordinal);
            if (separatorIdx >= 0)
                content = content[..separatorIdx].Trim();

            if (!string.IsNullOrWhiteSpace(content))
                sections.Add((name, content));
        }

        return sections;
    }
}
