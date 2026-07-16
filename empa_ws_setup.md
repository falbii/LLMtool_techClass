Fix — create a real copilot.exe that wraps the .cmd:

# Create a tiny shim project
mkdir "$env:TEMP\copilot-shim"
cd "$env:TEMP\copilot-shim"
dotnet new console --force

# Write the shim
@"
using System.Diagnostics;
var p = new Process();
p.StartInfo = new ProcessStartInfo {
    FileName = "cmd.exe",
    Arguments = "/c copilot.cmd " + string.Join(" ", args),
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    CreateNoWindow = true
};
p.Start();
Console.Write(p.StandardOutput.ReadToEnd());
p.WaitForExit();
return p.ExitCode;
"@ | Out-File Program.cs -Encoding utf8

# Publish as a single exe
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o out

# Copy to npm folder as copilot.exe
Copy-Item "out\copilot-shim.exe" "C:\Users\chyi\AppData\Roaming\npm\copilot.exe"