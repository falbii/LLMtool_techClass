using System.Text;

namespace TechClassificationApp;

// Reads and writes the classification CSV in a fixed column order.
public static class TechnologyCsv
{
    public static readonly string[] HeaderOrder =
    [
        "tech_id", "process_type", "description", "unit_operation",
        "main_sector", "main_category", "category_spec", "tech_type",
        "year", "reference_unit_size", "reference_unit_size_unit",
        "location", "currency", "trl", "tech_maturity",
        "efficiency", "efficiency_unit",
        "carriers_in", "main_input", "ratios_in", "units_in",
        "carriers_out", "main_out", "ratios_out", "units_out",
        "capex", "capex_unit", "opex", "opex_unit",
        "lifetime", "lifetime_unit", "ref_year", "summary"
    ];

    // When model is supplied, a rightmost "model" column records which model produced the rows.
    // Left optional so the benchmark's combined CSV (which prepends its own Model column) is unaffected.
    public static void WriteCsv(string filePath, IEnumerable<TechnologyRecord> rows, string? model = null)
    {
        bool includeModel = !string.IsNullOrWhiteSpace(model);
        var header = includeModel ? HeaderOrder.Append("model") : HeaderOrder;

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', header.Select(TechClassifierHelpers.EscapeCsv)));

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
                TechClassifierHelpers.FormatInt(row.Year),
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
                TechClassifierHelpers.FormatDecimal(row.Opex),
                row.OpexUnit ?? string.Empty,
                TechClassifierHelpers.FormatDouble(row.Lifetime),
                row.LifetimeUnit ?? string.Empty,
                TechClassifierHelpers.FormatInt(row.RefYear),
                row.Summary ?? string.Empty
            };
            if (includeModel)
                fields.Add(model!);

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
