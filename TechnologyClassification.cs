using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PdfAnalysisApp;

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
    public int? CostBaseYear { get; set; }
    public string? CostBaseLocation { get; set; }
    public string? Currency { get; set; }
    public int? DataReferenceYear { get; set; }
    public int? Trl { get; set; }
    public string? TechMaturity { get; set; }
    public double? OverallEfficiency { get; set; }
    public string? OverallEfficiencyInputCarrier { get; set; }
    public string? OverallEfficiencyOutputCarrier { get; set; }
    public List<string> CarriersIn { get; set; } = new();
    public string? MainInput { get; set; }
    public List<double> InputShares { get; set; } = new();
    public List<string> InputShareUnits { get; set; } = new();
    public List<double> RatiosIn { get; set; } = new();
    public List<string> UnitsIn { get; set; } = new();
    public List<string> CarriersOut { get; set; } = new();
    public string? MainOut { get; set; }
    public List<double> RatiosOut { get; set; } = new();
    public List<string> UnitsOut { get; set; } = new();
    public List<double> OutputShares { get; set; } = new();
    public List<string> OutputShareUnits { get; set; } = new();
    public double? MinInstallationSize { get; set; }
    public string? MinInstallationSizeUnit { get; set; }
    public double? LifetimeYears { get; set; }
    public decimal? CapexOneTimeEur { get; set; }
    public decimal? CapexPowerCapacityEurPerKw { get; set; }
    public decimal? OpexOneTimeEur { get; set; }
    public double? OpexFixPctOfCapex { get; set; }
    public decimal? OpexFixPowerCapacityEurPerKwYr { get; set; }
}

