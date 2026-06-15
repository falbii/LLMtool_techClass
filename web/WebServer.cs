using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.FileProviders;

namespace TechClassificationApp;

// Local web UI: serves web/wwwroot/index.html on localhost and exposes the same
// operations as the console loop through a small JSON/SSE API. Single-user by
// design — one shared state object, one operation at a time (Gate).
public static class WebServer
{
    private const string Url = "http://localhost:5179";

    // Every way a browser can legitimately name this server. Used by the
    // CSRF guard below: a cross-site request carries the attacker's page as
    // its Origin, which won't be in this list.
    private static readonly string[] AllowedOrigins =
        ["http://localhost:5179", "http://127.0.0.1:5179", "http://[::1]:5179"];

    private sealed class WebAppState(IChatClient client)
    {
        public IChatClient Client { get; } = client;
        public IChatSession? Session;
        public Workspace? Workspace;
        public string? SelectedPdf;
        // Same role as pdfInjectedIntoSession in the console loop: the PDF whose
        // text was already sent into the chat session (don't re-send each turn).
        public string? PdfInjectedIntoSession;
        public readonly SemaphoreSlim Gate = new(1, 1);
    }

    private sealed record SessionRequest(string Model);
    private sealed record SelectRequest(string Name);
    private sealed record ChatRequest(string Text);

    public static async Task RunAsync(
        IChatClient client, string pdfDir, string cacheDir, string mdDir, string csvDir)
    {
        var state = new WebAppState(client);

        var builder = WebApplication.CreateBuilder();
        builder.Logging.SetMinimumLevel(LogLevel.Warning); // keep the terminal readable
        var app = builder.Build();

        // The static files (index.html/app.js/style.css) now live under web/wwwroot
        // instead of the ASP.NET-default ./wwwroot. Serve them through an explicit
        // physical file provider so they resolve regardless of the conventional web
        // root. Rooted at the content root, which is the launch directory.
        var webRoot = Path.Combine(app.Environment.ContentRootPath, "web", "wwwroot");
        var webFiles = new PhysicalFileProvider(webRoot);

        // --- CSRF / DNS-rebinding guard --------------------------------------
        // Binding to localhost keeps remote machines out, but not other websites
        // open in the same browser: a malicious page can still fire requests at
        // http://localhost:5179 (CSRF), and DNS rebinding can point a foreign
        // hostname at 127.0.0.1. Two header checks close both holes:
        //  - Host must be a loopback name (blocks DNS rebinding);
        //  - Origin, when the browser sends one, must be ours (blocks CSRF —
        //    cross-site requests always carry the foreign page's Origin).
        app.Use(async (ctx, next) =>
        {
            var host = ctx.Request.Host.Host;
            var origin = ctx.Request.Headers.Origin.ToString();
            var hostOk = host is "localhost" or "127.0.0.1" or "::1" or "[::1]";
            var originOk = string.IsNullOrEmpty(origin) ||
                AllowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
            if (!hostOk || !originOk)
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                await ctx.Response.WriteAsJsonAsync(new { error = "Cross-origin requests are not allowed." });
                return;
            }
            await next();
        });

