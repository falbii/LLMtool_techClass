using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using GitHub.Copilot.SDK;
using PdfAnalysisApp;

namespace PdfAnalysisApp;

public static class Commands
{
    /// <summary>
    /// Displays all available PDFs in the folder and optionally lets the user pick one to analyze.
    /// Returns the selected file path or null if none selected.
    /// </summary>
    public static async Task<string?> HandleListPdfsAsync(string pdfFolder)
    {
        if (!Directory.Exists(pdfFolder))
        {
            Console.WriteLine("📁 PDF folder not found or not accessible.");
            return null;
        }
        
        var pdfs = Directory.GetFiles(pdfFolder, "*.pdf")
            .OrderBy(f => Path.GetFileName(f))
            .ToArray();
        if (pdfs.Length == 0)
        {
            Console.WriteLine("📁 No PDFs found in pdf_to_analyze folder.");
            return null;
        }
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📁 Available PDFs:");
        Console.ResetColor();
        for (int i = 0; i < pdfs.Length; i++)
        {
            var fileInfo = new FileInfo(pdfs[i]);
            var sizeKb = fileInfo.Length / 1024;
            Console.WriteLine($"   {i + 1}. {Path.GetFileName(pdfs[i])} ({sizeKb} KB)");
        }
        
        Console.WriteLine();
        Console.Write("Enter number or filename to analyze, or press Enter to cancel: ");
        var choice = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(choice))
            return null;
        
