using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Diagnostics;
using Murder.Editor.CustomEditors;
using Murder.Editor.Utilities.Serialization;
using Murder.Serialization;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.Data;

/// <summary>
/// This is a class that leverages the file system watcher to automatically hot reload resources.
/// </summary>
public partial class EditorDataManager
{
    private readonly object _shadersReloadingLock = new();
    private FileSystemWatcher? _shaderFileSystemWatcher = null;

    public bool ShadersNeedReloading { get; private set; }

    private readonly object _localizationReloadingLock = new();
    private FileSystemWatcher? _localizationFileSystemWatcher = null;

    [MemberNotNullWhen(true, nameof(_shaderFileSystemWatcher))]
    private bool InitializeShaderFileSystemWather()
    {
        if (!EditorSettings.AutomaticallyHotReloadShaderChanges)
        {
            // Do nothing if it's disabled.
            return false;
        }

        string shaderPath = FileHelper.GetPath(
            Path.Join(EditorSettings.RawResourcesPath, GameProfile.ShadersPath, "src"));

        if (!Directory.Exists(shaderPath))
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
        SaveAsset(EditorSettings);

        if (_shaderFileSystemWatcher is null)
        {
            InitializeShaderFileSystemWather();
        }

        if (_shaderFileSystemWatcher is not null)
        {
            _shaderFileSystemWatcher.EnableRaisingEvents = value;
        }
    }

    public void ToggleHotReloadDialogue(bool value)
    {
        EditorSettings.EnableDialogueHotReload = value;
        SaveAsset(EditorSettings);
    }

    [MemberNotNullWhen(true, nameof(_localizationFileSystemWatcher))]
    private bool InitializeLocalizationFileSystemWather()
    {
        if (!EditorSettings.AutomaticallyHotReloadLocalizationChanges)
        {
            // Do nothing if it's disabled.
            return false;
        }

        string path = LocalizationExporter.GetFullRawLocalizationPath();

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        _localizationFileSystemWatcher = new FileSystemWatcher(path);
        _localizationFileSystemWatcher.Changed += SetLocalizationNeedReloading;
        _localizationFileSystemWatcher.EnableRaisingEvents = Architect.EditorSettings.AutomaticallyHotReloadLocalizationChanges;
        
        return true;
    }

    private void SetLocalizationNeedReloading(object sender, FileSystemEventArgs e)
    {
        lock (_localizationReloadingLock)
        {
            if (ReloadLocalization())
            {
                EditorScene? editorScene = Architect.Instance.ActiveScene as EditorScene;
                editorScene?.ReloadEditorsOfType<CharacterEditor>();
            }
        }
    }

    public bool ReloadLocalization()
    {
        if (_localizationFileSystemWatcher is null)
        {
            return false;
        }

        DateTime lastModifiedFileTime = DateTime.MinValue;
        string lastModifiedFile = string.Empty;

        // a bit of a heuristic here, but we'll only update the last file modified.
        string path = _localizationFileSystemWatcher.Path;
        foreach (string file in Directory.GetFiles(path))
        {
            DateTime modifiedTime = File.GetLastWriteTime(file);
            if (modifiedTime > lastModifiedFileTime)
            {
                lastModifiedFileTime = modifiedTime;
                lastModifiedFile = file;
            }
        }

        if (string.IsNullOrEmpty(lastModifiedFile))
        {
            GameLogger.Warning($"Unable to find the last modified file at {path}");
            return false;
        }

        LocalizationAsset? targetAsset = null;

        ImmutableDictionary<Guid, GameAsset> assets = Game.Data.FilterAllAssets(typeof(LocalizationAsset));
        foreach ((Guid g, GameAsset asset) in assets)
        {
            string assetLocalizationPath = LocalizationExporter.GetFullRawLocalizationPath(asset.Name);
            if (string.Equals(lastModifiedFile, assetLocalizationPath, StringComparison.InvariantCultureIgnoreCase))
            {
                targetAsset = asset as LocalizationAsset;
                break;
            }
        }

        if (targetAsset is null)
        {
            GameLogger.Warning($"Unable to find an asset that matches {lastModifiedFile}");
            return false;
        }

        LocalizationExporter.ImportFromCsv(targetAsset);
        GameLogger.Log($"Finished hot reloading localization asset for {targetAsset.Name}!");

        return true;
    }

    public void ToggleHotReloadLocalization(bool value)
    {
        EditorSettings.AutomaticallyHotReloadLocalizationChanges = value;
        SaveAsset(EditorSettings);

        if (_localizationFileSystemWatcher is null)
        {
            InitializeLocalizationFileSystemWather();
        }

        if (_localizationFileSystemWatcher is not null)
        {
            _localizationFileSystemWatcher.EnableRaisingEvents = value;
        }
    }
}
