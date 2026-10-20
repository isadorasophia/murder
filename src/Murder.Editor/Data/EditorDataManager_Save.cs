using Murder.Assets;
using Murder.Assets.Save;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace Murder.Editor.Data;

public partial class EditorDataManager
{
    private readonly Dictionary<Guid, SaveDataAssetInfo> _saveAssetsForEditor = new();
    private ImmutableArray<GameAsset>? _cachedSaveAssets = null;

    public ImmutableArray<GameAsset> GetAllSaveAssets()
    {
        if (_cachedSaveAssets is null)
        {
            var builder = ImmutableArray.CreateBuilder<GameAsset>();
            foreach ((Guid g, SaveDataAssetInfo info) in _saveAssetsForEditor)
            {
                builder.Add(info.Asset);
            }

            _cachedSaveAssets = builder.ToImmutable();
        }

        return _cachedSaveAssets.Value;
    }

    public int GetSlotForSaveAsset(Guid g)
    {
        if (_saveAssetsForEditor.TryGetValue(g, out SaveDataAssetInfo dataInfo))
        {
            return dataInfo.SaveSlot;
        }

        return -1;
    }

    /// <summary>
    /// Optional hook method for notifications when a file is dropped to the game.
    /// </summary>
    /// <param name="path">Target path that has been dropped.</param>
    public void OnFileDrop(string path)
    {
        if (!path.EndsWith(".zip"))
        {
            GameLogger.Log($"We can't really do anything by dropping a file {path}.");
            return;
        }

        // Try to import the save, if this is the case.
        _ = ExtractAndImportSaveZip(path);
    }

    private async ValueTask ExtractAndImportSaveZip(string path)
    {
        int slot = Game.Data.GetNextAvailableSlot();

        // Let's start by extracting to the directory.
        string destinationPath = Path.Join(Game.Data.SaveBasePath, $"{slot}");
        Serialization.FileManager.CreateDirectoryPathIfNotExists(destinationPath);

        using ZipArchive stream = ZipFile.OpenRead(path);
        stream.ExtractToDirectory(destinationPath);

        bool success = await Architect.EditorData.ImportExistingSaveAsync(slot);
        if (!success)
        {
            // Nevermind, undo the directory.
            Directory.Delete(destinationPath, recursive: true);
            return;
        }

        GameLogger.Log($"Successfully imported save on slot {slot}!");
    }

    /// <summary>
    /// Track a save file and serialize the tracker to track this change.
    /// </summary>
    private async ValueTask<bool> ImportExistingSaveAsync(int slot)
    {
        string destinationPath = Path.Join(Game.Data.SaveBasePath, $"{slot}");

        string saveDataPath = Path.Combine(destinationPath, SaveDataInfo.GetFullPackedSavePath(slot));
        if (!File.Exists(saveDataPath))
        {
            GameLogger.Error("Invalid save data file. Zip file is not supported!");
            return false;
        }

        PackedSaveData? packedSaveData = Game.Data.FileManager.UnpackContent<PackedSaveData>(saveDataPath);
        if (packedSaveData is null)
        {
            GameLogger.Error("Unsupported save data file! Undoing the work to extract it.");
            return false;
        }

        SaveData saveData = packedSaveData.Data;

        if (saveData.SaveVersion < _game?.Version)
        {
            GameLogger.Error("Save version is not supported. If we load that, it will very likely break.");
            return false;
        }

        if (saveData.SaveSlot != slot)
        {
            // Cheat our way into modifying the save slot.
            FieldInfo? fSaveSlot = typeof(SaveData).GetField(nameof(SaveData.SaveSlot));
            if (fSaveSlot is null)
            {
                return false;
            }

            fSaveSlot.SetValue(saveData, slot);
        }

        _runtimeSaveSlots.Add(slot, new SaveDataInfo(saveData.SaveVersion, saveData.SaveName));

        // Immediately serialize the tracker and the save file.
        string packedDataJson = Serialization.FileManager.SerializeToJson(packedSaveData);

        await Task.WhenAll(
            SerializeSaveTrackerAsync(),
            FileManager.PackContentAsync(packedDataJson, path: SaveDataInfo.GetFullPackedSavePath(slot)));

        await LoadAllSaveAssets();

        return true;
    }

