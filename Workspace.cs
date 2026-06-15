namespace TechClassificationApp;

// App-wide runtime context created once at startup and passed to the command handlers:
// the chat backend connection, the chosen model, and the workspace directory layout.
public sealed record Workspace(
    IChatClient Client,
    string Model,
    string PdfDir,
    string CacheDir,
    string MdDir,
    string CsvDir,
    string BenchmarkDir,
    string CheckDir);
