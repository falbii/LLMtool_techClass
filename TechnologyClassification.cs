using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PdfAnalysisApp;

/// <summary>
/// Represents a single technology record extracted from a PDF, covering
/// process description, inputs/outputs, costs, efficiency, and maturity.
/// </summary>
public sealed class TechnologyClassification
{
    public string? DatapaperTechId { get; set; }
    public string? ProcessType { get; set; }
    public string? Description { get; set; }
    public string? UnitOperation { get; set; }
    public string? Summary { get; set; }
    public string? MainSector { get; set; }
    public string? MainCategory { get; set; }
    public string? CategorySpec { get; set; }
    public string? TechType { get; set; }
    public double? ReferenceUnitSize { get; set; }
    public string? ReferenceUnitSizeUnit { get; set; }
    public int? BaseYear { get; set; }
    public string? Location { get; set; }
    public string? Currency { get; set; }
    public int? DataReferenceYear { get; set; }
    public int? Trl { get; set; }
    public string? TechMaturity { get; set; }
    public double? OverallEfficiency { get; set; }
    public string? EfficiencyUnit { get; set; }
    public List<string> CarriersIn { get; set; } = new();
    public string? MainInput { get; set; }
    public List<double> RatiosIn { get; set; } = new();
    public List<string> UnitsIn { get; set; } = new();
    public List<string> CarriersOut { get; set; } = new();
    public string? MainOut { get; set; }
    public List<double> RatiosOut { get; set; } = new();
    public List<string> UnitsOut { get; set; } = new();
    public double? MinInstallationSize { get; set; }
    public string? MinInstallationSizeUnit { get; set; }
    public double? LifetimeYears { get; set; }
    public decimal? Capex { get; set; }
    public string? CapexUnit { get; set; }
    public decimal? OpexFix { get; set; }
    public string? OpexFixUnit { get; set; }

}

/// <summary>
/// Provides parsing, validation, merging, and prompt-building logic
/// for <see cref="TechnologyClassification"/> records.
/// </summary>
public static class TechnologyClassifier
{
    /// <summary>Case-insensitive comparer used for merge-key lookups.</summary>
    private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Builds a <see cref="TechnologyClassification"/> from a key/value row.
    /// Always returns <c>true</c> with the populated record; any fields that could
    /// not be parsed are left <c>null</c> and a warning is added to <paramref name="errors"/>.
    /// </summary>
    public static bool TryClassify(
        IDictionary<string, string> row,
        out TechnologyClassification classification,
        out List<string> errors)
    {
        errors = new List<string>();

        // Build the normalised lookup once so GetValue doesn't repeat the work per call.
        var lookup = row.ToDictionary(
            k => NormalizeHeader(k.Key),
            v => v.Value,
            StringComparer.OrdinalIgnoreCase);

        var tech = new TechnologyClassification
        {
            DatapaperTechId = GetValue(lookup, "Datapaper Tech ID", "tech_id"),
            ProcessType = GetValue(lookup, "ProcessType", "process_type"),
            Description = GetValue(lookup, "description"),
            UnitOperation = GetValue(lookup, "unit_operation"),
            Summary = GetValue(lookup, "summary"),
            MainSector = GetValue(lookup, "main_sector"),
            MainCategory = GetValue(lookup, "main_category"),
            CategorySpec = GetValue(lookup, "category_spec"),
            TechType = GetValue(lookup, "tech_type"),
            ReferenceUnitSize = ParseDouble(GetValue(lookup, "reference_unit_size"), "reference_unit_size", errors),
            ReferenceUnitSizeUnit = GetValue(lookup, "reference_unit_size_unit", "Reference Unit Size Unit"),
            BaseYear = ParseInt(GetValue(lookup, "base year", "cost_base_year"), "cost_base_year", errors),
            Location = GetValue(lookup, "Location"),
            Currency = GetValue(lookup, "Currency", "currency"),
            DataReferenceYear = ParseInt(GetValue(lookup, "Data Reference Year", "data_reference_year"), "data_reference_year", errors),
            Trl = ParseInt(GetValue(lookup, "trl_(1-9)", "trl"), "trl_(1-9)", errors),
            TechMaturity = GetValue(lookup, "tech_maturity"),
            OverallEfficiency = ParseDouble(GetValue(lookup, "efficiency", "lhv_efficiency", "overall_efficiency"), "efficiency", errors),
            EfficiencyUnit = GetValue(lookup, "efficiency_unit"),
            CarriersIn = ParseStringList(GetValue(lookup, "carriers_in")),
            MainInput = GetValue(lookup, "main_input"),
            RatiosIn = ParseDoubleList(GetValue(lookup, "ratios_in"), "ratios_in", errors),
            UnitsIn = ParseStringList(GetValue(lookup, "units_in")),
            CarriersOut = ParseStringList(GetValue(lookup, "carriers_out")),
            MainOut = GetValue(lookup, "main_out"),
            RatiosOut = ParseDoubleList(GetValue(lookup, "ratios_out"), "ratios_out", errors),
            UnitsOut = ParseStringList(GetValue(lookup, "units_out")),
            LifetimeYears = ParseDouble(GetValue(lookup, "lifetime_yr"), "lifetime_yr", errors),
            Capex = ParseDecimal(GetValue(lookup, "capex"), "capex", errors),
            CapexUnit = GetValue(lookup, "capex_unit"),
            OpexFix = ParseDecimal(GetValue(lookup, "opex_fix"), "opex_fix", errors),
            OpexFixUnit = GetValue(lookup, "opex_fix_unit"),
        };

        var minInstallRaw = GetValue(lookup, "min_installation_size");
        var minInstallParsed = ParseValueWithUnit(minInstallRaw);
        if (minInstallParsed != null)
        {
            tech.MinInstallationSize = minInstallParsed.Value.value;
            tech.MinInstallationSizeUnit = minInstallParsed.Value.unit;
        }
        else if (!string.IsNullOrWhiteSpace(minInstallRaw))
        {
            errors.Add("min_installation_size: unable to parse numeric value and unit");
        }

        // Always return the record — parse errors are warnings, not fatal.
        // Unparseable optional fields are left null; HasMeaningfulData filters incomplete records.
        classification = tech;
        return true;
    }

