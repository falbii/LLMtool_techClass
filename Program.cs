using System.Text;
using GitHub.Copilot.SDK;
using Refractored.GitHub.Copilot.SDK.Helpers;
using PdfAnalysisApp;

// Required for PDF text extraction (iText uses legacy code pages)
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// ──────────────── Banner ────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║          Smart Document Analysis Tool - Copilot SDK          ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.ResetColor();
Console.WriteLine();

// ──────────────── Prerequisites ────────────────
Console.WriteLine("🔍 Checking prerequisites...\n");
var status = await CliChecker.CheckCopilotStatusAsync();

if (!CliChecker.IsReady(status))
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Press any key to exit...");
    Console.ResetColor();
    Console.ReadKey(true);
    return;
}

// ──────────────── Available models ────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("📊 Available Models & Billing Info:\n");
Console.ResetColor();

CopilotClient? client = null;
CopilotSession? session = null;

try
{
    client = new CopilotClient();
    await client.StartAsync();
    
    var modelsWithInfo = await ModelSelector.GetModelsWithInfoAsync(client);
    if (modelsWithInfo != null && modelsWithInfo.Length > 0)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("   Model                          Multiplier  Reasoning");
        Console.WriteLine("   ─────────────────────────────  ──────────  ─────────");
        Console.ResetColor();
        
        foreach (var modelInfo in modelsWithInfo)
        {
            var multiplier = modelInfo.Billing?.Multiplier.ToString("F2") ?? "N/A";
            var reasoning = modelInfo.SupportedReasoningEfforts is { Count: > 0 } ? "Yes" : "No";
            Console.WriteLine($"   {modelInfo.Id,-30} {multiplier,10}  {reasoning,9}");
        }
        Console.WriteLine();
    }

    // ──────────────── Model selection ────────────────
    var model = await ModelSelector.SelectModelAsync();
    if (model == null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Press any key to exit...");
        Console.ResetColor();
        Console.ReadKey(true);
        return;
    }
    Console.WriteLine();

    // ──────────────── Session creation ────────────────
    await Program.RunWithSpinnerAsync($" Creating session with {model}", async () =>
    {
        session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = model,
            Streaming = true
        });
    });
    
    if (session == null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("❌ Failed to create session.");
        Console.ResetColor();
        Console.ReadKey(true);
        return;
    }
    
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"   Session ID: {session.SessionId}\n");
    Console.ResetColor();

    // ──────────────── Ensure working directories exist ────────────────
    string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "pdf_to_analyze");
    Directory.CreateDirectory(pdfFolder);
    string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "output");
    Directory.CreateDirectory(outputFolder);

    string? currentPdfFile = null;

    // ──────────────── Interactive chat loop ────────────────
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("💬 Interactive Chat - just ask something or use predefined commands:\n");
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("⚡ Available Commands:");
    Console.ResetColor();
    Console.WriteLine("  'commands' or 'help' - Display all available commands");
    Console.WriteLine("  'exit' or 'quit'     - Exit the program");
    Console.WriteLine("  'upload <path>'      - Upload a PDF to analyze (or drop PDFs in ./pdf_to_analyze/)");
    Console.WriteLine("  'list'               - List available PDFs");
    Console.WriteLine("  'analyze <file>'     - Analyze a PDF (use filename or list number)");
    Console.WriteLine("  'current'            - Show current PDF");
    Console.WriteLine("  'auto-summarize'     - Extract technology summaries to TXT");
    Console.WriteLine("  'auto-classify'      - Classify technologies and export CSV");
    Console.WriteLine("  'batch-analyze <q>'  - Analyze all PDFs with a question\n");

    while (true)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("You: ");
        Console.ResetColor();

        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input))
            continue;

        // ── Exit ──
        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\n👋 Goodbye!");
            break;
        }

        // ── List available PDFs ──
        if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
        {
            var selected = await Commands.HandleListPdfsAsync(pdfFolder);
            if (!string.IsNullOrEmpty(selected))
                currentPdfFile = selected;
            continue;
        }

        // ── Help / commands ──
        if (input.Equals("commands", StringComparison.OrdinalIgnoreCase) ||
            input.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            Commands.HandleCommandsCommand();
            continue;
        }

        // ── Show current PDF ──
        if (input.Equals("current", StringComparison.OrdinalIgnoreCase))
        {
            Commands.HandleCurrentCommand(currentPdfFile);
            continue;
        }

        // ── Batch-analyze all PDFs ──
        if (input.StartsWith("batch-analyze ", StringComparison.OrdinalIgnoreCase))
        {
            var question = input.Length > 14 ? input[14..].Trim() : string.Empty;
            if (string.IsNullOrEmpty(question))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("❌ Please provide a question. Usage: batch-analyze <your question>");
                Console.ResetColor();
                continue;
            }

            try
            {
                await Commands.HandleBatchAnalyzeAsync(session, pdfFolder, question);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error during batch analysis: {ex.Message}");
                Console.ResetColor();
            }
            continue;
        }

        // ── Upload a PDF ──
        if (input.StartsWith("upload ", StringComparison.OrdinalIgnoreCase))
        {
            var filePath = input.Length > 7 ? input[7..].Trim().Trim('"') : string.Empty;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"   Checking: {filePath}");
            Console.ResetColor();

            if (File.Exists(filePath))
            {
                currentPdfFile = await Commands.HandleUploadPdfAsync(filePath, pdfFolder);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ File not found.");
                Console.ResetColor();
            }
            continue;
        }

        // ── Analyze a specific PDF ──
        if (input.StartsWith("analyze ", StringComparison.OrdinalIgnoreCase))
        {
            var filename = input.Length > 8 ? input[8..].Trim() : string.Empty;
            var result = Commands.HandleAnalyzeCommand(filename, pdfFolder);
            if (result != null)
                currentPdfFile = result;
            continue;
        }

        // ── Auto-summarize current PDF ──
        if (input.Equals("auto-summarize", StringComparison.OrdinalIgnoreCase))
        {
            if (currentPdfFile == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("❌ No PDF loaded. Use 'upload' or 'analyze' to select one.");
                Console.ResetColor();
                continue;
            }

            await Commands.HandleAutoSummarizeAsync(session, currentPdfFile, outputFolder);
            continue;
        }

        // ── Auto-classify current PDF ──
        if (input.Equals("auto-classify", StringComparison.OrdinalIgnoreCase))
        {
            if (currentPdfFile == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("❌ No PDF loaded. Use 'upload' or 'analyze' to select one.");
                Console.ResetColor();
                continue;
            }

            await Commands.HandleAutoClassifyAsync(session, currentPdfFile, outputFolder);
            continue;
        }

        // ── Free-form question (with optional PDF context) ──
        string finalMessage = input;
        if (currentPdfFile != null && File.Exists(currentPdfFile))
        {
            finalMessage = await Commands.PrepareMessageWithPdfContextAsync(currentPdfFile, input);
        }

        await Program.SendMessageWithSpinnerAsync(session, finalMessage);
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    Console.ResetColor();
}
finally
{
    if (session != null)
        await session.DisposeAsync();
    if (client != null)
        await client.DisposeAsync();
    Console.WriteLine("\n🛑 Client stopped.");
}

