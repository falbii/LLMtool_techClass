using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static TechClassificationApp.TechClassifierHelpers;

namespace TechClassificationApp;

// The /classify pipeline stage: reads the summary MD sections, asks the model to convert them to
// structured JSON rows in batches (with retry + per-technology fallback), validates/merges the
// records, and writes 02_output/22_tech_classification_csv/<name>_classification.csv. A final
// deterministic verification pass flags numbers that can't be found in the source
// (report: 02_output/23_validation/classification_csv_check/).
public static class TechnologyClassifier
{
    private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

    // Always returns a record — parse errors are warnings, not failures.
    // HasMeaningfulData is the downstream gate that decides if the record is usable.
    public static TechnologyRecord ParseRecord(IDictionary<string, string> row, out List<string> errors)
    {
        errors = new List<string>();

        var lookup = row.ToDictionary(
            k => NormalizeHeader(k.Key),
            v => v.Value,
            StringComparer.OrdinalIgnoreCase);

        var tech = new TechnologyRecord
        {
            DatapaperTechId      = GetValue(lookup, "tech_id", "Datapaper Tech ID"),
            ProcessType          = GetValue(lookup, "ProcessType", "process_type"),
            Description          = GetValue(lookup, "description"),
            UnitOperation        = GetValue(lookup, "unit_operation"),
            Summary              = GetValue(lookup, "summary"),
            MainSector           = GetValue(lookup, "main_sector"),
            MainCategory         = GetValue(lookup, "main_category"),
            CategorySpec         = GetValue(lookup, "category_spec"),
            TechType             = GetValue(lookup, "tech_type"),
            ReferenceUnitSize    = ParseDouble(GetValue(lookup, "reference_unit_size"), "reference_unit_size", errors, magnitude: true),
            ReferenceUnitSizeUnit= GetValue(lookup, "reference_unit_size_unit", "Reference Unit Size Unit"),
            // Old header names accepted as aliases so pre-rename CSVs still merge correctly.
            Year                 = ParseInt(GetValue(lookup, "year", "base_year"), "year", errors),
            Location             = GetValue(lookup, "Location"),
            Currency             = GetValue(lookup, "Currency", "currency"),
            RefYear              = ParseInt(GetValue(lookup, "ref_year", "Data Reference Year"), "ref_year", errors),
            Trl                  = ParseInt(GetValue(lookup, "trl", "trl_(1-9)"), "trl", errors),
            TechMaturity         = GetValue(lookup, "tech_maturity"),
            OverallEfficiency    = ParseDouble(GetValue(lookup, "efficiency", "lhv_efficiency", "overall_efficiency"), "efficiency", errors),
            EfficiencyUnit       = GetValue(lookup, "efficiency_unit"),
            CarriersIn           = ParseStringList(GetValue(lookup, "carriers_in")),
            MainInput            = GetValue(lookup, "main_input"),
            RatiosIn             = ParseDoubleList(GetValue(lookup, "ratios_in"), "ratios_in", errors),
            UnitsIn              = ParseStringList(GetValue(lookup, "units_in")),
            CarriersOut          = ParseStringList(GetValue(lookup, "carriers_out")),
            MainOut              = GetValue(lookup, "main_out"),
            RatiosOut            = ParseDoubleList(GetValue(lookup, "ratios_out"), "ratios_out", errors),
            UnitsOut             = ParseStringList(GetValue(lookup, "units_out")),
            Lifetime             = ParseDouble(GetValue(lookup, "lifetime", "lifetime_yr"), "lifetime", errors, magnitude: true),
            LifetimeUnit         = GetValue(lookup, "lifetime_unit"),
            Capex                = ParseDecimal(GetValue(lookup, "capex"), "capex", errors, magnitude: true),
            CapexUnit            = GetValue(lookup, "capex_unit"),
            Opex                 = ParseDecimal(GetValue(lookup, "opex", "opex_fix"), "opex", errors, magnitude: true),
            OpexUnit             = GetValue(lookup, "opex_unit", "opex_fix_unit"),
        };

        return tech;
    }