    /// <summary>
    /// Looks up a value from a pre-normalised lookup dictionary using one or more possible header names.
    /// </summary>
    private static string? GetValue(Dictionary<string, string> lookup, params string[] headers)
    {
        foreach (var header in headers)
        {
            var key = NormalizeHeader(header);
            if (lookup.TryGetValue(key, out var value))
                return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        return null;
    }

    /// <summary>
    /// Strips non-alphanumeric characters and lowercases the header so that
    /// "Cost Base Year", "cost_base_year", and "CostBaseYear" all match.
    /// </summary>
    private static string NormalizeHeader(string header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return string.Empty;
            
        // Normalize by removing non-alphanumeric and converting to lowercase
        // This allows flexible matching while maintaining case-insensitivity
        var normalized = Regex.Replace(header, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase).ToLowerInvariant();
        return string.IsNullOrEmpty(normalized) ? header.ToLowerInvariant() : normalized;
    }

    /// <summary>Splits a comma-separated string into a list of trimmed strings.</summary>
    private static List<string> ParseStringList(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<string>();

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    /// <summary>Parses a comma-separated list of doubles, collecting errors for unparseable items.</summary>
    private static List<double> ParseDoubleList(string? value, string fieldName, List<string> errors)
    {
        var results = new List<double>();
        if (string.IsNullOrWhiteSpace(value))
            return results;

        var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            if (double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                results.Add(parsed);
            }
            else
            {
                errors.Add($"{fieldName}: invalid number '{part}'");
            }
        }

        return results;
    }

    private static double? ParseDouble(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        errors.Add($"{fieldName}: invalid number '{value}'");
        return null;
    }

    /// <summary>Parses an optional integer, recording an error when the value is present but invalid.</summary>
    private static int? ParseInt(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        errors.Add($"{fieldName}: invalid integer '{value}'");
        return null;
    }

    /// <summary>Parses an optional decimal, recording an error when the value is present but invalid.</summary>
    private static decimal? ParseDecimal(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        errors.Add($"{fieldName}: invalid decimal '{value}'");
        return null;
    }

    /// <summary>Extracts a leading number and optional trailing unit text from a string like "100 MW".</summary>
    private static (double value, string unit)? ParseValueWithUnit(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var match = Regex.Match(value.Trim(), "^(?<num>[-+]?[0-9]*[.]?[0-9]+)\\s*(?<unit>.*)$");
        if (!match.Success)
            return null;

        if (!double.TryParse(match.Groups["num"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            return null;

        var unit = match.Groups["unit"].Value.Trim();
        return (parsed, string.IsNullOrEmpty(unit) ? string.Empty : unit);
    }

    /// <summary>
    /// Stage 1: Builds prompt to find all technology names in the PDF.
    /// </summary>
    public static string BuildFindTechnologiesPrompt(List<string> chunks)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("Give me ALL the technologies you can find in this PDF.");
        sb.AppendLine();
        sb.AppendLine("Be COMPREHENSIVE and include:");
        sb.AppendLine("- Every process, unit operation, and equipment type mentioned");
        sb.AppendLine("- ALL technology variants and subtypes");
        sb.AppendLine("- Technologies mentioned in tables, figures, and text");
        sb.AppendLine("- Both main technologies and supporting/auxiliary technologies");
        sb.AppendLine();
        sb.AppendLine("CRITICAL RULES:");
        sb.AppendLine("- List each technology ONLY ONCE by its base name");
        sb.AppendLine("- Do NOT create separate entries for different years, timeframes, or scenarios");
        sb.AppendLine("  (e.g. list 'Alkaline water electrolysis' once, NOT separately for 2030/2035/2050)");
        sb.AppendLine("- Do NOT create separate entries for different regions or locations");
        sb.AppendLine("  (e.g. list 'Photovoltaic systems' once, NOT separately for Spain/Chile/etc.)");
        sb.AppendLine("- Do NOT add qualifiers like '(near future)', '(long-term)', '(2035)' to names");
        sb.AppendLine("- Do NOT duplicate a technology under both a generic and a specific name");
        sb.AppendLine("  (e.g. list 'Fischer-Tropsch synthesis' OR 'Fischer-Tropsch fuels', not both,");
        sb.AppendLine("   unless the PDF truly treats them as distinct processes)");
        sb.AppendLine("- One technology per line, plain name only");
        sb.AppendLine();
        sb.AppendLine("OUTPUT FORMAT (plain list, one per line):");
        sb.AppendLine("Alkaline water electrolysis");
        sb.AppendLine("Proton exchange membrane electrolysis");
        sb.AppendLine("Solid oxide electrolysis");
        sb.AppendLine();
        
        if (chunks.Count == 1)
        {
            sb.AppendLine("PDF Content:");
            sb.AppendLine(chunks[0]);
        }
        else
        {
            sb.AppendLine($"PDF Content ({chunks.Count} parts):");
            for (int i = 0; i < chunks.Count; i++)
            {
                sb.AppendLine($"--- Part {i + 1} ---");
                sb.AppendLine(chunks[i]);
                sb.AppendLine();
            }
        }
        
        sb.AppendLine();
        sb.AppendLine("Return ONLY the technology names list. No commentary.");
        
        return sb.ToString();
    }

    /// <summary>
    /// Builds a prompt to extract detailed free-text summaries for multiple
    /// technologies at once. Used by the <c>auto-summarize</c> command.
    /// </summary>
    public static string BuildBatchDetailedExtractionPrompt(List<string> chunks, List<string> technologyNames)
    {
        var sb = new StringBuilder();

        sb.AppendLine("You are an expert energy systems data extractor specialised in techno-economic analysis. Extract data precisely.");
        sb.AppendLine();
        sb.AppendLine($"TASK: Extract ALL data for these {technologyNames.Count} technologies from this PDF:");
        sb.AppendLine();
        for (int i = 0; i < technologyNames.Count; i++)
            sb.AppendLine($"{i + 1}. {technologyNames[i]}");
        sb.AppendLine();
        sb.AppendLine("For EACH technology listed above, extract and report EVERYTHING you find:");
        sb.AppendLine("- Process description and operating conditions");
        sb.AppendLine("- ALL inputs (materials, energy, consumables) with quantities and units");
        sb.AppendLine("- ALL outputs (products, byproducts) with quantities and units");
        sb.AppendLine("- CAPEX (capital costs) in any format mentioned");
        sb.AppendLine("- OPEX (operating costs) as % or absolute values");
        sb.AppendLine("- Efficiency values and units");
        sb.AppendLine("- Technology readiness level (TRL) or maturity");
        sb.AppendLine("- Lifetime, reference capacity, or scale information");
        sb.AppendLine("- Year/time horizon the data applies to");
        sb.AppendLine("- Location or region if specified");
        sb.AppendLine("- LCA or environmental impact data if mentioned, like GHG emissions");
        sb.AppendLine("- Any other technical or economic data you find relevant, like unit size, energy requirements, etc.");
        sb.AppendLine();
        sb.AppendLine("BE COMPREHENSIVE:");
        sb.AppendLine("- Include ALL numeric values you find (even if scattered across pages)");
        sb.AppendLine("- Report units exactly as stated (MWh/t, kg/t, EUR/kW, etc.)");
        sb.AppendLine("- If data varies by location, report baseline + variations");
        sb.AppendLine("- Do NOT create separate technology sections for different years or scenarios");
        sb.AppendLine("- Within each technology section, organise data into sub-sections by year/time horizon");
        sb.AppendLine();
        sb.AppendLine("OUTPUT FORMAT:");
        sb.AppendLine("=== TECHNOLOGY 1: [Name] ===");
        sb.AppendLine();
        sb.AppendLine("--- Year: 2025 (current/baseline) ---");
        sb.AppendLine("[All data for this technology at this year]");
        sb.AppendLine();
        sb.AppendLine("--- Year: 2035 (near future) ---");
        sb.AppendLine("[All data for this technology at this year]");
        sb.AppendLine();
        sb.AppendLine("--- Year: 2050 (long-term) ---");
        sb.AppendLine("[All data for this technology at this year]");
        sb.AppendLine();
        sb.AppendLine("=== TECHNOLOGY 2: [Name] ===");
        sb.AppendLine("[same sub-section structure by year]");
        sb.AppendLine("etc.");
        sb.AppendLine();

        if (chunks.Count == 1)
        {
            sb.AppendLine("PDF Content:");
            sb.AppendLine(chunks[0]);
        }
        else
        {
            sb.AppendLine($"PDF Content ({chunks.Count} parts):");
            for (int i = 0; i < chunks.Count; i++)
            {
                sb.AppendLine($"--- Part {i + 1} ---");
                sb.AppendLine(chunks[i]);
                sb.AppendLine();
            }
        }

        sb.AppendLine();
        sb.AppendLine($"Return detailed summaries for ALL {technologyNames.Count} technologies listed above.");
        return sb.ToString();
    }

    /// <summary>
    /// Parses a batched extraction response into individual technology detail sections.
    /// Expects sections separated by <c>=== TECHNOLOGY N: [Name] ===</c>.
    /// </summary>
    public static List<string> ParseBatchedExtractionResponse(string response, int expectedCount)
    {
        var details = new List<string>();
        if (string.IsNullOrWhiteSpace(response))
        {
            for (int i = 0; i < expectedCount; i++)
                details.Add($"No data found for technology {i + 1}");
            return details;
        }

        // Split by the section markers
        var sections = Regex.Split(response, @"===\s*TECHNOLOGY\s+\d+:.*?===", RegexOptions.IgnoreCase);

        // First section is usually intro text before the first technology — skip it
        for (int i = 1; i < sections.Length; i++)
        {
            var section = sections[i].Trim();
            if (!string.IsNullOrWhiteSpace(section))
                details.Add(section);
        }

        // Fallback: split the response evenly if markers were not found
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

    /// <summary>
    /// Builds a single prompt that reads the PDF content and directly produces
    /// structured JSON for a batch of technology names — replacing the old
    /// Stage 2 (free-text extraction) + Stage 3 (JSON conversion) two-step flow.
    /// The PDF content is sent only once per batch instead of twice.
    /// </summary>
    public static string BuildDirectExtractionPrompt(List<string> chunks, List<string> technologyNames)
    {
        if (chunks == null || chunks.Count == 0)
            throw new ArgumentException("PDF chunks cannot be null or empty");
        if (technologyNames == null || technologyNames.Count == 0)
            throw new ArgumentException("Technology names cannot be null or empty");

        var sb = new StringBuilder();

        sb.AppendLine($"TASK: For each of the {technologyNames.Count} technologies listed below, find ALL relevant data in the PDF and return a JSON array.");
        sb.AppendLine();

        // Technology list
        for (int i = 0; i < technologyNames.Count; i++)
            sb.AppendLine($"  {i + 1}. {technologyNames[i]}");
        sb.AppendLine();

        AppendJsonSchemaAndRules(sb, "PDF");

        // Append PDF content
        if (chunks.Count == 1)
        {
            sb.AppendLine("PDF CONTENT:");
            sb.AppendLine(chunks[0]);
        }
        else
        {
            sb.AppendLine($"PDF CONTENT ({chunks.Count} parts):");
            for (int i = 0; i < chunks.Count; i++)
            {
                sb.AppendLine($"--- Part {i + 1} ---");
                sb.AppendLine(chunks[i]);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Builds a prompt that classifies technologies from a pre-extracted TXT
    /// summary (produced by <c>auto-summarize</c>) instead of raw PDF content.
    /// </summary>
    public static string BuildClassificationFromSummaryPrompt(
        List<(string Name, string Content)> technologySections)
    {
        if (technologySections == null || technologySections.Count == 0)
            throw new ArgumentException("Technology sections cannot be null or empty");

        var sb = new StringBuilder();

        sb.AppendLine($"TASK: Convert the following {technologySections.Count} technology summaries into a JSON array.");
        sb.AppendLine("The summaries were previously extracted from a PDF and are organised by technology and year.");
        sb.AppendLine();

        AppendJsonSchemaAndRules(sb, "summary");

        sb.AppendLine("TECHNOLOGY SUMMARIES:");
        sb.AppendLine();
        foreach (var (name, content) in technologySections)
        {
            sb.AppendLine($"=== {name} ===");
            sb.AppendLine(content);
            sb.AppendLine();
        }

        sb.AppendLine("OUTPUT INSTRUCTIONS (mandatory):");
        sb.AppendLine("- Your entire response MUST be a single raw JSON array: [ ... ]");
        sb.AppendLine("- Start your response with [ and end it with ]");
        sb.AppendLine("- Do NOT write any explanation, preamble, summary, or markdown");
        sb.AppendLine("- Do NOT say what you are doing — just output the JSON");

        return sb.ToString();
    }

    /// <summary>
    /// Builds a short follow-up prompt for batches 2+ during auto-classify.
    /// The schema and rules are already in the session context from batch 1,
    /// so only the new technology summaries are sent.
    /// </summary>
    public static string BuildFollowUpClassificationPrompt(
        List<(string Name, string Content)> technologySections)
    {
        if (technologySections == null || technologySections.Count == 0)
            throw new ArgumentException("Technology sections cannot be null or empty");

        var sb = new StringBuilder();
        sb.AppendLine($"Same schema and rules as before. Convert these {technologySections.Count} technology summaries into a JSON array:");
        sb.AppendLine();
        foreach (var (name, content) in technologySections)
        {
            sb.AppendLine($"=== {name} ===");
            sb.AppendLine(content);
            sb.AppendLine();
        }
        sb.AppendLine("Return ONLY the JSON array.");
        return sb.ToString();
    }

    /// <summary>
    /// Shared helper: appends the JSON schema and extraction rules to a prompt.
    /// </summary>
    private static void AppendJsonSchemaAndRules(StringBuilder sb, string sourceLabel)
    {
        sb.AppendLine("You are an expert of energy systems data extractor specialised in techno-economic analysis. Extract data precisely and return only valid JSON.");
        sb.AppendLine();
        sb.AppendLine("JSON Schema:");
        sb.AppendLine("[{");
        sb.AppendLine("  \"Datapaper Tech ID\": \"id\",");
        sb.AppendLine("  \"description\": \"1-2 sentences\",");
        sb.AppendLine("  \"summary\": \"paragraph\",");
        sb.AppendLine("  \"unit_operation\": \"name\",");
        sb.AppendLine("  \"ProcessType\": \"e.g. Conversion, Storage, Capture, Transport, EndUse, etc (what it does)\",");
        sb.AppendLine("  \"main_sector\": \"e.g. Electricity, Heat, Chemicals, Fuels, Industry, Buildings, etc (broadest)\",");
        sb.AppendLine("  \"main_category\": \"e.g. Electrolysis, CO2 Capture, Syngas Production, etc (field)\",");
        sb.AppendLine("  \"category_spec\": \"e.g. Alkaline, PEM, Solid sorbent, Aqueous, etc (type)\",");
        sb.AppendLine("  \"tech_type\": \"specific name found in source (most specific)\",");
        sb.AppendLine("  \"carriers_in\": \"c1,c2,c3 (any carriers)\",");
        sb.AppendLine("  \"main_input\": \"primary carrier\",");
        sb.AppendLine("  \"ratios_in\": \"r1,r2,r3\",");
        sb.AppendLine("  \"units_in\": \"u1,u2,u3\",");
        sb.AppendLine("  \"carriers_out\": \"c1,c2,c3 (any carriers)\",");
        sb.AppendLine("  \"main_out\": \"primary carrier\",");
        sb.AppendLine("  \"ratios_out\": \"r1,r2,r3\",");
        sb.AppendLine("  \"units_out\": \"u1,u2,u3\",");
        sb.AppendLine("  \"reference_unit_size\": <num|null>,");
        sb.AppendLine("  \"reference_unit_size_unit\": \"e.g. MW, t/yr, kg/s (any unit found)\",");
        sb.AppendLine("  \"efficiency\": <0-1 decimal|null> (prefer LHV if available),");
        sb.AppendLine("  \"efficiency_unit\": \"e.g. %, kWh/kg, J/mol (any unit found)\",");
        sb.AppendLine("  \"trl_(1-9)\": <1-9|null>,");
        sb.AppendLine("  \"tech_maturity\": \"e.g. Mature, Developing, Emerging (use source terminology)\",");
        sb.AppendLine("  \"base_year\": <year|null>,");
        sb.AppendLine("  \"location\": \"e.g. Germany, Europe, Chile, Iceland (any location)\",");
        sb.AppendLine("  \"Currency\": \"e.g. EUR, USD, GBP (any currency found)\",");
        sb.AppendLine("  \"capex\": <num|null>,");
        sb.AppendLine("  \"capex_unit\": \"e.g. EUR, EUR/kW, EUR/t (any unit found)\",");
        sb.AppendLine("  \"opex_fix\": <num|null>,");
        sb.AppendLine("  \"opex_fix_unit\": \"e.g. EUR/year, % of Capex, EUR/kW/year (any unit)\",");
        sb.AppendLine("  \"lifetime_yr\": <num|null>,");
        sb.AppendLine("  \"Data Reference Year\": <year|null>");
        sb.AppendLine("}]");
        sb.AppendLine();

        sb.AppendLine("HIERARCHY (General → Specific):");
        sb.AppendLine("- ProcessType, main_sector, main_category, category_spec, tech_type: classify the technology into a hierarchy");
        sb.AppendLine();

        sb.AppendLine("Rules:");
        sb.AppendLine("- One object per technology; multiple years → separate objects");
        sb.AppendLine($"- Use null where {sourceLabel} has no data");
        sb.AppendLine("- efficiency: 0-1 decimal (65% → 0.65, prefer LHV)");
        sb.AppendLine("- ratios: one per carrier, same order as carriers");
        sb.AppendLine("- costs: convert to single currency number (€28.4M → 28400000)");
        sb.AppendLine("- Return ONLY JSON array, no markdown or commentary");
        sb.AppendLine();
    }

    /// <summary>
    /// Parses technology names from Stage 1 response.
    /// </summary>
    public static List<string> ParseTechnologyNames(string response)
    {
        var names = new List<string>();
        if (string.IsNullOrWhiteSpace(response))
            return names;
        
        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var line in lines)
        {
            // Skip markdown headers or very short lines
            if (line.StartsWith("#") || line.Length < 3)
                continue;
            
            // Skip lines that are just section headers (not technical names)
            var lower = line.ToLower();
            if ((lower.Contains("technology") || lower.Contains("list")) && 
                (lower.StartsWith("technology") || lower.StartsWith("list") || lower.EndsWith(":")))
                continue;
                
            // Clean up numbering, bullets, etc.
            var cleaned = Regex.Replace(line, @"^\d+\.\s*", ""); // Remove "1. "
            cleaned = Regex.Replace(cleaned, @"^[-*•]\s*", "");   // Remove "- " or "* "
            cleaned = cleaned.Trim();
            
            // Additional validation: must have some alphabetic characters
            if (!string.IsNullOrWhiteSpace(cleaned) && Regex.IsMatch(cleaned, @"[a-zA-Z]{2,}"))
                names.Add(cleaned);
        }
        
        return names;
    }

    /// <summary>
    /// Extracts JSON from a Copilot response, handling markdown fences and direct JSON.
    /// </summary>
    public static string ExtractJson(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return string.Empty;

        // Strip markdown code fences (```json ... ``` or ``` ... ```)
        // Use a greedy inner match to capture the last closing fence.
        var fenceMatch = Regex.Match(response,
            @"```(?:\w*)\s*\r?\n?(?<json>[\s\S]*?)\r?\n?\s*```",
            RegexOptions.IgnoreCase);
        if (fenceMatch.Success)
        {
            var inner = fenceMatch.Groups["json"].Value.Trim();
            if (inner.Length > 0)
                return inner;
        }

        // Brute-force: strip all leading/trailing backticks and language tags
        var cleaned = response.Trim();
        while (cleaned.StartsWith("`"))
            cleaned = cleaned.TrimStart('`');
        cleaned = Regex.Replace(cleaned, @"^json\s*", "", RegexOptions.IgnoreCase).TrimStart();
        while (cleaned.EndsWith("`"))
            cleaned = cleaned.TrimEnd('`');
        cleaned = cleaned.Trim();

        // Try direct JSON bracket match
        var start = cleaned.IndexOf('[');
        var end = cleaned.LastIndexOf(']');
        if (start >= 0 && end > start)
        {
            var extracted = cleaned.Substring(start, end - start + 1).Trim();
            if (extracted.StartsWith("[") && extracted.EndsWith("]"))
                return extracted;
        }

        // No valid JSON array found — return empty to avoid passing prose to the parser
        return string.Empty;
    }

    /// <summary>
    /// Parses JSON array into a list of dictionaries for classification.
    /// </summary>
    public static List<Dictionary<string, string>> ParseRowsFromJson(string json)
    {
        var rows = new List<Dictionary<string, string>>();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            return rows;

        foreach (var element in doc.RootElement.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.Object)
                continue;

            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in element.EnumerateObject())
            {
                var value = ConvertJsonValueToString(prop.Value);
                if (value != null)
                    row[prop.Name] = value;
            }

            rows.Add(row);
        }

        return rows;
    }

    /// <summary>
    /// Converts a JSON element to a string representation.
    /// </summary>
    private static string? ConvertJsonValueToString(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                return element.GetRawText();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean() ? "true" : "false";
            case JsonValueKind.Array:
                var items = new List<string>();
                foreach (var item in element.EnumerateArray())
                {
                    var value = ConvertJsonValueToString(item);
                    if (!string.IsNullOrWhiteSpace(value))
                        items.Add(value);
                }
                return string.Join(", ", items);
            case JsonValueKind.Object:
                return element.ToString();
            default:
                return element.ToString();
        }
    }

    /// <summary>
    /// Generates a unique technology ID from classification data.
    /// </summary>
    public static string GenerateTechId(TechnologyClassification tech, HashSet<string> usedIds)
    {
        var parts = new List<string>();

        // Extract from main carriers
        if (!string.IsNullOrWhiteSpace(tech.MainInput))
            parts.Add(ExtractAbbreviation(tech.MainInput));
        else if (tech.CarriersIn.Count > 0)
            parts.Add(ExtractAbbreviation(tech.CarriersIn[0]));

        // Extract from unit operation
        if (!string.IsNullOrWhiteSpace(tech.UnitOperation))
        {
            var abbrev = ExtractAbbreviation(tech.UnitOperation);
            if (!parts.Contains(abbrev))
                parts.Add(abbrev);
        }

        // Extract key words from process type
        if (!string.IsNullOrWhiteSpace(tech.ProcessType))
        {
            var abbrev = ExtractAbbreviation(tech.ProcessType);
            if (!parts.Contains(abbrev))
                parts.Add(abbrev);
        }

        // Extract from main output if not already included
        if (!string.IsNullOrWhiteSpace(tech.MainOut) && parts.Count < 4)
        {
            var abbrev = ExtractAbbreviation(tech.MainOut);
            if (!parts.Contains(abbrev))
                parts.Add(abbrev);
        }

        var baseId = string.Join("_", parts).ToUpperInvariant();
        if (baseId.Length > 20)
            baseId = baseId[..Math.Min(20, baseId.Length)];

        // Ensure uniqueness
        if (!usedIds.Contains(baseId))
            return baseId;

        var counter = 2;
        while (usedIds.Contains($"{baseId}_{counter}"))
            counter++;
        return $"{baseId}_{counter}";
    }

    /// <summary>
    /// Extracts abbreviation from technology term.
    /// </summary>
    public static string ExtractAbbreviation(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "UNK";

        text = text.Trim();

        // Known mappings (technology specific)
        var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Alkaline", "AEC" },
            { "PEM", "PEM" },
            { "SOEC", "SOEC" },
            { "Fischer-Tropsch", "FT" },
            { "Methanation", "MET" },
            { "Methane", "CH4" },
            { "Ammonia", "NH3" },
            { "Haber-Bosch", "HB" },
            { "Direct Air Capture", "DAC" },
            { "DAC", "DAC" },
            { "Water-Gas Shift", "WGS" },
            { "CO2 reduction", "CO2R" },
            { "Electrochemical", "EC" },
            { "Hydrogen", "H2" },
            { "Oxygen", "O2" },
            { "Carbon monoxide", "CO" },
            { "Urea", "UREA" },
            { "Dimethyl ether", "DME" },
            { "Methanol", "MEOH" }
        };

        // Check exact matches first
        foreach (var kvp in mappings)
        {
            if (text.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        // Check if text contains known abbreviations
        foreach (var kvp in mappings)
        {
            if (text.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        // Generate abbreviation from first letters of words
        var words = Regex.Split(text, @"\W+").Where(w => !string.IsNullOrEmpty(w)).ToList();
        if (words.Count > 0)
        {
            if (words.Count == 1)
                return words[0].Length > 3 ? words[0].Substring(0, 3).ToUpperInvariant() : words[0].ToUpperInvariant();
            else
                return string.Concat(words.Select(w => w[0])).ToUpperInvariant();
        }

        return "UNK";
    }

    /// <summary>
    /// Checks if a technology classification has meaningful data.
    /// </summary>
    public static bool HasMeaningfulData(TechnologyClassification tech)
    {
        // Require at least 2-3 meaningful fields to avoid keeping nearly-empty rows
        int fieldCount = 0;
        
        if (!string.IsNullOrWhiteSpace(tech.Description)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.UnitOperation)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.Summary)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.ProcessType)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainSector)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainCategory)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.CategorySpec)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.TechType)) fieldCount++;
        if (tech.BaseYear.HasValue) fieldCount++;
        if (tech.DataReferenceYear.HasValue) fieldCount++;
        if (tech.Trl.HasValue) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.TechMaturity)) fieldCount++;
        if (tech.OverallEfficiency.HasValue) fieldCount++;
        if (tech.CarriersIn.Count > 0 || tech.CarriersOut.Count > 0) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainInput) || !string.IsNullOrWhiteSpace(tech.MainOut)) fieldCount++;
        if (tech.LifetimeYears.HasValue || tech.Capex.HasValue || tech.OpexFix.HasValue) fieldCount++;

        // Require minimum 2 fields populated
        return fieldCount >= 2;
    }

    /// <summary>
    /// Merges rows that describe the same technology/year combination, filling in
    /// missing fields from duplicate entries rather than creating extra rows.
    /// </summary>
    public static List<TechnologyClassification> MergeByTechnologyAndYear(IEnumerable<TechnologyClassification> rows)
    {
        var merged = new List<TechnologyClassification>();
        var indexByKey = new Dictionary<string, int>(KeyComparer);

        var rowIndex = 0;
        foreach (var row in rows)
        {
            var key = BuildTechnologyYearKey(row, rowIndex);
            if (indexByKey.TryGetValue(key, out var index))
            {
                MergeMissingFields(merged[index], row);
            }
            else
            {
                merged.Add(Clone(row));
                indexByKey[key] = merged.Count - 1;
            }

            rowIndex++;
        }

        EnsureUniqueDatapaperIds(merged);
        return merged;
    }

    /// <summary>
    /// Builds a conservative merge key from technology name + explicit year source (+ variant).
    /// Rows without a valid year are intentionally kept unique to prevent accidental cross-year merges.
    /// </summary>
    private static string BuildTechnologyYearKey(TechnologyClassification row, int rowIndex)
    {
        var technologyName = FirstNonEmpty(
            row.TechType,
            row.Description,
            row.UnitOperation,
            row.MainCategory,
            row.ProcessType,
            row.DatapaperTechId,
            "unknown");

        var normalizedTechnology = NormalizeKeyText(technologyName);

        var dataReferenceYear = NormalizeYear(row.DataReferenceYear);
        var baseYear = NormalizeYear(row.BaseYear);

        if (!dataReferenceYear.HasValue && !baseYear.HasValue)
        {
            return $"{normalizedTechnology}|UNMERGEABLE|{rowIndex}";
        }

        var yearToken = dataReferenceYear.HasValue && baseYear.HasValue
            ? $"DR{dataReferenceYear.Value}|BY{baseYear.Value}"
            : dataReferenceYear.HasValue
                ? $"DR{dataReferenceYear.Value}"
                : $"BY{baseYear!.Value}";

        // Include category/tech variants to avoid collapsing distinct sub-types.
        var categorySpec = NormalizeKeyText(row.CategorySpec ?? "");
        var variant = NormalizeKeyText(row.TechType ?? "");

        if (!string.IsNullOrWhiteSpace(categorySpec))
            return $"{normalizedTechnology}|{yearToken}|{categorySpec}";

        if (!string.IsNullOrWhiteSpace(variant) && variant != normalizedTechnology)
            return $"{normalizedTechnology}|{yearToken}|{variant}";

        return $"{normalizedTechnology}|{yearToken}";
    }

    private static int? NormalizeYear(int? year) => year is >= 1900 ? year : null;

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return string.Empty;
    }

    private static string NormalizeKeyText(string value)
    {
        var lowered = value.Trim().ToLowerInvariant();
        return Regex.Replace(lowered, @"\s+", " ");
    }

    /// <summary>Creates a deep copy of a <see cref="TechnologyClassification"/>.</summary>
    private static TechnologyClassification Clone(TechnologyClassification source)
    {
        return new TechnologyClassification
        {
            DatapaperTechId = source.DatapaperTechId,
            ProcessType = source.ProcessType,
            Description = source.Description,
            UnitOperation = source.UnitOperation,
            Summary = source.Summary,
            MainSector = source.MainSector,
            MainCategory = source.MainCategory,
            CategorySpec = source.CategorySpec,
            TechType = source.TechType,
            ReferenceUnitSize = source.ReferenceUnitSize,
            ReferenceUnitSizeUnit = source.ReferenceUnitSizeUnit,
            BaseYear = source.BaseYear,
            Location = source.Location,
            Currency = source.Currency,
            DataReferenceYear = source.DataReferenceYear,
            Trl = source.Trl,
            TechMaturity = source.TechMaturity,
            OverallEfficiency = source.OverallEfficiency,
            EfficiencyUnit = source.EfficiencyUnit,
            CarriersIn = new List<string>(source.CarriersIn),
            MainInput = source.MainInput,
            RatiosIn = new List<double>(source.RatiosIn),
            UnitsIn = new List<string>(source.UnitsIn),
            CarriersOut = new List<string>(source.CarriersOut),
            MainOut = source.MainOut,
            RatiosOut = new List<double>(source.RatiosOut),
            UnitsOut = new List<string>(source.UnitsOut),
            MinInstallationSize = source.MinInstallationSize,
            MinInstallationSizeUnit = source.MinInstallationSizeUnit,
            LifetimeYears = source.LifetimeYears,
            Capex = source.Capex,
            CapexUnit = source.CapexUnit,
            OpexFix = source.OpexFix,
            OpexFixUnit = source.OpexFixUnit
        };
    }

    /// <summary>Copies non-null scalar fields and non-empty lists from <paramref name="source"/> into <paramref name="target"/>.</summary>
    private static void MergeMissingFields(TechnologyClassification target, TechnologyClassification source)
    {
        target.DatapaperTechId ??= source.DatapaperTechId;
        target.ProcessType ??= source.ProcessType;
        target.Description ??= source.Description;
        target.UnitOperation ??= source.UnitOperation;
        target.Summary ??= source.Summary;
        target.MainSector ??= source.MainSector;
        target.MainCategory ??= source.MainCategory;
        target.CategorySpec ??= source.CategorySpec;
        target.TechType ??= source.TechType;
        target.ReferenceUnitSize ??= source.ReferenceUnitSize;
        target.ReferenceUnitSizeUnit ??= source.ReferenceUnitSizeUnit;
        target.BaseYear ??= source.BaseYear;
        target.Location ??= source.Location;
        target.Currency ??= source.Currency;
        target.DataReferenceYear ??= source.DataReferenceYear;
        target.Trl ??= source.Trl;
        target.TechMaturity ??= source.TechMaturity;
        target.OverallEfficiency ??= source.OverallEfficiency;
        target.EfficiencyUnit ??= source.EfficiencyUnit;
        target.MainInput ??= source.MainInput;
        target.MainOut ??= source.MainOut;
        target.MinInstallationSize ??= source.MinInstallationSize;
        target.MinInstallationSizeUnit ??= source.MinInstallationSizeUnit;
        target.LifetimeYears ??= source.LifetimeYears;
        target.Capex ??= source.Capex;
        target.CapexUnit ??= source.CapexUnit;
        target.OpexFix ??= source.OpexFix;
        target.OpexFixUnit ??= source.OpexFixUnit;

        if (target.CarriersIn.Count == 0 && source.CarriersIn.Count > 0) target.CarriersIn = new List<string>(source.CarriersIn);
        if (target.RatiosIn.Count == 0 && source.RatiosIn.Count > 0) target.RatiosIn = new List<double>(source.RatiosIn);
        if (target.UnitsIn.Count == 0 && source.UnitsIn.Count > 0) target.UnitsIn = new List<string>(source.UnitsIn);
        if (target.CarriersOut.Count == 0 && source.CarriersOut.Count > 0) target.CarriersOut = new List<string>(source.CarriersOut);
        if (target.RatiosOut.Count == 0 && source.RatiosOut.Count > 0) target.RatiosOut = new List<double>(source.RatiosOut);
        if (target.UnitsOut.Count == 0 && source.UnitsOut.Count > 0) target.UnitsOut = new List<string>(source.UnitsOut);
    }

    /// <summary>Ensures every row has a unique <c>DatapaperTechId</c>, appending a numeric suffix when needed.</summary>
    private static void EnsureUniqueDatapaperIds(List<TechnologyClassification> rows)
    {
        var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.DatapaperTechId))
                row.DatapaperTechId = GenerateTechId(row, usedIds);

            var id = row.DatapaperTechId!;
            if (!usedIds.Add(id))
            {
                var counter = 2;
                var candidate = $"{id}_{counter}";
                while (!usedIds.Add(candidate))
                {
                    counter++;
                    candidate = $"{id}_{counter}";
                }
                row.DatapaperTechId = candidate;
            }
        }
    }
}

