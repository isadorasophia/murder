using Murder.Diagnostics;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Murder.Editor.Utilities
{
    internal static class ShellServices
    {
        private static string GetShell()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/bash";
        }

        private static string GetShellArguments(string command)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? $"/c \"{command}\""
                : $"-c \"{command}\"";
        }

        public static void ExecuteCommand(string command, string workingDirectory)
        {

            var processStartInfo = new ProcessStartInfo
            {
                FileName = GetShell(),
                Arguments = GetShellArguments(command),
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            GameLogger.Log($"$({workingDirectory})>");
            GameLogger.Log(command);

            process.WaitForExit();
            GameLogger.Log(output);

            if (!string.IsNullOrWhiteSpace(error))
                GameLogger.Error("Error: " + error);

            GameLogger.Log($"== DONE ==");
        }
    }
}