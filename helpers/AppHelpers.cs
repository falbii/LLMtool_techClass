namespace TechClassificationApp;

// Single place that defines how sessions are created, so a policy change (or a different
// backend) happens once instead of at 8 call sites. Provider-specific session config
// (streaming, permissions, ...) lives in the IChatClient implementation.
public static class Sessions
{
    public static Task<IChatSession> NewAsync(IChatClient client, string model) =>
        client.CreateSessionAsync(model);
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