/// <summary>
/// Reads and writes <see cref="TechnologyClassification"/> records as RFC-4180 CSV files.
/// </summary>
public static class TechnologyClassificationCsv
{
    /// <summary>Column order used in the output CSV header row.</summary>
    public static readonly string[] HeaderOrder =
    {
        "Datapaper Tech ID",
        "ProcessType",
        "description",
        "unit_operation",
        "main_sector",
        "main_category",
        "category_spec",
        "tech_type",
        "base_year",
        "reference_unit_size",
        "reference_unit_size_unit",
        "location",
        "Currency",
        "trl_(1-9)",
        "tech_maturity",
        "efficiency",
        "efficiency_unit",
        "carriers_in",
        "main_input",
        "ratios_in",
        "units_in",
        "carriers_out",
        "main_out",
        "ratios_out",
        "units_out",
        "capex",
        "capex_unit",
        "opex_fix",
        "opex_fix_unit",
        "lifetime_yr",
        "Data Reference Year",
        "summary"
    };

    /// <summary>Writes a collection of classification records to a UTF-8 CSV file.</summary>
    public static void WriteCsv(string filePath, IEnumerable<TechnologyClassification> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', HeaderOrder.Select(EscapeCsv)));

        foreach (var row in rows)
        {
            var fields = new List<string>
            {
                row.DatapaperTechId ?? string.Empty,
                row.ProcessType ?? string.Empty,
                row.Description ?? string.Empty,
                row.UnitOperation ?? string.Empty,
                row.MainSector ?? string.Empty,
                row.MainCategory ?? string.Empty,
                row.CategorySpec ?? string.Empty,
                row.TechType ?? string.Empty,
                FormatInt(row.BaseYear),
                FormatDouble(row.ReferenceUnitSize),
                row.ReferenceUnitSizeUnit ?? string.Empty,
                row.Location ?? string.Empty,
                row.Currency ?? string.Empty,
                FormatInt(row.Trl),
                row.TechMaturity ?? string.Empty,
                FormatDouble(row.OverallEfficiency),
                row.EfficiencyUnit ?? string.Empty, 
                JoinList(row.CarriersIn.Cast<string?>()),
                row.MainInput ?? string.Empty,
                JoinList(row.RatiosIn.Select(d => FormatDouble(d)).Cast<string?>()),
                JoinList(row.UnitsIn.Cast<string?>()),
                JoinList(row.CarriersOut.Cast<string?>()),
                row.MainOut ?? string.Empty,
                JoinList(row.RatiosOut.Select(d => FormatDouble(d)).Cast<string?>()),
                JoinList(row.UnitsOut.Cast<string?>()),
                FormatDecimal(row.Capex),     
                row.CapexUnit ?? string.Empty, 
                FormatDecimal(row.OpexFix),   
                row.OpexFixUnit ?? string.Empty,
                FormatDouble(row.LifetimeYears),
                FormatInt(row.DataReferenceYear),
                row.Summary ?? string.Empty
            };

            sb.AppendLine(string.Join(',', fields.Select(EscapeCsv)));
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    /// <summary>Reads an existing CSV back into a list of classification records.</summary>
    public static List<TechnologyClassification> ReadCsv(string filePath)
    {
        var results = new List<TechnologyClassification>();

        if (!File.Exists(filePath))
            return results;

        var content = File.ReadAllText(filePath, Encoding.UTF8);
        var records = ParseCsvRecords(content);
        if (records.Count <= 1)
            return results;

        var headers = records[0];
        for (int i = 1; i < records.Count; i++)
        {
            var values = records[i];
            if (values.All(string.IsNullOrWhiteSpace))
                continue;

            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int col = 0; col < headers.Count; col++)
            {
                var value = col < values.Count ? values[col] : string.Empty;
                row[headers[col]] = value;
            }

            if (TechnologyClassifier.TryClassify(row, out var classification, out _))
                results.Add(classification);
        }

        return results;
    }

    /// <summary>
    /// Hand-rolled CSV parser that correctly handles quoted fields containing
    /// commas, newlines, and escaped double-quotes.
    /// </summary>
    private static List<List<string>> ParseCsvRecords(string content)
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

                currentRecord = new List<string>();
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

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var needsQuotes = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
        var escaped = value.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{escaped}\"" : escaped;
    }

    private static string JoinList(IEnumerable<string?> values)
    {
        return string.Join(", ", values.Where(v => !string.IsNullOrWhiteSpace(v))!);
    }

    private static string FormatDouble(double? value)
    {
        return value?.ToString("0.########", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static string FormatDecimal(decimal? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static string FormatInt(int? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static string FormatValueWithUnit(double? value, string? unit)
    {
        if (value == null)
            return string.Empty;

        var formatted = FormatDouble(value);
        return string.IsNullOrWhiteSpace(unit) ? formatted : $"{formatted} {unit}";
    }

    private static string FormatOverallEfficiencyIo(string? input, string? output)
    {
        if (string.IsNullOrWhiteSpace(input) && string.IsNullOrWhiteSpace(output))
            return string.Empty;

        if (string.IsNullOrWhiteSpace(output))
            return input ?? string.Empty;

        if (string.IsNullOrWhiteSpace(input))
            return output;

        return $"{input}, {output}";
    }
}
