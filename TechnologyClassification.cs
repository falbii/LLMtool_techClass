using System.Globalization;
using System.Linq;
using System.Text;
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
        return Regex.Replace(header ?? string.Empty, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase).ToLowerInvariant();
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
