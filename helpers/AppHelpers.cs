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
    // Raised for every line in addition to the console write, so other UIs
    // (the web progress panel) can mirror pipeline output without the
    // pipeline classes knowing about them. Args: level ("error", "success",
    // "warn", "info", "dim"), text.
    public static event Action<string, string>? MessageLogged;

    public static void Error(string message)   => Write(ConsoleColor.Red, "error", message);
    public static void Success(string message) => Write(ConsoleColor.Green, "success", message);
    public static void Warn(string message)    => Write(ConsoleColor.Yellow, "warn", message);
    public static void Info(string message)    => Write(ConsoleColor.Cyan, "info", message);
    public static void Dim(string message)     => Write(ConsoleColor.DarkGray, "dim", message);

    // Uncolored output that still reaches MessageLogged subscribers — use instead
    // of raw Console.WriteLine for lines the web progress panel should also show.
    public static void Plain(string message)
    {
        Console.WriteLine(message);
        MessageLogged?.Invoke("plain", message);
    }

    // Emit ONLY to MessageLogged subscribers, without writing to the console. Used to
    // mirror spinner labels (e.g. "Condensing part 1/3") to the web progress panel:
    // the spinner already animates that text on the terminal itself, so writing it again
    // would duplicate it there — but the web panel, which only sees MessageLogged, needs
    // it to show the same per-step progress the terminal shows instead of just hanging.
    public static void Emit(string level, string message) => MessageLogged?.Invoke(level, message);

    private static void Write(ConsoleColor color, string level, string message)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
        MessageLogged?.Invoke(level, message);
    }
}
