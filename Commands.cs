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
        if (!File.Exists(pdfFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ PDF not found.");
            Console.ResetColor();
            return null;
        }

        Directory.CreateDirectory(outputFolder);

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
            var cts = new CancellationTokenSource();
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
                    classifications.Add(classification);
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

            TechnologyClassificationCsv.WriteCsv(outputPath, completeClassifications);

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
        var headerList = string.Join(", ", TechnologyClassificationCsv.HeaderOrder);
        var sb = new StringBuilder();
        sb.AppendLine("You are extracting ALL individual technologies and sub-processes from a PDF.");
        sb.AppendLine("YOUR GOAL: Extract EVERY technology mentioned - narrative text, tables, lists, figures, appendices.");
        sb.AppendLine("COMPLETENESS IS CRITICAL: If the paper has 115 technologies, you must extract all 115.");
        sb.AppendLine();
        sb.AppendLine("CRITICAL: Pay special attention to tables, formulas, and numerical data—these contain key technology parameters.");
        sb.AppendLine("CRITICAL: If a technology is mentioned for DIFFERENT TIME HORIZONS (e.g., 2030 vs 2050, near-term vs long-term),");
        sb.AppendLine("         create SEPARATE rows for each time variant:");
        sb.AppendLine("         - Append '_2030' suffix to Datapaper Tech ID for 2030 variants");
        sb.AppendLine("         - Append '_2050' suffix to Datapaper Tech ID for 2050 variants");
        sb.AppendLine("         - Append '_LongTerm' or '_NearTerm' if time periods are used instead of years");
        sb.AppendLine("         Example: If 'Alkaline Electrolysis' appears as both 2030 projection and 2050 projection, extract:");
        sb.AppendLine("           1. ALK_ELY_2030 (with 2030-specific data)");
        sb.AppendLine("           2. ALK_ELY_2050 (with 2050-specific data)");
        sb.AppendLine();
        sb.AppendLine("IMPORTANT: Break down composite/integrated processes into their constituent unit operations.");
        sb.AppendLine("Example: If the PDF mentions 'Electrolysis + Fischer-Tropsch for fuel synthesis', extract:");
        sb.AppendLine("  1. Electrolysis (AEC) as a separate row");
        sb.AppendLine("  2. Fischer-Tropsch (FT) synthesis as a separate row");
        sb.AppendLine("Do NOT create single rows for integrated pathways—extract each technology individually.");
        sb.AppendLine();
        sb.AppendLine("Return ONLY a JSON array where each item is a single technology/unit operation with keys:");
        sb.AppendLine(headerList);
        sb.AppendLine();
        sb.AppendLine("Rules:");
        sb.AppendLine("- Extract EVERY distinct technology, process, and unit operation from:");
        sb.AppendLine("  * Main narrative text");
        sb.AppendLine("  * ALL tables (especially technology lists like 'Table 11', 'Table of Technologies', etc.)");
        sb.AppendLine("  * Figure captions and diagrams");
        sb.AppendLine("  * Appendices and supplementary sections");
        sb.AppendLine("  * Network models and system diagrams");
        sb.AppendLine("- For integrated pathways, create separate rows for each component technology.");
        sb.AppendLine("- For 'description': Write a SHORT description of the technology:");
        sb.AppendLine("    * Keep it concise: 5-15 words maximum");
        sb.AppendLine("    * Examples: 'Alkaline water electrolysis', 'Low-temperature solid sorbent DAC', 'Fischer-Tropsch synthesis'");
        sb.AppendLine("    * Just the technology name/type, nothing more");
        sb.AppendLine("- For 'summary': Write a COMPREHENSIVE summary at the end:");
        sb.AppendLine("    * Include ALL relevant details from the paper about this technology");
        sb.AppendLine("    * Cover: purpose, operating conditions, key parameters, applications, context");
        sb.AppendLine("    * Include specifics like temperature ranges, pressure, catalysts, conversion rates if mentioned");
        sb.AppendLine("    * This is where you put the detailed information previously in description");
        sb.AppendLine("    * Can be multiple sentences and comprehensive");
        sb.AppendLine("- For 'Datapaper Tech ID': Generate a SHORT UNIQUE CODE based on:");
        sb.AppendLine("    * Main input/output carriers (e.g., CO2, H2, electricity)");
        sb.AppendLine("    * Key words from unit_operation and ProcessType");
        sb.AppendLine("    * Example format: CO2_AEC_ELY (CO2 input, Alkaline Electrochemical Cell, Electrolysis)");
        sb.AppendLine("    * Make it 3-5 uppercase parts separated by underscores");
        sb.AppendLine("    * IF this technology has multiple time variants, APPEND the time to the ID:");
        sb.AppendLine("      - CO2_AEC_ELY_2030 for 2030 variant with specific 2030 costs/performance");
        sb.AppendLine("      - CO2_AEC_ELY_2050 for 2050 variant with specific 2050 costs/performance");
        sb.AppendLine("      - CO2_AEC_ELY_NearTerm or CO2_AEC_ELY_LongTerm if years not specified");
        sb.AppendLine("- For 'main_sector': Classify into broader industry sectors:");
        sb.AppendLine("    * Examples: 'Energy', 'Chemicals', 'CCU', 'Materials', 'Transport', 'Heat Supply', 'Environmental'");
        sb.AppendLine("    * Base on the primary application domain of the technology");
        sb.AppendLine("- For 'main_category': Use specific technology categories:");
        sb.AppendLine("    * Examples: 'Hydrogen Production', 'CO2 Capture', 'Fuel Synthesis', 'Chemical Synthesis', ");
        sb.AppendLine("    *           'Electrolysis', 'Thermal Processing', 'Catalytic Conversion', 'Gas Separation'");
        sb.AppendLine("    * Be descriptive of the main process type");
        sb.AppendLine("- For 'category_spec': More specific subcategories:");
        sb.AppendLine("    * Examples: 'Alkaline', 'PEM', 'SOEC', 'Fischer-Tropsch', 'Direct Synthesis', 'Water-Gas Shift'");
        sb.AppendLine("    * Differentiate between similar technologies");
        sb.AppendLine("- For 'tech_type': The specific technology name/type:");
        sb.AppendLine("    * Examples: 'Alkaline Water Electrolysis', 'Solid Sorbent DAC', 'FT Synthesis via RWGS'");
        sb.AppendLine("    * Use full descriptive names from industry standards or the paper");
        sb.AppendLine("- For 'reference_unit_size_unit': The unit of the reference capacity:");
        sb.AppendLine("    * Examples: 'MW', 'MWh', 'kt/y', 't/h', 'kW', 'kg/h', 'MJ/h'");
        sb.AppendLine("    * If reference_unit_size is provided, this MUST be filled with the appropriate unit");
        sb.AppendLine("- For 'cost_base_year': CRITICAL - The specific year from the paper that the cost/economic data refers to:");
        sb.AppendLine("    * This is the year in which the technology costs were evaluated or estimated");
        sb.AppendLine("    * Examples: 2020, 2025, 2030, 2050 (any year mentioned in the paper)");
        sb.AppendLine("    * MUST match exactly with the year stated in the paper for that technology's data");
        sb.AppendLine("    * This is NOT about when the paper was written, but when the data applies");
        sb.AppendLine("- For 'Data Reference Year': The year of the reference document itself (the paper publication year):");
        sb.AppendLine("    * This is when the paper/document was published or released");
        sb.AppendLine("    * Examples: 2020, 2022, 2024 (the year of the document)");
        sb.AppendLine("    * Different from cost_base_year which refers to when specific tech data applies");
        sb.AppendLine("- For 'trl_(1-9)': Technology Readiness Level (1-9 scale):");
        sb.AppendLine("    * If explicitly mentioned in the paper, extract that value");
        sb.AppendLine("    * If NOT in the paper, use your knowledge to estimate based on the technology maturity:");
        sb.AppendLine("    * Examples: TRL 3-4 for early-stage techs, TRL 7-8 for mature/commercial technologies");
        sb.AppendLine("    * Alkaline electrolysis = mature = TRL 8-9");
        sb.AppendLine("    * Direct Air Capture variations = developing = TRL 6-7");
        sb.AppendLine("    * Novel synthesis routes = early-stage = TRL 4-5");
        sb.AppendLine("- For 'tech_maturity': Text description of technology maturity:");
        sb.AppendLine("    * If mentioned in the paper, extract that description");
        sb.AppendLine("    * If NOT in the paper, assign based on TRL and your knowledge:");
        sb.AppendLine("    * Examples: 'Early-stage', 'Developing', 'Near-commercial', 'Mature', 'Commercial'");
        sb.AppendLine("    * Must be consistent with the TRL value assigned");
        sb.AppendLine("- Use empty string when a value is truly unknown or not present in the paper.");
        sb.AppendLine("- Use comma-separated strings for list fields (e.g., carriers_in, output_shares).");
        sb.AppendLine("- Keep numeric fields as numbers (not words).");
        sb.AppendLine();
        sb.AppendLine("  CRITICAL - EXTRACT ALL TECHNOLOGIES FROM TABLES:");
        sb.AppendLine("  * The PDF contains [TABLE REGION] and [TECHNOLOGY LIST TABLE] markers");
        sb.AppendLine("  * [TABLE REGION - IMPORTANT NUMERICAL DATA] = extract data values (costs, efficiencies, capacities)");
        sb.AppendLine("  * [TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES] = EVERY technology name listed must become a row");
        sb.AppendLine("  * For technology list tables: Create one JSON row per technology name, even if data is minimal");
        sb.AppendLine("  * Do NOT skip technologies just because they lack numerical data in the table");
        sb.AppendLine("  * Example: If Table 11 lists 50 technologies, extract all 50 as separate rows");
        sb.AppendLine("  * Use table context (headers, captions) to fill in category/sector fields for listed technologies");
        sb.AppendLine("  * For tables with multiple time periods (2030, 2040, 2050): create separate rows for each technology-year combination");
        sb.AppendLine();
        sb.AppendLine("- For formulas or calculated values: extract the numerical result, not the formula text");
        sb.AppendLine("- Use your knowledge of these technologies to fill in sector/category/type even if not explicitly stated in the paper.");
        sb.AppendLine("- Do not add extra commentary, only the JSON array.");
        sb.AppendLine();
        sb.AppendLine();

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

        return sb.ToString();
    }

    private static string ExtractJson(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return string.Empty;

        var fenceMatch = Regex.Match(response, "```(?:json)?\\s*(?<json>[\\s\\S]*?)```", RegexOptions.IgnoreCase);
        if (fenceMatch.Success)
            return fenceMatch.Groups["json"].Value.Trim();

        var start = response.IndexOf('[');
        var end = response.LastIndexOf(']');
        if (start >= 0 && end > start)
            return response.Substring(start, end - start + 1).Trim();

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
            baseId = baseId.Substring(0, 20);

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
        // Keep rows with any meaningful content beyond an empty shell.
        if (!string.IsNullOrWhiteSpace(tech.Description))
            return true;
        if (!string.IsNullOrWhiteSpace(tech.UnitOperation))
            return true;
        if (!string.IsNullOrWhiteSpace(tech.Summary))
            return true;
        if (!string.IsNullOrWhiteSpace(tech.ProcessType))
            return true;
        if (!string.IsNullOrWhiteSpace(tech.MainSector))
            return true;
        if (!string.IsNullOrWhiteSpace(tech.MainCategory))
            return true;
        if (!string.IsNullOrWhiteSpace(tech.CategorySpec))
            return true;
        if (!string.IsNullOrWhiteSpace(tech.TechType))
            return true;
        if (tech.CostBaseYear.HasValue || tech.DataReferenceYear.HasValue || tech.Trl.HasValue)
            return true;
        if (!string.IsNullOrWhiteSpace(tech.TechMaturity))
            return true;
        if (tech.OverallEfficiency.HasValue)
            return true;
        if (tech.CarriersIn.Count > 0 || tech.CarriersOut.Count > 0)
            return true;
        if (!string.IsNullOrWhiteSpace(tech.MainInput) || !string.IsNullOrWhiteSpace(tech.MainOut))
            return true;
        if (tech.InputShares.Count > 0 || tech.RatiosIn.Count > 0 || tech.RatiosOut.Count > 0 || tech.OutputShares.Count > 0)
            return true;
        if (tech.LifetimeYears.HasValue || tech.CapexOneTimeEur.HasValue || tech.CapexPowerCapacityEurPerKw.HasValue || tech.OpexOneTimeEur.HasValue || tech.OpexFixPctOfCapex.HasValue)
            return true;

        return false;
    }
}