public partial class Program
{
    /// <summary>
    /// Runs <paramref name="action"/> while displaying a console spinner with <paramref name="message"/>.
    /// </summary>
    public static async Task RunWithSpinnerAsync(string message, Func<Task> action)
    {
        var spinnerChars = new[] { '|', '/', '-', '\\' };
        using var cts = new CancellationTokenSource();
        
        Console.CursorVisible = false;
        Console.Write($"  {message}...");
        
        var spinnerTask = Task.Run(async () =>
        {
            int i = 0;
            var left = Console.CursorLeft;
            while (!cts.Token.IsCancellationRequested)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(spinnerChars[i++ % spinnerChars.Length]);
                try { await Task.Delay(100, cts.Token); } catch { break; }
            }
        });

        try
        {
            await action();
            cts.Cancel();
            await spinnerTask;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" →");
            Console.ResetColor();
            Console.WriteLine();
        }
        catch
        {
            cts.Cancel();
            await spinnerTask;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("✗");
            Console.ResetColor();
            Console.WriteLine();
            throw;
        }
        finally
        {
            Console.CursorVisible = true;
        }
    }

    /// <summary>
    /// Sends a message to the Copilot session, streaming the response to the console
    /// with a spinner shown while waiting for the first token.
    /// </summary>
    public static async Task SendMessageWithSpinnerAsync(CopilotSession session, string message)
    {
        var spinnerChars = new[] { '|', '/', '-', '\\' };
        var done = new TaskCompletionSource();
        var hasStartedResponse = false;
        using var spinnerCts = new CancellationTokenSource();
        
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("\nCopilot: ");
        Console.ResetColor();
        var spinnerLeft = Console.CursorLeft;
        var spinnerTop = Console.CursorTop;
        
        var spinnerTask = Task.Run(async () =>
        {
            int i = 0;
            while (!spinnerCts.Token.IsCancellationRequested)
            {
                Console.SetCursorPosition(spinnerLeft, spinnerTop);
                Console.Write(spinnerChars[i++ % spinnerChars.Length]);
                try { await Task.Delay(100, spinnerCts.Token); } catch { break; }
            }
        });

        var hasStartedReasoning = false;

        var subscription = session.On(evt =>
        {
            switch (evt)
            {
                case AssistantReasoningDeltaEvent reasoningDelta:
                    if (!hasStartedReasoning)
                    {
                        spinnerCts.Cancel();
                        Console.SetCursorPosition(spinnerLeft, spinnerTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(spinnerLeft, spinnerTop);
                        Console.CursorVisible = true;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("💭 ");
                        hasStartedReasoning = true;
                    }
                    Console.Write(reasoningDelta.Data.DeltaContent);
                    break;

                case AssistantMessageDeltaEvent delta:
                    if (!hasStartedResponse)
                    {
                        if (hasStartedReasoning)
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("\nCopilot: ");
                            Console.ResetColor();
                        }
                        else
                        {
                            spinnerCts.Cancel();
                            Console.SetCursorPosition(spinnerLeft, spinnerTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(spinnerLeft, spinnerTop);
                            Console.CursorVisible = true;
                        }
                        hasStartedResponse = true;
                    }
                    Console.Write(delta.Data.DeltaContent);
                    break;
                    
                case AssistantMessageEvent msg:
                    if (!hasStartedResponse)
                    {
                        if (hasStartedReasoning)
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("\nCopilot: ");
                            Console.ResetColor();
                        }
                        else
                        {
                            spinnerCts.Cancel();
                            Console.SetCursorPosition(spinnerLeft, spinnerTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(spinnerLeft, spinnerTop);
                            Console.CursorVisible = true;
                        }
                        Console.Write(msg.Data.Content);
                    }
                    break;
                    
                case SessionIdleEvent:
                    done.TrySetResult();
                    break;
                    
                case SessionErrorEvent err:
                    spinnerCts.Cancel();
                    Console.CursorVisible = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n❌ Error: {err.Data.Message}");
                    Console.ResetColor();
                    done.TrySetResult();
                    break;
            }
        });

        try
        {
            await session.SendAsync(new MessageOptions { Prompt = message });
            await done.Task;
            spinnerCts.Cancel();
            await spinnerTask;
            Console.WriteLine("\n");
        }
        finally
        {
            spinnerCts.Cancel();
            Console.CursorVisible = true;
            subscription.Dispose();
        }
    }

    /// <summary>
    /// Sends a message and collects the full response as a string.
    /// The response is also printed to the console with a spinner.
    /// </summary>
    public static async Task<string> SendMessageAndCollectResponseAsync(CopilotSession session, string message)
    {
        var done = new TaskCompletionSource();
        var response = new StringBuilder();
        var hasDelta = false;
        var spinnerChars = new[] { '|', '/', '-', '\\' };
        using var spinnerCts = new CancellationTokenSource();

        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("\nCopilot: ");
        Console.ResetColor();
        var spinnerLeft = Console.CursorLeft;
        var spinnerTop = Console.CursorTop;

        var spinnerTask = Task.Run(async () =>
        {
            int i = 0;
            while (!spinnerCts.Token.IsCancellationRequested)
            {
                Console.SetCursorPosition(spinnerLeft, spinnerTop);
                Console.Write(spinnerChars[i++ % spinnerChars.Length]);
                try { await Task.Delay(100, spinnerCts.Token); } catch { break; }
            }
        });

        var subscription = session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageDeltaEvent delta:
                    hasDelta = true;
                    response.Append(delta.Data.DeltaContent);
                    break;
                case AssistantMessageEvent msg:
                    if (!hasDelta)
                        response.Append(msg.Data.Content);
                    break;
                case SessionIdleEvent:
                    done.TrySetResult();
                    break;
                case SessionErrorEvent err:
                    done.TrySetException(new InvalidOperationException(err.Data.Message));
                    break;
            }
        });

        try
        {
            await session.SendAsync(new MessageOptions { Prompt = message });
            await done.Task;
            spinnerCts.Cancel();
            await spinnerTask;
            Console.WriteLine();
            return response.ToString();
        }
        finally
        {
            spinnerCts.Cancel();
            Console.CursorVisible = true;
            subscription.Dispose();
        }
    }

    /// <summary>
    /// Sends a message and collects the full response silently (no console output).
    /// Times out after 15 minutes to guard against hung service calls.
    /// </summary>
    public static async Task<string> SendMessageAndCollectResponseSilentAsync(CopilotSession session, string message)
    {
        var done = new TaskCompletionSource();
        var response = new StringBuilder();
        var hasDelta = false;
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(15));

        var subscription = session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageDeltaEvent delta:
                    hasDelta = true;
                    response.Append(delta.Data.DeltaContent);
                    break;
                case AssistantMessageEvent msg:
                    if (!hasDelta)
                        response.Append(msg.Data.Content);
                    break;
                case SessionIdleEvent:
                    done.TrySetResult();
                    break;
                case SessionErrorEvent err:
                    done.TrySetException(new InvalidOperationException(err.Data.Message));
                    break;
            }
        });

        try
        {
            await session.SendAsync(new MessageOptions { Prompt = message });
            // Wait for response with 15-minute timeout
            using var timeoutTask = timeoutCts.Token.Register(() => done.TrySetException(
                new TimeoutException("Copilot service did not respond within 15 minutes.")));
            
            await done.Task;
            return response.ToString();
        }
        catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
        {
            throw new TimeoutException("Copilot service request timed out after 15 minutes.");
        }
        finally
        {
            subscription.Dispose();
        }
    }
}
