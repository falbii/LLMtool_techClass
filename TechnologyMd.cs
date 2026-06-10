using System.Text;
using System.Text.RegularExpressions;

namespace TechClassificationApp;

// Owns the human-readable technology summary Markdown format — the file that auto-summarize writes
// and auto-classify reads back. Keeping the writer and the reader together here guarantees they
// agree on the header/separator markers: previously the write lived in TechnologySummarizer and the
// parse in TechnologyClassifier, so a format change in one could silently break the other.
public static class TechnologyMd
{
    // Matches each "## TECHNOLOGY N: <name>" section header written by WriteAsync.
    private static readonly Regex SectionHeaderPattern = new(
        @"^#{2,3}\s*TECHNOLOGY\s+\d+:\s*(?<name>[^\r\n]+?)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

    // Writes the full summary file. Called incrementally after each batch so progress survives a
    // mid-run failure; technologyDetails may be shorter than technologyNames while still in progress.
    // finishedAt is null until the run completes, so intermediate writes show "(in progress)".
    public static async Task WriteAsync(
        string mdPath, string sourcePdf, string model,
        DateTime startedAt, DateTime? finishedAt,
        List<string> technologyNames, List<string> technologyDetails)
    {
        var md = new StringBuilder();
        md.AppendLine($"# Technology Extraction Data — {Path.GetFileName(sourcePdf)}");
        md.AppendLine();
        md.AppendLine($"- **Model:** {model}");
        md.AppendLine($"- **Started:** {startedAt:yyyy-MM-dd HH:mm:ss}");
        if (finishedAt.HasValue)
        {
            var duration = finishedAt.Value - startedAt;
            md.AppendLine($"- **Finished:** {finishedAt.Value:yyyy-MM-dd HH:mm:ss} (duration {duration:hh\\:mm\\:ss})");
        }
        else
        {
            md.AppendLine("- **Finished:** (in progress)");
        }
        md.AppendLine($"- **Total Technologies:** {technologyNames.Count}");
        md.AppendLine($"- **Completed Technologies:** {technologyDetails.Count}");
        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();

        for (int i = 0; i < technologyDetails.Count && i < technologyNames.Count; i++)
        {
            md.AppendLine($"## TECHNOLOGY {i + 1}: {technologyNames[i]}");
            md.AppendLine();
            md.AppendLine(technologyDetails[i]);
            md.AppendLine();
            md.AppendLine("---");
            md.AppendLine();
        }

        await File.WriteAllTextAsync(mdPath, md.ToString(), Encoding.UTF8);
    }

    // Reads the summary file and splits it into one (name, content) pair per technology section.
    // Returns an empty list if the file is missing or contains no sections.
    public static List<(string Name, string Content)> ReadSections(string mdPath)
    {
        var sections = new List<(string Name, string Content)>();
        if (!File.Exists(mdPath))
            return sections;

        var mdContent = File.ReadAllText(mdPath, Encoding.UTF8);
        if (string.IsNullOrWhiteSpace(mdContent))
            return sections;

        var matches = SectionHeaderPattern.Matches(mdContent);
        for (int i = 0; i < matches.Count; i++)
        {
            string name = matches[i].Groups["name"].Value.Trim();
            var contentStart = matches[i].Index + matches[i].Length;
            var contentEnd = (i + 1 < matches.Count) ? matches[i + 1].Index : mdContent.Length;

            var content = mdContent[contentStart..contentEnd].Trim();

            // Trim the horizontal rule WriteAsync appends after each section. Only a rule at the
            // very end is removed, so a "---" the model emitted inside the content is left alone.
            content = Regex.Replace(content, @"\r?\n-{3,}\s*$", "").Trim();

            if (!string.IsNullOrWhiteSpace(content))
                sections.Add((name, content));
        }

        return sections;
    }
}
