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
    /// Builds the classification prompt for extracting technologies from PDF content.
    /// </summary>
    public static string BuildClassificationPrompt(List<string> chunks)
    {
        var sb = new StringBuilder();
        
        // === GOAL & OVERVIEW ===
        sb.AppendLine("You are extracting ALL individual technologies and sub-processes from a technical PDF.");
        sb.AppendLine("GOAL: Extract EVERY distinct technology - return a JSON array with complete details.");
        sb.AppendLine();

        // === JSON EXAMPLE (CRITICAL) ===
        sb.AppendLine("EXAMPLE OUTPUT (exact JSON structure required):");
        sb.AppendLine("[");
        sb.AppendLine("  {");
        sb.AppendLine("    \"Datapaper Tech ID\": \"H2O_AEC_ELY_2030\",");
        sb.AppendLine("    \"description\": \"Alkaline water electrolysis\",");
        sb.AppendLine("    \"summary\": \"Mature water electrolysis technology using alkaline cells at 60-80°C...\",");
        sb.AppendLine("    \"unit_operation\": \"Electrolysis\",");
        sb.AppendLine("    \"ProcessType\": \"Alkaline\",");
        sb.AppendLine("    \"main_sector\": \"Energy\",");
        sb.AppendLine("    \"main_category\": \"Hydrogen Production\",");
        sb.AppendLine("    \"category_spec\": \"Alkaline\",");
        sb.AppendLine("    \"tech_type\": \"Alkaline Water Electrolysis\",");
        sb.AppendLine("    \"carriers_in\": \"water, electricity\",");
        sb.AppendLine("    \"main_input\": \"water\",");
        sb.AppendLine("    \"ratios_in\": \"9, 55.1\",");
        sb.AppendLine("    \"units_in\": \"mol, kWh/kg\",");
        sb.AppendLine("    \"carriers_out\": \"hydrogen, oxygen\",");
        sb.AppendLine("    \"main_out\": \"hydrogen\",");
        sb.AppendLine("    \"ratios_out\": \"2, 1\",");
        sb.AppendLine("    \"units_out\": \"mol, mol\",");
        sb.AppendLine("    \"reference_unit_size\": 1,");
        sb.AppendLine("    \"reference_unit_size_unit\": \"MW\",");
        sb.AppendLine("    \"trl_(1-9)\": 9,");
        sb.AppendLine("    \"tech_maturity\": \"Mature\",");
        sb.AppendLine("    \"cost_base_year\": 2035,");
        sb.AppendLine("    \"Currency\": \"EUR\",");
        sb.AppendLine("    \"capex_power_capacity_eur_per_kw\": 790.5,");
        sb.AppendLine("    \"opex_fix_pct_of_capex\": 0.489,");
        sb.AppendLine("    \"Data Reference Year\": 2024");
        sb.AppendLine("  },");
        sb.AppendLine("  {");
        sb.AppendLine("    \"Datapaper Tech ID\": \"CO2_LT_DAC_2035\",");
        sb.AppendLine("    \"description\": \"Low-temperature solid sorbent direct air capture\",");
        sb.AppendLine("    \"summary\": \"Temperature-vacuum swing adsorption with amine sorbent. Near-future: 0.48 MWh/t electricity, 2.53 MWh/t heat, 7.5 kg/t sorbent. CAPEX 730 EUR/t-yr. Location factors: Iceland 1.00, Netherlands 1.32, Spain 1.69.\",");
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

        // === WHAT IS A TECHNOLOGY? ===
        sb.AppendLine("WHAT IS A 'TECHNOLOGY'?");
        sb.AppendLine("- A single unit operation or process (e.g., 'Alkaline Electrolysis' OR 'PEM Electrolysis' - not both in one row)");
        sb.AppendLine("- NOT brand names: 'Siemens Electrolyzer' → extract as 'Alkaline Water Electrolysis'");
        sb.AppendLine("- NOT marketing terms: 'NextGen DAC v2.0' → extract as 'Adsorption-based Direct Air Capture'");
        sb.AppendLine("- Break integrated pathways: 'Electrolysis + Fischer-Tropsch' → 2 separate rows");
        sb.AppendLine("- DUPLICATES: Same name twice → extract once; Same name + different time horizons → extract twice with time suffix");
        sb.AppendLine();
        
        sb.AppendLine("⚠️  CRITICAL: EXTRACT DATA FROM DESCRIPTIVE TEXT, NOT JUST TABLES!");
        sb.AppendLine("If the paper says: 'LT DAC consumes 0.48 MWh/t electricity' → EXTRACT IT to ratios_in!");
        sb.AppendLine("If the paper says: 'CAPEX is 730 EUR per tonne CO2 per year' → EXTRACT IT to capex_power_capacity_eur_per_kw!");
        sb.AppendLine("Don't leave fields empty when data is buried in paragraphs, sentences, or figure captions.");
        sb.AppendLine();

        // === FIELD REQUIREMENTS ===
        sb.AppendLine("REQUIRED FIELDS (must populate):");
        sb.AppendLine("  description, unit_operation, main_sector, main_category, carriers_in, carriers_out");
        sb.AppendLine();
        sb.AppendLine("OPTIONAL FIELDS (if in paper):");
        sb.AppendLine("  ratios_in, ratios_out, capex_*, opex_*, cost_base_year, lifetime_yr");
        sb.AppendLine();
        sb.AppendLine("ESTIMATABLE FIELDS (OK to estimate if not in paper):");
        sb.AppendLine("  trl_(1-9), tech_maturity - use industry knowledge for well-known technologies");
        sb.AppendLine();
        sb.AppendLine("USE EMPTY STRING \"\" for optional fields not in paper - NEVER leave blank or use null");
        sb.AppendLine();

        // === CRITICAL RULES ===
        sb.AppendLine("CRITICAL RULES:");
        sb.AppendLine("1. COMPLETENESS: Extract EVERY technology from text, tables, figures, appendices");
        sb.AppendLine("2. CARRIERS & RATIOS: NEVER mix numbers with carrier names");
        sb.AppendLine("   - carriers_in: 'water, electricity' (names only)");
        sb.AppendLine("   - ratios_in: '9, 0.5' (numbers only)");
        sb.AppendLine("   - Both must have SAME count");
        sb.AppendLine("3. TIME HORIZONS: Different data for 2030 vs 2050 → create separate rows");
        sb.AppendLine("   - Add '_2030', '_2050' suffix to Datapaper Tech ID");
        sb.AppendLine("4. CURRENCY: Always use 'EUR' for cost fields; convert if needed");
        sb.AppendLine("5. TABLES: Extract ALL technologies from technology lists, even if minimal data");
        sb.AppendLine("6. NUMERIC DATA: Extract values only (not formulas)");
        sb.AppendLine();
        
        // === QUANTITATIVE DATA EXTRACTION ===
        sb.AppendLine("EXTRACTING QUANTITATIVE DATA (CRITICAL):");
        sb.AppendLine("Extract ALL numeric values from text - don't skip consumption/cost data just because it's in prose!");
        sb.AppendLine();
        sb.AppendLine("ENERGY/MATERIAL CONSUMPTION → ratios_in/units_in:");
        sb.AppendLine("  Example: 'Uses 0.48 MWh/t CO2 electricity and 2.53 MWh/t heat'");
        sb.AppendLine("  → carriers_in: 'CO2, electricity, heat'");
        sb.AppendLine("  → ratios_in: '1, 0.48, 2.53'");
        sb.AppendLine("  → units_in: 't, MWh/t, MWh/t'");
        sb.AppendLine();
        sb.AppendLine("  Example: 'Sorbent consumption 7.5 kg/t CO2'");
        sb.AppendLine("  → ADD to carriers_in: 'sorbent'");
        sb.AppendLine("  → ADD to ratios_in: '7.5'");
        sb.AppendLine("  → ADD to units_in: 'kg/t'");
        sb.AppendLine();
        sb.AppendLine("PRODUCTION/OUTPUT → ratios_out/units_out:");
        sb.AppendLine("  Example: 'Produces 1 t CO2 captured'");
        sb.AppendLine("  → carriers_out: 'CO2'");
        sb.AppendLine("  → ratios_out: '1'");
        sb.AppendLine("  → units_out: 't'");
        sb.AppendLine();
        sb.AppendLine("CAPEX FORMATS - Extract as-is with proper field:");
        sb.AppendLine("  '€730/t-yr' or '730 EUR per t CO2 per year' → capex_power_capacity_eur_per_kw: 730");
        sb.AppendLine("  '€1200/kW' → capex_power_capacity_eur_per_kw: 1200");
        sb.AppendLine("  '€28.4M at 229 kt/y' → capex_one_time_eur: 28400000, reference_unit_size: 229000, reference_unit_size_unit: 't/y'");
        sb.AppendLine();
        sb.AppendLine("LOCATION-SPECIFIC DATA:");
        sb.AppendLine("  If paper mentions 'Netherlands 1.32, Spain 1.69' for location factors:");
        sb.AppendLine("  → Use BASELINE/reference values in main fields");
        sb.AppendLine("  → Mention location adjustments in summary only");
        sb.AppendLine();
        sb.AppendLine("NEAR-TERM vs LONG-TERM:");
        sb.AppendLine("  'Near-future 0.48 MWh/t, long-term 0.38 MWh/t'");
        sb.AppendLine("  → Create TWO separate rows (one 2035, one 2050)");
        sb.AppendLine("  → Each with its own ratios_in values");
        sb.AppendLine();

        // === FIELD DEFINITIONS (CONDENSED) ===
        sb.AppendLine("FIELD GUIDANCE:");
        sb.AppendLine("- Datapaper Tech ID: 3-5 uppercase words separated by _ (e.g., H2O_AEC_ELY_2030)");
        sb.AppendLine("- description: Short name only (5-15 words)");
        sb.AppendLine("- summary: Comprehensive details from the paper (operating conditions, parameters, context, ALL numeric values)");
        sb.AppendLine("- unit_operation: The main unit of the process (AEC Electrolyzer, Geothermal CHP, H2-fired gas turbine, etc.)");
        sb.AppendLine("- ProcessType: The process type (Fuel synthesis, Power Generation, Storage, CO2 Capture, etc.)");
        sb.AppendLine("- main_sector: Broad category (Energy, Chemicals, CCU, Materials, Transport, Heat Supply)");
        sb.AppendLine("- main_category: Process category (Hydrogen Production, CO2 Capture, etc.)");
        sb.AppendLine("- category_spec: Specific type (Alkaline, PEM, SOEC, Fischer-Tropsch, etc.)");
        sb.AppendLine("- tech_type: Full descriptive name (e.g., 'Alkaline Water Electrolysis')");
        sb.AppendLine("- carriers_in: Comma-separated input materials INCLUDING consumables (e.g., 'water, electricity, sorbent')");
        sb.AppendLine("- ratios_in: Comma-separated numeric coefficients ONLY - extract from 'X MWh/t', 'Y kg/t', etc. (e.g., '1, 0.48, 7.5' - NO UNITS)");
        sb.AppendLine("- units_in: Units for each ratio (e.g., 't, MWh/t, kg/t') - use 't' for main product, 'MWh/t' for energy per tonne, 'kg/t' for materials per tonne");
        sb.AppendLine("- main_input: Most important input (must be in carriers_in)");
        sb.AppendLine("- carriers_out: Comma-separated products (e.g., 'hydrogen, oxygen')");
        sb.AppendLine("- ratios_out: Comma-separated numeric coefficients ONLY (e.g., '2, 1')");
        sb.AppendLine("- units_out: Units for each output ratio (e.g., 'mol, mol' or 't, kg')");
        sb.AppendLine("- main_out: Most important product (must be in carriers_out)");
        sb.AppendLine("- trl_(1-9): Technology Readiness Level 1-9; estimate if needed (8-9=mature, 6-7=developing, 4-5=early)");
        sb.AppendLine("- tech_maturity: Text description (Early-stage, Developing, Near-commercial, Mature, Commercial)");
        sb.AppendLine("- overall_efficiency: Round-trip efficiency as decimal (0.85 for 85%)");
        sb.AppendLine("- reference_unit_size: Numeric value for capacity (e.g., 1 for 1 MW electrolyzer, or 1000 for 1000 t/y DAC - NO UNITS)");
        sb.AppendLine("- reference_unit_size_unit: Capacity unit (MW, MWh, kt/y, t/y, t/h, kW, kg/h, MJ/h)");
        sb.AppendLine("- cost_base_year: CRITICAL - Year costs apply to (2020, 2030, 2035, 2050, etc.)");
        sb.AppendLine("- Currency: Always 'EUR'");
        sb.AppendLine("- capex_one_time_eur: Fixed non-scalable capital cost per unit of capacity (e.g., 28400000 for 28.4M EUR)");
        sb.AppendLine("- capex_power_capacity_eur_per_kw: Scalable capital cost (€/kW, €/t-yr, etc.) - use this for '730 EUR/t-yr' or '1200 EUR/kW'");
        sb.AppendLine("- opex_one_time_eur: Initial one-time operating setup cost");
        sb.AppendLine("- opex_fix_pct_of_capex: Annual fixed cost as % of CAPEX (e.g., 0.03 for 3% or 0.489 for 48.9%)");
        sb.AppendLine("- opex_fix_power_capacity_eur_per_kw_yr: Annual fixed cost per kW capacity");
        sb.AppendLine("- lifetime_yr: Expected operational lifetime in years");
        sb.AppendLine("- Data Reference Year: Year the paper was published (2020, 2024, etc.)");
        sb.AppendLine();

        // === EDGE CASES ===
        sb.AppendLine("EDGE CASES:");
        sb.AppendLine("DUPLICATES: Same tech name appears twice");
        sb.AppendLine("  → Extract once if same data; Extract twice if different costs/TRL for 2030 vs 2050");
        sb.AppendLine("TIME HORIZONS: 'This technology projected for 2030 with cost X, 2050 with cost Y'");
        sb.AppendLine("  → Create TWO rows: one with cost_base_year=2030, one with cost_base_year=2050");
        sb.AppendLine("CONFLICTING DATA: Different sources give different TRL/costs");
        sb.AppendLine("  → Use most recent or credible; note discrepancy in summary");
        sb.AppendLine("COST RANGES: '€1000-1500/kW' → use midpoint (1250)");
        sb.AppendLine("INCOMPLETE DATA: Missing some fields → OK; only use empty string for missing optional fields");
        sb.AppendLine();

        // === PDF CONTENT ===
        if (chunks.Count == 1)
        {
            sb.AppendLine("PDF Content:");
            sb.AppendLine(chunks[0]);
        }
        else
        {
            sb.AppendLine($"PDF Content split into {chunks.Count} parts:");
            for (int i = 0; i < chunks.Count; i++)
            {
                sb.AppendLine($"Part {i + 1}:");
                sb.AppendLine(chunks[i]);
                sb.AppendLine();
            }
        }

        sb.AppendLine();
        sb.AppendLine("Return ONLY valid JSON array. No commentary.");

        return sb.ToString();
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
