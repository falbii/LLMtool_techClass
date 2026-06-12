namespace TechClassificationApp;

// The minimal model info the app needs, independent of any provider SDK.
public sealed record ChatModelInfo(string Id, bool SupportsReasoning);

// Provider-neutral chat backend. The app depends only on these two interfaces;
// CopilotChatClient (CopilotChatBackend.cs) is the Copilot SDK implementation,
// and other providers (Anthropic, OpenAI, ...) can be added as new
// implementations without touching the rest of the app.
public interface IChatClient : IAsyncDisposable
{
    Task<IReadOnlyList<ChatModelInfo>> ListModelsAsync();
    Task<IChatSession> CreateSessionAsync(string model);
}

// One conversation with the model. Implementations own whatever per-session
// state their provider needs (server-side session, local message history, ...).
public interface IChatSession : IAsyncDisposable
{
    string SessionId { get; }

    // Sends a prompt and waits for the complete response, which is also returned.
    // onReasoningDelta/onContentDelta fire as tokens arrive; a non-streaming
    // provider may invoke onContentDelta once with the whole text. Throws on
    // provider error; cancelling the token aborts the wait with an
    // OperationCanceledException.
    Task<string> SendAsync(
        string prompt,
        Action<string>? onReasoningDelta = null,
        Action<string>? onContentDelta = null,
        CancellationToken cancellationToken = default);
}
