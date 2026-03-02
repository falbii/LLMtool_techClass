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
        Console.WriteLine("📋 Classifying with Copilot and writing to CSV...\n");
        Console.ResetColor();

        try
        {
            var pdfText = await PdfAnalyzer.ExtractTextFromPdfAsync(pdfFile);
            var chunks = PdfAnalyzer.ChunkPdfContent(pdfText);

            // === STAGE 1: Find all technology names ===
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
            foreach (var techName in technologyNames)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"     • {techName}");
                Console.ResetColor();
            }
            Console.WriteLine();

            // === STAGE 2: Extract detailed data for each technology (BATCHED) ===
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   2. Extracting data for each technology...\n");
            Console.ResetColor();

            var technologyDetails = new List<string>();
            var failedExtractions = 0;
            const int batchSize = 15; // Process 15 technologies per API call
            int totalBatches = (int)Math.Ceiling((double)technologyNames.Count / batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                var batchStart = batchIndex * batchSize;
                var batchCount = Math.Min(batchSize, technologyNames.Count - batchStart);
                var batchTechs = technologyNames.Skip(batchStart).Take(batchCount).ToList();
                
                Console.Write($"   Batch {batchIndex + 1}/{totalBatches} (technologies {batchStart + 1}-{batchStart + batchCount})... ");
                
                try
                {
                    var detailPrompt = TechnologyClassifier.BuildBatchDetailedExtractionPrompt(chunks, batchTechs);
                    var detailResponse = await Program.SendMessageAndCollectResponseSilentAsync(session, detailPrompt);
                    
                    if (string.IsNullOrWhiteSpace(detailResponse))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ Empty response");
                        Console.ResetColor();
                        
                        // Add placeholders for failed batch
                        for (int i = 0; i < batchTechs.Count; i++)
                            technologyDetails.Add($"No data found for {batchTechs[i]}");
                        failedExtractions += batchTechs.Count;
                    }
                    else
                    {
                        // Parse the batched response
                        var batchDetails = TechnologyClassifier.ParseBatchedExtractionResponse(detailResponse, batchTechs.Count);
                        
                        // CRITICAL: Ensure we got exactly the expected count to avoid index mismatches
                        if (batchDetails.Count < batchTechs.Count)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"⚠️ Partial ({batchDetails.Count}/{batchTechs.Count})");
                            Console.ResetColor();
                            
                            // Pad with placeholders for missing technologies
                            for (int i = batchDetails.Count; i < batchTechs.Count; i++)
                                batchDetails.Add($"Incomplete data for {batchTechs[i]}");
                            failedExtractions += (batchTechs.Count - batchDetails.Count);
                        }
                        else if (batchDetails.Count > batchTechs.Count)
                        {
                            // Trim excess items
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"⚠️ Excess data ({batchDetails.Count}/{batchTechs.Count})");
                            Console.ResetColor();
                            batchDetails = batchDetails.Take(batchTechs.Count).ToList();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"✓");
                            Console.ResetColor();
                        }
                        
                        technologyDetails.AddRange(batchDetails);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ {ex.Message}");
                    Console.ResetColor();
                    
                    // Add placeholders for failed batch
                    for (int i = 0; i < batchTechs.Count; i++)
                        technologyDetails.Add($"Extraction failed for {batchTechs[i]}: {ex.Message}");
                    failedExtractions += batchTechs.Count;
                }
                
                // Verify counts after each batch for debugging
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"       (Total extractions: {technologyDetails.Count}/{batchStart + batchCount})");
                Console.ResetColor();
            }
            Console.WriteLine();
            
            if (failedExtractions > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️  {failedExtractions} extractions had issues - results may be incomplete");
                Console.ResetColor();
                Console.WriteLine();
            }

            // Verify data integrity before saving
            if (technologyNames.Count != technologyDetails.Count)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ CRITICAL ERROR: Data count mismatch!");
                Console.WriteLine($"   Expected: {technologyNames.Count} technologies");
                Console.WriteLine($"   Extracted: {technologyDetails.Count} details");
                Console.WriteLine($"   Missing: {technologyNames.Count - technologyDetails.Count}");
                Console.ResetColor();
                Console.WriteLine();
                
                // Pad with placeholders to continue processing
                while (technologyDetails.Count < technologyNames.Count)
                {
                    var missingIndex = technologyDetails.Count;
                    technologyDetails.Add($"ERROR: Missing data for {technologyNames[missingIndex]}");
                }
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️  Added {technologyNames.Count - technologyDetails.Count} placeholders to continue.");
                Console.ResetColor();
                Console.WriteLine();
            }

            // Save intermediate extraction data to TXT file
            try
            {
                var txtPath = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(pdfFile)}.txt");
                var txtContent = new StringBuilder();
                
                txtContent.AppendLine("═══════════════════════════════════════════════════════════════");
                txtContent.AppendLine($"Technology Extraction Data - {Path.GetFileName(pdfFile)}");
                txtContent.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                txtContent.AppendLine($"Total Technologies: {technologyNames.Count}");
                txtContent.AppendLine("═══════════════════════════════════════════════════════════════");
                txtContent.AppendLine();
                
                for (int i = 0; i < Math.Min(technologyNames.Count, technologyDetails.Count); i++)
                {
                    txtContent.AppendLine($"═══ TECHNOLOGY {i + 1}: {technologyNames[i]} ═══");
                    txtContent.AppendLine();
                    txtContent.AppendLine(technologyDetails[i]);
                    txtContent.AppendLine();
                    txtContent.AppendLine("───────────────────────────────────────────────────────────────");
                    txtContent.AppendLine();
                }
                
                File.WriteAllText(txtPath, txtContent.ToString(), Encoding.UTF8);
                
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"   💾 Saved extraction data: {Path.GetFileName(txtPath)}");
                Console.ResetColor();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   ⚠️  Could not save TXT file: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine();
            }

            // === STAGE 3: Convert to structured JSON (BATCHED) ===
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   3. Creating CSV file...");
            Console.ResetColor();

            // Final validation (should already be synchronized from Stage 2)
            if (technologyNames.Count != technologyDetails.Count)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ UNEXPECTED: Data still mismatched after padding!");
                Console.WriteLine($"   Names: {technologyNames.Count}, Details: {technologyDetails.Count}");
                Console.ResetColor();
                return null;
            }

            // Process in batches to avoid token limits (128k tokens)
            var allRows = new List<IDictionary<string, string>>();
            const int structureBatchSize = 15; // Convert 15 technologies per API call
            int totalStructureBatches = (int)Math.Ceiling((double)technologyNames.Count / structureBatchSize);

            for (int batchIndex = 0; batchIndex < totalStructureBatches; batchIndex++)
            {
                var batchStart = batchIndex * structureBatchSize;
                var batchCount = Math.Min(structureBatchSize, technologyNames.Count - batchStart);
                var batchNames = technologyNames.Skip(batchStart).Take(batchCount).ToList();
                var batchDetails = technologyDetails.Skip(batchStart).Take(batchCount).ToList();

                Console.Write($"   Converting batch {batchIndex + 1}/{totalStructureBatches} (techs {batchStart + 1}-{batchStart + batchCount})... ");

                try
                {
                    var structurePrompt = TechnologyClassifier.BuildStructuringPrompt(batchNames, batchDetails);
                    var jsonResponse = await Program.SendMessageAndCollectResponseSilentAsync(session, structurePrompt);

                    var json = TechnologyClassifier.ExtractJson(jsonResponse);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ No JSON returned");
                        Console.ResetColor();
                        continue;
                    }

                    var batchRows = TechnologyClassifier.ParseRowsFromJson(json);
                    allRows.AddRange(batchRows);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓");
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
                Console.WriteLine("⚠️  No technologies were successfully structured.");
                Console.ResetColor();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"   📋 Total structured unique: {allRows.Count} technologies");
            
            // Show data flow summary
            if (allRows.Count < technologyNames.Count)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   ⚠️  Data loss detected: {technologyNames.Count} from Stage 2 → {allRows.Count} in Stage 3 (lost {technologyNames.Count - allRows.Count})");
            }
            Console.ResetColor();
            Console.WriteLine();
            
            var rows = allRows;

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

            // Filter out incomplete technologies (only basic fields populated)
            var completeClassifications = classifications.Where(c => TechnologyClassifier.HasMeaningfulData(c)).ToList();
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
                Console.Write("   💾 Writing CSV file...");
                TechnologyClassificationCsv.WriteCsv(outputPath, completeClassifications);
                Console.WriteLine(" Done\n");
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
                Console.WriteLine($"   ❌ {filteredCount} incomplete records filtered out");
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