        app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = webFiles }); // "GET /" -> web/wwwroot/index.html
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = webFiles,
            // Always revalidate: without this, browsers may keep an old
            // style.css/app.js after the files change, leaving the page with
            // new HTML but stale styling. Localhost, so the cost is nil.
            OnPrepareResponse = ctx =>
                ctx.Context.Response.Headers.CacheControl = "no-cache",
        });

        // --- Setup -------------------------------------------------------------

        app.MapGet("/api/models", async () =>
        {
            var models = await state.Client.ListModelsAsync();
            return Results.Json(models
                .OrderBy(m => m.Id, StringComparer.OrdinalIgnoreCase)
                .Select(m => new { id = m.Id, supportsReasoning = m.SupportsReasoning }));
        });

        app.MapPost("/api/session", async (SessionRequest req) =>
        {
            if (string.IsNullOrWhiteSpace(req.Model))
                return Results.BadRequest(new { error = "No model given." });
            if (!await state.Gate.WaitAsync(0))
                return Results.Conflict(new { error = "Another operation is running." });
            try
            {
                if (state.Session != null)
                    await state.Session.DisposeAsync();
                state.Session = await Sessions.NewAsync(state.Client, req.Model);
                state.Workspace = new Workspace(state.Client, req.Model, pdfDir, cacheDir, mdDir, csvDir);
                state.PdfInjectedIntoSession = null;
                ConsoleEx.Info($"🌐 Web session started with {req.Model} (id: {state.Session.SessionId})");
                return Results.Json(new { model = req.Model, sessionId = state.Session.SessionId });
            }
            catch (Exception ex)
            {
                return Results.Json(new { error = ex.Message }, statusCode: 500);
            }
            finally
            {
                state.Gate.Release();
            }
        });

        app.MapGet("/api/state", () => Results.Json(new
        {
            model = state.Workspace?.Model,
            selectedPdf = state.SelectedPdf is null ? null : Path.GetFileName(state.SelectedPdf),
            hasSession = state.Session != null,
            busy = state.Gate.CurrentCount == 0,
        }));

        // --- PDFs --------------------------------------------------------------

        app.MapGet("/api/pdfs", () => Results.Json(
            Directory.GetFiles(pdfDir, "*.pdf")
                .Select(Path.GetFileName)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)));

        app.MapPost("/api/pdfs/upload", async (HttpContext http) =>
        {
            // Form is read manually (no IFormFile binding) to keep the endpoint
            // free of antiforgery requirements — this is a localhost-only app.
            var form = await http.Request.ReadFormAsync();
            var file = form.Files.Count > 0 ? form.Files[0] : null;
            if (file is null || file.Length == 0)
                return Results.BadRequest(new { error = "No file received." });

            var name = Path.GetFileName(file.FileName);
            if (!name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return Results.BadRequest(new { error = "Only .pdf files are accepted." });

            // Gate the write: an ungated upload could overwrite the very PDF a
            // running pipeline is reading from.
            if (!await state.Gate.WaitAsync(0))
                return Results.Conflict(new { error = "Another operation is running." });
            try
            {
                var target = Path.Combine(pdfDir, name);
                await using (var stream = File.Create(target))
                    await file.CopyToAsync(stream);

                state.SelectedPdf = target;
                // Re-uploading a file with the same name changes its content, so the
                // text already injected into the session no longer matches it.
                if (target.Equals(state.PdfInjectedIntoSession, StringComparison.OrdinalIgnoreCase))
                    state.PdfInjectedIntoSession = null;
                ConsoleEx.Success($"📄 Uploaded: {name}");
                return Results.Json(new { name });
            }
            finally
            {
                state.Gate.Release();
            }
        });

        app.MapPost("/api/select", (SelectRequest req) =>
        {
            var path = Path.Combine(pdfDir, Path.GetFileName(req.Name ?? string.Empty));
            if (!File.Exists(path))
                return Results.BadRequest(new { error = "PDF not found." });
            state.SelectedPdf = path;
            return Results.Json(new { name = Path.GetFileName(path) });
        });

        // --- Chat (SSE token stream) --------------------------------------------

        app.MapPost("/api/chat", async (HttpContext http, ChatRequest req) =>
        {
            if (state.Session is null || state.Workspace is null)
            {
                await WriteJsonErrorAsync(http, 400, "Start a session first (pick a model).");
                return;
            }
            if (string.IsNullOrWhiteSpace(req.Text))
            {
                await WriteJsonErrorAsync(http, 400, "Empty message.");
                return;
            }
            if (!await state.Gate.WaitAsync(0))
            {
                await WriteJsonErrorAsync(http, 409, "Another operation is running.");
                return;
            }

            try
            {
                // First question about a PDF injects its (condensed) text, exactly
                // like the console loop; afterwards the session already holds it.
                var finalMessage = req.Text;
                if (state.SelectedPdf != null && File.Exists(state.SelectedPdf) &&
                    !state.SelectedPdf.Equals(state.PdfInjectedIntoSession, StringComparison.OrdinalIgnoreCase))
                {
                    var (prompt, pdfIncluded) = await CommandHandlers.BuildPromptWithPdfContextAsync(
                        state.Workspace, state.SelectedPdf, req.Text);
                    finalMessage = prompt;
                    if (pdfIncluded)
                        state.PdfInjectedIntoSession = state.SelectedPdf;
                }

                http.Response.ContentType = "text/event-stream";
                http.Response.Headers.CacheControl = "no-cache";

                // SendAsync's callbacks are synchronous; a channel bridges them to
                // the async HTTP response writer. The request-aborted token flows into
                // SendAsync so an abandoned browser tab cancels the model call instead
                // of letting it run (and bill tokens) to completion.
                var channel = Channel.CreateUnbounded<(string Evt, string Data)>();
                var session = state.Session;
                var sendTask = Task.Run(async () =>
                {
                    try
                    {
                        await session.SendAsync(finalMessage,
                            onReasoningDelta: c => channel.Writer.TryWrite(("reasoning", c)),
                            onContentDelta: c => channel.Writer.TryWrite(("token", c)),
                            cancellationToken: http.RequestAborted);
                        channel.Writer.TryWrite(("done", string.Empty));
                    }
                    catch (OperationCanceledException)
                    {
                        // Browser tab closed mid-answer; nothing to report.
                    }
                    catch (Exception ex)
                    {
                        channel.Writer.TryWrite(("error", ex.Message));
                    }
                    finally
                    {
                        channel.Writer.Complete();
                    }
                });

                try
                {
                    await foreach (var (evt, data) in channel.Reader.ReadAllAsync(http.RequestAborted))
                        await WriteSseAsync(http.Response, evt, new { text = data });
                }
                catch (OperationCanceledException)
                {
                    // Browser tab closed mid-answer; nothing to do.
                }
                finally
                {
                    // Whatever happened to the response, wait for the model call to
                    // finish before the outer finally releases the gate — otherwise a
                    // new request could hit the same session concurrently.
                    await sendTask;
                }
            }
            finally
            {
                state.Gate.Release();
            }
        });

        // --- Pipeline commands ---------------------------------------------------

        // The pipelines report failures through ConsoleEx (mirrored to the progress
        // panel) and signal them via their return value: a null output path or false.
        app.MapPost("/api/run/summarize", () => RunGatedAsync(state, needsPdf: true,
            async (ws, pdf) =>
            {
                var output = await TechnologySummarizer.RunAsync(ws, pdf!);
                return (output != null, output);
            }));

        app.MapPost("/api/run/classify", () => RunGatedAsync(state, needsPdf: true,
            async (ws, pdf) =>
            {
                var output = await TechnologyClassifier.RunAsync(ws, pdf!);
                return (output != null, output);
            }));

        app.MapPost("/api/run/condense-check", () => RunGatedAsync(state, needsPdf: true,
            async (ws, pdf) => (await CommandHandlers.HandleCondenseCheckAsync(ws, pdf!), null)));

        app.MapPost("/api/run/benchmark", () => RunGatedAsync(state, needsPdf: false,
            async (ws, _) => (await CommandHandlers.HandleBenchmarkAsync(ws), null)));

        // --- Progress (SSE mirror of ConsoleEx) ----------------------------------

        app.MapGet("/api/progress", async (HttpContext http) =>
        {
            http.Response.ContentType = "text/event-stream";
            http.Response.Headers.CacheControl = "no-cache";

            var channel = Channel.CreateUnbounded<(string Level, string Text)>();
            void Handler(string level, string text) => channel.Writer.TryWrite((level, text));
            ConsoleEx.MessageLogged += Handler;
            try
            {
                await foreach (var (level, text) in channel.Reader.ReadAllAsync(http.RequestAborted))
                    await WriteSseAsync(http.Response, "log", new { level, text });
            }
            catch (OperationCanceledException)
            {
                // Page closed/reloaded; EventSource reconnects on its own.
            }
            finally
            {
                ConsoleEx.MessageLogged -= Handler;
            }
        });

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            ConsoleEx.Info($"🌐 Web UI running at {Url}  (Ctrl+C to stop)");
            try
            {
                Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true });
            }
            catch
            {
                ConsoleEx.Warn($"Could not open the browser automatically — open {Url} yourself.");
            }
        });

        await app.RunAsync(Url);
    }

    // Runs one long pipeline operation with the busy-guard. The action reports
    // success/failure explicitly (the pipelines swallow their own exceptions and
    // log through ConsoleEx), plus an optional output path on success.
    private static async Task<IResult> RunGatedAsync(
        WebAppState state, bool needsPdf, Func<Workspace, string?, Task<(bool Ok, string? Output)>> action)
    {
        if (state.Workspace is not { } ws)
            return Results.BadRequest(new { error = "Start a session first (pick a model)." });
        var pdf = state.SelectedPdf;
        if (needsPdf && (pdf is null || !File.Exists(pdf)))
            return Results.BadRequest(new { error = "No PDF selected. Use the PDF dropdown or upload one." });
        if (!await state.Gate.WaitAsync(0))
            return Results.Conflict(new { error = "Another operation is already running." });
        try
        {
            var (ok, output) = await action(ws, pdf);
            return ok
                ? Results.Json(new { output })
                : Results.Json(new { error = "Operation failed — see the progress log for details." }, statusCode: 500);
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = ex.Message }, statusCode: 500);
        }
        finally
        {
            state.Gate.Release();
        }
    }

    // One SSE frame: "event: <name>\ndata: <json>\n\n". JSON encoding keeps
    // newlines inside chunks from breaking the SSE line protocol.
    private static async Task WriteSseAsync(HttpResponse response, string evt, object payload)
    {
        await response.WriteAsync($"event: {evt}\ndata: {JsonSerializer.Serialize(payload)}\n\n");
        await response.Body.FlushAsync();
    }

    private static async Task WriteJsonErrorAsync(HttpContext http, int statusCode, string error)
    {
        http.Response.StatusCode = statusCode;
        await http.Response.WriteAsJsonAsync(new { error });
    }
}
