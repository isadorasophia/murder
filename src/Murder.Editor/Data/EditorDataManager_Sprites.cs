using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Importers;
using Murder.Editor.Utilities;
using Murder.Serialization;
using System.Collections.Immutable;
using System.Reflection;

namespace Murder.Editor.Data;

public partial class EditorDataManager
{
    private Action? _onHotReloadSprites = null;

    public override void TrackOnHotReloadSprite(Action action)
    {
        _onHotReloadSprites += action;
    }

    public override void UntrackOnHotReloadSprite(Action action)
    {
        _onHotReloadSprites -= action;
    }

    public ValueTask ReloadSprites()
    {
        if (LoadContentProgress is not null && !LoadContentProgress.IsCompleted)
        {
            // absolutely do not hot reload while loading.
            return default;
        }

        bool changed = FetchResourcesForImporters(reload: true, skipIfNoChangesFound: true);
        if (!changed)
        {
            return default;
        }

        LoadResourceImporters(reload: true, skipIfNoChangesFound: true);
        _onHotReloadSprites?.Invoke();

        return default;
    }

    /// <summary>
    /// Load all the resource importers with a synchronous implementation.
    /// This requires that <see cref="FetchResourcesForImporters"/> has been called first.
    /// </summary>
    /// <param name="force">
    /// Whether it should force reloading all assets.
    /// </param>
    /// <param name="skipIfNoChangesFound">
    /// Whether it should run if there were no changes. If this value is true and force is true,
    /// this will not reload changes.
    /// </param>
    private void LoadResourceImporters(bool reload, bool skipIfNoChangesFound)
    {
        foreach (ResourceImporter importer in AllImporters)
        {
            if (importer.SupportsAsyncLoading && !reload)
            {
                // Skip any async importers here (reloading doesn't really support async yet).
                continue;
            }

            if (skipIfNoChangesFound && !importer.HasChanges)
            {
                continue;
            }

            _ = importer.LoadStagedContentAsync(reload);
        }
    }

    private void FlushResourceImporters()
    {
        foreach (ResourceImporter importer in AllImporters)
        {
            if (!importer.SupportsAsyncLoading)
            {
                // Skip any async importers here.
                continue;
            }

            importer.Flush();
        }
    }

    /// <summary>
    /// Load all the resource importers with an asynchronous implementation.
    /// This requires that <see cref="FetchResourcesForImporters"/> has been called first.
    /// </summary>
    private async ValueTask LoadResourceImportersAsync(bool reload, bool skipIfNoChangesFound)
    {
        foreach (ResourceImporter importer in AllImporters)
        {
            if (!importer.SupportsAsyncLoading)
            {
                // Skip any sync importers here.
                continue;
            }

            if (skipIfNoChangesFound && !importer.HasChanges)
            {
                continue;
            }

            await importer.LoadStagedContentAsync(reload);
        }
    }

    /// <summary>
    /// Apply any changes made to the sprites with the event manager.
    /// This is called when the event manager had updates (from another machine, usually) 
    /// but the sprites did not.
    /// </summary>
    private void ApplyEventManagerChangesIfNeeded()
    {
        if (TryGetSpriteEventData() is not SpriteEventDataManagerAsset manager)
        {
            return;
        }

        if (manager.GetEditorAssetPath() is not string path)
        {
            return;
        }

        DateTime lastTimeFetched = EditorSettings.LastMetadataImported;
        // something about this is off, so I'll just skip for now.
        //if (File.GetLastWriteTime(path) < lastTimeFetched)
        //{
        //    return;
        //}

        using PerfTimeRecorder recorder = new("Update Sprite Events");

        // Apply all filters tied to the manager.
        foreach ((Guid sprite, SpriteEventData data) in manager.Events)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(sprite) is not SpriteAsset spriteAsset)
            {
                continue;
            }

            foreach ((string animation, Dictionary<int, string> events) in data.Events)
            {
                foreach ((int frame, string message) in events)
                {
                    spriteAsset.AddMessageToAnimationFrame(animation, frame, message);
                }
            }

            foreach ((string animation, HashSet<int> frames) in data.DeletedEvents)
            {
                foreach (int frame in frames)
                {
                    spriteAsset.RemoveMessageFromAnimationFrame(animation, frame);
                }
            }

