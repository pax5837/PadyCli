using System.Diagnostics;

namespace PadyCli.ConsoleApp.Features;

internal class ProcessRunner
{
    public void Run(string command)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe", // Use "cmd.exe" or other CLI if preferred
            Arguments = $"-Command \"{command}\"",
            RedirectStandardOutput = true, // To capture output
            RedirectStandardError = true,  // To capture errors
            UseShellExecute = false,       // Must be false to redirect output
            CreateNoWindow = true          // Run without creating a window
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        // Event handlers to capture output/error data in real-time
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        // Output the results (for debugging purposes)
        System.Console.WriteLine("Output:");
        // System.Console.WriteLine(output);

        // if (!string.IsNullOrWhiteSpace(errors))
        // {
        //     System.Console.WriteLine("Errors:");
        //     System.Console.WriteLine(errors);
        // }
    }
}