public static class TechnologyClassifier
{
    public static bool TryClassify(
        IDictionary<string, string> row,
        out TechnologyClassification classification,
        out List<string> errors)
    {
        errors = new List<string>();

        var tech = new TechnologyClassification
        {
            DatapaperTechId = GetValue(row, "Datapaper Tech ID", "tech_id"),
            ProcessType = GetValue(row, "ProcessType", "process_type"),
            Description = GetValue(row, "description"),
            UnitOperation = GetValue(row, "unit_operation"),
            Summary = GetValue(row, "summary"),
            MainSector = GetValue(row, "main_sector"),
            MainCategory = GetValue(row, "main_category"),
            CategorySpec = GetValue(row, "category_spec"),
            TechType = GetValue(row, "tech_type"),
            ReferenceUnitSize = ParseDouble(GetValue(row, "reference_unit_size"), "reference_unit_size", errors),
            ReferenceUnitSizeUnit = GetValue(row, "reference_unit_size_unit", "Reference Unit Size Unit"),
            CostBaseYear = ParseInt(GetValue(row, "cost_base_year"), "cost_base_year", errors),
            CostBaseLocation = GetValue(row, "Cost Base", "cost_base", "CostBase", "Location"),
            Currency = GetValue(row, "Currency", "currency"),
            DataReferenceYear = ParseInt(GetValue(row, "Data Reference Year", "data_reference_year"), "data_reference_year", errors),
            Trl = ParseInt(GetValue(row, "trl_(1-9)", "trl"), "trl_(1-9)", errors),
            TechMaturity = GetValue(row, "tech_maturity"),
            OverallEfficiency = ParseDouble(GetValue(row, "overall_efficiency"), "overall_efficiency", errors),
            CarriersIn = ParseStringList(GetValue(row, "carriers_in")),
            MainInput = GetValue(row, "main_input"),
            InputShares = ParseDoubleList(GetValue(row, "Input Shares", "input_shares"), "input_shares", errors),
            InputShareUnits = ParseStringList(GetValue(row, "Input Units - Shares", "input_units_shares")),
            RatiosIn = ParseDoubleList(GetValue(row, "ratios_in"), "ratios_in", errors),
            UnitsIn = ParseStringList(GetValue(row, "units_in")),
            CarriersOut = ParseStringList(GetValue(row, "carriers_out")),
            MainOut = GetValue(row, "main_out"),
            RatiosOut = ParseDoubleList(GetValue(row, "ratios_out"), "ratios_out", errors),
            UnitsOut = ParseStringList(GetValue(row, "units_out")),
            OutputShares = ParseDoubleList(GetValue(row, "Output Shares", "output_shares"), "output_shares", errors),
            OutputShareUnits = ParseStringList(GetValue(row, "Output Units - Shares", "output_units_shares")),
            LifetimeYears = ParseDouble(GetValue(row, "lifetime_yr"), "lifetime_yr", errors),
            CapexOneTimeEur = ParseDecimal(GetValue(row, "capex_one_time_eur"), "capex_one_time_eur", errors),
            CapexPowerCapacityEurPerKw = ParseDecimal(GetValue(row, "capex_power_capacity_eur_per_kw"), "capex_power_capacity_eur_per_kw", errors),
            OpexOneTimeEur = ParseDecimal(GetValue(row, "opex_one_time_eur"), "opex_one_time_eur", errors),
            OpexFixPctOfCapex = ParseDouble(GetValue(row, "opex_fix_pct_of_capex"), "opex_fix_pct_of_capex", errors),
            OpexFixPowerCapacityEurPerKwYr = ParseDecimal(GetValue(row, "opex_fix_power_capacity_eur_per_kw_yr"), "opex_fix_power_capacity_eur_per_kw_yr", errors)
        };

        var overallIo = GetValue(row, "Input, Output for Overall Efficiency", "overall_efficiency_io");
        if (!string.IsNullOrWhiteSpace(overallIo))
        {
            var parts = overallIo.Split(',', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
                tech.OverallEfficiencyInputCarrier = parts[0];
            if (parts.Length > 1)
                tech.OverallEfficiencyOutputCarrier = parts[1];
        }

        var minInstallRaw = GetValue(row, "min_installation_size");
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

        classification = tech;
        return errors.Count == 0;
    }

    private static string? GetValue(IDictionary<string, string> row, params string[] headers)
    {
        if (headers.Length == 0)
            return null;

        var lookup = row.ToDictionary(k => NormalizeHeader(k.Key), v => v.Value, StringComparer.OrdinalIgnoreCase);
        foreach (var header in headers)
        {
            var key = NormalizeHeader(header);
            if (lookup.TryGetValue(key, out var value))
                return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        return null;
    }

    private static string NormalizeHeader(string header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return string.Empty;
            
        // Normalize by removing non-alphanumeric and converting to lowercase
        // This allows flexible matching while maintaining case-insensitivity
        var normalized = Regex.Replace(header, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase).ToLowerInvariant();
        return string.IsNullOrEmpty(normalized) ? header.ToLowerInvariant() : normalized;
    }

    private static List<string> ParseStringList(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<string>();

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

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

    private static int? ParseInt(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        errors.Add($"{fieldName}: invalid integer '{value}'");
        return null;
    }

    private static decimal? ParseDecimal(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        errors.Add($"{fieldName}: invalid decimal '{value}'");
        return null;
    }

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
        sb.AppendLine("- ALL timeframes/years if specified (2020, 2030, 2035, 2050, etc.)");
        sb.AppendLine("- ALL regional variations if mentioned");
        sb.AppendLine("- ALL maturity levels (current, near-term, long-term, etc.)");
        sb.AppendLine("- ALL technology variants and subtypes");
        sb.AppendLine("- Technologies mentioned in tables, figures, and text");
        sb.AppendLine("- Both main technologies and supporting/auxiliary technologies");
        sb.AppendLine();
        sb.AppendLine("IMPORTANT:");
        sb.AppendLine("- List EVERY technology separately - if a technology has data for 2030, 2035, and 2050, list all three");
        sb.AppendLine("- Include the timeframe/year in parentheses if specified");
        sb.AppendLine("- One technology per line");
        sb.AppendLine("- Do NOT include branding or company names unless they are the only identifier for the technology, but do include technology variants (e.g., 'Alkaline water electrolysis')");
        sb.AppendLine();
        sb.AppendLine("OUTPUT FORMAT (plain list, one per line):");
        sb.AppendLine("Alkaline water electrolysis (2035)");
        sb.AppendLine("Alkaline water electrolysis (2050)");
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
    /// Stage 2: Builds prompt to extract detailed data for MULTIPLE technologies at once (BATCHED).
    /// </summary>
    public static string BuildBatchDetailedExtractionPrompt(List<string> chunks, List<string> technologyNames)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"TASK: Extract ALL data for these {technologyNames.Count} technologies from this PDF:");
        sb.AppendLine();
        for (int i = 0; i < technologyNames.Count; i++)
        {
            sb.AppendLine($"{i + 1}. {technologyNames[i]}");
        }
        sb.AppendLine();
        sb.AppendLine("For EACH technology listed above, extract and report EVERYTHING you find:");
        sb.AppendLine("- Process description and operating conditions");
        sb.AppendLine("- ALL inputs (materials, energy, consumables) with quantities and units");
        sb.AppendLine("- ALL outputs (products, byproducts) with quantities and units");
        sb.AppendLine("- CAPEX (capital costs) in any format mentioned");
        sb.AppendLine("- OPEX (operating costs) as % or absolute values");
        sb.AppendLine("- Efficiency values");
        sb.AppendLine("- Technology readiness level (TRL) or maturity");
        sb.AppendLine("- Lifetime, reference capacity, or scale information");
        sb.AppendLine("- Year/time horizon the data applies to");
        sb.AppendLine();
        sb.AppendLine("BE COMPREHENSIVE:");
        sb.AppendLine("- Include ALL numeric values you find (even if scattered across pages)");
        sb.AppendLine("- Report units exactly as stated (MWh/t, kg/t, EUR/kW, etc.)");
        sb.AppendLine("- If data varies by location, report baseline + variations");
        sb.AppendLine();
        sb.AppendLine("OUTPUT FORMAT:");
        sb.AppendLine("Organize your response with clear section headers for each technology:");
        sb.AppendLine();
        sb.AppendLine("=== TECHNOLOGY 1: [Name] ===");
        sb.AppendLine("[All data for technology 1]");
        sb.AppendLine();
        sb.AppendLine("=== TECHNOLOGY 2: [Name] ===");
        sb.AppendLine("[All data for technology 2]");
        sb.AppendLine();
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
    /// Stage 2 (Legacy): Builds prompt to extract detailed data for ONE specific technology.
    /// </summary>
    public static string BuildDetailedExtractionPrompt(List<string> chunks, string technologyName)
    {
        return BuildBatchDetailedExtractionPrompt(chunks, new List<string> { technologyName });
    }

    /// <summary>
    /// Stage 3: Builds prompt to convert detailed extractions into structured JSON.
    /// </summary>
    public static string BuildStructuringPrompt(List<string> technologyNames, List<string> technologyDetails)
    {
        // Validate inputs
        if (technologyNames == null || technologyDetails == null)
            throw new ArgumentNullException("Technology names and details cannot be null");
            
        if (technologyNames.Count != technologyDetails.Count)
            throw new ArgumentException($"Mismatch: {technologyNames.Count} names but {technologyDetails.Count} details");
            
        var sb = new StringBuilder();
        
        sb.AppendLine("TASK: Convert the following technology summaries into structured JSON format.");
        sb.AppendLine();
        sb.AppendLine("You have detailed summaries for each technology. Now structure them into this exact JSON schema:");
        sb.AppendLine();
        
        // Show the JSON structure with one detailed example
        sb.AppendLine("REQUIRED JSON STRUCTURE:");
        sb.AppendLine("[");
        sb.AppendLine("  {");
        sb.AppendLine("    \"Datapaper Tech ID\": \"LT_DAC_2035\",");
        sb.AppendLine("    \"description\": \"Low-temperature solid sorbent direct air capture\",");
        sb.AppendLine("    \"summary\": \"Complete summary from detailed extraction\",");
        sb.AppendLine("    \"unit_operation\": \"Direct air capture unit\",");
        sb.AppendLine("    \"ProcessType\": \"CO2 Capture\",");
        sb.AppendLine("    \"main_sector\": \"CCU\",");
        sb.AppendLine("    \"main_category\": \"CO2 Capture\",");
        sb.AppendLine("    \"category_spec\": \"Low-temperature solid sorbent\",");
        sb.AppendLine("    \"tech_type\": \"Low-temperature solid sorbent DAC\",");
        sb.AppendLine("    \"carriers_in\": \"air, electricity, heat, sorbent\",");
        sb.AppendLine("    \"main_input\": \"air\",");
        sb.AppendLine("    \"ratios_in\": \"1, 0.48, 2.53, 7.5\",");
        sb.AppendLine("    \"units_in\": \"t, MWh/t, MWh/t, kg/t\",");
        sb.AppendLine("    \"carriers_out\": \"CO2\",");
        sb.AppendLine("    \"main_out\": \"CO2\",");
        sb.AppendLine("    \"ratios_out\": \"1\",");
        sb.AppendLine("    \"units_out\": \"t\",");
        sb.AppendLine("    \"reference_unit_size\": 1,");
        sb.AppendLine("    \"reference_unit_size_unit\": \"t/yr\",");
        sb.AppendLine("    \"trl_(1-9)\": 6,");
        sb.AppendLine("    \"tech_maturity\": \"Developing\",");
        sb.AppendLine("    \"cost_base_year\": 2035,");
        sb.AppendLine("    \"Currency\": \"EUR\",");
        sb.AppendLine("    \"capex_power_capacity_eur_per_kw\": 730,");
        sb.AppendLine("    \"Data Reference Year\": 2024");
        sb.AppendLine("  }");
        sb.AppendLine("]");
        sb.AppendLine();
        
        sb.AppendLine("FIELD MAPPING RULES:");
        sb.AppendLine("- carriers_in: List all inputs (materials, energy, consumables)");
        sb.AppendLine("- ratios_in: Numeric consumption values ONLY (e.g., from '0.48 MWh/t' use '0.48')");
        sb.AppendLine("- units_in: Units for each ratio (e.g., 'MWh/t', 'kg/t')");
        sb.AppendLine("- Same pattern for outputs (carriers_out, ratios_out, units_out)");
        sb.AppendLine("- capex_power_capacity_eur_per_kw: Use for '€730/t-yr' or '€1200/kW' formats");
        sb.AppendLine("- capex_one_time_eur: Use for large fixed costs like '€28.4M'");
        sb.AppendLine("- summary: Include the full detailed extraction text");
        sb.AppendLine("- If extraction failed or has no data, populate best you can with empty strings");
        sb.AppendLine();
        sb.AppendLine("IMPORTANT - DEDUPLICATION RULES:");
        sb.AppendLine("- Use the exact field names and data types as shown in the example");
        sb.AppendLine("- INCLUDE ALL technology variants with distinct technical data (method, efficiency, cost)");
        sb.AppendLine("- INCLUDE same technology with different timeframes (e.g., '2035 vs 2050') - these are different rows");
        sb.AppendLine("- INCLUDE alternative names for the same core technology (e.g., 'Water electrolysis' vs 'Alkaline water electrolysis')");
        sb.AppendLine("- ONLY exclude if: exact same technology name + exact same year + exact same data values");
        sb.AppendLine("- Remove company/brand names ONLY if they duplicate the core technology (e.g., 'Company XYZ water electrolysis' → 'Water electrolysis')");
        sb.AppendLine("- When in doubt, INCLUDE the entry - better to have a duplicate than lose a technology");
        
        sb.AppendLine("TECHNOLOGY SUMMARIES TO CONVERT:");
        sb.AppendLine();
        
        // Safe iteration with index bounds check
        int count = Math.Min(technologyNames.Count, technologyDetails.Count);
        for (int i = 0; i < count; i++)
        {
            sb.AppendLine($"=== Technology {i + 1}: {technologyNames[i]} ===");
            sb.AppendLine(technologyDetails[i]);
            sb.AppendLine();
        }
        
        sb.AppendLine();
        sb.AppendLine("Return ONLY valid JSON array with all technologies. No commentary.");
        
        return sb.ToString();
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
    /// Parses batched extraction response into individual technology details.
    /// Expected format: sections separated by "=== TECHNOLOGY N: [Name] ==="
    /// </summary>
    public static List<string> ParseBatchedExtractionResponse(string response, int expectedCount)
    {
        var details = new List<string>();
        if (string.IsNullOrWhiteSpace(response))
        {
            // Return empty placeholders
            for (int i = 0; i < expectedCount; i++)
                details.Add($"No data found for technology {i + 1}");
            return details;
        }

        // Split by the section markers
        var sections = Regex.Split(response, @"===\s*TECHNOLOGY\s+\d+:.*?===", RegexOptions.IgnoreCase);
        
        // First section is usually header/intro text before first technology, skip it
        for (int i = 1; i < sections.Length; i++)
        {
            var section = sections[i].Trim();
            if (!string.IsNullOrWhiteSpace(section))
                details.Add(section);
        }

        // If parsing failed, try to split the response evenly
        if (details.Count < expectedCount)
        {
            details.Clear();
            var lines = response.Split('\n');
            var linesPerTech = Math.Max(1, lines.Length / expectedCount);
            
            for (int i = 0; i < expectedCount; i++)
            {
                var start = i * linesPerTech;
                var count = (i == expectedCount - 1) ? (lines.Length - start) : linesPerTech;
                var section = string.Join('\n', lines.Skip(start).Take(count));
                details.Add(section.Trim());
            }
        }

        return details;
    }

    /// <summary>
    /// Extracts JSON from a Copilot response, handling markdown fences and direct JSON.
    /// </summary>
    public static string ExtractJson(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return string.Empty;

        // Try markdown fence first
        var fenceMatch = Regex.Match(response, "```(?:json)?\\s*(?<json>[\\s\\S]*?)```", RegexOptions.IgnoreCase);
        if (fenceMatch.Success)
            return fenceMatch.Groups["json"].Value.Trim();

        // Try direct JSON bracket match
        var start = response.IndexOf('[');
        var end = response.LastIndexOf(']');
        if (start >= 0 && end > start)
        {
            var extracted = response.Substring(start, end - start + 1).Trim();
            // Validate that we have balanced brackets
            if (extracted.StartsWith("[") && extracted.EndsWith("]"))
                return extracted;
        }

        return response.Trim();
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
            { "Electrolysis", "ELY" },
            { "Alkaline", "ALK" },
            { "PEM", "PEM" },
            { "SOEC", "SOEC" },
            { "Fischer-Tropsch", "FT" },
            { "FT", "FT" },
            { "Methanation", "MET" },
            { "Methane", "CH4" },
            { "Ammonia", "NH3" },
            { "Haber-Bosch", "HB" },
            { "Direct Air Capture", "DAC" },
            { "DAC", "DAC" },
            { "Water-Gas Shift", "WGS" },
            { "RWGS", "RWGS" },
            { "CO2 reduction", "CO2R" },
            { "Electrochemical", "ECH" },
            { "Synthesis", "SYN" },
            { "Conversion", "CNV" },
            { "CO2", "CO2" },
            { "H2", "H2" },
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
        if (tech.CostBaseYear.HasValue) fieldCount++;
        if (tech.DataReferenceYear.HasValue) fieldCount++;
        if (tech.Trl.HasValue) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.TechMaturity)) fieldCount++;
        if (tech.OverallEfficiency.HasValue) fieldCount++;
        if (tech.CarriersIn.Count > 0 || tech.CarriersOut.Count > 0) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainInput) || !string.IsNullOrWhiteSpace(tech.MainOut)) fieldCount++;
        if (tech.InputShares.Count > 0 || tech.RatiosIn.Count > 0) fieldCount++;
        if (tech.LifetimeYears.HasValue || tech.CapexOneTimeEur.HasValue || tech.OpexOneTimeEur.HasValue) fieldCount++;

