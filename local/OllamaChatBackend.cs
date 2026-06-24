using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechClassificationApp;

// IChatClient/IChatSession implementation backed by a local Ollama server
// (https://ollama.com). Lets the app run fully offline against locally pulled
// models instead of GitHub Copilot. Selected at startup with the --local (or
// --ollama) flag; the rest of the app only ever sees the provider-neutral
// IChatClient interface, exactly like CopilotChatClient.
public sealed class OllamaChatClient : IChatClient
{
    // Default Ollama endpoint, overridable via OLLAMA_HOST (the same variable the
    // official Ollama CLI and client libraries honor).
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("OLLAMA_HOST") is { Length: > 0 } host
            ? Normalize(host)
            : "http://localhost:11434";

    private readonly HttpClient _http;

    private OllamaChatClient(HttpClient http) => _http = http;

    public static async Task<OllamaChatClient> ConnectAsync()
    {
        var http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        // No client-side timeout: chat completions stream for as long as the model
        // takes to generate (per-call cancellation flows through the token instead).
        http.Timeout = Timeout.InfiniteTimeSpan;

        // Fail fast with a clear message if the server isn't up, rather than letting
        // the first model call surface a raw socket error much later.
        try
        {
            using var ping = await http.GetAsync("/api/tags");
            ping.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            http.Dispose();
            throw new InvalidOperationException(
                $"Could not reach Ollama at {BaseUrl}. Is it running? Start it with 'ollama serve'. ({ex.Message})");
        }

        ConsoleEx.Dim($"   Ollama sampling: num_ctx={OllamaChatSession.NumCtx}, " +
            $"temperature={OllamaChatSession.Temperature.ToString(CultureInfo.InvariantCulture)}, seed={OllamaChatSession.Seed} " +
            "(override with OLLAMA_NUM_CTX / OLLAMA_TEMPERATURE / OLLAMA_SEED)");
        return new OllamaChatClient(http);
    }

    public async Task<IReadOnlyList<ChatModelInfo>> ListModelsAsync()
    {
        var tags = await _http.GetFromJsonAsync<TagsResponse>("/api/tags");
        return (tags?.Models ?? [])
            // Ollama's tag listing doesn't advertise reasoning support, so it's left
            // false; thinking tokens still surface at chat time if the model emits them.
            .Select(m => new ChatModelInfo(m.Name, SupportsReasoning: false))
            .ToArray();
    }

    public Task<IChatSession> CreateSessionAsync(string model) =>
        Task.FromResult<IChatSession>(new OllamaChatSession(_http, model));

    public ValueTask DisposeAsync()
    {
        _http.Dispose();
        return ValueTask.CompletedTask;
    }

    // OLLAMA_HOST may be a bare "host:port" or a full URL; normalize to a URL.
    private static string Normalize(string host) =>
        host.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? host : $"http://{host}";

    private sealed record TagsResponse(
        [property: JsonPropertyName("models")] IReadOnlyList<TagModel>? Models);

    private sealed record TagModel(
        [property: JsonPropertyName("name")] string Name);
}

// One conversation against Ollama. /api/chat is stateless, so the session owns the
// running message history and resends it on every turn.
internal sealed class OllamaChatSession(HttpClient http, string model) : IChatSession
{
    // Ollama defaults num_ctx to a small window (~2k–4k tokens) and *silently truncates*
    // anything longer. Our prompts prepend a whole (condensed) PDF, which easily exceeds
    // that — so without a larger context the model never sees the document. Default to a
    // generous window; override with OLLAMA_NUM_CTX if a model/RAM needs something smaller.
    internal static readonly int NumCtx =
        int.TryParse(Environment.GetEnvironmentVariable("OLLAMA_NUM_CTX"), out var n) && n > 0
            ? n
            : 65536;

    // Determinism levers. The pipeline is an extraction task (one right answer per cell), so
    // greedy decoding (temperature 0) is the correct default, not a creativity penalty. A fixed
    // seed pins the remaining run-to-run jitter for reproducible tables — note this buys
    // repeatability, NOT accuracy: a pinned seed reproduces a wrong cell as faithfully as a right
    // one. Both are overridable (OLLAMA_TEMPERATURE / OLLAMA_SEED) for stability probing.
    internal static readonly double Temperature =
        double.TryParse(Environment.GetEnvironmentVariable("OLLAMA_TEMPERATURE"),
            NumberStyles.Float, CultureInfo.InvariantCulture, out var t) && t >= 0
            ? t
            : 0.0;

    internal static readonly int Seed =
        int.TryParse(Environment.GetEnvironmentVariable("OLLAMA_SEED"), out var s)
            ? s
            : 0;

    public string SessionId { get; } = Guid.NewGuid().ToString("N");

    private readonly List<Message> _history = [];

    public async Task<string> SendAsync(
        string prompt,
        Action<string>? onReasoningDelta = null,
        Action<string>? onContentDelta = null,
        CancellationToken cancellationToken = default)
    {
        _history.Add(new Message("user", prompt));

        using var httpReq = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(
                new ChatRequest(model, _history, Stream: true, new ChatOptions(NumCtx, Temperature, Seed))),
        };

        using var resp = await http.SendAsync(
            httpReq, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Ollama chat request failed ({(int)resp.StatusCode}): {body}");
        }

        var content = new StringBuilder();
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        // /api/chat streams newline-delimited JSON, one object per chunk.
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var chunk = JsonSerializer.Deserialize<ChatStreamChunk>(line);
            if (chunk?.Error is { Length: > 0 } err)
                throw new InvalidOperationException($"Ollama error: {err}");

            if (chunk?.Message is { } msg)
            {
                // Reasoning models populate "thinking" when thinking output is enabled;
                // route it to the reasoning callback, the rest to the content callback.
                if (!string.IsNullOrEmpty(msg.Thinking))
                    onReasoningDelta?.Invoke(msg.Thinking);
                if (!string.IsNullOrEmpty(msg.Content))
                {
                    content.Append(msg.Content);
                    onContentDelta?.Invoke(msg.Content);
                }
            }

            if (chunk?.Done == true)
                break;
        }

        var full = content.ToString();
        _history.Add(new Message("assistant", full));
        return full;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private sealed record ChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] IReadOnlyList<Message> Messages,
        [property: JsonPropertyName("stream")] bool Stream,
        [property: JsonPropertyName("options")] ChatOptions Options);

    private sealed record ChatOptions(
        [property: JsonPropertyName("num_ctx")] int NumCtx,
        [property: JsonPropertyName("temperature")] double Temperature,
        [property: JsonPropertyName("seed")] int Seed);

    private sealed record Message(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private sealed record ChatStreamChunk(
        [property: JsonPropertyName("message")] StreamMessage? Message,
        [property: JsonPropertyName("done")] bool Done,
        [property: JsonPropertyName("error")] string? Error);

    private sealed record StreamMessage(
        [property: JsonPropertyName("content")] string? Content,
        [property: JsonPropertyName("thinking")] string? Thinking);
}
