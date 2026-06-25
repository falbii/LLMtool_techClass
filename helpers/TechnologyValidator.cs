using System.Globalization;

namespace TechClassificationApp;

// Deterministic post-parse validation, run after ParseRecord and before the merge. Every check is
// a non-destructive warning surfaced through the existing parsing-notes channel — the record is
// never mutated, keeping the tool's "flag, never silently edit" stance for ALL fields (efficiency
// included). No LLM, no randomness — same input, same notes.
internal static class TechnologyValidator
{
    private const int MinYear = 1900;
    private const int MaxYear = 2100;

    // Does not mutate `tech`; returns human-readable notes for any values that look off.
    public static List<string> NormalizeAndValidate(TechnologyRecord tech)
    {
        var notes = new List<string>();

        // C2 — efficiency scale. The prompt asks for a 0-1 decimal. Flag anything outside that
        // range so it's visible, but DON'T rewrite the model's value: a number > 1 is not always
        // a percent the model forgot to divide — it can be a real coefficient (e.g. a heat-pump
        // COP of 3.5), and silently dividing those by 100 produces nonsense like 0.035.
        if (tech.OverallEfficiency is { } eff && (eff <= 0 || eff > 1))
            notes.Add($"efficiency: {Fmt(eff)} is outside the expected 0-1 range (left as-is)");

        // C5 — deterministic range / schema gates (warnings only).
        if (tech.Trl is { } trl && (trl < 1 || trl > 9))
            notes.Add($"trl: {trl} is outside the valid 1-9 range");

        if (tech.Year is { } year && (year < MinYear || year > MaxYear))
            notes.Add($"year: {year} is implausible (expected {MinYear}-{MaxYear})");

        if (tech.RefYear is { } refYear && (refYear < MinYear || refYear > MaxYear))
            notes.Add($"ref_year: {refYear} is implausible (expected {MinYear}-{MaxYear})");

        if (tech.Capex.HasValue && string.IsNullOrWhiteSpace(tech.Currency))
            notes.Add("capex present but currency is empty");
        if (tech.Opex.HasValue && string.IsNullOrWhiteSpace(tech.Currency))
            notes.Add("opex present but currency is empty");

        // The prompt promises ratios / units are order-aligned with their carriers; flag breaks.
        AddLengthMismatch(notes, "ratios_in", tech.RatiosIn.Count, "carriers_in", tech.CarriersIn.Count);
        AddLengthMismatch(notes, "units_in", tech.UnitsIn.Count, "ratios_in", tech.RatiosIn.Count);
        AddLengthMismatch(notes, "ratios_out", tech.RatiosOut.Count, "carriers_out", tech.CarriersOut.Count);
        AddLengthMismatch(notes, "units_out", tech.UnitsOut.Count, "ratios_out", tech.RatiosOut.Count);

        return notes;
    }

    // Only flags when both sides are populated — a missing list is incomplete data, not a mismatch.
    private static void AddLengthMismatch(List<string> notes, string aName, int aCount, string bName, int bCount)
    {
        if (aCount > 0 && bCount > 0 && aCount != bCount)
            notes.Add($"{aName} ({aCount}) and {bName} ({bCount}) length mismatch");
    }

    private static string Fmt(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);
}
