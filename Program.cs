using System.Text;
using GitHub.Copilot.SDK;
using Refractored.GitHub.Copilot.SDK.Helpers;
using TechClassificationApp;

// iText7 relies on legacy code-page encodings for some PDFs; must register before any extraction.
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║          Smart Document Analysis Tool - Copilot SDK          ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.ResetColor();
Console.WriteLine();

Console.WriteLine("🔍 Checking prerequisites...\n");
var status = await CliChecker.CheckCopilotStatusAsync();

if (!CliChecker.IsReady(status))
{
    ConsoleEx.Warn("Press any key to exit...");
    Console.ReadKey(true);
    return;
}

CopilotClient? client = null;
CopilotSession? session = null;

try
{
    client = new CopilotClient(new CopilotClientOptions());
    await client.StartAsync();

    var modelsWithInfo = await GetModelsWithInfoAsync(client);

    var model = await SelectModelAsync(modelsWithInfo);
    if (model == null)
    {
        ConsoleEx.Warn("Press any key to exit...");
        Console.ReadKey(true);
        return;
    }
    Console.WriteLine();

    await Program.RunWithSpinnerAsync($" Creating session with {model}", async () =>
    {
        session = await Sessions.NewAsync(client, model);
    });

    if (session == null)
    {
        ConsoleEx.Error("❌ Failed to create session.");
        Console.ReadKey(true);
        return;
    }

    ConsoleEx.Dim($"   Session ID: {session.SessionId}\n");

    string baseDir = Directory.GetCurrentDirectory();
    string pdfInputDirectory = Path.Combine(baseDir, "1_pdf_to_analyze");
    string cacheDirectory = Path.Combine(baseDir, "2_md_condensed_pdf");
    string txtDirectory = Path.Combine(baseDir, "3_output", "1_txt_summary");
    string csvDirectory = Path.Combine(baseDir, "3_output", "2_csv_classification");
    Directory.CreateDirectory(pdfInputDirectory);
    Directory.CreateDirectory(cacheDirectory);
    Directory.CreateDirectory(txtDirectory);
    Directory.CreateDirectory(csvDirectory);

    var workspace = new Workspace(client, model, pdfInputDirectory, cacheDirectory, txtDirectory, csvDirectory);

    string? selectedPdfPath = null;

    ConsoleEx.Info("💬 Interactive Chat - just ask something or use predefined commands:\n");
    ConsoleEx.Warn("⚡ Available Commands:");
    Console.WriteLine("  'commands' or 'help'   - Display all available commands");
    Console.WriteLine("  'exit' or 'quit'       - Exit the program");
    Console.WriteLine("  'upload <path>'        - Upload a PDF to analyze (or drop PDFs in ./1_pdf_to_analyze/)");
    Console.WriteLine("  'list'                 - List available PDFs and choose one to analyze");
    Console.WriteLine("  'current'              - Show current PDF");
    Console.WriteLine("  'auto-summarize'       - Extract technology summaries to TXT");
    Console.WriteLine("  'auto-classify' (beta) - Classify technologies and export CSV");
    Console.WriteLine("  'batch-analyze <q>'    - Analyze all PDFs with a question");
    Console.WriteLine("  'benchmark'            - Compare all models on the Allgoewer paper\n");

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

        if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
        {
            var selected = await CommandHandlers.HandleListPdfsAsync(pdfInputDirectory);
            if (!string.IsNullOrEmpty(selected))
                selectedPdfPath = selected;
            continue;
        }

        if (input.Equals("commands", StringComparison.OrdinalIgnoreCase) ||
            input.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            CommandHandlers.HandleCommandsCommand();
            continue;
        }

        if (input.Equals("current", StringComparison.OrdinalIgnoreCase))
        {
            CommandHandlers.HandleCurrentCommand(selectedPdfPath);
            continue;
        }

        if (input.StartsWith("batch-analyze ", StringComparison.OrdinalIgnoreCase))
        {
            var question = input.Length > 14 ? input[14..].Trim() : string.Empty;
            if (string.IsNullOrEmpty(question))
            {
                ConsoleEx.Warn("❌ Please provide a question. Usage: batch-analyze <your question>");
                continue;
            }

            try
            {
                await CommandHandlers.HandleBatchAnalyzeAsync(workspace, session, question);
            }
            catch (Exception ex)
            {
                ConsoleEx.Error($"❌ Error during batch analysis: {ex.Message}");
            }
            continue;
        }

        if (input.StartsWith("upload ", StringComparison.OrdinalIgnoreCase))
        {
            var filePath = input.Length > 7 ? input[7..].Trim().Trim('"') : string.Empty;
            ConsoleEx.Dim($"   Checking: {filePath}");

            if (File.Exists(filePath))
                selectedPdfPath = await CommandHandlers.HandleUploadPdfAsync(filePath, pdfInputDirectory);
            else
            {
                ConsoleEx.Error("❌ File not found.");
            }
            continue;
        }

        if (input.Equals("auto-summarize", StringComparison.OrdinalIgnoreCase))
        {
            if (selectedPdfPath == null)
            {
                ConsoleEx.Warn("❌ No PDF loaded. Use 'upload' or 'list' to select one.");
                continue;
            }

            await TechnologySummarizer.RunAsync(workspace, selectedPdfPath);
            continue;
        }

        if (input.Equals("auto-classify", StringComparison.OrdinalIgnoreCase))
        {
            if (selectedPdfPath == null)
            {
                ConsoleEx.Warn("❌ No PDF loaded. Use 'upload' or 'list' to select one.");
                continue;
            }

            await TechnologyClassifier.RunAsync(workspace, selectedPdfPath);
            continue;
        }

        if (input.Equals("benchmark", StringComparison.OrdinalIgnoreCase))
        {
            await CommandHandlers.HandleBenchmarkAsync(workspace);
            continue;
        }

        string finalMessage = input;
        if (selectedPdfPath != null && File.Exists(selectedPdfPath))
            finalMessage = await CommandHandlers.BuildPromptWithPdfContextAsync(workspace, selectedPdfPath, input);

        await Program.SendMessageWithSpinnerAsync(session, finalMessage);
    }
}
catch (Exception ex)
{
    ConsoleEx.Error($"\n❌ Error: {ex.Message}");
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
    private static readonly char[] SpinnerChars = ['|', '/', '-', '\\'];

    private static async Task<ModelInfo[]> GetModelsWithInfoAsync(CopilotClient client)
    {
        var models = await client.ListModelsAsync();
        return models.OrderBy(model => model.Id, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static Task<string?> SelectModelAsync(ModelInfo[] models)
    {
        if (models.Length == 0)
        {
            ConsoleEx.Error("❌ No models available.");
            return Task.FromResult<string?>(null);
        }

        ConsoleEx.Info("📊 Available Models & Billing Info:\n");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("   Model                          Reasoning");
        Console.WriteLine("   ─────────────────────────────  ─────────");
        Console.ResetColor();

        for (int i = 0; i < models.Length; i++)
        {
            var reasoning = models[i].SupportedReasoningEfforts is { Count: > 0 } ? "Yes" : "No";
            Console.WriteLine($"   {i + 1}. {models[i].Id,-30} {reasoning,9}");
        }

        ConsoleEx.Info("\nSelect a model by number or type the model id:\n");

        Console.Write("Model: ");

        var choice = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(choice))
            return Task.FromResult<string?>(null);

        if (int.TryParse(choice, out var selectedIndex) && selectedIndex >= 1 && selectedIndex <= models.Length)
            return Task.FromResult<string?>(models[selectedIndex - 1].Id);

        return Task.FromResult<string?>(models.FirstOrDefault(model =>
            model.Id.Equals(choice, StringComparison.OrdinalIgnoreCase))?.Id);
    }

    // Generic overload lets callers capture a return value; the void overload below delegates to this.
    public static async Task<T> RunWithSpinnerAsync<T>(string message, Func<Task<T>> action)
    {
        using var cts = new CancellationTokenSource();

        Console.CursorVisible = false;
        Console.Write($"  {message}...");

        var spinnerTask = Task.Run(async () =>
        {
            int i = 0;
            while (!cts.Token.IsCancellationRequested)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(SpinnerChars[i++ % SpinnerChars.Length]);
                try { await Task.Delay(100, cts.Token); } catch { break; }
            }
        });

        try
        {
            var result = await action();
            cts.Cancel();
            await spinnerTask;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("✓");
            Console.ResetColor();
            Console.WriteLine();
            return result;
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

    public static Task RunWithSpinnerAsync(string message, Func<Task> action)
        => RunWithSpinnerAsync<int>(message, async () => { await action(); return 0; });

    // Shows a spinner while waiting for the first token, then streams delta/reasoning events in real time.
    public static async Task SendMessageWithSpinnerAsync(CopilotSession session, string message)
    {
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
                Console.Write(SpinnerChars[i++ % SpinnerChars.Length]);
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
                        // After reasoning, start a new "Copilot:" line for the final answer.
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
                    // Fired by non-streaming models; skip if deltas already covered the content.
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
            Console.WriteLine("\n");
        }
        finally
        {
            spinnerCts.Cancel();
            await spinnerTask;
            Console.CursorVisible = true;
            subscription.Dispose();
        }
    }

    // Collects the full response as a string while showing a "Copilot: " spinner.
    // Use SendMessageAndStreamToConsoleAsync instead when real-time output is needed.
    public static async Task<string> SendMessageAndCollectResponseAsync(CopilotSession session, string message)
    {
        var done = new TaskCompletionSource();
        var response = new StringBuilder();
        var hasDelta = false;
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
                Console.Write(SpinnerChars[i++ % SpinnerChars.Length]);
                try { await Task.Delay(100, spinnerCts.Token); } catch { break; }
            }
        });

        var subscription = session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageDeltaEvent delta:
                    // hasDelta prevents double-appending when the SDK fires both delta and full-message events.
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
            Console.WriteLine();
            return response.ToString();
        }
        finally
        {
            spinnerCts.Cancel();
            await spinnerTask;
            Console.CursorVisible = true;
            subscription.Dispose();
        }
    }

    // Silent collection with a 15-minute guard. Used for batch/background calls where no spinner is needed.
    // The SDK's SendAsync has no cancellation token; the timeout is injected by failing the TCS from a
    // background CancellationToken.Register callback.
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

    // Streams delta tokens to the console in real time (dim grey, word-wrapped at 100 chars)
    // and returns the full collected response. The spinner is cancelled on the first token arrival.
    public static async Task<string> SendMessageAndStreamToConsoleAsync(
        CopilotSession session, string message, string linePrefix = "   ")
    {
        var done = new TaskCompletionSource();
        var response = new StringBuilder();
        var hasDelta = false;
        var lineLen = 0;
        const int wrapAt = 100;
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(15));

        using var spinnerCts = new CancellationTokenSource();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"{linePrefix}⏳ waiting...");
        // -1 to position the spinner over the last character of "waiting..."
        var waitLeft = Console.CursorLeft - 1;
        var waitTop = Console.CursorTop;
        var spinnerTask = Task.Run(async () =>
        {
            int i = 0;
            while (!spinnerCts.Token.IsCancellationRequested)
            {
                Console.SetCursorPosition(waitLeft, waitTop);
                Console.Write(SpinnerChars[i++ % SpinnerChars.Length]);
                try { await Task.Delay(100, spinnerCts.Token); } catch { break; }
            }
        });

        var subscription = session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageDeltaEvent delta:
                    if (!hasDelta)
                    {
                        // First token: clear the spinner line and start fresh with the prefix.
                        spinnerCts.Cancel();
                        Console.SetCursorPosition(0, waitTop);
                        Console.Write(new string(' ', Console.WindowWidth - 1));
                        Console.SetCursorPosition(0, waitTop);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(linePrefix);
                        lineLen = 0;
                    }
                    hasDelta = true;
                    var chunk = delta.Data.DeltaContent ?? string.Empty;
                    response.Append(chunk);
                    foreach (var ch in chunk)
                    {
                        if (ch == '\n')
                        {
                            Console.WriteLine();
                            Console.Write(linePrefix);
                            lineLen = 0;
                        }
                        else
                        {
                            if (lineLen >= wrapAt)
                            {
                                Console.WriteLine();
                                Console.Write(linePrefix);
                                lineLen = 0;
                            }
                            Console.Write(ch);
                            lineLen++;
                        }
                    }
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
            using var timeoutTask = timeoutCts.Token.Register(() => done.TrySetException(
                new TimeoutException("Copilot service did not respond within 15 minutes.")));
            await done.Task;
            Console.WriteLine();
            Console.ResetColor();
            return response.ToString();
        }
        catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
        {
            Console.ResetColor();
            throw new TimeoutException("Copilot service request timed out after 15 minutes.");
        }
        finally
        {
            spinnerCts.Cancel();
            await spinnerTask;
            subscription.Dispose();
        }
    }
}
