using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TechClassificationApp;

// Deterministic (non-LLM) check that the numbers in the extracted records actually appear in
// the source text. The classification pipeline copies every value through several LLM passes
// (condense → find → summarize → classify), any of which can silently alter a digit, a unit,
// or a decimal mark. This flags values that cannot be found in the source so a human can review
// them. It never edits, fills, or drops data — verification only.
//
// "Deterministic" here means the verifier itself contains no model call and no randomness: given
// the same records and the same source text it returns the identical report every run. It checks
// numeric *presence* in the source, not semantic correctness.
public static class GroundingVerifier
{
    public sealed record Finding(string TechId, string Field, string Value);

    public sealed class GroundingReport
    {
        public int TotalValues { get; init; }
        public int GroundedValues { get; init; }
        public IReadOnlyList<Finding> Ungrounded { get; init; } = [];
        public int UngroundedCount => Ungrounded.Count;
        public double GroundedPercent => TotalValues == 0 ? 100.0 : 100.0 * GroundedValues / TotalValues;
    }

    // A run of digits with optional internal '.'/',' separators. Spaces are intentionally NOT part
    // of a number so "500 1400" parses as two values, not one merged 5001400.
    private static readonly Regex NumberToken =
        new(@"[-+]?\d+(?:[.,]\d+)*", RegexOptions.Compiled);

    public static GroundingReport Verify(IEnumerable<TechnologyRecord> records, string sourceText)
    {
        var index = BuildNumericIndex(sourceText);
        var ungrounded = new List<Finding>();
        int total = 0, grounded = 0;

        foreach (var rec in records)
        {
            var id = string.IsNullOrWhiteSpace(rec.DatapaperTechId) ? "(unnamed)" : rec.DatapaperTechId!;
            foreach (var (field, value, exact) in NumericFields(rec))
            {
                total++;
                if (IsGrounded(value, exact, index))
                    grounded++;
                else
                    ungrounded.Add(new Finding(id, field, FormatValue(value)));
            }
        }

        return new GroundingReport
        {
            TotalValues = total,
            GroundedValues = grounded,
            Ungrounded = ungrounded
        };
    }

    public static string FormatReport(string pdfName, GroundingReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Numeric grounding report — {pdfName}");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Checked {report.TotalValues} numeric values; " +
                      $"{report.GroundedValues} found in source ({report.GroundedPercent:0.#}%).");
        sb.AppendLine();
        if (report.UngroundedCount == 0)
        {
            sb.AppendLine("All numeric values were found in the source text.");
            return sb.ToString();
        }

