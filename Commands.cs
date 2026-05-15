using System.Text;
using System.Text.RegularExpressions;
using GitHub.Copilot.SDK;
using Refractored.GitHub.Copilot.SDK.Helpers;

namespace PdfAnalysisApp;

public static class Commands
{
    /// <summary>
    /// Displays all available PDFs in the folder and optionally lets the user pick one to analyze.
    /// Returns the selected file path, or <c>null</c> if no valid selection was made.
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

        // Print each PDF with a 1-based index and file size
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📁 Available PDFs:");
        Console.ResetColor();

        for (int i = 0; i < pdfs.Length; i++)
        {
            var sizeKb = new FileInfo(pdfs[i]).Length / 1024;
            Console.WriteLine($"   {i + 1}. {Path.GetFileName(pdfs[i])} ({sizeKb} KB)");
        }

        Console.WriteLine();
        Console.Write("Enter number or filename to analyze, or press Enter to cancel: ");
        var choice = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(choice))
            return null;

        // Delegate to HandleAnalyzeCommand which resolves index or name
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
            Console.WriteLine("❌ No PDF loaded. Use 'upload' or 'list' to select one.");
    }

    /// <summary>
    /// Prints the full list of available commands and usage tips to the console.
    /// </summary>
    public static void HandleCommandsCommand()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("⚡ Available Commands:");
        Console.ResetColor();
        Console.WriteLine("  'commands' or 'help'   - Display all available commands");
        Console.WriteLine("  'exit' or 'quit'       - Exit the program");
        Console.WriteLine("  'upload <path>'        - Upload a PDF to analyze (or drop PDFs in ./pdf_to_analyze/)");
        Console.WriteLine("  'list'                 - List available PDFs and choose one to analyze");
        Console.WriteLine("  'current'              - Show current PDF");
        Console.WriteLine("  'auto-summarize'       - Extract technology summaries to TXT");
        Console.WriteLine("  'auto-classify' (beta) - Classify technologies and export CSV");
        Console.WriteLine("  'batch-analyze <q>'    - Analyze all PDFs with a question");
        Console.WriteLine("  'benchmark'            - Compare all models on the Allgoewer paper");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("💡 Tips:");
        Console.ResetColor();
        Console.WriteLine("  • Or just ask a question normally for AI analysis");
        if (Directory.Exists("./pdf_to_analyze"))
            Console.WriteLine("  • Drop PDFs in the ./pdf_to_analyze/ folder for quick access");
        Console.WriteLine();
    }

    /// <summary>
    /// Copies a PDF file into the <paramref name="pdfFolder"/> for analysis.
    /// Returns the destination path on success, or <c>null</c> on failure.
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
            var destFile = Path.Combine(pdfFolder, Path.GetFileName(sourceFile));

            await Program.RunWithSpinnerAsync($" Uploading {Path.GetFileName(sourceFile)}", async () =>
            {
                await Task.Run(() => File.Copy(sourceFile, destFile, true));
            });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded: {Path.GetFileName(sourceFile)}");
            Console.ResetColor();
            return destFile;
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
    /// Resolves a PDF file from user input (1-based index, full filename, or name without extension)
    /// and returns its full path. Returns <c>null</c> if no match is found.
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

        // 1) Try numeric index (1-based position from the 'list' command)
        if (int.TryParse(input, out int listNumber) && listNumber >= 1 && listNumber <= pdfFiles.Length)
        {
            selectedFile = pdfFiles[listNumber - 1];
        }
        else
        {
            // 2) Try exact filename match (with .pdf)
            selectedFile = pdfFiles.FirstOrDefault(f =>
                Path.GetFileName(f).Equals(input, StringComparison.OrdinalIgnoreCase));

            // 3) Try match without the .pdf extension
            selectedFile ??= pdfFiles.FirstOrDefault(f =>
                Path.GetFileNameWithoutExtension(f).Equals(input, StringComparison.OrdinalIgnoreCase));
        }

        if (selectedFile != null && File.Exists(selectedFile))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Loaded: {Path.GetFileName(selectedFile)}");
            Console.ResetColor();
            return selectedFile;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ PDF '{input}' not found. Use 'list' to see available PDFs.");
        Console.ResetColor();
        return null;
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
    /// Extracts text from every PDF in <paramref name="pdfFolder"/>, builds a combined prompt
    /// with the user's <paramref name="question"/>, and sends it to Copilot for analysis.
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
            // Extract and chunk each PDF in sequence
            var pdfChunks = new Dictionary<string, List<string>>();
            foreach (var pdfFile in pdfFiles)
            {
                var pdfText = await PdfAnalyzer.ExtractTextFromPdfAsync(pdfFile);
                pdfChunks[pdfFile] = PdfAnalyzer.ChunkPdfContent(pdfText);
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

    /// <summary>
    /// Runs the <c>auto-summarize</c> pipeline on a single PDF:
    ///   Stage 1 – Discover technology names.
    ///   Stage 2 – Extract detailed free-text summaries per technology (batched).
    /// Saves the combined extraction to a TXT file in <paramref name="outputFolder"/>.
    /// Returns the TXT path on success, or <c>null</c> on failure.
    /// </summary>
    public static async Task<string?> HandleAutoSummarizeAsync(
        CopilotClient client, string model, string pdfFile, string outputFolder)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ PDF not found or invalid path.");
            Console.ResetColor();
            return null;
        }

        try { Directory.CreateDirectory(outputFolder); }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Cannot create output folder: {ex.Message}");
            Console.ResetColor();
            return null;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📝 Summarising technologies from PDF...\n");
        Console.ResetColor();

        var session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = model,
            Streaming = true,
            OnPermissionRequest = PermissionHandler.ApproveAll
        });

        try
        {
            var pdfText = await PdfAnalyzer.ExtractTextFromPdfAsync(pdfFile);
            var chunks = PdfAnalyzer.ChunkPdfContent(pdfText);

            // ── Stage 1: Find all technology names ──
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   1. Finding technologies...");
            Console.ResetColor();

            var findNamesPrompt = TechnologyClassifier.BuildFindTechnologiesPrompt(chunks);
            var namesResponse = await RunWithSpinner("   Scanning PDF",
                async () => await Program.SendMessageAndCollectResponseSilentAsync(session, findNamesPrompt));

            var technologyNames = TechnologyClassifier.ParseTechnologyNames(namesResponse);
            if (technologyNames.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  No technologies found in PDF.");
                Console.ResetColor();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   Found {technologyNames.Count} technologies:");
            Console.ResetColor();
            foreach (var name in technologyNames)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"     • {name}");
                Console.ResetColor();
            }
            Console.WriteLine();

            // ── Stage 2: Extract detailed data per technology (batched) ──
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   2. Generating detailed summaries...\n");
            Console.ResetColor();

            var technologyDetails = new List<string>();
            const int batchSize = 5;
            int totalBatches = (int)Math.Ceiling((double)technologyNames.Count / batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                if (batchIndex > 0)
                    await Task.Delay(3000); // avoid Copilot rate limits

                var batchStart = batchIndex * batchSize;
                var batchCount = Math.Min(batchSize, technologyNames.Count - batchStart);
                var batchTechs = technologyNames.Skip(batchStart).Take(batchCount).ToList();

                Console.Write($"   Batch {batchIndex + 1}/{totalBatches} (technologies {batchStart + 1}-{batchStart + batchCount})... ");

                try
                {
                    var prompt = TechnologyClassifier.BuildBatchDetailedExtractionPrompt(chunks, batchTechs);
                    var response = await Program.SendMessageAndCollectResponseSilentAsync(session, prompt);

                    if (string.IsNullOrWhiteSpace(response))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ Empty response");
                        Console.ResetColor();
                        for (int i = 0; i < batchCount; i++)
                            technologyDetails.Add($"No data found for {batchTechs[i]}");
                    }
                    else
                    {
                        var parsed = TechnologyClassifier.ParseBatchedExtractionResponse(response, batchCount);
                        // Pad or trim to match expected count
                        while (parsed.Count < batchCount)
                            parsed.Add($"Incomplete data for {batchTechs[parsed.Count]}");
                        if (parsed.Count > batchCount)
                            parsed = parsed.Take(batchCount).ToList();
                        technologyDetails.AddRange(parsed);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("✓");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ {ex.Message}");
                    Console.ResetColor();
                    for (int i = 0; i < batchCount; i++)
                        technologyDetails.Add($"Extraction failed for {batchTechs[i]}: {ex.Message}");
                }
            }
            Console.WriteLine();

            // Pad if needed
            while (technologyDetails.Count < technologyNames.Count)
                technologyDetails.Add($"ERROR: Missing data for {technologyNames[technologyDetails.Count]}");

            // ── Save to TXT ──
            var txtPath = Path.Combine(outputFolder,
                $"{Path.GetFileNameWithoutExtension(pdfFile)}.txt");

            var txtContent = new StringBuilder();
            txtContent.AppendLine("═══════════════════════════════════════════════════════════════");
            txtContent.AppendLine($"Technology Extraction Data - {Path.GetFileName(pdfFile)}");
            txtContent.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            txtContent.AppendLine($"Total Technologies: {technologyNames.Count}");
            txtContent.AppendLine("═══════════════════════════════════════════════════════════════");
            txtContent.AppendLine();

            for (int i = 0; i < technologyNames.Count; i++)
            {
                txtContent.AppendLine($"═══ TECHNOLOGY {i + 1}: {technologyNames[i]} ═══");
                txtContent.AppendLine();
                txtContent.AppendLine(technologyDetails[i]);
                txtContent.AppendLine();
                txtContent.AppendLine("───────────────────────────────────────────────────────────────");
                txtContent.AppendLine();
            }

            File.WriteAllText(txtPath, txtContent.ToString(), Encoding.UTF8);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"   📁 Saved to: {txtPath}");
            Console.WriteLine($"   ✓ {technologyNames.Count} technologies extracted and summarised");
            Console.WriteLine();
            Console.WriteLine($"✅ Summarisation complete!");
            Console.ResetColor();

            return txtPath;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Auto-summarize failed: {ex.Message}");
            Console.ResetColor();
            return null;
        }
        finally
        {
            await session.DisposeAsync();
        }
    }

    /// <summary>
    /// Runs the <c>auto-classify</c> pipeline using the TXT summary produced
    /// by <c>auto-summarize</c>. Reads the TXT, splits it into technology
    /// sections, and converts each batch to structured JSON → CSV.
    /// If the TXT does not exist, prompts the user to run <c>auto-summarize</c> first.
    /// Returns the output CSV path on success, or <c>null</c> on failure.
    /// </summary>
    public static async Task<string?> HandleAutoClassifyAsync(CopilotClient client, string model, string pdfFile, string outputFolder)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ PDF not found or invalid path.");
            Console.ResetColor();
            return null;
        }

        // Derive expected TXT path from the PDF name
        var txtPath = Path.Combine(outputFolder,
            $"{Path.GetFileNameWithoutExtension(pdfFile)}.txt");

        if (!File.Exists(txtPath))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"❌ Please run 'auto-summarize' first to extract technology summaries.");
            Console.WriteLine($"    No summary file found at: {txtPath}");
            Console.ResetColor();
            return null;
        }

        try { Directory.CreateDirectory(outputFolder); }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Cannot create output folder: {ex.Message}");
            Console.ResetColor();
            return null;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("📋 Classifying from TXT summary and writing to CSV...\n");
        Console.ResetColor();

        try
        {
            // Read and parse TXT into technology sections
            var txtContent = File.ReadAllText(txtPath, Encoding.UTF8);
            var sections = ParseTxtIntoSections(txtContent);

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

            // === Convert TXT sections to structured JSON in batches ===
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   2. Converting summaries to structured data...\n");
            Console.ResetColor();

            var allRows = new List<IDictionary<string, string>>();
            const int batchSize = 5;
            int totalBatches = (int)Math.Ceiling((double)sections.Count / batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                if (batchIndex > 0)
                    await Task.Delay(3000); // avoid Copilot rate limits

                var batchStart = batchIndex * batchSize;
                var batchCount = Math.Min(batchSize, sections.Count - batchStart);
                var batchSections = sections.Skip(batchStart).Take(batchCount).ToList();

                Console.WriteLine($"   Batch {batchIndex + 1}/{totalBatches} (technologies {batchStart + 1}-{batchStart + batchCount})");

                // Fresh session per batch: keeps context constant size regardless of batch number
                await using var batchSession = await client.CreateSessionAsync(new SessionConfig
                {
                    Model = model,
                    Streaming = true,
                    OnPermissionRequest = PermissionHandler.ApproveAll
                });

                try
                {
                    // Always send the full schema prompt — no accumulated context across batches
                    var prompt = TechnologyClassifier.BuildClassificationFromSummaryPrompt(batchSections);

                    var jsonResponse = await Program.SendMessageAndStreamToConsoleAsync(batchSession, prompt);
                    var json = TechnologyClassifier.ExtractJson(jsonResponse);

                    if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith("[") || !json.TrimEnd().EndsWith("]"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"   ⚠️ No valid JSON returned — retrying");
                        Console.ResetColor();

                        // Retry once (silent — we already saw the first attempt)
                        jsonResponse = await Program.SendMessageAndCollectResponseSilentAsync(batchSession, prompt);
                        json = TechnologyClassifier.ExtractJson(jsonResponse);
                        if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith("[") || !json.TrimEnd().EndsWith("]"))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"   ✗ Retry also failed — skipping batch");
                            Console.ResetColor();
                            continue;
                        }
                    }

                    var batchRows = TechnologyClassifier.ParseRowsFromJson(json);
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

            var rows = allRows;

            // Parse each JSON row into a strongly-typed classification record
            var classifications = new List<TechnologyClassification>();
            var rowErrors = new List<string>();
            var usedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Console.Write("   🔍 Parsing and validating...");
            for (int i = 0; i < rows.Count; i++)
            {
                if (TechnologyClassifier.TryClassify(rows[i], out var classification, out var errors))
                {
                    // Ensure Datapaper Tech ID is generated
                    if (string.IsNullOrWhiteSpace(classification.DatapaperTechId))
                    {
                        classification.DatapaperTechId = TechnologyClassifier.GenerateTechId(classification, usedIds);
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

            // Drop records that lack enough populated fields to be useful
            var completeClassifications = classifications
                .Where(TechnologyClassifier.HasMeaningfulData)
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
                outputFolder,
                $"{Path.GetFileNameWithoutExtension(pdfFile)}_classification.csv");

            // Merge rows that share the same technology + year within the current run.
            // Different years for the same technology remain as separate rows.
            var mergedClassifications = TechnologyClassifier.MergeByTechnologyAndYear(completeClassifications);

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

            var mergedCount = completeClassifications.Count - mergedClassifications.Count;

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

    /// <summary>
    /// Parses a TXT file (produced by <c>auto-summarize</c>) into named
    /// technology sections. Expects headers like
    /// <c>═══ TECHNOLOGY N: [Name] ═══</c>.
    /// </summary>
    private static List<(string Name, string Content)> ParseTxtIntoSections(string txtContent)
    {
        var sections = new List<(string Name, string Content)>();
        if (string.IsNullOrWhiteSpace(txtContent))
            return sections;

        // Match section headers produced by HandleAutoSummarizeAsync
        var headerPattern = new Regex(
            @"═{3,}\s*TECHNOLOGY\s+\d+:\s*(?<name>.+?)\s*═{3,}",
            RegexOptions.IgnoreCase);

        var matches = headerPattern.Matches(txtContent);
        for (int i = 0; i < matches.Count; i++)
        {
            string name = matches[i].Groups["name"].Value.Trim();
            var contentStart = matches[i].Index + matches[i].Length;
            var contentEnd = (i + 1 < matches.Count)
                ? matches[i + 1].Index
                : txtContent.Length;

            var content = txtContent.Substring(contentStart, contentEnd - contentStart).Trim();

            // Skip the separator line (───…) at the end of each section
            var separatorIdx = content.LastIndexOf("───");
            if (separatorIdx >= 0)
                content = content.Substring(0, separatorIdx).Trim();

            if (!string.IsNullOrWhiteSpace(content))
                sections.Add((name, content));
        }

        return sections;
    }

    /// Benchmark constants

    private const string BenchmarkPdfName = "Allgoewer_2024.pdf";

    private const string BenchmarkPrompt =
        "Based on this paper, list the main low-carbon hydrogen production technologies discussed. " +
        "For each technology provide: (1) Technology Readiness Level (TRL), " +
        "(2) production cost range in USD/kg H2, and (3) key efficiency metric. " +
        "Be concise and use a structured format.";

    /// <summary>
    /// Runs the same benchmark prompt against every available model using the Allgoewer paper,
    /// prints a comparison table, and saves results to a CSV in <paramref name="outputFolder"/>.
    /// </summary>
    public static async Task HandleBenchmarkAsync(CopilotClient client, string pdfFolder, string outputFolder)
    {
        // Resolve the Allgoewer PDF
        var pdfPath = Path.Combine(pdfFolder, BenchmarkPdfName);
        if (!File.Exists(pdfPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Benchmark PDF not found: {BenchmarkPdfName}");
            Console.ResetColor();
            return;
        }

        // Build the prompt with PDF context once (same for all models)
        string fullPrompt = string.Empty;
        await Program.RunWithSpinnerAsync(" Extracting PDF text", async () =>
        {
            fullPrompt = await PrepareMessageWithPdfContextAsync(pdfPath, BenchmarkPrompt);
        });

        // Get available models
        var modelsWithInfo = await client.ListModelsAsync();
        if (modelsWithInfo == null || modelsWithInfo.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ No models available.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"🏁 Benchmarking {modelsWithInfo.Count} models on: {BenchmarkPdfName}");
        Console.ResetColor();
        Console.WriteLine();

        var results = new List<(string Model, double Multiplier, long LatencyMs, int WordCount, string Response, string Status)>();

        foreach (var modelInfo in modelsWithInfo)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"   ▶ {modelInfo.Id,-35}");
            Console.ResetColor();

            CopilotSession? benchSession = null;
            try
            {
                benchSession = await client.CreateSessionAsync(new SessionConfig
                {
                    Model = modelInfo.Id,
                    Streaming = true
                });

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var response = await Program.SendMessageAndCollectResponseSilentAsync(benchSession, fullPrompt);
                sw.Stop();

                var words = response.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                var multiplier = modelInfo.Billing?.Multiplier ?? 1.0;
                results.Add((modelInfo.Id, multiplier, sw.ElapsedMilliseconds, (int)words, response, "OK"));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {sw.ElapsedMilliseconds,6} ms  {words,5} words");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                results.Add((modelInfo.Id, modelInfo.Billing?.Multiplier ?? 1.0, 0, 0, string.Empty, $"ERROR: {ex.Message}"));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ❌ {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                if (benchSession != null)
                    await benchSession.DisposeAsync();
            }
        }

        // ── Summary table ──
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("📊 Benchmark Results:");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"   {"Model",-35} {"Multiplier",10}  {"Latency (ms)",13}  {"Words",6}  {"Est.Cost*",10}  Status");
        Console.WriteLine($"   {"─────────────────────────────────",35} {"──────────",10}  {"─────────────",13}  {"─────",6}  {"──────────",10}  ──────");
        Console.ResetColor();

        foreach (var r in results)
        {
            var estCost = r.Status == "OK" ? (r.Multiplier * r.WordCount / 1000.0).ToString("F3") : "N/A";
            Console.WriteLine($"   {r.Model,-35} {r.Multiplier,10:F2}  {r.LatencyMs,13}  {r.WordCount,6}  {estCost,10}  {r.Status}");
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n   * Est.Cost = multiplier × words/1000 (relative proxy, not real billing)");
        Console.ResetColor();

        // ── Save CSV ──
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var csvPath = Path.Combine(outputFolder, $"benchmark_{timestamp}.csv");
        var csv = new StringBuilder();
        csv.AppendLine("Model,Multiplier,LatencyMs,WordCount,EstCostProxy,Status");
        foreach (var r in results)
        {
            var estCost = r.Status == "OK" ? (r.Multiplier * r.WordCount / 1000.0).ToString("F3") : "N/A";
            csv.AppendLine($"{r.Model},{r.Multiplier:F2},{r.LatencyMs},{r.WordCount},{estCost},{r.Status}");
        }
        await File.WriteAllTextAsync(csvPath, csv.ToString());

        // ── Save TXT with full responses ──
        var txtPath = Path.Combine(outputFolder, $"benchmark_{timestamp}.txt");
        var txt = new StringBuilder();
        txt.AppendLine("═══════════════════════════════════════════════════════════════");
        txt.AppendLine($"Benchmark Responses - {BenchmarkPdfName}");
        txt.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        txt.AppendLine($"Prompt: {BenchmarkPrompt}");
        txt.AppendLine("═══════════════════════════════════════════════════════════════");
        txt.AppendLine();
        foreach (var r in results)
        {
            txt.AppendLine($"═══ MODEL: {r.Model} ═══");
            txt.AppendLine($"Status: {r.Status}  |  Latency: {r.LatencyMs} ms  |  Words: {r.WordCount}");
            txt.AppendLine();
            txt.AppendLine(r.Status == "OK" ? r.Response : $"(no response — {r.Status})");
            txt.AppendLine();
            txt.AppendLine("───────────────────────────────────────────────────────────────");
            txt.AppendLine();
        }
        await File.WriteAllTextAsync(txtPath, txt.ToString(), Encoding.UTF8);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n💾 Results saved → {csvPath}");
        Console.WriteLine($"💾 Responses saved → {txtPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static async Task<T> RunWithSpinner<T>(string message, Func<Task<T>> action)
    {
        Console.Write($"{message}... ");
        var spinnerChars = new[] { '|', '/', '-', '\\' };
        using var cts = new CancellationTokenSource();
        var spinnerLeft = Console.CursorLeft;
        var spinnerTop = Console.CursorTop;

        var spinnerTask = Task.Run(async () =>
        {
            var idx = 0;
            while (!cts.Token.IsCancellationRequested)
            {
                Console.SetCursorPosition(spinnerLeft, spinnerTop);
                Console.Write(spinnerChars[idx++ % spinnerChars.Length]);
                try
                {
                    await Task.Delay(100, cts.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }, cts.Token);

        try
        {
            var result = await action();
            cts.Cancel();
            await spinnerTask;

            Console.SetCursorPosition(spinnerLeft, spinnerTop);
            Console.Write("✓");
            Console.WriteLine();
            return result;
        }
        catch
        {
            cts.Cancel();
            await spinnerTask;

            Console.SetCursorPosition(spinnerLeft, spinnerTop);
            Console.Write("✗");
            Console.WriteLine();
            throw;
        }
    }
}

