namespace TechClassificationApp;

// App-wide runtime context created once at startup and passed to the command handlers:
// the chat backend connection, the chosen model, and the workspace directory layout.
//   PdfDir           01_input/11_pdf_to_analyze          input PDFs
//   CacheDir         01_input/12_condensed_md            condensed .md cache (regenerable)
//   TechListDir      01_input/13_technology_list_md      frozen per-PDF technology lists
//   MdDir            02_output/21_tech_summary_md        summarize output
//   CsvDir           02_output/22_tech_classification_csv classify output
//   BenchmarkDir     02_output/23_validation/benchmark   benchmark results
//   CheckDir         02_output/23_validation/condensed_md_check      condense fidelity reports
//   ClassifyCheckDir 02_output/23_validation/classification_csv_check classify verification reports
public sealed record Workspace(
    IChatClient Client,
    string Model,
    string PdfDir,
    string CacheDir,
    string TechListDir,
    string MdDir,
    string CsvDir,
    string BenchmarkDir,
    string CheckDir,
    string ClassifyCheckDir);