        // Try to analyze the selected file
        var selected = HandleAnalyzeCommand(choice, pdfFolder);
        if (selected != null)
            return selected;
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("No valid selection made.");
        Console.ResetColor();
        return null;
    }

    /// <summary>
    /// Displays the current PDF file being analyzed.
    /// </summary>
    public static void HandleCurrentCommand(string? currentPdfFile)
    {
        if (currentPdfFile != null)
            Console.WriteLine($"📄 Current PDF: {Path.GetFileName(currentPdfFile)}");
        else
            Console.WriteLine("❌ No PDF loaded. Use 'upload' or 'analyze' to select one.");
    }

    /// <summary>
    /// Displays all available commands.
    /// </summary>
    public static void HandleCommandsCommand()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("⚡ Available Commands:");
        Console.ResetColor();
        Console.WriteLine("  'commands' or 'help' - Display all available commands");
        Console.WriteLine("  'exit' or 'quit'     - Exit the program");
        Console.WriteLine("  'upload <path>'      - Upload a PDF to analyze (or drop PDFs in ./pdf_to_analyze/)");
        Console.WriteLine("  'list'               - List available PDFs");
        Console.WriteLine("  'analyze <file>'     - Analyze a PDF (use filename or list number)");
        Console.WriteLine("  'current'            - Show current PDF");
        Console.WriteLine("  'auto-classify'      - Classify technologies and export CSV");
        Console.WriteLine("  'batch-analyze <q>'  - Analyze all PDFs with a question\n");
        Console.ResetColor();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("💡 Tips:");
        Console.ResetColor();
        Console.WriteLine("  • Or just ask a question normally for AI analysis");
        if (File.Exists("./pdf_to_analyze"))
            Console.WriteLine("  • Drop PDFs in the ./pdf_to_analyze/ folder for quick access");
        Console.WriteLine();
    }

    /// <summary>
    /// Uploads a PDF file to the pdf_to_analyze folder.
    /// </summary>
    public static async Task<string?> HandleUploadPdfAsync(string sourceFile, string pdfFolder)
    {
        if (!File.Exists(sourceFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Source file not found.");
            Console.ResetColor();
            return null;
        }
        
        try
        {
            await Program.RunWithSpinnerAsync($" Uploading {Path.GetFileName(sourceFile)}", async () =>
            {
                string destFile = Path.Combine(pdfFolder, Path.GetFileName(sourceFile));
                await Task.Run(() => File.Copy(sourceFile, destFile, true));
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded: {Path.GetFileName(sourceFile)}");
            Console.ResetColor();
            return Path.Combine(pdfFolder, Path.GetFileName(sourceFile));
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error uploading PDF: {ex.Message}");
            Console.ResetColor();
            return null;
        }
    }

    /// <summary>
    /// Loads a PDF file for analysis.
    /// Accepts: full filename, filename without extension, or list number.
    /// </summary>
    public static string? HandleAnalyzeCommand(string input, string pdfFolder)
    {
        if (!Directory.Exists(pdfFolder) || string.IsNullOrEmpty(input))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Invalid input or folder.");
            Console.ResetColor();
            return null;
        }
        
        var pdfFiles = Directory.GetFiles(pdfFolder, "*.pdf")
            .OrderBy(f => Path.GetFileName(f))
            .ToArray();

        if (pdfFiles.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ No PDFs found in pdf_to_analyze folder.");
            Console.ResetColor();
            return null;
        }

        string? selectedFile = null;

        // Try to parse as number (1-indexed list position)
        if (int.TryParse(input, out int listNumber) && listNumber >= 1 && listNumber <= pdfFiles.Length)
        {
            selectedFile = pdfFiles[listNumber - 1];
        }
        else
        {
            // Try exact match with .pdf extension
            selectedFile = pdfFiles.FirstOrDefault(f => 
                Path.GetFileName(f).Equals(input, StringComparison.OrdinalIgnoreCase));

            // If not found, try matching without extension
            if (selectedFile == null)
            {
                selectedFile = pdfFiles.FirstOrDefault(f => 
                    Path.GetFileNameWithoutExtension(f).Equals(input, StringComparison.OrdinalIgnoreCase));
            }
        }

        if (selectedFile != null && File.Exists(selectedFile))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded: {Path.GetFileName(selectedFile)}");
            Console.ResetColor();
            return selectedFile;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ PDF '{input}' not found. Use 'list' to see available PDFs.");
            Console.ResetColor();
            return null;
        }
    }

    /// <summary>
    /// Prepares a message with PDF context for analysis.
    /// </summary>
    public static async Task<string> PrepareMessageWithPdfContextAsync(string pdfFile, string userQuestion)
    {
        try
        {
            var pdfText = await PdfAnalyzer.ExtractTextFromPdfAsync(pdfFile);
            var chunks = PdfAnalyzer.ChunkPdfContent(pdfText);
            
            var prompt = PdfAnalyzer.BuildAnalysisPrompt(chunks, userQuestion);
            return prompt;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Could not extract PDF text: {ex.Message}. Sending question without PDF context.");
            Console.ResetColor();
            return userQuestion;
        }
    }

    /// <summary>
    /// Handles batch analysis of all PDFs in the folder.
    /// </summary>
    public static async Task HandleBatchAnalyzeAsync(CopilotSession session, string pdfFolder, string question)
    {
        if (!Directory.Exists(pdfFolder))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ PDF folder not found.");
            Console.ResetColor();
            return;
        }
        
        var pdfFiles = Directory.GetFiles(pdfFolder, "*.pdf");
        if (pdfFiles.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("❌ No PDF files found in pdf_to_analyze folder.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"📊 Batch analyzing {pdfFiles.Length} PDF(s)...\n");
        Console.ResetColor();

        try
        {
            var pdfChunks = new Dictionary<string, List<string>>();
            
            foreach (var pdfFile in pdfFiles)
            {
                var pdfText = await PdfAnalyzer.ExtractTextFromPdfAsync(pdfFile);
                var chunks = PdfAnalyzer.ChunkPdfContent(pdfText);
                pdfChunks[pdfFile] = chunks;
            }

            var prompt = PdfAnalyzer.BuildBatchAnalysisPrompt(pdfChunks, question);
            await Program.SendMessageWithSpinnerAsync(session, prompt);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error extracting PDFs: {ex.Message}");
            Console.ResetColor();
        }
    }

    public static async Task<string?> HandleAutoClassifyAsync(CopilotSession session, string pdfFile, string outputFolder)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ PDF not found or invalid path.");
            Console.ResetColor();
            return null;
        }

        try
        {
            Directory.CreateDirectory(outputFolder);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Cannot create output folder: {ex.Message}");
            Console.ResetColor();
            return null;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("🧠 Classifying technologies from PDF...\n");
        Console.ResetColor();

        try
        {
            var pdfText = await PdfAnalyzer.ExtractTextFromPdfAsync(pdfFile);
            var chunks = PdfAnalyzer.ChunkPdfContent(pdfText);
            var prompt = BuildClassificationPrompt(chunks);

            Console.Write("  📤 Classifying with Copilot and writing to CSV... ");
            var spinnerChars = new[] { '|', '/', '-', '\\' };
            using var cts = new CancellationTokenSource();
            var spinnerLeft = Console.CursorLeft;
            var spinnerTop = Console.CursorTop;

            var spinnerTask = Task.Run(async () =>
            {
                int i = 0;
                while (!cts.Token.IsCancellationRequested)
                {
                    Console.SetCursorPosition(spinnerLeft, spinnerTop);
                    Console.Write(spinnerChars[i++ % spinnerChars.Length]);
                    try { await Task.Delay(100, cts.Token); } catch { break; }
                }
            });

            string response = await Program.SendMessageAndCollectResponseSilentAsync(session, prompt);
            
            cts.Cancel();
            await spinnerTask;
            Console.SetCursorPosition(spinnerLeft, spinnerTop);
            Console.WriteLine(" Done\n");

            var json = ExtractJson(response);
            if (string.IsNullOrWhiteSpace(json))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Copilot response did not include JSON.");
                Console.ResetColor();
                return null;
            }

            var rows = ParseRowsFromJson(json);
            if (rows.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  No technologies were found in the response.");
                Console.ResetColor();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  📋 Found {rows.Count} technologies\n");
            Console.ResetColor();

            var classifications = new List<TechnologyClassification>();
            var rowErrors = new List<string>();
            var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Console.Write("  🔍 Parsing and validating...");
            for (int i = 0; i < rows.Count; i++)
            {
                if (TechnologyClassifier.TryClassify(rows[i], out var classification, out var errors))
                {
                    // Ensure Datapaper Tech ID is generated
                    if (string.IsNullOrWhiteSpace(classification.DatapaperTechId))
                    {
                        classification.DatapaperTechId = GenerateTechId(classification, usedIds);
                    }
                    else if (usedIds.Contains(classification.DatapaperTechId))
                    {
                        // Make ID unique if duplicate
                        var originalId = classification.DatapaperTechId;
                        var counter = 2;
                        while (usedIds.Contains($"{originalId}_{counter}"))
                            counter++;
                        classification.DatapaperTechId = $"{originalId}_{counter}";
                    }
                    usedIds.Add(classification.DatapaperTechId);
                    classifications.Add(classification);
                }
                else
                {
                    // Only track errors, don't add incomplete classifications
                    foreach (var error in errors)
                        rowErrors.Add($"Row {i + 1}: {error}");
                }
            }
            Console.WriteLine(" Done\n");

            // Filter out incomplete technologies (only basic fields populated)
            var completeClassifications = classifications.Where(c => HasMeaningfulData(c)).ToList();
            var filteredCount = classifications.Count - completeClassifications.Count;

            if (completeClassifications.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  No technologies with sufficient data found.");
                Console.ResetColor();
                return null;
            }

            var outputPath = Path.Combine(
                outputFolder,
                $"{Path.GetFileNameWithoutExtension(pdfFile)}_classification.csv");

            try
            {
                TechnologyClassificationCsv.WriteCsv(outputPath, completeClassifications);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Failed to write CSV file: {ex.Message}");
                Console.ResetColor();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Classification complete!");
            Console.WriteLine($"   📁 Saved to: {outputPath}");
            Console.WriteLine($"   ✓ {completeClassifications.Count} technologies exported");
            if (filteredCount > 0)
                Console.WriteLine($"   ⊘ {filteredCount} incomplete records filtered out");
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

    private static string BuildClassificationPrompt(List<string> chunks)
    {
        var sb = new StringBuilder();
        
        // === GOAL & OVERVIEW ===
        sb.AppendLine("You are extracting ALL individual technologies and sub-processes from a technical PDF.");
        sb.AppendLine("GOAL: Extract EVERY distinct technology - return a JSON array with complete details.");
        sb.AppendLine();

        // === JSON EXAMPLE (CRITICAL) ===
        sb.AppendLine("EXAMPLE OUTPUT (exact JSON structure required):");
        sb.AppendLine("[");
        sb.AppendLine("  {");
        sb.AppendLine("    \"Datapaper Tech ID\": \"H2O_AEC_ELY_2030\",");
        sb.AppendLine("    \"description\": \"Alkaline water electrolysis\",");
        sb.AppendLine("    \"summary\": \"Mature water electrolysis technology using alkaline cells at 60-80°C...\",");
        sb.AppendLine("    \"unit_operation\": \"Electrolysis\",");
        sb.AppendLine("    \"ProcessType\": \"Alkaline\",");
        sb.AppendLine("    \"main_sector\": \"Energy\",");
        sb.AppendLine("    \"main_category\": \"Hydrogen Production\",");
        sb.AppendLine("    \"category_spec\": \"Alkaline\",");
        sb.AppendLine("    \"tech_type\": \"Alkaline Water Electrolysis\",");
        sb.AppendLine("    \"carriers_in\": \"water, electricity\",");
        sb.AppendLine("    \"main_input\": \"water\",");
        sb.AppendLine("    \"ratios_in\": \"9, 0.5\",");
        sb.AppendLine("    \"units_in\": \"mol, MWh\",");
        sb.AppendLine("    \"carriers_out\": \"hydrogen, oxygen\",");
        sb.AppendLine("    \"main_out\": \"hydrogen\",");
        sb.AppendLine("    \"ratios_out\": \"2, 1\",");
        sb.AppendLine("    \"units_out\": \"mol, mol\",");
        sb.AppendLine("    \"trl_(1-9)\": 8,");
        sb.AppendLine("    \"tech_maturity\": \"Mature\",");
        sb.AppendLine("    \"cost_base_year\": 2030,");
        sb.AppendLine("    \"Currency\": \"EUR\",");
        sb.AppendLine("    \"capex_power_capacity_eur_per_kw\": 1200,");
        sb.AppendLine("    \"opex_fix_pct_of_capex\": 0.03,");
        sb.AppendLine("    \"Data Reference Year\": 2024");
        sb.AppendLine("  }");
        sb.AppendLine("]");
        sb.AppendLine();

        // === WHAT IS A TECHNOLOGY? ===
        sb.AppendLine("WHAT IS A 'TECHNOLOGY'?");
        sb.AppendLine("- A single unit operation or process (e.g., 'Alkaline Electrolysis' OR 'PEM Electrolysis' - not both in one row)");
        sb.AppendLine("- NOT brand names: 'Siemens Electrolyzer' → extract as 'Alkaline Water Electrolysis'");
        sb.AppendLine("- NOT marketing terms: 'NextGen DAC v2.0' → extract as 'Adsorption-based Direct Air Capture'");
        sb.AppendLine("- Break integrated pathways: 'Electrolysis + Fischer-Tropsch' → 2 separate rows");
        sb.AppendLine("- DUPLICATES: Same name twice → extract once; Same name + different time horizons → extract twice with time suffix");
        sb.AppendLine();

        // === FIELD REQUIREMENTS ===
        sb.AppendLine("REQUIRED FIELDS (must populate):");
        sb.AppendLine("  description, unit_operation, main_sector, main_category, carriers_in, carriers_out");
        sb.AppendLine();
        sb.AppendLine("OPTIONAL FIELDS (if in paper):");
        sb.AppendLine("  ratios_in, ratios_out, capex_*, opex_*, cost_base_year, lifetime_yr");
        sb.AppendLine();
        sb.AppendLine("ESTIMATABLE FIELDS (OK to estimate if not in paper):");
        sb.AppendLine("  trl_(1-9), tech_maturity - use industry knowledge for well-known technologies");
        sb.AppendLine();
        sb.AppendLine("USE EMPTY STRING \"\" for optional fields not in paper - NEVER leave blank or use null");
        sb.AppendLine();

        // === CRITICAL RULES ===
        sb.AppendLine("CRITICAL RULES:");
        sb.AppendLine("1. COMPLETENESS: Extract EVERY technology from text, tables, figures, appendices");
        sb.AppendLine("2. CARRIERS & RATIOS: NEVER mix numbers with carrier names");
        sb.AppendLine("   - carriers_in: 'water, electricity' (names only)");
        sb.AppendLine("   - ratios_in: '9, 0.5' (numbers only)");
        sb.AppendLine("   - Both must have SAME count");
        sb.AppendLine("3. TIME HORIZONS: Different data for 2030 vs 2050 → create separate rows");
        sb.AppendLine("   - Add '_2030', '_2050' suffix to Datapaper Tech ID");
        sb.AppendLine("4. CURRENCY: Always use 'EUR' for cost fields; convert if needed");
        sb.AppendLine("5. TABLES: Extract ALL technologies from technology lists, even if minimal data");
        sb.AppendLine("6. NUMERIC DATA: Extract values only (not formulas)");
        sb.AppendLine();

        // === FIELD DEFINITIONS (CONDENSED) ===
        sb.AppendLine("FIELD GUIDANCE:");
        sb.AppendLine("- Datapaper Tech ID: 3-5 uppercase words separated by _ (e.g., H2O_AEC_ELY_2030)");
        sb.AppendLine("- description: Short name only (5-15 words)");
        sb.AppendLine("- summary: Comprehensive details from the paper (operating conditions, parameters, context)");
        sb.AppendLine("- unit_operation: The main unit of the process (AEC Electrolyzer, Geothermal CHP, H2-fired gas turbine, etc.)");
        sb.AppendLine("- ProcessType: The process type (Fuel synthesis, Power Generation, Storage, CO2 Capture, etc.)");
        sb.AppendLine("- main_sector: Broad category (Energy, Chemicals, CCU, Materials, Transport, Heat Supply)");
        sb.AppendLine("- main_category: Process category (Hydrogen Production, CO2 Capture, etc.)");
        sb.AppendLine("- category_spec: Specific type (Alkaline, PEM, SOEC, Fischer-Tropsch, etc.)");
        sb.AppendLine("- tech_type: Full descriptive name (e.g., 'Alkaline Water Electrolysis')");
        sb.AppendLine("- carriers_in: Comma-separated input materials (e.g., 'water, electricity')");
        sb.AppendLine("- ratios_in: Comma-separated numeric coefficients ONLY (e.g., '9, 0.5' - NO UNITS)");
        sb.AppendLine("- units_in: Units for each ratio (e.g., 'mol, MWh')");
        sb.AppendLine("- main_input: Most important input (must be in carriers_in)");
        sb.AppendLine("- carriers_out: Comma-separated products (e.g., 'hydrogen, oxygen')");
        sb.AppendLine("- ratios_out: Comma-separated numeric coefficients ONLY (e.g., '2, 1')");
        sb.AppendLine("- units_out: Units for each output ratio (e.g., 'mol, mol')");
        sb.AppendLine("- main_out: Most important product (must be in carriers_out)");
        sb.AppendLine("- trl_(1-9): Technology Readiness Level 1-9; estimate if needed (8-9=mature, 6-7=developing, 4-5=early)");
        sb.AppendLine("- tech_maturity: Text description (Early-stage, Developing, Near-commercial, Mature, Commercial)");
        sb.AppendLine("- overall_efficiency: Round-trip efficiency as decimal or percentage (0.85 or 85)");
        sb.AppendLine("- reference_unit_size_unit: Capacity unit (MW, MWh, kt/y, t/h, kW, kg/h, MJ/h)");
        sb.AppendLine("- cost_base_year: CRITICAL - Year costs apply to (2020, 2030, 2050, etc.)");
        sb.AppendLine("- Currency: Always 'EUR'");
        sb.AppendLine("- capex_one_time_eur: Fixed non-scalable capital cost per unit of capacity");
        sb.AppendLine("- capex_power_capacity_eur_per_kw: Scalable capital cost; Total CAPEX = fixed + (per_kw x capacity)");
        sb.AppendLine("- opex_one_time_eur: Initial one-time operating setup cost");
        sb.AppendLine("- opex_fix_pct_of_capex: Annual fixed cost as % of CAPEX (e.g., 0.03 for 3%)");
        sb.AppendLine("- opex_fix_power_capacity_eur_per_kw_yr: Annual fixed cost per kW capacity");
        sb.AppendLine("- lifetime_yr: Expected operational lifetime in years");
        sb.AppendLine("- Data Reference Year: Year the paper was published (2020, 2024, etc.)");
        sb.AppendLine();

        // === EDGE CASES ===
        sb.AppendLine("EDGE CASES:");
        sb.AppendLine("DUPLICATES: Same tech name appears twice");
        sb.AppendLine("  → Extract once if same data; Extract twice if different costs/TRL for 2030 vs 2050");
        sb.AppendLine("TIME HORIZONS: 'This technology projected for 2030 with cost X, 2050 with cost Y'");
        sb.AppendLine("  → Create TWO rows: one with cost_base_year=2030, one with cost_base_year=2050");
        sb.AppendLine("CONFLICTING DATA: Different sources give different TRL/costs");
        sb.AppendLine("  → Use most recent or credible; note discrepancy in summary");
        sb.AppendLine("COST RANGES: '€1000-1500/kW' → use midpoint (1250)");
        sb.AppendLine("INCOMPLETE DATA: Missing some fields → OK; only use empty string for missing optional fields");
        sb.AppendLine();

        // === SPECIAL TABLE MAPPING ===
        sb.AppendLine("SPECIAL TABLE MAPPING (LT DAC):");
        sb.AppendLine("For low-temperature solid sorbent direct air capture (LT DAC) tables, ALWAYS extract numeric values and map as follows:");
        sb.AppendLine("- carriers_in: 'electricity, low-temperature heat, sorbent'");
        sb.AppendLine("- ratios_in: electricity demand, heat demand, sorbent consumption (numbers only, in table order)");
        sb.AppendLine("- units_in: 'MWh/tCO2, GJ/tCO2, g/kgCO2'");
        sb.AppendLine("- capex_one_time_eur: use the table CAPEX value (if given as €/tCO2/yr, keep numeric and note unit in summary)");
        sb.AppendLine("- opex_one_time_eur: use the table sorbent cost (€/tCO2) numeric value and note unit in summary");
        sb.AppendLine("If the table lists emissions (e.g., kgCO2e/kgCO2), include the numeric value in summary.");
        sb.AppendLine();

        // === PDF CONTENT ===
        if (chunks.Count == 1)
        {
            sb.AppendLine("PDF Content:");
            sb.AppendLine(chunks[0]);
        }
        else
        {
            sb.AppendLine($"PDF Content split into {chunks.Count} parts:");
            for (int i = 0; i < chunks.Count; i++)
            {
                sb.AppendLine($"Part {i + 1}:");
                sb.AppendLine(chunks[i]);
                sb.AppendLine();
            }
        }

        sb.AppendLine();
        sb.AppendLine("Return ONLY valid JSON array. No commentary.");

        return sb.ToString();
    }

    private static string ExtractJson(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return string.Empty;

        // Try markdown fence first
        var fenceMatch = Regex.Match(response, "```(?:json)?\\s*(?<json>[\\s\\S]*?)```", RegexOptions.IgnoreCase);
        if (fenceMatch.Success)
            return fenceMatch.Groups["json"].Value.Trim();

        // Try direct JSON bracket match
        var start = response.IndexOf('[');
        var end = response.LastIndexOf(']');
        if (start >= 0 && end > start)
        {
            var extracted = response.Substring(start, end - start + 1).Trim();
            // Validate that we have balanced brackets
            if (extracted.StartsWith("[") && extracted.EndsWith("]"))
                return extracted;
        }

        return response.Trim();
    }

    private static List<Dictionary<string, string>> ParseRowsFromJson(string json)
    {
        var rows = new List<Dictionary<string, string>>();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            return rows;

        foreach (var element in doc.RootElement.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.Object)
                continue;

            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in element.EnumerateObject())
            {
                var value = ConvertJsonValueToString(prop.Value);
                if (value != null)
                    row[prop.Name] = value;
            }

            rows.Add(row);
        }

        return rows;
    }

    private static string? ConvertJsonValueToString(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                return element.GetRawText();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean() ? "true" : "false";
            case JsonValueKind.Array:
                var items = new List<string>();
                foreach (var item in element.EnumerateArray())
                {
                    var value = ConvertJsonValueToString(item);
                    if (!string.IsNullOrWhiteSpace(value))
                        items.Add(value);
                }
                return string.Join(", ", items);
            case JsonValueKind.Object:
                return element.ToString();
            default:
                return element.ToString();
        }
    }

    private static string GenerateTechId(TechnologyClassification tech, HashSet<string> usedIds)
    {
        var parts = new List<string>();

        // Extract from main carriers
        if (!string.IsNullOrWhiteSpace(tech.MainInput))
            parts.Add(ExtractAbbreviation(tech.MainInput));
        else if (tech.CarriersIn.Count > 0)
            parts.Add(ExtractAbbreviation(tech.CarriersIn[0]));

        // Extract from unit operation
        if (!string.IsNullOrWhiteSpace(tech.UnitOperation))
        {
            var abbrev = ExtractAbbreviation(tech.UnitOperation);
            if (!parts.Contains(abbrev))
                parts.Add(abbrev);
        }

        // Extract key words from process type
        if (!string.IsNullOrWhiteSpace(tech.ProcessType))
        {
            var abbrev = ExtractAbbreviation(tech.ProcessType);
            if (!parts.Contains(abbrev))
                parts.Add(abbrev);
        }

        // Extract from main output if not already included
        if (!string.IsNullOrWhiteSpace(tech.MainOut) && parts.Count < 4)
        {
            var abbrev = ExtractAbbreviation(tech.MainOut);
            if (!parts.Contains(abbrev))
                parts.Add(abbrev);
        }

        var baseId = string.Join("_", parts).ToUpperInvariant();
        if (baseId.Length > 20)
            baseId = baseId[..Math.Min(20, baseId.Length)];

        // Ensure uniqueness
        if (!usedIds.Contains(baseId))
            return baseId;

        var counter = 2;
        while (usedIds.Contains($"{baseId}_{counter}"))
            counter++;
        return $"{baseId}_{counter}";
    }

    private static string ExtractAbbreviation(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "UNK";

        text = text.Trim();

        // Known mappings (technology specific)
        var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Electrolysis", "ELY" },
            { "Alkaline", "ALK" },
            { "PEM", "PEM" },
            { "SOEC", "SOEC" },
            { "Fischer-Tropsch", "FT" },
            { "FT", "FT" },
            { "Methanation", "MET" },
            { "Methane", "CH4" },
            { "Ammonia", "NH3" },
            { "Haber-Bosch", "HB" },
            { "Direct Air Capture", "DAC" },
            { "DAC", "DAC" },
            { "Water-Gas Shift", "WGS" },
            { "RWGS", "RWGS" },
            { "CO2 reduction", "CO2R" },
            { "Electrochemical", "ECH" },
            { "Synthesis", "SYN" },
            { "Conversion", "CNV" },
            { "CO2", "CO2" },
            { "H2", "H2" },
            { "Hydrogen", "H2" },
            { "Oxygen", "O2" },
            { "Carbon monoxide", "CO" },
            { "Urea", "UREA" },
            { "Dimethyl ether", "DME" },
            { "Methanol", "MEOH" }
        };

        // Check exact matches first
        foreach (var kvp in mappings)
        {
            if (text.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        // Check if text contains known abbreviations
        foreach (var kvp in mappings)
        {
            if (text.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        // Generate abbreviation from first letters of words
        var words = Regex.Split(text, @"\W+").Where(w => !string.IsNullOrEmpty(w)).ToList();
        if (words.Count > 0)
        {
            if (words.Count == 1)
                return words[0].Length > 3 ? words[0].Substring(0, 3).ToUpperInvariant() : words[0].ToUpperInvariant();
            else
                return string.Concat(words.Select(w => w[0])).ToUpperInvariant();
        }

        return "UNK";
    }

    private static bool HasMeaningfulData(TechnologyClassification tech)
    {
        // Require at least 2-3 meaningful fields to avoid keeping nearly-empty rows
        int fieldCount = 0;
        
        if (!string.IsNullOrWhiteSpace(tech.Description)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.UnitOperation)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.Summary)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.ProcessType)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainSector)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainCategory)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.CategorySpec)) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.TechType)) fieldCount++;
        if (tech.CostBaseYear.HasValue) fieldCount++;
        if (tech.DataReferenceYear.HasValue) fieldCount++;
        if (tech.Trl.HasValue) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.TechMaturity)) fieldCount++;
        if (tech.OverallEfficiency.HasValue) fieldCount++;
        if (tech.CarriersIn.Count > 0 || tech.CarriersOut.Count > 0) fieldCount++;
        if (!string.IsNullOrWhiteSpace(tech.MainInput) || !string.IsNullOrWhiteSpace(tech.MainOut)) fieldCount++;
        if (tech.InputShares.Count > 0 || tech.RatiosIn.Count > 0) fieldCount++;
        if (tech.LifetimeYears.HasValue || tech.CapexOneTimeEur.HasValue || tech.OpexOneTimeEur.HasValue) fieldCount++;

        // Require minimum 2 fields populated
        return fieldCount >= 2;
    }
}
