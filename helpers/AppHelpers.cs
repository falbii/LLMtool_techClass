using GitHub.Copilot.SDK;
using Refractored.GitHub.Copilot.SDK.Helpers;

namespace TechClassificationApp;

// Single place that defines how sessions are created, so streaming/permission policy
// changes happen once instead of at 8 call sites.
public static class Sessions
{
    public static Task<CopilotSession> NewAsync(CopilotClient client, string model) =>
        client.CreateSessionAsync(new SessionConfig
        {
            Model = model,
            Streaming = true,
            OnPermissionRequest = PermissionHandler.ApproveAll
        });
}

// Color-coded console output helpers. Replace the repeated
// "ForegroundColor = X; WriteLine(...); ResetColor()" triplet.
public static class ConsoleEx
{
    public static void Error(string message)   => Write(ConsoleColor.Red, message);
    public static void Success(string message) => Write(ConsoleColor.Green, message);
    public static void Warn(string message)    => Write(ConsoleColor.Yellow, message);
    public static void Info(string message)    => Write(ConsoleColor.Cyan, message);
    public static void Dim(string message)     => Write(ConsoleColor.DarkGray, message);

    private static void Write(ConsoleColor color, string message)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
