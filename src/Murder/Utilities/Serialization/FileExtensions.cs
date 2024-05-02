using System.Text.RegularExpressions;

namespace Murder.Serialization;

public static partial class FileHelper
{
    public static string EscapePath(this string path)
    {
        return path
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Returns the assembly extension for the target operating system.
    /// For example, if targeting Windows, this returns ".dll".
    /// </summary>
    /// <returns>A dot followed by the extension string.</returns>
    /// <exception cref="PlatformNotSupportedException">
    /// If the current operating has not been implemented yet.
    /// </exception>
    public static string ExtensionForOperatingSystem()
    {
        if (OperatingSystem.IsWindows())
        {
            return ".dll";
        }
        else if (OperatingSystem.IsLinux())
        {
            return ".so";
        }
        else if (OperatingSystem.IsMacOS())
        {
            return ".dylib";
        }

        throw new PlatformNotSupportedException("Consoles are not supported yet?");
    }

    [GeneratedRegex("[^a-zA-Z0-9\\/\\\\_ -]")]
    private static partial Regex CleanAssetNameRegex();

    public static ReadOnlySpan<char> Clean(string str)
    {
        Regex rgx = CleanAssetNameRegex();
        return rgx.Replace(str, "").EscapePath();
    }

    public static string GetPathWithoutExtension(string path)
    {
        return Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
    }
}