    public static string BuildClassificationFromSummaryPrompt(List<(string Name, string Content)> technologySections)
    {
        if (technologySections == null || technologySections.Count == 0)
            throw new ArgumentException("Technology sections cannot be null or empty");

        var template = LoadPromptTemplate("classification_from_summary.md");

        return template
            .Replace("{{TECHNOLOGY_COUNT}}", technologySections.Count.ToString(CultureInfo.InvariantCulture))
            .Replace("{{SOURCE_LABEL}}", "summary")
            .Replace("{{TECHNOLOGY_SECTIONS}}", BuildTechnologySectionsContent(technologySections));
    }

    // Cheap shape check that the extracted text looks like a JSON array, before attempting to parse.
    public static bool IsValidJsonArray(string? json) =>
        !string.IsNullOrWhiteSpace(json) && json.TrimStart().StartsWith('[') && json.TrimEnd().EndsWith(']');

    public static string ExtractJson(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return string.Empty;

        var fenceMatch = Regex.Match(response,
            @"```(?:\w*)\s*\r?\n?(?<json>[\s\S]*?)\r?\n?\s*```",
            RegexOptions.IgnoreCase);
        if (fenceMatch.Success)
        {
            var inner = fenceMatch.Groups["json"].Value.Trim();
            if (inner.Length > 0) return inner;
        }

        var cleaned = response.Trim();
        while (cleaned.StartsWith('`')) cleaned = cleaned.TrimStart('`');
        cleaned = Regex.Replace(cleaned, @"^json\s*", "", RegexOptions.IgnoreCase).TrimStart();
        while (cleaned.EndsWith('`')) cleaned = cleaned.TrimEnd('`');
        cleaned = cleaned.Trim();

        var start = cleaned.IndexOf('[');
        var end = cleaned.LastIndexOf(']');
        if (start >= 0 && end > start)
        {
            var extracted = cleaned.Substring(start, end - start + 1).Trim();
            if (extracted.StartsWith('[') && extracted.EndsWith(']'))
                return extracted;
        }

        return string.Empty;
    }

