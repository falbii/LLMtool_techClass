using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using GitHub.Copilot.SDK;
using static TechClassificationApp.TechClassifierUtils;

namespace TechClassificationApp;

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
    public int? BaseYear { get; set; }
    public string? Location { get; set; }
    public string? Currency { get; set; }
    public int? DataReferenceYear { get; set; }
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
    public double? MinInstallationSize { get; set; }
    public string? MinInstallationSizeUnit { get; set; }
    public double? LifetimeYears { get; set; }
    public decimal? Capex { get; set; }
    public string? CapexUnit { get; set; }
    public decimal? OpexFix { get; set; }
    public string? OpexFixUnit { get; set; }
}

public static class TechnologyClassifier
{
    private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

    private static readonly Regex SummarySectionHeaderPattern = new(
        @"═{3,}\s*TECHNOLOGY\s+\d+:\s*(?<name>.+?)\s*═{3,}",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
            DatapaperTechId      = GetValue(lookup, "Datapaper Tech ID", "tech_id"),
            ProcessType          = GetValue(lookup, "ProcessType", "process_type"),
            Description          = GetValue(lookup, "description"),
            UnitOperation        = GetValue(lookup, "unit_operation"),
            Summary              = GetValue(lookup, "summary"),
            MainSector           = GetValue(lookup, "main_sector"),
            MainCategory         = GetValue(lookup, "main_category"),
            CategorySpec         = GetValue(lookup, "category_spec"),
            TechType             = GetValue(lookup, "tech_type"),
            ReferenceUnitSize    = ParseDouble(GetValue(lookup, "reference_unit_size"), "reference_unit_size", errors),
            ReferenceUnitSizeUnit= GetValue(lookup, "reference_unit_size_unit", "Reference Unit Size Unit"),
            BaseYear             = ParseInt(GetValue(lookup, "base year", "cost_base_year"), "cost_base_year", errors),
            Location             = GetValue(lookup, "Location"),
            Currency             = GetValue(lookup, "Currency", "currency"),
            DataReferenceYear    = ParseInt(GetValue(lookup, "Data Reference Year", "data_reference_year"), "data_reference_year", errors),
            Trl                  = ParseInt(GetValue(lookup, "trl_(1-9)", "trl"), "trl_(1-9)", errors),
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
            LifetimeYears        = ParseDouble(GetValue(lookup, "lifetime_yr"), "lifetime_yr", errors),
            Capex                = ParseDecimal(GetValue(lookup, "capex"), "capex", errors),
            CapexUnit            = GetValue(lookup, "capex_unit"),
            OpexFix              = ParseDecimal(GetValue(lookup, "opex_fix"), "opex_fix", errors),
            OpexFixUnit          = GetValue(lookup, "opex_fix_unit"),
        };

