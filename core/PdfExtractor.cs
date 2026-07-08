using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text;
using System.Text.RegularExpressions;

namespace TechClassificationApp;

public static class PdfExtractor
{
    private static readonly string[] LineSeparators = ["\n", "\r\n"];

    private static readonly Regex NumericPattern =
        new(@"\d+[\d.,]*", RegexOptions.Compiled);

    private static readonly Regex ListStructurePattern =
        new(@"^(\d+\.|\d+\)|\-|\•|\*|[A-Z]{2,}[\s_]|[A-Z][a-z]+\s[A-Z])", RegexOptions.Compiled);

    private static readonly Regex CapitalizedWordsPattern =
        new(@"\b[A-Z][A-Za-z]{2,}", RegexOptions.Compiled);

    public static async Task<string> ExtractTextAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"PDF file not found: {filePath}");

        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
            throw new InvalidOperationException("PDF file is empty.");

        if (fileInfo.Length > 50 * 1024 * 1024)
            Console.WriteLine("⚠️  Warning: PDF file is very large. Processing may take time or memory.");

        return await Task.Run(() =>
        {
            var text = new StringBuilder();
            try
            {
                using var reader = new PdfReader(filePath);
                using var pdfDoc = new PdfDocument(reader);

                int totalPages = pdfDoc.GetNumberOfPages();
                if (totalPages == 0)
                    throw new InvalidOperationException("PDF contains no pages.");

                for (int pageNum = 1; pageNum <= totalPages; pageNum++)
                {
                    try
                    {
                        var page = pdfDoc.GetPage(pageNum);
                        var content = PdfTextExtractor.GetTextFromPage(page);
                        // [PAGE N] / [END OF PAGE] markers let the AI attribute data to specific pages.
                        text.AppendLine($"[PAGE {pageNum}]");
                        AppendContentWithTableMarkers(text, content);
                        text.AppendLine("[END OF PAGE]");
                        text.AppendLine();
                    }
                    catch (Exception ex)
                    {
                        // Per-page catch: one corrupt/encrypted page should not abort the whole extraction.
                        text.AppendLine($"[ERROR extracting page {pageNum}: {ex.Message}]");
                        text.AppendLine();
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

    private static void AppendContentWithTableMarkers(StringBuilder output, string pageContent)
    {
        var lines = pageContent.Split(LineSeparators, StringSplitOptions.None);
        var tableBuffer = new List<string>();
        var tableType = "";

        foreach (var line in lines)
        {
            var numericCount = NumericPattern.Count(line);
            var hasStructure = line.Contains('|') || line.Contains('\t') ||
                               (numericCount >= 3 && line.Length > 20);

            var isTechNameLine =
                (ListStructurePattern.IsMatch(line.TrimStart()) ||
                 CapitalizedWordsPattern.Count(line) >= 2)
                && line.Trim().Length > 10;

            var isTableLine = (hasStructure && numericCount >= 2) || isTechNameLine;

            if (isTableLine)
            {
                if (tableBuffer.Count == 0)
                    tableType = numericCount >= 2 ? "DATA" : "TECH_LIST";
                tableBuffer.Add(line);
            }
            else
            {
                FlushTableBuffer(output, tableBuffer, tableType);
                tableBuffer.Clear();
                tableType = "";
                output.AppendLine(line);
            }
        }

        FlushTableBuffer(output, tableBuffer, tableType);
    }

    private static void FlushTableBuffer(StringBuilder output, List<string> buffer, string tableType)
    {
        if (buffer.Count == 0)
            return;

        // Fewer than 3 lines is likely a false-positive (e.g. a lone header row or two isolated numbers).
        if (buffer.Count >= 3)
        {
            var marker = tableType == "DATA"
                ? "[TABLE REGION - IMPORTANT NUMERICAL DATA]"
                : "[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]";
            output.AppendLine(marker);
            foreach (var line in buffer)
                output.AppendLine(line);
            output.AppendLine("[END TABLE]");
        }
        else
        {
            foreach (var line in buffer)
                output.AppendLine(line);
        }
    }

    public static List<string> SplitIntoChunks(string content, int maxCharsPerChunk = 30000, int overlapLines = 3)
    {
        var chunks = new List<string>();
        var lines = content.Split(LineSeparators, StringSplitOptions.None);
        var currentChunk = new StringBuilder();
        var currentChunkLines = new List<string>();

        if (overlapLines < 0)
            overlapLines = 0;

        foreach (var line in lines)
        {
            if (currentChunk.Length + line.Length > maxCharsPerChunk && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
                currentChunk.Clear();

                // Carry the last N lines of the previous chunk into the next one so the model
                // doesn't miss data that spans a split point.
                var overlap = currentChunkLines
                    .Skip(Math.Max(0, currentChunkLines.Count - overlapLines))
                    .ToList();

                currentChunkLines.Clear();
                foreach (var overlapLine in overlap)
                {
                    currentChunk.AppendLine(overlapLine);
                    currentChunkLines.Add(overlapLine);
                }
            }

            currentChunk.AppendLine(line);
            currentChunkLines.Add(line);
        }

        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString());

        return chunks;
    }

    public static string BuildSingleDocumentPrompt(List<string> chunks, string userQuestion)
    {
        if (chunks.Count == 1)
            return $"Here's a PDF document I need you to analyze:\n\n{chunks[0]}\n\nQuestion: {userQuestion}";

        var sb = new StringBuilder();
        sb.AppendLine($"I have a multi-page PDF document split into {chunks.Count} parts.\n");
        for (int i = 0; i < chunks.Count; i++)
        {
            sb.AppendLine($"**Part {i + 1}:**");
            sb.AppendLine(chunks[i]);
            sb.AppendLine();
        }
        sb.Append($"Question: {userQuestion}");
        return sb.ToString();
    }

    public static string BuildMultiDocumentPrompt(Dictionary<string, List<string>> pdfChunks, string userQuestion)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"I have {pdfChunks.Count} PDF documents to analyze:\n");

        int docNum = 1;
        foreach (var kvp in pdfChunks)
        {
            var chunks = kvp.Value;
            sb.AppendLine($"**Document {docNum}: {Path.GetFileName(kvp.Key)}**");
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks.Count > 1)
                    sb.AppendLine($"[Part {i + 1}/{chunks.Count}]");
                sb.AppendLine(chunks[i]);
            }
            sb.AppendLine("\n---\n");
            docNum++;
        }

        sb.Append($"Question: {userQuestion}");
        return sb.ToString();
    }
}