    /// <summary>
    /// List of save states (tracked by Guid) that can be loaded on a new game.
    /// </summary>
    public (Guid Guid, string Name)[] GetAllAvailableStartGameFrom() => Architect.Game?.GetAllAvailableStartGameFrom() ?? [];

    private async Task LoadAllSaveAssets()
    {
        _saveAssetsForEditor.Clear();

        await Task.Yield();

        using PerfTimeRecorder recorder = new("Loading Saves (for editor)");

        SaveDataTracker tracker = Tracker;
        foreach ((int slot, SaveDataInfo save) in tracker.Data)
        {
            string saveDataPath = SaveDataInfo.GetFullPackedSavePath(slot);
            string saveDataAssetsPath = SaveDataInfo.GetFullPackedAssetsSavePath(slot);
            if (!File.Exists(saveDataPath) || !File.Exists(saveDataAssetsPath))
            {
                continue;
            }

            if (save.Version < Game.Data.CurrentGameVersion)
            {
                GameLogger.Warning($"Skipping loading save {save.Name} due to older version ({save.Version}).");
                continue;
            }

            PackedSaveData? packedData = FileManager.UnpackContent<PackedSaveData>(saveDataPath);
            PackedSaveAssetsData? packedAssetsData = null;

            try
            {
                packedAssetsData = FileManager.UnpackContent<PackedSaveAssetsData>(saveDataAssetsPath);
            }
            catch (JsonException)
            {
                await ConvertSave(save, saveDataAssetsPath);
                packedAssetsData = FileManager.UnpackContent<PackedSaveAssetsData>(saveDataAssetsPath);
            }

            if (packedData is null || packedAssetsData is null)
            {
                continue;
            }

            _saveAssetsForEditor[packedData.Data.Guid] = new(slot, packedData.Data);

            foreach (GameAsset asset in packedAssetsData.Assets)
            {
                _saveAssetsForEditor[asset.Guid] = new(slot, asset);
            }
        }

        _cachedSaveAssets = null;
    }

    private Task ConvertSave(SaveDataInfo info, string path) => _game?.ConvertSave(info, path) ?? Task.CompletedTask;

    public override void DeleteAllSaves()
    {
        _saveAssetsForEditor.Clear();
        _cachedSaveAssets = null;

        base.DeleteAllSaves();
    }