        sb.AppendLine($"{report.UngroundedCount} value(s) NOT found verbatim in the source — review for LLM drift:");
        sb.AppendLine();
        foreach (var f in report.Ungrounded)
            sb.AppendLine($"  • [{f.TechId}] {f.Field} = {f.Value}");
        return sb.ToString();
    }

    // Exposed for the condense-check diagnostic: every numeric value found in a text (a multiset —
    // duplicates are kept). Pure regex + arithmetic, no model call. Callers can Distinct() it.
    public static List<double> ExtractNumbers(string text) => BuildNumericIndex(text);

    // True if value occurs in the index within a tight tolerance. Near-exact (not the 0.5% used for
    // cross-stage grounding) because condensation is supposed to preserve numbers verbatim, so any
    // real deviation is itself the signal we want to surface.
    public static bool Contains(List<double> index, double value)
        => ContainsWithin(index, value, Math.Max(Math.Abs(value) * 1e-9, 1e-9));

    // The numeric fields worth grounding, with a flag for exact (integer) matching.
    // Years and TRL are matched exactly; measured quantities allow a small tolerance and
    // percentage-form equivalence (handled in IsGrounded).
    private static IEnumerable<(string field, double value, bool exact)> NumericFields(TechnologyRecord r)
    {
        if (r.ReferenceUnitSize.HasValue)   yield return ("reference_unit_size", r.ReferenceUnitSize.Value, false);
        if (r.BaseYear.HasValue)            yield return ("cost_base_year", r.BaseYear.Value, true);
        if (r.DataReferenceYear.HasValue)   yield return ("data_reference_year", r.DataReferenceYear.Value, true);
        if (r.Trl.HasValue)                 yield return ("trl", r.Trl.Value, true);
        if (r.OverallEfficiency.HasValue)   yield return ("efficiency", r.OverallEfficiency.Value, false);
        if (r.MinInstallationSize.HasValue) yield return ("min_installation_size", r.MinInstallationSize.Value, false);
        if (r.LifetimeYears.HasValue)       yield return ("lifetime_yr", r.LifetimeYears.Value, false);
        if (r.Capex.HasValue)               yield return ("capex", (double)r.Capex.Value, false);
        if (r.OpexFix.HasValue)             yield return ("opex_fix", (double)r.OpexFix.Value, false);
        for (int i = 0; i < r.RatiosIn.Count; i++)  yield return ($"ratios_in[{i}]", r.RatiosIn[i], false);
        for (int i = 0; i < r.RatiosOut.Count; i++) yield return ($"ratios_out[{i}]", r.RatiosOut[i], false);
    }

    private static bool IsGrounded(double value, bool exact, List<double> index)
    {
        if (exact)
            // Half-unit window so an integer matches its own token but not a neighbour.
            return ContainsWithin(index, value, 0.5);

        // Try the value as-is plus its percentage equivalents: a stored 0.65 should match a "65%"
        // in the source, and a stored 65 should match "0.65".
        foreach (var candidate in new[] { value, value * 100.0, value / 100.0 })
        {
            var tol = Math.Max(Math.Abs(candidate) * 0.005, 1e-6); // 0.5% relative — catches drift, allows rounding
            if (ContainsWithin(index, candidate, tol))
                return true;
        }
        return false;
    }

    private static bool ContainsWithin(List<double> values, double target, double tol)
    {
        // Linear scan: numeric token counts per document are small (hundreds to low thousands).
        foreach (var v in values)
            if (Math.Abs(v - target) <= tol)
                return true;
        return false;
    }

    private static List<double> BuildNumericIndex(string text)
    {
        var values = new List<double>();
        if (string.IsNullOrEmpty(text))
            return values;

        foreach (Match m in NumberToken.Matches(text))
            foreach (var v in InterpretToken(m.Value))
                values.Add(v);

        return values;
    }

    // Turns one raw number token into the value(s) it could represent, coping with both US
    // (1,234.5) and EU (1.234,5) conventions. Where a single separator is genuinely ambiguous
    // (e.g. "1,450" — thousands or decimal?), BOTH interpretations are emitted; for a membership
    // test that only ever makes matching more lenient, which is the safe bias for a flagging tool.
    private static IEnumerable<double> InterpretToken(string token)
    {
        var sign = 1.0;
        var s = token;
        if (s.StartsWith('+')) s = s[1..];
        else if (s.StartsWith('-')) { sign = -1.0; s = s[1..]; }

        int lastDot = s.LastIndexOf('.');
        int lastComma = s.LastIndexOf(',');
        int dotCount = s.Count(c => c == '.');
        int commaCount = s.Count(c => c == ',');

        var results = new List<double>();

        if (lastDot >= 0 && lastComma >= 0)
        {
            // Both separators present → the rightmost is the decimal point, the other groups thousands.
            char dec = lastDot > lastComma ? '.' : ',';
            char grp = dec == '.' ? ',' : '.';
            var cleaned = s.Replace(grp.ToString(), "").Replace(dec, '.');
            AddIfParsed(results, sign, cleaned);
        }
        else if (lastComma >= 0 || lastDot >= 0)
        {
            char sep = lastComma >= 0 ? ',' : '.';
            int sepCount = lastComma >= 0 ? commaCount : dotCount;
            int afterLen = s.Length - s.LastIndexOf(sep) - 1;

            if (sepCount > 1)
            {
                // Repeated same separator → grouping only (e.g. 1.234.567).
                AddIfParsed(results, sign, s.Replace(sep.ToString(), ""));
            }
            else if (afterLen == 3)
            {
                // Ambiguous (1,450 / 1.450): emit both thousands and decimal readings.
                AddIfParsed(results, sign, s.Replace(sep.ToString(), ""));
                AddIfParsed(results, sign, s.Replace(sep, '.'));
            }
            else
            {
                // Single separator, not 3 trailing digits → decimal.
                AddIfParsed(results, sign, s.Replace(sep, '.'));
            }
        }
        else
        {
            AddIfParsed(results, sign, s);
        }

        return results;
    }

    private static void AddIfParsed(List<double> results, double sign, string s)
    {
        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            results.Add(sign * v);
    }

    private static string FormatValue(double v) =>
        v.ToString("0.########", CultureInfo.InvariantCulture);
}
