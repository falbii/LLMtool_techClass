using GitHub.Copilot.SDK;

namespace TechClassificationApp;

// App-wide runtime context created once at startup and passed to the command handlers:
// the Copilot connection, the chosen model, and the workspace directory layout.
public sealed record Workspace(
    CopilotClient Client,
    string Model,
    string PdfDir,
    string CacheDir,
    string TxtDir,
    string CsvDir);