    /// <summary>
    /// This is somewhat of an odd method. This mocks packaging the save file in order to save
    /// an asset that has been edited through the editor.
    /// </summary>
    /// <param name="asset">Target asset to be saved (or deleted).</param>
    public async ValueTask SerializeSaveForAssetAsync(GameAsset asset)
    {
        if (PendingSave is not null && !PendingSave.Value.IsCompleted)
        {
            // Save is already in progress.
            return;
        }

        SaveDataTracker tracker = Tracker;
        foreach ((int slot, SaveDataInfo save) in tracker.Data)
        {
            string saveDataPath = SaveDataInfo.GetFullPackedSavePath(slot);
            string saveDataAssetsPath = SaveDataInfo.GetFullPackedAssetsSavePath(slot);
            if (!File.Exists(saveDataPath) || !File.Exists(saveDataAssetsPath))
            {
                continue;
            }

            PackedSaveData? packedData = FileManager.UnpackContent<PackedSaveData>(saveDataPath);
            PackedSaveAssetsData? packedAssetsData = FileManager.UnpackContent<PackedSaveAssetsData>(saveDataAssetsPath);
            if (packedData is null || packedAssetsData is null)
            {
                continue;
            }

            bool match = false;
            if (asset.Guid == packedData.Data.Guid)
            {
                match = true;

                if (asset.TaggedForDeletion)
                {
                    GameLogger.Warning("Unable to delete the root save data. At least, this was not implemented yet.");
                }
            }

            // Override the save slot, which came from the editor data.
            _runtimeSaveSlots[slot] = save;

            if (!_saveAssetsForEditor.TryGetValue(packedData.Data.Guid, out SaveDataAssetInfo saveDataInfo))
            {
                GameLogger.Error($"Why has save slot {slot} not loaded in the editor?");
                continue;
            }

            // start by mocking as if the save was loaded.
            _activeSaveData = (SaveData)saveDataInfo.Asset;
            _currentSaveAssets.Clear();

            foreach (GameAsset packedAsset in packedAssetsData.Assets)
            {
                if (packedAsset.Guid == asset.Guid)
                {
                    match = true;
                }

                if (asset.TaggedForDeletion)
                {
                    _saveAssetsForEditor.Remove(asset.Guid);
                    continue;
                }

                // load the asset on runtime
                if (packedAsset.Guid == asset.Guid)
                {
                    _saveAssetsForEditor[packedAsset.Guid] = new(slot, asset);
                }

                // TODO: Temporarily here while we're porting old saves
                //if (_saveAssetsForEditor[packedAsset.Guid].Asset is SavedWorld savedWorld &&
                //    savedWorld.WorldGuid == Guid.Empty)
                //{
                //    foreach ((Guid worldGuid, Guid savedAssetGuid) in _activeSaveData.SavedWorlds)
                //    {
                //        if (savedAssetGuid == packedAsset.Guid)
                //        {
                //            // opsie... let's do this in secret, I really don't wanna make this not readonly just for a workaround.
                //            FieldInfo? worldReferenceGuid = typeof(SavedWorld).GetField("_worldReferenceGuid", BindingFlags.NonPublic | BindingFlags.Instance);
                //            if (worldReferenceGuid is not null)
                //            {
                //                worldReferenceGuid.SetValue(savedWorld, worldGuid);
                //            }
                //        }
                //    }
                //}

                _currentSaveAssets[packedAsset.Guid] = _saveAssetsForEditor[packedAsset.Guid].Asset;
            }

            _cachedSaveAssets = null;

            if (match)
            {
                // cool! we found it! so serialize it.
                PendingSave = SerializeSaveAsync(overridePath: null);
                await PendingSave.Value;

                asset.FileChanged = false;
                break;
            }
        }

        _cachedSaveAssets = null;

        _activeSaveData = null;
        _currentSaveAssets.Clear();
    }

    public void ResetSaveAsset(GameAsset assetToBeReverted)
    {
        int slot = GetSlotForSaveAsset(assetToBeReverted.Guid);
        if (slot == -1)
        {
            GameLogger.Error($"Unable to find valid save slot for {assetToBeReverted.Guid}.");
            return;
        }

        string saveDataPath = SaveDataInfo.GetFullPackedSavePath(slot);
        string saveDataAssetsPath = SaveDataInfo.GetFullPackedAssetsSavePath(slot);
        if (!File.Exists(saveDataPath) || !File.Exists(saveDataAssetsPath))
        {
            GameLogger.Error($"Unable to find valid save slot {slot}.");
            return;
        }

        PackedSaveAssetsData? packedAssetsData = FileManager.UnpackContent<PackedSaveAssetsData>(saveDataAssetsPath);
        if (packedAssetsData is null)
        {
            GameLogger.Error($"Unable to load save slot {slot}.");
            return;
        }

        foreach (GameAsset packedAsset in packedAssetsData.Assets)
        {
            if (packedAsset.Guid == assetToBeReverted.Guid)
            {
                // found it!
                _saveAssetsForEditor[packedAsset.Guid] = new(slot, packedAsset);
                _cachedSaveAssets = null;

                if (Architect.Instance.ActiveScene is EditorScene editor)
                {
                    editor.OpenAssetEditor(packedAsset, true);
                }

                _activeSaveData = null;
                _currentSaveAssets.Clear();

                return;
            }
        }

        GameLogger.Error($"Unable to find packed save asset for {assetToBeReverted.Guid}.");
    }
}