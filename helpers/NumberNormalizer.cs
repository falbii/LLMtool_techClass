using System.Globalization;
using System.Text.RegularExpressions;

namespace TechClassificationApp;

// Turns a model-emitted numeric string into a single canonical invariant-culture numeric
// string, coping with mixed US (1,234.5) and EU (1.234,5) conventions. The classify prompt
// tells the model to copy numbers "exactly as the source states them", so values arrive with
// whatever separators the paper used. Parsing those with InvariantCulture alone is wrong twice:
//   • "1,500"  -> parse FAILS (NumberStyles.Float rejects ',') -> value silently dropped
//   • "1.500"  -> parsed as 1.5 (1000x too small)
// Normalizing first fixes both.
//
// `magnitude` resolves the one irreducible ambiguity — a single separator with exactly three
// trailing digits ("1,500" / "1.500"): for large quantities (capex, opex, sizes, lifetimes)
// that is thousands grouping; for 0-1-ish quantities (efficiency, ratios) it is a decimal.
// Returns null when the token holds no parseable number.
internal static class NumberNormalizer
{
    private static readonly Regex FirstNumber = new(@"[-+]?\d[\d.,]*", RegexOptions.Compiled);

    public static string? ToInvariant(string? raw, bool magnitude)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        // Take the first number-like run only, so trailing units ("1500 EUR/kW") or a stray
        // range ("1500-2000") don't merge into one garbage value.
        var match = FirstNumber.Match(raw);
        if (!match.Success)
            return null;

        var s = match.Value.TrimEnd('.', ',');
        var sign = string.Empty;
        if (s.StartsWith('+')) s = s[1..];
        else if (s.StartsWith('-')) { sign = "-"; s = s[1..]; }
        if (s.Length == 0)
            return null;

        int lastDot = s.LastIndexOf('.');
        int lastComma = s.LastIndexOf(',');
        int dotCount = s.Count(c => c == '.');
        int commaCount = s.Count(c => c == ',');

        string normalized;
        if (lastDot >= 0 && lastComma >= 0)
        {
            // Both separators present: the rightmost is the decimal point, the other groups thousands.
            char dec = lastDot > lastComma ? '.' : ',';
            char grp = dec == '.' ? ',' : '.';
            normalized = s.Replace(grp.ToString(), string.Empty).Replace(dec, '.');
        }
        else if (lastComma >= 0 || lastDot >= 0)
        {
            char sep = lastComma >= 0 ? ',' : '.';
            int sepCount = lastComma >= 0 ? commaCount : dotCount;
            int afterLen = s.Length - s.LastIndexOf(sep) - 1;

            if (sepCount > 1)
                normalized = s.Replace(sep.ToString(), string.Empty);          // grouping only: 1.234.567
            else if (afterLen == 3)
                normalized = magnitude
                    ? s.Replace(sep.ToString(), string.Empty)                  // thousands: 1,500 -> 1500
                    : s.Replace(sep, '.');                                     // decimal:   0,650 -> 0.650
            else
                normalized = s.Replace(sep, '.');                             // single sep, not 3 trailing -> decimal
        }
        else
        {
            normalized = s;
        }

        var result = sign + normalized;
        return double.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
            ? result
            : null;
    }
}