        var minInstallRaw = GetValue(lookup, "min_installation_size");
        var minInstallParsed = ParseValueWithUnit(minInstallRaw);
        if (minInstallParsed != null)
        {
            tech.MinInstallationSize = minInstallParsed.Value.value;
            tech.MinInstallationSizeUnit = minInstallParsed.Value.unit;
        }
        else if (!string.IsNullOrWhiteSpace(minInstallRaw))
        {
            errors.Add("min_installation_size: unable to parse numeric value and unit");
        }

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
        if (tech.BaseYear.HasValue) fieldCount++;
        if (tech.DataReferenceYear.HasValue) fieldCount++;
        if (tech.Trl.HasValue) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.TechMaturity)) fieldCount++;
        if (tech.OverallEfficiency.HasValue) fieldCount++;
        if (tech.CarriersIn.Count > 0 || tech.CarriersOut.Count > 0) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainInput) || !string.IsNullOrWhiteSpace(tech.MainOut)) fieldCount++;
        if (tech.LifetimeYears.HasValue || tech.Capex.HasValue || tech.OpexFix.HasValue) fieldCount++;

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
        var dataReferenceYear = row.DataReferenceYear is >= 1900 ? row.DataReferenceYear : null;
        var baseYear = row.BaseYear is >= 1900 ? row.BaseYear : null;

        // No year means we can't confirm two rows represent the same data point.
        // The rowIndex suffix keeps them permanently separate to avoid silent data loss.
        if (!dataReferenceYear.HasValue && !baseYear.HasValue)
            return $"{normalizedTechnology}|UNMERGEABLE|{rowIndex}";

        var yearToken = dataReferenceYear.HasValue && baseYear.HasValue
            ? $"DR{dataReferenceYear.Value}|BY{baseYear.Value}"
            : dataReferenceYear.HasValue
                ? $"DR{dataReferenceYear.Value}"
                : $"BY{baseYear!.Value}";

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
        BaseYear              = source.BaseYear,
        Location              = source.Location,
        Currency              = source.Currency,
        DataReferenceYear     = source.DataReferenceYear,
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
        MinInstallationSize   = source.MinInstallationSize,
        MinInstallationSizeUnit = source.MinInstallationSizeUnit,
        LifetimeYears         = source.LifetimeYears,
        Capex                 = source.Capex,
        CapexUnit             = source.CapexUnit,
        OpexFix               = source.OpexFix,
        OpexFixUnit           = source.OpexFixUnit
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
        target.BaseYear              ??= source.BaseYear;
        target.Location              ??= source.Location;
        target.Currency              ??= source.Currency;
        target.DataReferenceYear     ??= source.DataReferenceYear;
        target.Trl                   ??= source.Trl;
        target.TechMaturity          ??= source.TechMaturity;
        target.OverallEfficiency     ??= source.OverallEfficiency;
        target.EfficiencyUnit        ??= source.EfficiencyUnit;
        target.MainInput             ??= source.MainInput;
        target.MainOut               ??= source.MainOut;
        target.MinInstallationSize   ??= source.MinInstallationSize;
        target.MinInstallationSizeUnit ??= source.MinInstallationSizeUnit;
        target.LifetimeYears         ??= source.LifetimeYears;
        target.Capex                 ??= source.Capex;
        target.CapexUnit             ??= source.CapexUnit;
        target.OpexFix               ??= source.OpexFix;
        target.OpexFixUnit           ??= source.OpexFixUnit;

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
            if (string.IsNullOrWhiteSpace(row.DatapaperTechId))
                row.DatapaperTechId = GenerateTechId(row, usedIds);

            var id = row.DatapaperTechId!;
            if (!usedIds.Add(id))
            {
                var counter = 2;
                var candidate = $"{id}_{counter}";
                while (!usedIds.Add(candidate)) { counter++; candidate = $"{id}_{counter}"; }
                row.DatapaperTechId = candidate;
            }
        }
    }

    // --- Classification pipeline ---

    public static async Task<string?> RunAsync(
        CopilotClient client, string model, string pdfFile, string outputDirectory)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ PDF not found or invalid path.");
            Console.ResetColor();
            return null;
        }

        var txtPath = Path.Combine(outputDirectory,
            $"{Path.GetFileNameWithoutExtension(pdfFile)}.txt");

        if (!File.Exists(txtPath))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Please run 'auto-summarize' first to extract technology summaries.");
            Console.WriteLine($"    No summary file found at: {txtPath}");
            Console.ResetColor();
            return null;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📋 Classifying from TXT summary and writing to CSV...\n");
        Console.ResetColor();

        try
        {
            var txtContent = await File.ReadAllTextAsync(txtPath, Encoding.UTF8);
            var sections = ParseSectionsFromSummaryFile(txtContent);

            if (sections.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  No technology sections found in TXT file.");
                Console.ResetColor();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   1. Parsing TXT sections...");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   Found {sections.Count} technology sections in TXT");
            Console.ResetColor();
            foreach (var (name, _) in sections)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"     • {name}");
                Console.ResetColor();
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   2. Converting summaries to structured data...\n");
            Console.ResetColor();

            var allRows = new List<IDictionary<string, string>>();
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

                Console.WriteLine($"   Batch {batchIndex + 1}/{totalBatches} (technologies {batchStart + 1}-{batchStart + batchCount})");

                // Fresh session per batch: accumulated context from prior batches would grow the prompt
                // on every iteration and could bias the model toward prior extractions.
                await using var batchSession = await client.CreateSessionAsync(new SessionConfig
                {
                    Model = model,
                    Streaming = true,
                    OnPermissionRequest = PermissionHandler.ApproveAll
                });

                try
                {
                    var prompt = BuildClassificationFromSummaryPrompt(batchSections);
                    var jsonResponse = await Program.SendMessageAndStreamToConsoleAsync(batchSession, prompt);
                    var json = ExtractJson(jsonResponse);

                    if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith('[') || !json.TrimEnd().EndsWith(']'))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"   ⚠️ No valid JSON returned — retrying");
                        Console.ResetColor();

                        // New session for retry: batchSession already has the failed response in its context,
                        // which tends to reproduce the same malformed JSON.
                        await using var retrySession = await client.CreateSessionAsync(new SessionConfig
                        {
                            Model = model,
                            Streaming = true,
                            OnPermissionRequest = PermissionHandler.ApproveAll
                        });

                        jsonResponse = await Program.SendMessageAndCollectResponseSilentAsync(retrySession, prompt);
                        json = ExtractJson(jsonResponse);

                        if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith('[') || !json.TrimEnd().EndsWith(']'))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"   ✗ Retry also failed — skipping batch");
                            Console.ResetColor();
                            continue;
                        }
                    }

                    var batchRows = ParseRowsFromJson(json);
                    allRows.AddRange(batchRows);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"   ✓ ({batchRows.Count} rows)");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ {ex.Message}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine();

            if (allRows.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  No technologies were successfully extracted.");
                Console.ResetColor();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"   📋 Total extracted rows: {allRows.Count}");
            Console.ResetColor();
            Console.WriteLine();

            var classifications = new List<TechnologyRecord>();
            var rowErrors = new List<string>();

            Console.Write("   🔍 Parsing and validating...");
            for (int i = 0; i < allRows.Count; i++)
            {
                var classification = ParseRecord(allRows[i], out var errors);
                foreach (var error in errors)
                    rowErrors.Add($"Row {i + 1}: {error}");
                classifications.Add(classification);
            }
            Console.WriteLine(" Done\n");

            var completeClassifications = classifications
                .Where(HasMeaningfulData)
                .ToList();
            var filteredCount = classifications.Count - completeClassifications.Count;

            if (completeClassifications.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  No technologies with sufficient data found.");
                Console.ResetColor();
                return null;
            }

            var outputPath = Path.Combine(
                outputDirectory,
                $"{Path.GetFileNameWithoutExtension(pdfFile)}_classification.csv");

            var existingRows = File.Exists(outputPath)
                ? TechnologyClassificationCsv.ReadCsv(outputPath)
                : new List<TechnologyRecord>();

            if (existingRows.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"   Merging with {existingRows.Count} existing rows from CSV...");
                Console.ResetColor();
            }

            var mergedClassifications = MergeByTechnologyAndYear(
                existingRows.Concat(completeClassifications));

            try
            {
                Console.Write("   💾 Writing CSV file...");
                TechnologyClassificationCsv.WriteCsv(outputPath, mergedClassifications);
                Console.WriteLine(" Done\n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Failed to write CSV file: {ex.Message}");
                Console.ResetColor();
                return null;
            }

            var mergedCount = (existingRows.Count + completeClassifications.Count) - mergedClassifications.Count;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"   📁 Saved to: {outputPath}");
            Console.WriteLine($"   ✓ {mergedClassifications.Count} rows exported");
            if (filteredCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   ⚠️  {filteredCount} incomplete records filtered out (too few populated fields)");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            if (mergedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   ⚠️  {mergedCount} duplicate rows merged (same technology + year)");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine();
            Console.WriteLine($"✅ Classification complete!");
            Console.ResetColor();

            if (rowErrors.Count > 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️  Parsing notes ({rowErrors.Count} items):");
                Console.ResetColor();
                foreach (var error in rowErrors.Take(10))
                    Console.WriteLine($"   {error}");
                if (rowErrors.Count > 10)
                    Console.WriteLine($"   ...and {rowErrors.Count - 10} more");
            }

            return outputPath;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Auto-classify failed: {ex.Message}");
            Console.ResetColor();
            return null;
        }
    }

    private static List<(string Name, string Content)> ParseSectionsFromSummaryFile(string txtContent)
    {
        var sections = new List<(string Name, string Content)>();
        if (string.IsNullOrWhiteSpace(txtContent))
            return sections;

        var matches = SummarySectionHeaderPattern.Matches(txtContent);
        for (int i = 0; i < matches.Count; i++)
        {
            string name = matches[i].Groups["name"].Value.Trim();
            var contentStart = matches[i].Index + matches[i].Length;
            var contentEnd = (i + 1 < matches.Count) ? matches[i + 1].Index : txtContent.Length;

            var content = txtContent[contentStart..contentEnd].Trim();

            // Trim the trailing "───" separator that TechnologySummarizer.WriteProgressAsync appends.
            var separatorIdx = content.LastIndexOf("───");
            if (separatorIdx >= 0)
                content = content[..separatorIdx].Trim();

            if (!string.IsNullOrWhiteSpace(content))
                sections.Add((name, content));
        }

        return sections;
    }
}

