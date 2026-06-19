using System.Globalization;

namespace TechClassificationApp;

// Deterministic post-parse normalization + validation, run after ParseRecord and before the
// merge. Only efficiency is *normalized* (a safe, unambiguous transform: a 65 / "65%" that the
// model emitted instead of the requested 0-1 fraction). Everything else is a non-destructive
// warning surfaced through the existing parsing-notes channel, keeping the tool's
// "flag, never silently edit" stance. No LLM, no randomness — same input, same notes.
internal static class TechnologyValidator
{
    private const int MinYear = 1900;
    private const int MaxYear = 2100;

    // Mutates `tech` only for the efficiency normalization; returns human-readable notes.
    public static List<string> NormalizeAndValidate(TechnologyRecord tech)
    {
        var notes = new List<string>();

        // C2 — efficiency scale. Prompt asks for a 0-1 decimal; models often emit 65 or "65%"
        // (the latter already failed to parse and is null here). A value > 1 is read as percent.
        if (tech.OverallEfficiency is { } eff)
        {
            if (eff > 1)
            {
                tech.OverallEfficiency = eff / 100.0;
                notes.Add($"efficiency: normalized {Fmt(eff)} → {Fmt(tech.OverallEfficiency.Value)} (assumed percent)");
            }
            var normalized = tech.OverallEfficiency.Value;
            if (normalized <= 0 || normalized > 1)
                notes.Add($"efficiency: {Fmt(normalized)} is outside the expected 0-1 range");
        }

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
