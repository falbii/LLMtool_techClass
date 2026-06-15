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

// --local (or --ollama) runs against a local Ollama server instead of GitHub
// Copilot. The rest of the app is provider-neutral, so only the client creation
// and this prerequisite check differ between the two modes.
bool useLocal = args.Contains("--local", StringComparer.OrdinalIgnoreCase)
    || args.Contains("--ollama", StringComparer.OrdinalIgnoreCase);

if (!useLocal)
{
    Console.WriteLine("🔍 Checking prerequisites...\n");
    var status = await CliChecker.CheckCopilotStatusAsync();

    if (!CliChecker.IsReady(status))
    {
        ConsoleEx.Warn("Press any key to exit...");
        Console.ReadKey(true);
        return;
    }
}

// Directory layout shared by console and web mode.
string baseDir = Directory.GetCurrentDirectory();
string pdfInputDirectory = Path.Combine(baseDir, "1_pdf_to_analyze");
string cacheDirectory = Path.Combine(baseDir, "2_md_condensed_pdf");
string mdDirectory = Path.Combine(baseDir, "3_output", "1_md_summary");
string csvDirectory = Path.Combine(baseDir, "3_output", "2_csv_classification");
Directory.CreateDirectory(pdfInputDirectory);
Directory.CreateDirectory(cacheDirectory);
Directory.CreateDirectory(mdDirectory);
Directory.CreateDirectory(csvDirectory);

IChatClient? client = null;
IChatSession? session = null;

