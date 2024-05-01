using Murder.Diagnostics;
using System.Diagnostics;

namespace Murder.Serialization;

public class EditorFileHelper
{
    public static void OpenFolder(string path)
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        if (!Directory.Exists(path))
        {
            GameLogger.Error(string.Format("{0} Directory does not exist!", path));
        }

        ProcessStartInfo startInfo;

        if (OperatingSystem.IsWindows())
        {
            startInfo = new ProcessStartInfo
            {
                Arguments = path,
                FileName = "explorer.exe"
            };
        }
        else if (OperatingSystem.IsMacOS())
        {
            startInfo = new ProcessStartInfo
            {
                Arguments = $"-R {path}",
                FileName = "open"
            };
        }
        else if (OperatingSystem.IsLinux())
        {
            startInfo = new ProcessStartInfo
            {
                FileName = $"xdg-open",
                Arguments = path,
                UseShellExecute = true,
            };
        }
        else
        {
            GameLogger.Error($"Open a folder in {Environment.OSVersion} has not been implemented.");
            return;
        }

        Process.Start(startInfo);
    }

    /// <summary>
    /// Copies all files from <paramref name="sourceDirectoryPath"/> to <paramref name="destDirectoryPath"/>.
    /// Do not delete existing files.
    /// </summary>
    /// <param name="sourceDirectoryPath">Full path to the source.</param>
    /// <param name="destDirectoryPath">Full path to the destination.</param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static int DirectoryDeepCopy(
        string sourceDirectoryPath,
        string destDirectoryPath)
    {
        GameLogger.Verify(Path.IsPathRooted(sourceDirectoryPath) && Path.IsPathRooted(destDirectoryPath));

        // If the source directory does not exist, throw an exception.
        if (!Directory.Exists(sourceDirectoryPath))
        {
            throw new DirectoryNotFoundException(
                $"Source directory does not exist or could not be found: {sourceDirectoryPath}");
        }

        // If the destination directory does not exist, create it.
        if (!Directory.Exists(destDirectoryPath))
        {
            Directory.CreateDirectory(destDirectoryPath);
        }

        // Create all of the directories
        foreach (string directoryPath in Directory.GetDirectories(sourceDirectoryPath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(directoryPath.Replace(sourceDirectoryPath, destDirectoryPath));
        }

        int totalOfFiles = 0;

        // Copy all the files.
        foreach (string newPath in Directory.GetFiles(sourceDirectoryPath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourceDirectoryPath, destDirectoryPath), true);
            totalOfFiles++;
        }

        return totalOfFiles;
    }

    /// <summary>
    /// This will iterate recursively over all files in <paramref name="path"/> and return
    /// the latest modified date.
    /// </summary>
    public static DateTime? TryGetLastWrite(string path)
    {
        if (File.Exists(path))
        {
            return File.GetLastWriteTime(path);
        }

        if (!Directory.Exists(path))
        {
            return default;
        }

        DirectoryInfo dir = new DirectoryInfo(path);
        DateTime last = dir.LastWriteTime;

        foreach (FileInfo file in dir.EnumerateFiles("*.*", SearchOption.AllDirectories))
        {
            if (file.LastWriteTime > last)
            {
                last = file.LastWriteTime;
            }
        }

        return last;
    }
}
