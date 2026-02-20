using System.Text;
using GitHub.Copilot.SDK;
using Refractored.GitHub.Copilot.SDK.Helpers;
using PdfAnalysisApp;

// Register encoding provider for PDF text extraction
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║          Smart Document Analysis Tool - Copilot SDK          ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.ResetColor();
Console.WriteLine();

// Step 1: Check prerequisites
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

// Step 2: Display available models with billing info
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

    // Step 3: Select model
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

    // Step 4: Create session
    await Program.RunWithSpinnerAsync($" Creating session with {model}", async () =>
    {
        session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = model,
            Streaming = true
        });
    });
    
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"   Session ID: {session!.SessionId}\n");
    Console.ResetColor();

    // Step 5: Create pdf_to_analyze and output folders
    string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "pdf_to_analyze");
    Directory.CreateDirectory(pdfFolder);
    string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "output");
    Directory.CreateDirectory(outputFolder);
    
    string? currentPdfFile = null;

    // Step 6: Interactive chat
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
    Console.WriteLine("  'auto-classify'      - Classify technologies and export CSV");
    Console.WriteLine("  'batch-analyze <q>'  - Analyze all PDFs with a question\n");
    Console.ResetColor();

    while (true)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("You: ");
        Console.ResetColor();

        var input = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(input))
            continue;

        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\n👋 Goodbye!");
            break;
        }

    // Handle PDF commands
        if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
        {
            var selected = await Commands.HandleListPdfsAsync(pdfFolder);
            if (!string.IsNullOrEmpty(selected))
                currentPdfFile = selected;
            continue;
        }

        if (input.Equals("commands", StringComparison.OrdinalIgnoreCase) ||
            input.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            Commands.HandleCommandsCommand();
            continue;
        }

        if (input.Equals("current", StringComparison.OrdinalIgnoreCase))
        {
            Commands.HandleCurrentCommand(currentPdfFile);
            continue;
        }

        if (input.StartsWith("batch-analyze ", StringComparison.OrdinalIgnoreCase))
        {
            var question = input[14..].Trim();
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

        if (input.StartsWith("upload ", StringComparison.OrdinalIgnoreCase))
        {
            var filePath = input[7..].Trim().Trim('"');
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

        if (input.StartsWith("analyze ", StringComparison.OrdinalIgnoreCase))
        {
            var filename = input[8..].Trim();
            var result = Commands.HandleAnalyzeCommand(filename, pdfFolder);
            if (result != null)
                currentPdfFile = result;
            continue;
        }

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

        // If PDF is loaded, add context to the message
        string finalMessage = input;
        if (currentPdfFile != null && File.Exists(currentPdfFile))
        {
            finalMessage = await Commands.PrepareMessageWithPdfContextAsync(currentPdfFile, input);
        }

        // Show spinner while waiting for response
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
    public static async Task RunWithSpinnerAsync(string message, Func<Task> action)
    {
        var spinnerChars = new[] { '|', '/', '-', '\\' };
        var cts = new CancellationTokenSource();
        
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

    public static async Task SendMessageWithSpinnerAsync(CopilotSession session, string message)
    {
        var spinnerChars = new[] { '|', '/', '-', '\\' };
        var done = new TaskCompletionSource();
        var hasStartedResponse = false;
        var spinnerCts = new CancellationTokenSource();
        
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

    public static async Task<string> SendMessageAndCollectResponseAsync(CopilotSession session, string message)
    {
        var done = new TaskCompletionSource();
        var response = new StringBuilder();
        var hasDelta = false;
        var spinnerChars = new[] { '|', '/', '-', '\\' };
        var spinnerCts = new CancellationTokenSource();

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

    public static async Task<string> SendMessageAndCollectResponseSilentAsync(CopilotSession session, string message)
    {
        var done = new TaskCompletionSource();
        var response = new StringBuilder();
        var hasDelta = false;

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
            return response.ToString();
        }
        finally
        {
            subscription.Dispose();
        }
    }
}
