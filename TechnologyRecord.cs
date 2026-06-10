namespace TechClassificationApp;

// One classified technology row — the structured form of a technology's data for a given year.
public sealed class TechnologyRecord
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
    // The year/time horizon this data point describes (e.g. a 2050 cost projection -> 2050).
    public int? Year { get; set; }
    public string? Location { get; set; }
    public string? Currency { get; set; }
    // Publication year of the source document — constant per paper, filled once per run.
    public int? RefYear { get; set; }
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
    public double? Lifetime { get; set; }
    public string? LifetimeUnit { get; set; }
    public decimal? Capex { get; set; }
    public string? CapexUnit { get; set; }
    // Operating costs, fixed + variable combined, as stated by the source.
    public decimal? Opex { get; set; }
    public string? OpexUnit { get; set; }
}
