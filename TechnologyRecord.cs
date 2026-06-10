namespace TechClassificationApp;

// One classified technology row — the structured form of a technology's data for a given year.
// Field meanings mirror prompt/classification_from_summary.md (FIELD DEFINITIONS).
public sealed class TechnologyRecord
{
    // Short unique id, abbreviation_year (e.g. AEL_2030); generated if the model omits it.
    public string? DatapaperTechId { get; set; }
    // Role in the energy system: Conversion, Storage, Capture, Transport, EndUse.
    public string? ProcessType { get; set; }
    // 1-2 sentences on what the technology is and does.
    public string? Description { get; set; }
    // Core process step performed (Electrolysis, Gasification, Fischer-Tropsch synthesis).
    public string? UnitOperation { get; set; }
    // Short paragraph condensing the section's key data.
    public string? Summary { get; set; }
    // Broadest sector served (Electricity, Heat, Chemicals, Fuels, Industry, Buildings).
    public string? MainSector { get; set; }
    // Technology family / field (Electrolysis, CO2 Capture, Syngas Production).
    public string? MainCategory { get; set; }
    // Variant within the family (Alkaline, PEM, Solid sorbent).
    public string? CategorySpec { get; set; }
    // Most specific name the source uses for this exact technology.
    public string? TechType { get; set; }
    // Capacity/size of the reference plant the data refers to, with its unit.
    public double? ReferenceUnitSize { get; set; }
    public string? ReferenceUnitSizeUnit { get; set; }
    // The year/time horizon this data point describes (e.g. a 2050 cost projection -> 2050).
    public int? Year { get; set; }
    // Country or region the data refers to.
    public string? Location { get; set; }
    // Currency the costs are stated in (EUR, USD, ...).
    public string? Currency { get; set; }
    // Publication year of the source document — constant per paper, filled once per run.
    public int? RefYear { get; set; }
    // Technology Readiness Level, 1-9, only if the source states one.
    public int? Trl { get; set; }
    // Source's own qualitative maturity wording (Mature, Developing, Emerging).
    public string? TechMaturity { get; set; }
    // Overall conversion efficiency as a 0-1 decimal (LHV preferred), with its basis/unit.
    public double? OverallEfficiency { get; set; }
    public string? EfficiencyUnit { get; set; }
    // All carriers entering the process; MainInput is the primary one.
    public List<string> CarriersIn { get; set; } = new();
    public string? MainInput { get; set; }
    // Input quantity per carrier, same order as CarriersIn, with one unit per ratio.
    public List<double> RatiosIn { get; set; } = new();
    public List<string> UnitsIn { get; set; } = new();
    // All carriers leaving the process; MainOut is the primary one.
    public List<string> CarriersOut { get; set; } = new();
    public string? MainOut { get; set; }
    // Output quantity per carrier, same order as CarriersOut, with one unit per ratio.
    public List<double> RatiosOut { get; set; } = new();
    public List<string> UnitsOut { get; set; } = new();
    // Technical or economic lifetime, with its unit (yr for plants; h or cycles for stacks).
    public double? Lifetime { get; set; }
    public string? LifetimeUnit { get; set; }
    // One-time capital investment cost, verbatim from the source, with its unit.
    public decimal? Capex { get; set; }
    public string? CapexUnit { get; set; }
    // Operating costs, fixed + variable combined, as stated by the source, with its unit.
    public decimal? Opex { get; set; }
    public string? OpexUnit { get; set; }
}
