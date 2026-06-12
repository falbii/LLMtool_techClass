using System.Text;
using GitHub.Copilot.SDK;
using Refractored.GitHub.Copilot.SDK.Helpers;

namespace TechClassificationApp;

// IChatClient/IChatSession implementation backed by the GitHub Copilot SDK.
// Every Copilot-specific type the app touches at runtime lives in this file
// (plus the CliChecker prerequisite check in Program.cs).
public sealed class CopilotChatClient : IChatClient
{
    private readonly CopilotClient _client;

    private CopilotChatClient(CopilotClient client) => _client = client;

    public static async Task<CopilotChatClient> ConnectAsync()
    {
        var client = new CopilotClient(new CopilotClientOptions());
        await client.StartAsync();
        return new CopilotChatClient(client);
    }

    public async Task<IReadOnlyList<ChatModelInfo>> ListModelsAsync()
    {
        var models = await _client.ListModelsAsync();
        return models
            .Select(m => new ChatModelInfo(m.Id, m.SupportedReasoningEfforts is { Count: > 0 }))
            .ToArray();
    }

    public async Task<IChatSession> CreateSessionAsync(string model)
    {
        var session = await _client.CreateSessionAsync(new SessionConfig
        {
            Model = model,
            Streaming = true,
            OnPermissionRequest = PermissionHandler.ApproveAll
        });
        return new CopilotChatSession(session);
    }

    public ValueTask DisposeAsync() => _client.DisposeAsync();
}

internal sealed class CopilotChatSession(CopilotSession session) : IChatSession
{
    public string SessionId => session.SessionId;

    public async Task<string> SendAsync(
        string prompt,
        Action<string>? onReasoningDelta = null,
        Action<string>? onContentDelta = null,
        CancellationToken cancellationToken = default)
    {
        var done = new TaskCompletionSource();
        var response = new StringBuilder();
        var hasDelta = false;

        var subscription = session.On(evt =>
        {
            switch (evt)
            {
                case AssistantReasoningDeltaEvent reasoningDelta:
                    onReasoningDelta?.Invoke(reasoningDelta.Data.DeltaContent ?? string.Empty);
                    break;

                case AssistantMessageDeltaEvent delta:
                    // hasDelta prevents double-appending when the SDK fires both delta
                    // and full-message events.
                    hasDelta = true;
                    var chunk = delta.Data.DeltaContent ?? string.Empty;
                    response.Append(chunk);
                    onContentDelta?.Invoke(chunk);
                    break;

                case AssistantMessageEvent msg:
                    // Non-streaming models deliver the whole message at once.
                    if (!hasDelta)
                    {
                        var content = msg.Data.Content ?? string.Empty;
                        response.Append(content);
                        onContentDelta?.Invoke(content);
                    }
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
            // The SDK's SendAsync has no cancellation token; cancellation is injected by
            // failing the TCS, which also covers a hang inside SendAsync itself.
            using var cancelRegistration = cancellationToken.Register(
                () => done.TrySetCanceled(cancellationToken));

            var sendTask = session.SendAsync(new MessageOptions { Prompt = prompt });
            if (await Task.WhenAny(sendTask, done.Task) == sendTask)
                await sendTask; // propagate any send failure before waiting on the response events

            await done.Task;
            return response.ToString();
        }
        finally
        {
            subscription.Dispose();
        }
    }

    public ValueTask DisposeAsync() => session.DisposeAsync();
}
