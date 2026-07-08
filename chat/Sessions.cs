namespace TechClassificationApp;

// Single place that defines how sessions are created, so a policy change (or a different
// backend) happens once instead of at 8 call sites. Provider-specific session config
// (streaming, permissions, ...) lives in the IChatClient implementation.
public static class Sessions
{
    public static Task<IChatSession> NewAsync(IChatClient client, string model) =>
        client.CreateSessionAsync(model);
}
