using Murder.Core;
using Murder.Serialization;
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Murder.Editor.Data;

/// <summary>
/// This is a class that leverages the file system watcher to automatically hot reload resources.
/// </summary>
public partial class EditorDataManager
{
    private readonly object _shadersReloadingLock = new();

    public bool ShadersNeedReloading { get; private set; }

    private FileSystemWatcher? _shaderFileSystemWatcher = null;

    [MemberNotNullWhen(true, nameof(_shaderFileSystemWatcher))]
    private bool InitializeShaderFileSystemWather()
    {
        if (!EditorSettings.AutomaticallyHotReloadShaderChanges)
        {
            // Do nothing if it's disabled.
            return false;
        }

        string shaderPath = FileManager.GetPath(
            Path.Join(EditorSettings.RawResourcesPath, GameProfile.ShadersPath, "src"));

        if (!FileHelper.Exists(shaderPath))
        {
            return false;
        }
        _shaderFileSystemWatcher = new FileSystemWatcher(shaderPath);

        _shaderFileSystemWatcher.Changed += SetShadersNeedReloading;
        _shaderFileSystemWatcher.Renamed += SetShadersNeedReloading;
        _shaderFileSystemWatcher.Created += SetShadersNeedReloading;

        _shaderFileSystemWatcher.EnableRaisingEvents = Architect.EditorSettings.AutomaticallyHotReloadShaderChanges;

        return true;
    }

    /// <summary>
    /// Reload shaders. This CANNOT be called while drawing! Or it will crash!
    /// </summary>
    public void ReloadShaders()
    {
        LoadShaders(breakOnFail: false, forceReload: true);
        InitShaders();

        ShadersNeedReloading = false;

        Architect.Instance.RefreshWindowsBufferAfterReloadingShaders();
    }

    private void SetShadersNeedReloading(object sender, FileSystemEventArgs e)
    {
        lock (_shadersReloadingLock)
        {
            ShadersNeedReloading = true;
        }
    }

    public void ToggleHotReloadShader(bool value)
    {
        EditorSettings.AutomaticallyHotReloadShaderChanges = value;

        if (_shaderFileSystemWatcher is null)
        {
            InitializeShaderFileSystemWather();
        }

        if (_shaderFileSystemWatcher is not null)
        {
            _shaderFileSystemWatcher.EnableRaisingEvents = value;
        }
    }
}