try
{
    if (useLocal)
    {
        client = await OllamaChatClient.ConnectAsync();
        ConsoleEx.Info("🦙 Using local Ollama models.\n");
    }
    else
    {
        client = await CopilotChatClient.ConnectAsync();
    }

    if (args.Contains("--web", StringComparer.OrdinalIgnoreCase))
    {
        // Web mode: the browser page replaces the console loop; model selection
        // and session creation happen through the page.
        await WebServer.RunAsync(client, pdfInputDirectory, cacheDirectory, mdDirectory, csvDirectory);
        return;
    }

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

    var workspace = new Workspace(client, model, pdfInputDirectory, cacheDirectory, mdDirectory, csvDirectory);

    string? selectedPdfPath = null;
    // Path of the PDF whose text has already been sent into the chat session. Free-form questions
    // inject the (condensed) document only on the first question about a given PDF; after that the
    // session already holds it, and re-sending would duplicate the whole document every turn.
    string? pdfInjectedIntoSession = null;

    ConsoleEx.Info("💬 Interactive Chat - just ask something or use predefined commands:");
    CommandHandlers.HandleCommandsCommand();

    while (true)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("You: ");
        Console.ResetColor();

        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input))
            continue;

        // '/' marks an explicit command ('/list', '/auto-summarize', ...). Bare names still work;
        // the difference is that an unknown '/xxx' fails fast below instead of being sent to the
        // model as a chat question, where a mistyped command would silently cost tokens.
        var isCommand = input.StartsWith('/');
        var command = isCommand ? input[1..].Trim() : input;

        if (command.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            command.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\n👋 Goodbye!");
            break;
        }

        if (command.Equals("list", StringComparison.OrdinalIgnoreCase))
        {
            var selected = await CommandHandlers.HandleListPdfsAsync(pdfInputDirectory);
            if (!string.IsNullOrEmpty(selected))
                selectedPdfPath = selected;
            continue;
        }

        if (command.Equals("commands", StringComparison.OrdinalIgnoreCase) ||
            command.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            CommandHandlers.HandleCommandsCommand();
            continue;
        }

        if (command.Equals("current", StringComparison.OrdinalIgnoreCase))
        {
            CommandHandlers.HandleCurrentCommand(selectedPdfPath);
            continue;
        }

        if (command.StartsWith("batch-analyze ", StringComparison.OrdinalIgnoreCase))
        {
            var question = command.Length > 14 ? command[14..].Trim() : string.Empty;
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

        if (command.StartsWith("upload ", StringComparison.OrdinalIgnoreCase))
        {
            var filePath = command.Length > 7 ? command[7..].Trim().Trim('"') : string.Empty;
            ConsoleEx.Dim($"   Checking: {filePath}");

            if (File.Exists(filePath))
                selectedPdfPath = await CommandHandlers.HandleUploadPdfAsync(filePath, pdfInputDirectory);
            else
            {
                ConsoleEx.Error("❌ File not found.");
            }
            continue;
        }

        if (command.Equals("auto-summarize", StringComparison.OrdinalIgnoreCase))
        {
            if (selectedPdfPath == null)
            {
                ConsoleEx.Warn("❌ No PDF loaded. Use 'upload' or 'list' to select one.");
                continue;
            }

            await TechnologySummarizer.RunAsync(workspace, selectedPdfPath);
            continue;
        }

        if (command.Equals("auto-classify", StringComparison.OrdinalIgnoreCase))
        {
            if (selectedPdfPath == null)
            {
                ConsoleEx.Warn("❌ No PDF loaded. Use 'upload' or 'list' to select one.");
                continue;
            }

            await TechnologyClassifier.RunAsync(workspace, selectedPdfPath);
            continue;
        }

        if (command.Equals("benchmark", StringComparison.OrdinalIgnoreCase))
        {
            await CommandHandlers.HandleBenchmarkAsync(workspace);
            continue;
        }

        if (command.Equals("condense-check", StringComparison.OrdinalIgnoreCase))
        {
            if (selectedPdfPath == null)
            {
                ConsoleEx.Warn("❌ No PDF loaded. Use 'upload' or 'list' to select one.");
                continue;
            }

            await CommandHandlers.HandleCondenseCheckAsync(workspace, selectedPdfPath);
            continue;
        }

        // Anything '/'-prefixed that didn't match above is a command error, never a chat message.
        if (isCommand)
        {
            if (command.Length == 0)
            {
                CommandHandlers.HandleCommandsCommand();
                continue;
            }
            ConsoleEx.Error($"❌ Unknown command: /{command} — type '/help' to list commands.");
            continue;
        }

        string finalMessage = input;
        if (selectedPdfPath != null && File.Exists(selectedPdfPath) &&
            !selectedPdfPath.Equals(pdfInjectedIntoSession, StringComparison.OrdinalIgnoreCase))
        {
            var (prompt, pdfIncluded) = await CommandHandlers.BuildPromptWithPdfContextAsync(workspace, selectedPdfPath, input);
            finalMessage = prompt;
            if (pdfIncluded)
                pdfInjectedIntoSession = selectedPdfPath;
        }

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

    // Pads text on both sides so it sits centered within the given column width.
    private static string Center(string text, int width)
    {
        if (text.Length >= width) return text;
        var left = (width - text.Length) / 2;
        return text.PadLeft(left + text.Length).PadRight(width);
    }

    private static async Task<ChatModelInfo[]> GetModelsWithInfoAsync(IChatClient client)
    {
        var models = await client.ListModelsAsync();
        return models.OrderBy(model => model.Id, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static Task<string?> SelectModelAsync(ChatModelInfo[] models)
    {
        if (models.Length == 0)
        {
            ConsoleEx.Error("❌ No models available.");
            return Task.FromResult<string?>(null);
        }

        ConsoleEx.Info("📊 Available Models:\n");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("    #  Model                           Reasoning");
        Console.WriteLine("   ──  ──────────────────────────────  ─────────");
        Console.ResetColor();

        for (int i = 0; i < models.Length; i++)
        {
            var reasoning = models[i].SupportsReasoning ? "Yes" : "No";
            Console.WriteLine($"   {i + 1,2}  {models[i].Id,-30}  {Center(reasoning, 9)}");
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

    // Shows a spinner while waiting for the first token, then streams reasoning/content deltas in real time.
    public static async Task SendMessageWithSpinnerAsync(IChatSession session, string message)
    {
        var hasStartedResponse = false;
        var hasStartedReasoning = false;
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

        void StopSpinner()
        {
            spinnerCts.Cancel();
            Console.SetCursorPosition(spinnerLeft, spinnerTop);
            Console.Write(" ");
            Console.SetCursorPosition(spinnerLeft, spinnerTop);
            Console.CursorVisible = true;
        }

        void OnReasoningDelta(string chunk)
        {
            if (!hasStartedReasoning)
            {
                StopSpinner();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("💭 ");
                hasStartedReasoning = true;
            }
            Console.Write(chunk);
        }

        void OnContentDelta(string chunk)
        {
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
                    StopSpinner();
                }
                hasStartedResponse = true;
            }
            Console.Write(chunk);
        }

        try
        {
            await session.SendAsync(message, OnReasoningDelta, OnContentDelta);
            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            // Chat errors are reported inline and don't abort the interactive loop.
            spinnerCts.Cancel();
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error: {ex.Message}");
            Console.ResetColor();
        }
        finally
        {
            spinnerCts.Cancel();
            await spinnerTask;
            Console.CursorVisible = true;
        }
    }

    // Collects the full response as a string while showing a "Copilot: " spinner.
    // Use SendMessageAndStreamToConsoleAsync instead when real-time output is needed.
    public static async Task<string> SendMessageAndCollectResponseAsync(IChatSession session, string message)
    {
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

        try
        {
            var response = await session.SendAsync(message);
            Console.WriteLine();
            return response;
        }
        finally
        {
            spinnerCts.Cancel();
            await spinnerTask;
            Console.CursorVisible = true;
        }
    }

    // Silent collection with a 15-minute guard. Used for batch/background calls where no spinner is needed.
    public static async Task<string> SendMessageAndCollectResponseSilentAsync(IChatSession session, string message)
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(15));
        try
        {
            return await session.SendAsync(message, cancellationToken: timeoutCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            throw new TimeoutException("The model service did not respond within 15 minutes.");
        }
    }

    // Streams delta tokens to the console in real time (dim grey, word-wrapped at 100 chars)
    // and returns the full collected response. The spinner is cancelled on the first token arrival.
    public static async Task<string> SendMessageAndStreamToConsoleAsync(
        IChatSession session, string message, string linePrefix = "   ")
    {
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

        void OnContentDelta(string chunk)
        {
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
        }

        try
        {
            var response = await session.SendAsync(message, onContentDelta: OnContentDelta,
                cancellationToken: timeoutCts.Token);
            Console.WriteLine();
            return response;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            throw new TimeoutException("The model service did not respond within 15 minutes.");
        }
        finally
        {
            spinnerCts.Cancel();
            await spinnerTask;
            Console.ResetColor();
        }
    }
}
