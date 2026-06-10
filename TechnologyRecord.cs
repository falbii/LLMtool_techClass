namespace TechClassificationApp;

// One classified technology row — the structured form of a technology's data for a given year.
// Field meanings mirror prompt/classification_from_summary.md (FIELD DEFINITIONS).
public sealed class TechnologyRecord
{
    public string? DatapaperTechId { get; set; }          // short unique id, abbreviation_year (e.g. AEL_2030); generated if missing
    public string? ProcessType { get; set; }              // role in the energy system: Conversion, Storage, Capture, Transport, EndUse
    public string? Description { get; set; }              // 1-2 sentences on what the technology is and does
    public string? UnitOperation { get; set; }            // core process step (Electrolysis, Gasification, FT synthesis)
    public string? Summary { get; set; }                  // short paragraph condensing the section's key data
    public string? MainSector { get; set; }               // broadest sector served (Electricity, Heat, Chemicals, Fuels, ...)
    public string? MainCategory { get; set; }             // technology family (Electrolysis, CO2 Capture, Syngas Production)
    public string? CategorySpec { get; set; }             // variant within the family (Alkaline, PEM, Solid sorbent)
    public string? TechType { get; set; }                 // most specific name the source uses
    public double? ReferenceUnitSize { get; set; }        // capacity/size of the reference plant the data refers to
    public string? ReferenceUnitSizeUnit { get; set; }    // its unit (MW, t/yr, kg/s)
    public int? Year { get; set; }                        // year/time horizon the data point DESCRIBES (2050 projection -> 2050)
    public string? Location { get; set; }                 // country or region the data refers to
    public string? Currency { get; set; }                 // currency the costs are stated in (EUR, USD)
    public int? RefYear { get; set; }                     // publication year of the source — constant per paper, filled once per run
    public int? Trl { get; set; }                         // Technology Readiness Level, 1-9, only if the source states one
    public string? TechMaturity { get; set; }             // source's own maturity wording (Mature, Developing, Emerging)
    public double? OverallEfficiency { get; set; }        // overall conversion efficiency, 0-1 decimal (LHV preferred)
    public string? EfficiencyUnit { get; set; }           // efficiency basis/unit ("% LHV", kWh/kg)
    public List<string> CarriersIn { get; set; } = new(); // all carriers entering the process
    public string? MainInput { get; set; }                // the primary input carrier
    public List<double> RatiosIn { get; set; } = new();   // input quantity per carrier, same order as CarriersIn
    public List<string> UnitsIn { get; set; } = new();    // one unit per input ratio, same order
    public List<string> CarriersOut { get; set; } = new();// all carriers leaving the process
    public string? MainOut { get; set; }                  // the primary output carrier
    public List<double> RatiosOut { get; set; } = new();  // output quantity per carrier, same order as CarriersOut
    public List<string> UnitsOut { get; set; } = new();   // one unit per output ratio, same order
    public double? Lifetime { get; set; }                 // technical/economic lifetime
    public string? LifetimeUnit { get; set; }             // its unit (yr for plants; h or cycles for stacks)
    public decimal? Capex { get; set; }                   // one-time capital investment cost, verbatim from the source
    public string? CapexUnit { get; set; }                // its unit (EUR, EUR/kW, MEUR)
    public decimal? Opex { get; set; }                    // operating costs, fixed + variable combined, as stated by the source
    public string? OpexUnit { get; set; }                 // its unit (EUR/yr, % of CAPEX/yr, EUR/MWh)
}
