﻿using System.Text.RegularExpressions;

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

    /// <summary>
    /// Gets the rooted path from a relative one
    /// </summary>
    /// <param name="paths">List of paths which will be joined.</param>
    /// <returns></returns>
    public static string GetPath(params string[] paths)
    {
        var path = Path.Join(paths);

        if (Path.IsPathRooted(path))
        {
            // Already rooted, so yay?
            return path;
        }

        return Path.GetFullPath(Path.Join(Microsoft.Xna.Framework.TitleLocation.Path, path));
    }

    private static string? _osVersion = null;

    /// <summary>
    /// Gets the base path for save files.
    /// </summary>
    public static string GetSaveBasePath(string gameName)
    {
        _osVersion ??= SDL2.SDL.SDL_GetPlatform();

        if (_osVersion.Equals("Windows") || _osVersion.Equals("Mac OS X") ||
            _osVersion.Equals("Linux") || _osVersion.Equals("FreeBSD") ||
            _osVersion.Equals("OpenBSD") || _osVersion.Equals("NetBSD"))
        {
            return Path.Join(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), gameName);
        }

        return SDL2.SDL.SDL_GetPrefPath(null, gameName);
    }
}
