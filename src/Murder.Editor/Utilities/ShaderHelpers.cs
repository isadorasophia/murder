using Murder.Diagnostics;
using Murder.Serialization;
using System.Runtime.InteropServices;

namespace Murder.Editor.Utilities;

internal static class ShaderHelpers
{
    private static string? _fxcPathCached = null;

    /// <summary>
    /// Microsoft's licensing terms prevent anyone from distributing the files directly, the only way to distribute
    /// them is to run the installer, that's also the only supported method from Microsoft to check that the correct 
    /// version installed. 
    /// 
    /// This means that we can only find valid fxc.exe executables if the user has either:
    ///   - Custom path defined in editor settings
    ///   - Installed DirectX 9 tools: https://www.microsoft.com/en-us/download/details.aspx?id=6812
    ///   - A valid Windows SDK with the tool
    /// </summary>
    /// <returns></returns>
    public static string? ProbeFxcPath()
    {
        if (_fxcPathCached is null)
        {
            _fxcPathCached = InitializeFxcPath();
        }

        return _fxcPathCached;
    }

    private static string? InitializeFxcPath()
    {
        string? editorSettingsFullPath = Architect.EditorSettings.FxcPath is null ?
            null : Path.Join(FileHelper.GetPath(Architect.EditorSettings.FxcPath), "fxc.exe");

        if (!string.IsNullOrEmpty(editorSettingsFullPath) && File.Exists(editorSettingsFullPath))
        {
            // User has defined a custom path.
            return editorSettingsFullPath;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            GameLogger.Error("Unable to find a fxc.exe for non-windows platforms, see Editor Settings (FxcPath). We need to implement this, actually!");
            return null;
        }

        // This assumes we are on a x64 platform.

        const string HardcodedDirextX9Path = "C:\\Program Files (x86)\\Microsoft DirectX SDK (June 2010)\\Utilities\\bin\\x64\\fxc.exe";
        if (File.Exists(HardcodedDirextX9Path))
        {
            return HardcodedDirextX9Path;
        }

        const string WindowsSDKPath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\";
        foreach (string file in Directory.GetFiles(WindowsSDKPath, "fxc.exe"))
        {
            if (file.Contains("\\x64\\"))
            {
                return file;
            }
        }

        return null;
    }
}