        // Require minimum 2 fields populated
        return fieldCount >= 2;
    }
}

public static class TechnologyClassificationCsv
{
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
        "reference_unit_size",
        "reference_unit_size_unit",
        "cost_base_year",
        "Currency",
        "trl_(1-9)",
        "tech_maturity",
        "overall_efficiency",
        "Input, Output for Overall Efficiency",
        "carriers_in",
        "main_input",
        "Input Shares",
        "Input Units - Shares",
        "ratios_in",
        "units_in",
        "carriers_out",
        "main_out",
        "ratios_out",
        "units_out",
        "Output Shares",
        "Output Units - Shares",
        "min_installation_size",
        "lifetime_yr",
        "capex_one_time_eur",
        "capex_power_capacity_eur_per_kw",
        "opex_one_time_eur",
        "opex_fix_pct_of_capex",
        "opex_fix_power_capacity_eur_per_kw_yr",
        "Data Reference Year",
        "summary"
    };

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
                FormatDouble(row.ReferenceUnitSize),
                row.ReferenceUnitSizeUnit ?? string.Empty,
                FormatInt(row.CostBaseYear),
                row.Currency ?? string.Empty,
                FormatInt(row.Trl),
                row.TechMaturity ?? string.Empty,
                FormatDouble(row.OverallEfficiency),
                FormatOverallEfficiencyIo(row.OverallEfficiencyInputCarrier, row.OverallEfficiencyOutputCarrier),
                JoinList(row.CarriersIn.Cast<string?>()),
                row.MainInput ?? string.Empty,
                JoinList(row.InputShares.Select(d => FormatDouble(d)).Cast<string?>()),
                JoinList(row.InputShareUnits.Cast<string?>()),
                JoinList(row.RatiosIn.Select(d => FormatDouble(d)).Cast<string?>()),
                JoinList(row.UnitsIn.Cast<string?>()),
                JoinList(row.CarriersOut.Cast<string?>()),
                row.MainOut ?? string.Empty,
                JoinList(row.RatiosOut.Select(d => FormatDouble(d)).Cast<string?>()),
                JoinList(row.UnitsOut.Cast<string?>()),
                JoinList(row.OutputShares.Select(d => FormatDouble(d)).Cast<string?>()),
                JoinList(row.OutputShareUnits.Cast<string?>()),
                FormatValueWithUnit(row.MinInstallationSize, row.MinInstallationSizeUnit),
                FormatDouble(row.LifetimeYears),
                FormatDecimal(row.CapexOneTimeEur),
                FormatDecimal(row.CapexPowerCapacityEurPerKw),
                FormatDecimal(row.OpexOneTimeEur),
                FormatDouble(row.OpexFixPctOfCapex),
                FormatDecimal(row.OpexFixPowerCapacityEurPerKwYr),
                FormatInt(row.DataReferenceYear),
                row.Summary ?? string.Empty
            };

            sb.AppendLine(string.Join(',', fields.Select(EscapeCsv)));
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
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
