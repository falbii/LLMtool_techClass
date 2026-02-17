// PDF Analysis Helper - Add this to a new file: PdfAnalyzer.cs

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace PdfAnalysisApp;

public static class PdfAnalyzer
{
    /// <summary>
    /// Extracts text from a PDF file.
    /// </summary>
    public static async Task<string> ExtractTextFromPdfAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            var text = new System.Text.StringBuilder();

            using (var reader = new PdfReader(filePath))
            using (var pdfDoc = new PdfDocument(reader))
            {
                for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
                {
                    var page = pdfDoc.GetPage(pageNum);
                    var content = PdfTextExtractor.GetTextFromPage(page);
                    text.AppendLine(content);
                }
            }

            return text.ToString();
        });
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