            if (spriteAsset.FileChanged)
            {
                SaveAsset(spriteAsset);
            }
        }

        EditorSettings.LastMetadataImported = DateTime.Now;
    }

    /// <summary>
    /// Initialize all resources tracked by the importers, if they changed since last import.
    /// </summary>
    private bool FetchResourcesForImporters(bool reload, bool skipIfNoChangesFound)
    {
        // Making sure we have an input directory
        if (!Directory.Exists(FileHelper.GetPath(EditorSettings.GameSourcePath)))
        {
            GameLogger.Warning($"Please specify a valid \"Game Source Path\" in \"Editor Settings\". Unable to find the resources to build the atlas from.");
            return false;
        }

        List<(ResourceImporter importer, ImporterSettingsAttribute filter)> importersWithFilters = new();
        foreach (ResourceImporter importer in AllImporters)
        {
            if (skipIfNoChangesFound && !importer.ShouldRecalculate())
            {
                importer.ClearStage();
                continue;
            }

            if (importer.GetType().GetCustomAttribute<ImporterSettingsAttribute>() is ImporterSettingsAttribute attribute)
            {
                importersWithFilters.Add((importer, attribute));

                // Prepare the importers for the files
                importer.ClearStage();
            }
            else
            {
                GameLogger.Error($"Importer {importer.GetType().Name} is missing an ImporterSettingsAttribute");
            }
        }

        if (importersWithFilters.Count == 0)
        {
            return false;
        }

        bool foundChanges = false;

        DateTime lastTimeFetched = reload ? EditorSettings.LastHotReloadImport : EditorSettings.LastImported;

        string rawResourcesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath);
        foreach (string file in Directory.GetFiles(rawResourcesPath, "*.*", SearchOption.AllDirectories))
        {
            // Skip files that begin with "_".
            if (Path.GetFileName(file).StartsWith('_'))
            {
                continue;
            }

            foreach ((ResourceImporter importer, ImporterSettingsAttribute filter) in importersWithFilters)
            {
                // Check if this file can be imported by current imported

                // Fist, check the extension.
                string extension = Path.GetExtension(file);
                if (!filter.FileExtensions.Contains(extension))
                {
                    continue;
                }

                // Now check the folder filter
                string folder = Path.GetRelativePath(rawResourcesPath, Path.GetDirectoryName(file)!);
                switch (filter.FilterType)
                {
                    case FilterType.All:
                        break;

                    case FilterType.OnlyTheseFolders:
                        if (EditorFileManager.IsPathInsideOf(folder, filter.FilterFolders))
                        {
                            break;
                        }
                        continue;

                    case FilterType.ExceptTheseFolders:
                        if (!EditorFileManager.IsPathInsideOf(folder, filter.FilterFolders))
                        {
                            break;
                        }
                        continue;

                    case FilterType.None:
                        continue;
                }

                bool hasInitializedAtlas =
                    !Directory.Exists(importer.GetSourcePackedPath()) ||
                    !Directory.Exists(importer.GetSourceResourcesPath());

                bool changed = hasInitializedAtlas || File.GetLastWriteTime(file) > lastTimeFetched;
                foundChanges |= changed;

                // If everything is good so far, put it on stage and check for changes
                importer.StageFile(file, changed);
                break;
            }
        }

        if (!foundChanges)
        {
            return false;
        }

        EditorSettings.LastHotReloadImport = DateTime.Now;

        if (!reload)
        {
            EditorSettings.LastImported = DateTime.Now;
            EditorSettings.LastMetadataImported = DateTime.Now;
        }

        SaveAsset(Architect.EditorSettings);
        return true;
    }

    private SpriteEventDataManagerAsset? _instance = null;

    public SpriteEventDataManagerAsset? TryGetSpriteEventData()
    {
        if (_instance is not null)
        {
            return _instance;
        }

        ImmutableDictionary<Guid, GameAsset> assets = Game.Data.FilterAllAssets(typeof(SpriteEventDataManagerAsset));
        if (assets.Count > 1)
        {
            GameLogger.Warning("How did we end up with more than one manager assets?");
        }

        foreach ((_, GameAsset asset) in assets)
        {
            if (asset is SpriteEventDataManagerAsset manager)
            {
                _instance = manager;
                return _instance;
            }
        }

        return null;
    }

    public SpriteEventDataManagerAsset GetOrCreateSpriteEventData()
    {
        if (TryGetSpriteEventData() is SpriteEventDataManagerAsset manager)
        {
            return manager;
        }

        // Otherwise, this means we need to actually create one...
        _instance = new();
        _instance.Name = "_EventManager";

        Architect.EditorData.SaveAsset(_instance);

        return _instance;
    }
}