public static class TechnologyClassificationCsv
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
        sb.AppendLine(string.Join(',', HeaderOrder.Select(TechClassifierUtils.EscapeCsv)));

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
                TechClassifierUtils.FormatInt(row.BaseYear),
                TechClassifierUtils.FormatDouble(row.ReferenceUnitSize),
                row.ReferenceUnitSizeUnit ?? string.Empty,
                row.Location ?? string.Empty,
                row.Currency ?? string.Empty,
                TechClassifierUtils.FormatInt(row.Trl),
                row.TechMaturity ?? string.Empty,
                TechClassifierUtils.FormatDouble(row.OverallEfficiency),
                row.EfficiencyUnit ?? string.Empty,
                TechClassifierUtils.JoinList(row.CarriersIn.Cast<string?>()),
                row.MainInput ?? string.Empty,
                TechClassifierUtils.JoinList(row.RatiosIn.Select(d => TechClassifierUtils.FormatDouble(d)).Cast<string?>()),
                TechClassifierUtils.JoinList(row.UnitsIn.Cast<string?>()),
                TechClassifierUtils.JoinList(row.CarriersOut.Cast<string?>()),
                row.MainOut ?? string.Empty,
                TechClassifierUtils.JoinList(row.RatiosOut.Select(d => TechClassifierUtils.FormatDouble(d)).Cast<string?>()),
                TechClassifierUtils.JoinList(row.UnitsOut.Cast<string?>()),
                TechClassifierUtils.FormatDecimal(row.Capex),
                row.CapexUnit ?? string.Empty,
                TechClassifierUtils.FormatDecimal(row.OpexFix),
                row.OpexFixUnit ?? string.Empty,
                TechClassifierUtils.FormatDouble(row.LifetimeYears),
                TechClassifierUtils.FormatInt(row.DataReferenceYear),
                row.Summary ?? string.Empty
            };

            sb.AppendLine(string.Join(',', fields.Select(TechClassifierUtils.EscapeCsv)));
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    public static List<TechnologyRecord> ReadCsv(string filePath)
    {
        var results = new List<TechnologyRecord>();
        if (!File.Exists(filePath)) return results;

        var content = File.ReadAllText(filePath, Encoding.UTF8);
        var records = TechClassifierUtils.ParseCsvRecords(content);
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
