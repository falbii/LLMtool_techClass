using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TechClassificationApp;

internal static class TechClassifierUtils
{
    // --- Row field parsing ---

    internal static string? GetValue(Dictionary<string, string> lookup, params string[] headers)
    {
        foreach (var header in headers)
        {
            var key = NormalizeHeader(header);
            if (lookup.TryGetValue(key, out var value))
                return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
        return null;
    }

    // Strips all non-alphanumeric characters and lowercases so "Cost Base Year",
    // "cost_base_year", and "CostBaseYear" all map to the same key.
    internal static string NormalizeHeader(string header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return string.Empty;

        var normalized = Regex.Replace(header, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase)
                              .ToLowerInvariant();
        return string.IsNullOrEmpty(normalized) ? header.ToLowerInvariant() : normalized;
    }

    internal static List<string> ParseStringList(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return [];

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    internal static List<double> ParseDoubleList(string? value, string fieldName, List<string> errors)
    {
        var results = new List<double>();
        if (string.IsNullOrWhiteSpace(value))
            return results;

        foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                results.Add(parsed);
            else
                errors.Add($"{fieldName}: invalid number '{part}'");
        }
        return results;
    }

    internal static double? ParseDouble(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)) return parsed;
        errors.Add($"{fieldName}: invalid number '{value}'");
        return null;
    }

    internal static int? ParseInt(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)) return parsed;
        errors.Add($"{fieldName}: invalid integer '{value}'");
        return null;
    }

    internal static decimal? ParseDecimal(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)) return parsed;
        errors.Add($"{fieldName}: invalid decimal '{value}'");
        return null;
    }

    internal static (double value, string unit)? ParseValueWithUnit(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var match = Regex.Match(value.Trim(), @"^(?<num>[-+]?[0-9]*[.]?[0-9]+)\s*(?<unit>.*)$");
        if (!match.Success) return null;

        if (!double.TryParse(match.Groups["num"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            return null;

        var unit = match.Groups["unit"].Value.Trim();
        return (parsed, string.IsNullOrEmpty(unit) ? string.Empty : unit);
    }

    // --- JSON helpers ---

    internal static string? ConvertJsonValueToString(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.String  => element.GetString(),
            JsonValueKind.Number  => element.GetRawText(),
            JsonValueKind.True    => "true",
            JsonValueKind.False   => "false",
            JsonValueKind.Array   => string.Join(", ",
                element.EnumerateArray()
                       .Select(ConvertJsonValueToString)
                       .Where(v => !string.IsNullOrWhiteSpace(v))),
            _ => element.ToString()
        };
    }

    // --- Prompt building ---

    internal static string BuildPdfContentSection(List<string> chunks)
    {
        var sb = new StringBuilder();
        if (chunks.Count == 1)
        {
            sb.AppendLine("PDF Content:");
            sb.AppendLine(chunks[0]);
            return sb.ToString().TrimEnd();
        }

        sb.AppendLine($"PDF Content ({chunks.Count} parts):");
        for (int i = 0; i < chunks.Count; i++)
        {
            sb.AppendLine($"--- Part {i + 1} ---");
            sb.AppendLine(chunks[i]);
            sb.AppendLine();
        }
        return sb.ToString().TrimEnd();
    }

    internal static string BuildTechnologySectionsContent(List<(string Name, string Content)> sections)
    {
        var sb = new StringBuilder();
        foreach (var (name, content) in sections)
        {
            sb.AppendLine($"=== {name} ===");
            sb.AppendLine(content);
            sb.AppendLine();
        }
        return sb.ToString().TrimEnd();
    }

    internal static string LoadPromptTemplate(string fileName)
        => File.ReadAllText(FindConfigFile("prompt", fileName));

    private static string FindConfigFile(string folder, string fileName)
    {
        string[] candidates =
        [
            Path.Combine(Directory.GetCurrentDirectory(), folder, fileName),
            Path.Combine(AppContext.BaseDirectory, folder, fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", folder, fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", folder, fileName)
        ];

        return candidates.Select(Path.GetFullPath).FirstOrDefault(File.Exists)
            ?? throw new FileNotFoundException($"Config file not found: {folder}/{fileName}");
    }

    // --- Merge key helpers ---

    internal static string FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        return string.Empty;
    }

    internal static string NormalizeKeyText(string value)
    {
        var lowered = value.Trim().ToLowerInvariant();
        return Regex.Replace(lowered, @"\s+", " ");
    }

    // --- CSV formatting ---

    internal static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var needsQuotes = value.Contains(',') || value.Contains('"') ||
                          value.Contains('\n') || value.Contains('\r');
        var escaped = value.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{escaped}\"" : escaped;
    }

    internal static string JoinList(IEnumerable<string?> values) =>
        string.Join(", ", values.Where(v => !string.IsNullOrWhiteSpace(v)));

    internal static string FormatDouble(double? value) =>
        value?.ToString("0.########", CultureInfo.InvariantCulture) ?? string.Empty;

    internal static string FormatDecimal(decimal? value) =>
        value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;

    internal static string FormatInt(int? value) =>
        value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;

    // --- CSV parsing ---

    internal static List<List<string>> ParseCsvRecords(string content)
    {
        var records = new List<List<string>>();
        var currentRecord = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (int i = 0; i < content.Length; i++)
        {
            var ch = content[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < content.Length && content[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (ch == ',' && !inQuotes)
            {
                currentRecord.Add(current.ToString());
                current.Clear();
            }
            else if ((ch == '\n' || ch == '\r') && !inQuotes)
            {
                if (ch == '\r' && i + 1 < content.Length && content[i + 1] == '\n')
                    i++;
                currentRecord.Add(current.ToString());
                current.Clear();
                if (currentRecord.Count > 1 || !string.IsNullOrWhiteSpace(currentRecord[0]))
                    records.Add(currentRecord);
                currentRecord = [];
            }
            else
            {
                current.Append(ch);
            }
        }

        currentRecord.Add(current.ToString());
        if (currentRecord.Count > 1 || !string.IsNullOrWhiteSpace(currentRecord[0]))
            records.Add(currentRecord);

        return records;
    }
}