    public static List<Dictionary<string, string>> ParseRowsFromJson(string json)
    {
        var rows = new List<Dictionary<string, string>>();
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array) return rows;

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.Object) continue;
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var prop in element.EnumerateObject())
                {
                    var value = ConvertJsonValueToString(prop.Value);
                    if (value != null) row[prop.Name] = value;
                }
                rows.Add(row);
            }
        }
        catch (JsonException)
        {
            // Return empty list; caller handles missing rows
        }
        return rows;
    }

    public static string GenerateTechId(TechnologyRecord tech, HashSet<string> usedIds)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(tech.MainInput))
            parts.Add(ExtractAbbreviation(tech.MainInput));
        else if (tech.CarriersIn.Count > 0)
            parts.Add(ExtractAbbreviation(tech.CarriersIn[0]));

        if (!string.IsNullOrWhiteSpace(tech.UnitOperation))
        {
            var abbrev = ExtractAbbreviation(tech.UnitOperation);
            if (!parts.Contains(abbrev)) parts.Add(abbrev);
        }

        if (!string.IsNullOrWhiteSpace(tech.ProcessType))
        {
            var abbrev = ExtractAbbreviation(tech.ProcessType);
            if (!parts.Contains(abbrev)) parts.Add(abbrev);
        }

        if (!string.IsNullOrWhiteSpace(tech.MainOut) && parts.Count < 4)
        {
            var abbrev = ExtractAbbreviation(tech.MainOut);
            if (!parts.Contains(abbrev)) parts.Add(abbrev);
        }

        var baseId = string.Join("_", parts).ToUpperInvariant();
        if (baseId.Length > 20) baseId = baseId[..20];

        return MakeUnique(baseId, usedIds);
    }

    // Returns baseId if unused, otherwise appends the lowest free _N suffix.
    // Does not mutate usedIds — the caller adds the returned id when it commits it.
    private static string MakeUnique(string baseId, HashSet<string> usedIds)
    {
        if (!usedIds.Contains(baseId)) return baseId;

        var counter = 2;
        while (usedIds.Contains($"{baseId}_{counter}")) counter++;
        return $"{baseId}_{counter}";
    }

    public static string ExtractAbbreviation(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "UNK";
        text = text.Trim();

        var words = Regex.Split(text, @"\W+").Where(w => !string.IsNullOrEmpty(w)).ToList();
        if (words.Count == 0) return "UNK";
        if (words.Count == 1)
            return words[0].Length > 3 ? words[0][..3].ToUpperInvariant() : words[0].ToUpperInvariant();

        return string.Concat(words.Select(w => w[0])).ToUpperInvariant();
    }

    public static bool HasMeaningfulData(TechnologyRecord tech)
    {
        int fieldCount = 0;

        if (!string.IsNullOrWhiteSpace(tech.Description)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.UnitOperation)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.Summary)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.ProcessType)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainSector)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainCategory)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.CategorySpec)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.TechType)) fieldCount++;
        if (tech.Year.HasValue) fieldCount++;
        if (tech.Trl.HasValue) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.TechMaturity)) fieldCount++;
        if (tech.OverallEfficiency.HasValue) fieldCount++;
        if (tech.CarriersIn.Count > 0 || tech.CarriersOut.Count > 0) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainInput) || !string.IsNullOrWhiteSpace(tech.MainOut)) fieldCount++;
        if (tech.Lifetime.HasValue || tech.Capex.HasValue || tech.Opex.HasValue) fieldCount++;

        // >= 2 rather than 1: a single populated field (e.g. just a generated ID) isn't usable data.
        return fieldCount >= 2;
    }

    public static List<TechnologyRecord> MergeByTechnologyAndYear(IEnumerable<TechnologyRecord> rows)
    {
        var merged = new List<TechnologyRecord>();
        var indexByKey = new Dictionary<string, int>(KeyComparer);

        var rowIndex = 0;
        foreach (var row in rows)
        {
            var key = BuildTechnologyYearKey(row, rowIndex);
            if (indexByKey.TryGetValue(key, out var index))
                MergeMissingFields(merged[index], row);
            else
            {
                merged.Add(Clone(row));
                indexByKey[key] = merged.Count - 1;
            }
            rowIndex++;
        }

        EnsureUniqueDatapaperIds(merged);
        return merged;
    }

    private static string BuildTechnologyYearKey(TechnologyRecord row, int rowIndex)
    {
        var technologyName = FirstNonEmpty(
            row.TechType, row.Description, row.UnitOperation,
            row.MainCategory, row.ProcessType, row.DatapaperTechId, "unknown");

        var normalizedTechnology = NormalizeKeyText(technologyName);
        var dataYear = row.Year is >= 1900 ? row.Year : null;

        // No year means we can't confirm two rows represent the same data point.
        // The rowIndex suffix keeps them permanently separate to avoid silent data loss.
        // RefYear is not part of the key: it is constant for all rows of a paper.
        if (!dataYear.HasValue)
            return $"{normalizedTechnology}|UNMERGEABLE|{rowIndex}";

        var yearToken = $"Y{dataYear.Value}";

        var categorySpec = NormalizeKeyText(row.CategorySpec ?? "");
        var variant = NormalizeKeyText(row.TechType ?? "");

        if (!string.IsNullOrWhiteSpace(categorySpec))
            return $"{normalizedTechnology}|{yearToken}|{categorySpec}";

        if (!string.IsNullOrWhiteSpace(variant) && variant != normalizedTechnology)
            return $"{normalizedTechnology}|{yearToken}|{variant}";

        return $"{normalizedTechnology}|{yearToken}";
    }

    private static TechnologyRecord Clone(TechnologyRecord source) => new()
    {
        DatapaperTechId       = source.DatapaperTechId,
        ProcessType           = source.ProcessType,
        Description           = source.Description,
        UnitOperation         = source.UnitOperation,
        Summary               = source.Summary,
        MainSector            = source.MainSector,
        MainCategory          = source.MainCategory,
        CategorySpec          = source.CategorySpec,
        TechType              = source.TechType,
        ReferenceUnitSize     = source.ReferenceUnitSize,
        ReferenceUnitSizeUnit = source.ReferenceUnitSizeUnit,
        Year                  = source.Year,
        Location              = source.Location,
        Currency              = source.Currency,
        RefYear               = source.RefYear,
        Trl                   = source.Trl,
        TechMaturity          = source.TechMaturity,
        OverallEfficiency     = source.OverallEfficiency,
        EfficiencyUnit        = source.EfficiencyUnit,
        CarriersIn            = [..source.CarriersIn],
        MainInput             = source.MainInput,
        RatiosIn              = [..source.RatiosIn],
        UnitsIn               = [..source.UnitsIn],
        CarriersOut           = [..source.CarriersOut],
        MainOut               = source.MainOut,
        RatiosOut             = [..source.RatiosOut],
        UnitsOut              = [..source.UnitsOut],
        Lifetime              = source.Lifetime,
        LifetimeUnit          = source.LifetimeUnit,
        Capex                 = source.Capex,
        CapexUnit             = source.CapexUnit,
        Opex                  = source.Opex,
        OpexUnit              = source.OpexUnit
    };

    private static void MergeMissingFields(TechnologyRecord target, TechnologyRecord source)
    {
        target.DatapaperTechId       ??= source.DatapaperTechId;
        target.ProcessType           ??= source.ProcessType;
        target.Description           ??= source.Description;
        target.UnitOperation         ??= source.UnitOperation;
        target.Summary               ??= source.Summary;
        target.MainSector            ??= source.MainSector;
        target.MainCategory          ??= source.MainCategory;
        target.CategorySpec          ??= source.CategorySpec;
        target.TechType              ??= source.TechType;
        target.ReferenceUnitSize     ??= source.ReferenceUnitSize;
        target.ReferenceUnitSizeUnit ??= source.ReferenceUnitSizeUnit;
        target.Year                  ??= source.Year;
        target.Location              ??= source.Location;
        target.Currency              ??= source.Currency;
        target.RefYear               ??= source.RefYear;
        target.Trl                   ??= source.Trl;
        target.TechMaturity          ??= source.TechMaturity;
        target.OverallEfficiency     ??= source.OverallEfficiency;
        target.EfficiencyUnit        ??= source.EfficiencyUnit;
        target.MainInput             ??= source.MainInput;
        target.MainOut               ??= source.MainOut;
        target.Lifetime              ??= source.Lifetime;
        target.LifetimeUnit          ??= source.LifetimeUnit;
        target.Capex                 ??= source.Capex;
        target.CapexUnit             ??= source.CapexUnit;
        target.Opex                  ??= source.Opex;
        target.OpexUnit              ??= source.OpexUnit;

        if (target.CarriersIn.Count  == 0 && source.CarriersIn.Count  > 0) target.CarriersIn  = [..source.CarriersIn];
        if (target.RatiosIn.Count    == 0 && source.RatiosIn.Count    > 0) target.RatiosIn    = [..source.RatiosIn];
        if (target.UnitsIn.Count     == 0 && source.UnitsIn.Count     > 0) target.UnitsIn     = [..source.UnitsIn];
        if (target.CarriersOut.Count == 0 && source.CarriersOut.Count > 0) target.CarriersOut = [..source.CarriersOut];
        if (target.RatiosOut.Count   == 0 && source.RatiosOut.Count   > 0) target.RatiosOut   = [..source.RatiosOut];
        if (target.UnitsOut.Count    == 0 && source.UnitsOut.Count    > 0) target.UnitsOut    = [..source.UnitsOut];
    }

    private static void EnsureUniqueDatapaperIds(List<TechnologyRecord> rows)
    {
        var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows)
        {
            var baseId = string.IsNullOrWhiteSpace(row.DatapaperTechId)
                ? GenerateTechId(row, usedIds)
                : row.DatapaperTechId!;
            row.DatapaperTechId = MakeUnique(baseId, usedIds);
            usedIds.Add(row.DatapaperTechId);
        }
    }

    // --- Classification pipeline ---

    public static async Task<string?> RunAsync(Workspace ws, string pdfFile)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
        {
            ConsoleEx.Error("❌ PDF not found or invalid path.");
            return null;
        }

        var mdPath = Path.Combine(ws.MdDir,
            $"{Path.GetFileNameWithoutExtension(pdfFile)}_summary.md");

        if (!File.Exists(mdPath))
        {
            ConsoleEx.Warn($"⚠️  Please run '/summarize' first to extract technology summaries.");
            ConsoleEx.Warn($"    No summary file found at: {mdPath}");
            return null;
        }

        ConsoleEx.Info("📋 Classifying from MD summary and writing to CSV...\n");

        try
        {
            var sections = TechnologyMd.ReadSections(mdPath);

            if (sections.Count == 0)
            {
                ConsoleEx.Warn("⚠️  No technology sections found in MD file.");
                return null;
            }

            ConsoleEx.Warn("   1. Parsing MD sections...");
            ConsoleEx.Success($"   Found {sections.Count} technology sections in MD");
            foreach (var (name, _) in sections)
                ConsoleEx.Dim($"     • {name}");
            Console.WriteLine();

            ConsoleEx.Warn("   2. Converting summaries to structured data...\n");

            var outputPath = Path.Combine(
                ws.CsvDir,
                $"{Path.GetFileNameWithoutExtension(pdfFile)}_classification.csv");

            if (File.Exists(outputPath))
                ConsoleEx.Dim("   Existing CSV will be overwritten with this run's results.");

            // ref_year is the source's publication year — one value per paper, stamped on every row.
            var refYear = ExtractSourceYear(ws, pdfFile);
            if (refYear.HasValue)
                ConsoleEx.Dim($"   ref_year (source publication year): {refYear}");
            else
                ConsoleEx.Warn("   ⚠️ Could not determine the source publication year — ref_year will be empty.");

            var newRecords = new List<TechnologyRecord>();
            var rowErrors = new List<string>();
            int rowsSeen = 0, writtenCount = 0, mergedCount = 0;

            const int batchSize = 5;
            int totalBatches = (int)Math.Ceiling((double)sections.Count / batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                // Rate-limit between batches to avoid hitting Copilot request quotas.
                if (batchIndex > 0)
                    await Task.Delay(3000);

                var batchStart = batchIndex * batchSize;
                var batchCount = Math.Min(batchSize, sections.Count - batchStart);
                var batchSections = sections.Skip(batchStart).Take(batchCount).ToList();

                ConsoleEx.Plain($"   Batch {batchIndex + 1}/{totalBatches} (technologies {batchStart + 1}-{batchStart + batchCount})");

                var batchRows = await ClassifyBatchAsync(ws, batchSections);

                var (records, errors) = ParseAndValidate(batchRows, rowsSeen);
                if (refYear.HasValue)
                    foreach (var record in records)
                        record.RefYear = refYear;
                rowsSeen += batchRows.Count;
                newRecords.AddRange(records);
                rowErrors.AddRange(errors);

                // Incremental save: rewrite the CSV after each batch so a later failure
                // doesn't throw away the batches already completed.
                var save = SaveCsv(newRecords, outputPath, ws.Model);
                if (save == null)
                    return null; // write failed — message already printed
                (writtenCount, mergedCount) = save.Value;
            }
            Console.WriteLine();

            if (newRecords.Count == 0)
            {
                ConsoleEx.Warn("⚠️  No technologies were successfully extracted.");
                return null;
            }

            if (writtenCount == 0)
            {
                ConsoleEx.Warn("⚠️  No technologies with sufficient data found.");
                return null;
            }

            var filteredCount = newRecords.Count - newRecords.Count(HasMeaningfulData);

            Console.WriteLine();
            ConsoleEx.Success($"   📁 Saved to: {outputPath}");
            ConsoleEx.Success($"   ✓ {writtenCount} rows exported");
            if (filteredCount > 0)
                ConsoleEx.Warn($"   ⚠️  {filteredCount} incomplete records filtered out (too few populated fields)");
            if (mergedCount > 0)
                ConsoleEx.Warn($"   ⚠️  {mergedCount} duplicate rows merged (same technology + year)");
            Console.WriteLine();
            ConsoleEx.Success("✅ Classification complete!");

            if (rowErrors.Count > 0)
            {
                Console.WriteLine();
                ConsoleEx.Warn($"⚠️  Parsing notes ({rowErrors.Count} items):");
                foreach (var error in rowErrors.Take(10))
                    ConsoleEx.Plain($"   {error}");
                if (rowErrors.Count > 10)
                    ConsoleEx.Plain($"   ...and {rowErrors.Count - 10} more");
            }

            await VerifyAgainstSourceAsync(ws, pdfFile, outputPath);

            return outputPath;
        }
        catch (Exception ex)
        {
            ConsoleEx.Error($"❌ Classify failed: {ex.Message}");
            return null;
        }
    }

    // Deterministic post-check: confirm the numbers written to the CSV actually appear in the
    // source text, flagging any that don't (likely digit/unit drift introduced by one of the LLM
    // passes). Verification only — it never edits the CSV. Best-effort: a failure here is reported
    // but never fails the run, since the data has already been saved.
    //
    // Verifies against the condensed .md (the source the extraction chain actually reads), falling
    // back to the raw PDF text if the cache is missing. Switch to raw to also catch condensation drift.
    private static async Task VerifyAgainstSourceAsync(Workspace ws, string pdfFile, string csvPath)
    {
        try
        {
            var records = TechnologyCsv.ReadCsv(csvPath);
            if (records.Count == 0)
                return;

            var cachePath = PdfCondenser.GetCachePath(pdfFile, ws.CacheDir);
            var sourceText = File.Exists(cachePath)
                ? await File.ReadAllTextAsync(cachePath, Encoding.UTF8)
                : await PdfExtractor.ExtractTextAsync(pdfFile);

            var report = CondensedVerifier.Verify(records, sourceText);

            Console.WriteLine();
            if (report.UnverifiedCount == 0)
            {
                ConsoleEx.Success($"   🔎 Verification: all {report.TotalValues} numeric values verified against the source.");
                return;
            }

            ConsoleEx.Warn($"   🔎 Verification: {report.UnverifiedCount}/{report.TotalValues} numeric value(s) not found in the source (possible LLM drift):");
            foreach (var f in report.Unverified.Take(15))
                ConsoleEx.Dim($"     • [{f.TechId}] {f.Field} = {f.Value}");
            if (report.UnverifiedCount > 15)
                ConsoleEx.Dim($"     ...and {report.UnverifiedCount - 15} more");

            var reportPath = Path.Combine(
                ws.ClassifyCheckDir,
                $"{Path.GetFileNameWithoutExtension(pdfFile)}_check_classification_with_pdf.txt");
            await File.WriteAllTextAsync(
                reportPath, CondensedVerifier.FormatReport(Path.GetFileName(pdfFile), report), Encoding.UTF8);
            ConsoleEx.Dim($"     📁 Full report: {reportPath}");
        }
        catch (Exception ex)
        {
            ConsoleEx.Dim($"   (verification check skipped: {ex.Message})");
        }
    }

    // The source's publication year: taken from the PDF filename (e.g. "Allgoewer_2024.pdf" -> 2024),
    // else the first plausible year near the top of the condensed text (papers carry their
    // publication year on the first page). Null if neither yields one.
    public static int? ExtractSourceYear(Workspace ws, string pdfFile)
    {
        var fileName = Path.GetFileNameWithoutExtension(pdfFile);
        var match = Regex.Match(fileName, @"\b(19|20)\d{2}\b");
        if (match.Success)
            return int.Parse(match.Value, CultureInfo.InvariantCulture);

        var cachePath = PdfCondenser.GetCachePath(pdfFile, ws.CacheDir);
        if (File.Exists(cachePath))
        {
            var text = File.ReadAllText(cachePath, Encoding.UTF8);
            // Skip the "<!-- condensed from ... on <date> -->" header: its condensation date
            // would otherwise be mistaken for the publication year.
            text = Regex.Replace(text, @"^\s*<!--.*?-->", "", RegexOptions.Singleline);
            var head = text.Length > 2000 ? text[..2000] : text;
            match = Regex.Match(head, @"\b(19|20)\d{2}\b");
            if (match.Success)
                return int.Parse(match.Value, CultureInfo.InvariantCulture);
        }

        return null;
    }

    // Stage 1 — classify one batch of sections with retry. If the whole batch fails, fall back
    // to classifying each technology individually so one malformed entry doesn't drop the rest.
    private static async Task<List<Dictionary<string, string>>> ClassifyBatchAsync(
        Workspace ws, List<(string Name, string Content)> batchSections)
    {
        var rows = await TryClassifyAsync(ws, batchSections, maxAttempts: 2, stream: true);
        if (rows != null)
        {
            ConsoleEx.Success($"   ✓ {rows.Count} rows");
            return rows;
        }

        // Per-technology fallback: re-run each one alone.
        ConsoleEx.Warn("   ⚠️ Batch failed — retrying each technology individually...");
        var collected = new List<Dictionary<string, string>>();
        foreach (var section in batchSections)
        {
            var single = await TryClassifyAsync(ws, [section], maxAttempts: 1, stream: false);
            if (single != null)
            {
                collected.AddRange(single);
                ConsoleEx.Success($"     ✓ {section.Name} ({single.Count} rows)");
            }
            else
            {
                ConsoleEx.Error($"     ✗ {section.Name} — skipped");
            }
        }

        return collected;
    }

    // Sends one classification request and returns parsed rows, or null if no attempt yields
    // valid JSON. Each attempt uses a fresh session — reusing one tends to reproduce the same
    // malformed output. The first attempt can stream to the console; retries run silently.
    private static async Task<List<Dictionary<string, string>>?> TryClassifyAsync(
        Workspace ws, List<(string Name, string Content)> sections, int maxAttempts, bool stream)
    {
        var prompt = BuildClassificationFromSummaryPrompt(sections);

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await using var session = await Sessions.NewAsync(ws.Client, ws.Model);

                var response = stream && attempt == 1
                    ? await Program.SendMessageAndStreamToConsoleAsync(session, prompt)
                    : await Program.SendMessageAndCollectResponseSilentAsync(session, prompt);

                var json = ExtractJson(response);
                if (IsValidJsonArray(json))
                    return ParseRowsFromJson(json);

                if (attempt < maxAttempts)
                    ConsoleEx.Warn("   ⚠️ No valid JSON returned — retrying");
            }
            catch (Exception ex)
            {
                ConsoleEx.Error($"   ✗ {ex.Message}");
                return null;
            }
        }

        return null;
    }

    // Stage 2 — turn raw JSON rows into records, collecting per-row parse notes.
    // rowOffset keeps the "Row N" labels continuous across incrementally-processed batches.
    private static (List<TechnologyRecord> records, List<string> errors) ParseAndValidate(
        List<Dictionary<string, string>> rows, int rowOffset)
    {
        var records = new List<TechnologyRecord>();
        var errors = new List<string>();

        for (int i = 0; i < rows.Count; i++)
        {
            var record = ParseRecord(rows[i], out var rowErrors);
            // Deterministic normalization (efficiency scale) + range/alignment checks.
            var notes = TechnologyValidator.NormalizeAndValidate(record);
            foreach (var error in rowErrors.Concat(notes))
                errors.Add($"Row {rowOffset + i + 1}: {error}");
            records.Add(record);
        }

        return (records, errors);
    }

    // Stage 3 — filter to records with usable data, collapse duplicate technology+year rows the
    // model emitted within this run, and write (replacing any previous CSV — each run starts clean).
    // Called after every batch for crash resilience. Returns (rowsWritten, rowsMerged),
    // or null if the write failed.
    private static (int written, int merged)? SaveCsv(
        List<TechnologyRecord> newRecords, string outputPath, string model)
    {
        var meaningful = newRecords.Where(HasMeaningfulData).ToList();

        var merged = MergeByTechnologyAndYear(meaningful);
        if (merged.Count == 0)
            return (0, 0);

        try
        {
            TechnologyCsv.WriteCsv(outputPath, merged, model);
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            ConsoleEx.Error($"❌ Failed to write CSV file: {ex.Message}");
            return null;
        }

        var mergedCount = meaningful.Count - merged.Count;
        return (merged.Count, mergedCount);
    }
}
