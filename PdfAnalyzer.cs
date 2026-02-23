// PDF Analysis Helper - Add this to a new file: PdfAnalyzer.cs

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace PdfAnalysisApp;

public static class PdfAnalyzer
{
    /// <summary>
    /// Extracts text from a PDF file with enhanced table detection and preservation of numerical data.
    /// </summary>
    public static async Task<string> ExtractTextFromPdfAsync(string filePath)
    {
        // Validate file exists and is readable
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"PDF file not found: {filePath}");
        
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
            throw new InvalidOperationException("PDF file is empty.");
        
        // Warn on very large files (over 50MB)
        if (fileInfo.Length > 50 * 1024 * 1024)
            Console.WriteLine("⚠️  Warning: PDF file is very large. Processing may take time or memory.");
        
        return await Task.Run(() =>
        {
            var text = new System.Text.StringBuilder();

            try
            {
                using (var reader = new PdfReader(filePath))
                using (var pdfDoc = new PdfDocument(reader))
                {
                    int totalPages = pdfDoc.GetNumberOfPages();
                    if (totalPages == 0)
                        throw new InvalidOperationException("PDF contains no pages.");
                    
                    for (int pageNum = 1; pageNum <= totalPages; pageNum++)
                    {
                        try
                        {
                            var page = pdfDoc.GetPage(pageNum);
                            var content = PdfTextExtractor.GetTextFromPage(page);
                            
                            // Add page marker for better context
                            text.AppendLine($"[PAGE {pageNum}]");
                            
                            // Process content with table region markers
                            AppendContentWithTableMarkers(text, content);
                            
                            text.AppendLine("[END OF PAGE]");
                            text.AppendLine();
                        }
                        catch (Exception ex)
                        {
                            text.AppendLine($"[ERROR extracting page {pageNum}: {ex.Message}]");
                            text.AppendLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to extract text from PDF: {ex.Message}", ex);
            }

            return text.ToString();
        });
    }

    /// <summary>
    /// Appends content with inline table region markers (doesn't duplicate content).
    /// </summary>
    private static void AppendContentWithTableMarkers(System.Text.StringBuilder output, string pageContent)
    {
        var lines = pageContent.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
        var tableBuffer = new List<string>();
        var lineCount = 0;
        var tableType = "";

        foreach (var line in lines)
        {
            // Count numeric values and structural indicators in the line
            var numericCount = System.Text.RegularExpressions.Regex.Matches(line, @"\d+[\d.,]*").Count;
            var hasStructure = line.Contains("|") || line.Contains("\t") || 
                              (numericCount >= 3 && line.Length > 20);
            
            // Detect technology name lists (even without numbers)
            var hasTechKeywords = System.Text.RegularExpressions.Regex.IsMatch(line, 
                @"\b(electrolysis|synthesis|capture|conversion|storage|production|supply|generation|reactor|turbine|boiler|pump|compressor|heat exchanger|separator|absorber|adsorber|catalytic|thermal|chemical|mechanical|electrical)\b", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            var hasListStructure = System.Text.RegularExpressions.Regex.IsMatch(line.TrimStart(), 
                @"^(\d+\.|\d+\)|\-|\•|\*|[A-Z]{2,}[\s_]|[A-Z][a-z]+\s[A-Z])");
            
            var hasMultipleCapitalizedWords = System.Text.RegularExpressions.Regex.Matches(line, @"\b[A-Z][A-Za-z]{2,}").Count >= 2;
            
            var isTechNameLine = (hasTechKeywords || hasListStructure || hasMultipleCapitalizedWords) && 
                                 line.Trim().Length > 10;
            
            var isTableLine = (hasStructure && numericCount >= 2) || isTechNameLine;

            if (isTableLine)
            {
                // Start of table region
                if (tableBuffer.Count == 0)
                {
                    // Determine table type
                    if (numericCount >= 2)
                        tableType = "DATA";
                    else
                        tableType = "TECH_LIST";
                        
                    var marker = tableType == "DATA" 
                        ? "[TABLE REGION - IMPORTANT NUMERICAL DATA]"
                        : "[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]";
                    output.AppendLine(marker);
                }
                
                output.AppendLine(line);
                tableBuffer.Add(line);
                lineCount++;
            }
            else
            {
                // Not a table line
                if (tableBuffer.Count > 0 && lineCount >= 3)
                {
                    // End previous table
                    output.AppendLine("[END TABLE]");
                    tableBuffer.Clear();
                    lineCount = 0;
                    tableType = "";
                }
                else if (tableBuffer.Count > 0)
                {
                    // Table was too short, discard markers
                    tableBuffer.Clear();
                    lineCount = 0;
                    tableType = "";
                }
                
                // Add normal line
                output.AppendLine(line);
            }
        }

        // Handle remaining table buffer at end of content
        if (tableBuffer.Count > 0 && lineCount >= 3)
        {
            output.AppendLine("[END TABLE]");
        }
    }

    /// <summary>
    /// Chunks PDF content into smaller pieces to avoid token limits.
    /// </summary>
    public static List<string> ChunkPdfContent(string content, int maxCharsPerChunk = 30000)
    {
        var chunks = new List<string>();
        var lines = content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
        var currentChunk = new System.Text.StringBuilder();

        foreach (var line in lines)
        {
            if (currentChunk.Length + line.Length > maxCharsPerChunk && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
                currentChunk.Clear();
            }

            currentChunk.AppendLine(line);
        }

        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString());

        return chunks;
    }

    /// <summary>
    /// Builds a prompt that includes PDF context and user question.
    /// </summary>
    public static string BuildAnalysisPrompt(List<string> chunks, string userQuestion)
    {
        if (chunks.Count == 1)
        {
            return $"Here's a PDF document I need you to analyze:\n\n{chunks[0]}\n\nQuestion: {userQuestion}";
        }

        var prompt = $"I have a multi-page PDF document split into {chunks.Count} parts.\n\n";
        for (int i = 0; i < chunks.Count; i++)
        {
            prompt += $"**Part {i + 1}:**\n{chunks[i]}\n\n";
        }
        prompt += $"Question: {userQuestion}";

        return prompt;
    }

    /// <summary>
    /// Builds a prompt for batch analysis of multiple PDFs.
    /// </summary>
    public static string BuildBatchAnalysisPrompt(Dictionary<string, List<string>> pdfChunks, string userQuestion)
    {
        var prompt = $"I have {pdfChunks.Count} PDF documents to analyze:\n\n";
        
        int docNum = 1;
        foreach (var kvp in pdfChunks)
        {
            var fileName = Path.GetFileName(kvp.Key);
            var chunks = kvp.Value;
            prompt += $"**Document {docNum}: {fileName}**\n";
            prompt += $"{chunks[0]}\n";
            if (chunks.Count > 1)
                prompt += $"[{chunks.Count - 1} additional parts available]\n";
            prompt += "\n---\n\n";
            docNum++;
        }
        
        prompt += $"Question: {userQuestion}";
        return prompt;
    }
}
