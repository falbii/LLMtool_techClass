using System.Text;

namespace TechClassificationApp;

// Reads and writes the classification CSV in a fixed column order.
public static class TechnologyCsv
{
    public static readonly string[] HeaderOrder =
    [
        "Datapaper Tech ID", "ProcessType", "description", "unit_operation",
        "main_sector", "main_category", "category_spec", "tech_type",
        "base_year", "reference_unit_size", "reference_unit_size_unit",
        "location", "Currency", "trl_(1-9)", "tech_maturity",
        "efficiency", "efficiency_unit",
        "carriers_in", "main_input", "ratios_in", "units_in",
        "carriers_out", "main_out", "ratios_out", "units_out",
        "capex", "capex_unit", "opex_fix", "opex_fix_unit",
        "lifetime_yr", "Data Reference Year", "summary"
    ];

    public static void WriteCsv(string filePath, IEnumerable<TechnologyRecord> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', HeaderOrder.Select(TechClassifierHelpers.EscapeCsv)));

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
                TechClassifierHelpers.FormatInt(row.BaseYear),
                TechClassifierHelpers.FormatDouble(row.ReferenceUnitSize),
                row.ReferenceUnitSizeUnit ?? string.Empty,
                row.Location ?? string.Empty,
                row.Currency ?? string.Empty,
                TechClassifierHelpers.FormatInt(row.Trl),
                row.TechMaturity ?? string.Empty,
                TechClassifierHelpers.FormatDouble(row.OverallEfficiency),
                row.EfficiencyUnit ?? string.Empty,
                TechClassifierHelpers.JoinList(row.CarriersIn.Cast<string?>()),
                row.MainInput ?? string.Empty,
                TechClassifierHelpers.JoinList(row.RatiosIn.Select(d => TechClassifierHelpers.FormatDouble(d)).Cast<string?>()),
                TechClassifierHelpers.JoinList(row.UnitsIn.Cast<string?>()),
                TechClassifierHelpers.JoinList(row.CarriersOut.Cast<string?>()),
                row.MainOut ?? string.Empty,
                TechClassifierHelpers.JoinList(row.RatiosOut.Select(d => TechClassifierHelpers.FormatDouble(d)).Cast<string?>()),
                TechClassifierHelpers.JoinList(row.UnitsOut.Cast<string?>()),
                TechClassifierHelpers.FormatDecimal(row.Capex),
                row.CapexUnit ?? string.Empty,
                TechClassifierHelpers.FormatDecimal(row.OpexFix),
                row.OpexFixUnit ?? string.Empty,
                TechClassifierHelpers.FormatDouble(row.LifetimeYears),
                TechClassifierHelpers.FormatInt(row.DataReferenceYear),
                row.Summary ?? string.Empty
            };

            sb.AppendLine(string.Join(',', fields.Select(TechClassifierHelpers.EscapeCsv)));
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    public static List<TechnologyRecord> ReadCsv(string filePath)
    {
        var results = new List<TechnologyRecord>();
        if (!File.Exists(filePath)) return results;

        var content = File.ReadAllText(filePath, Encoding.UTF8);
        var records = TechClassifierHelpers.ParseCsvRecords(content);
        if (records.Count <= 1) return results;

        var headers = records[0];
        for (int i = 1; i < records.Count; i++)
        {
            var values = records[i];
            if (values.All(string.IsNullOrWhiteSpace)) continue;

            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int col = 0; col < headers.Count; col++)
                row[headers[col]] = col < values.Count ? values[col] : string.Empty;

            results.Add(TechnologyClassifier.ParseRecord(row, out _));
        }

        return results;
    }
}
