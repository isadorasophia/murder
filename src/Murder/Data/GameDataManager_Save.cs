using Murder.Assets;
using Murder.Assets.Save;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Save;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Data
{
    public partial class GameDataManager
    {
        /// <summary>
        /// Creates an implementation of SaveData for the game.
        /// </summary>
        protected virtual SaveData CreateSaveDataWithVersion(int slot)
        {
            SaveData data = _game is not null ? _game.CreateSaveData(slot) : new SaveData(slot, _game?.Version ?? 0);
            data.Initialize();

            return data;
        }

        /// <summary>
        /// Directory used for saving custom data.
        /// </summary>
        public virtual string GameDirectory => _game?.Name ?? "Murder";

        /// <summary>
        /// Directory used for saving custom data.
        /// </summary>
        public virtual string SaveDirectory => _game?.SaveName ?? GameDirectory;

        public virtual float CurrentGameVersion => _game?.Version ?? 1;

        private string? _saveBasePath = null;

        /// <summary>
        /// Save directory path used when serializing user data.
        /// </summary>
        public string SaveBasePath 
        {
            get
            {
                if (_saveBasePath is not null)
                {
                    return _saveBasePath;
                }

                _saveBasePath = FileHelper.GetSaveBasePath(SaveDirectory);
                return _saveBasePath;
            }
        }

        /// <summary>
        /// This is the collection of save data according to the slots.
        /// </summary>
        protected readonly Dictionary<int, SaveDataInfo> _allSavedData = [];

        /// <summary>
        /// Stores all the save assets tied to the current <see cref="ActiveSaveData"/>.
        /// </summary>
        protected readonly ConcurrentDictionary<Guid, GameAsset> _currentSaveAssets = new();
        protected SaveData? _activeSaveData;

        private GamePreferences? _preferences;
        
        /// <summary>
        /// Used by the color picker to allow selecting common colors quicker.
        /// </summary>
        public ImmutableArray<Color> CurrentPalette = [];

        private bool _loadedSaveFiles = false;

        // Fancy variables for keeping track of async operations.
        private volatile bool _waitPendingSaveTrackerOperation = false;

        public bool WaitPendingSaveTrackerOperation 
        {
            get
            {
                if (_activeSaveData is not null && !_activeSaveData.HasFinishedSaveWorld())
                {
                    return true;
                }

                return _waitPendingSaveTrackerOperation;
            }
        }


        /// <summary>
        /// Active saved run in the game.
        /// </summary>
        public SaveData ActiveSaveData
        {
            get
            {
                if (_activeSaveData is null)
                {
                    GameLogger.Error("Unable to acquire an active save data!");
                    throw new InvalidOperationException();
                }

                return _activeSaveData;
            }
        }

        public GamePreferences Preferences
        {
            get
            {
                _preferences ??= GamePreferences.TryFetchPreferences() ??
                    _game?.CreateGamePreferences() ?? new GamePreferences();

                return _preferences;
            }
        }

        /// <summary>
        /// Active saved run in the game.
        /// </summary>
        public SaveData? TryGetActiveSaveData() => _activeSaveData;

        /// <summary>
        /// This resets the active save data.
        /// </summary>
        public bool ResetActiveSave()
        {
            if (_activeSaveData is null)
            {
                // No active save data, just dismiss.
                return false;
            }

            UnloadCurrentSave();
            LoadAllSaves();

            return true;
        }

        /// <summary>
        /// List all the available saves within the game.
        /// </summary>
        public Dictionary<int, SaveDataInfo> GetAllSaves() => _allSavedData;

        /// <summary>
        /// Find the next available save slot.
        /// This is slighly expensive as it takes O(n) to complete, but you're only calling this once
        /// so you're fine.
        /// </summary>
        public int GetNextAvailableSlot()
        {
            int i = 0;

            while (true)
            {
                if (!_allSavedData.ContainsKey(i))
                {
                    // free!
                    return i;
                }

                i++;
            }
        }

        /// <summary>
        /// Create a new save data based on a name.
        /// </summary>
        public virtual SaveData CreateSave(int slot = -1)
        {
            // We will actually wipe any previous saves at this point and create the new one.
            UnloadAllSaves();

            if (slot == -1)
            {
                // No slot was specified, so just get the latest one.
                slot = GetNextAvailableSlot();
            }

            SaveData data = CreateSaveDataWithVersion(slot);
            TrackSaveAssetAsActiveSave(data);

            return data;
        }

        public bool CanLoadSaveData(int slot)
        {
            if (slot == -1)
            {
                return _allSavedData.Count > 0;
            }

            return _allSavedData.ContainsKey(slot);
        }

        /// <summary>
        /// Load a save as the current save. If more than one, it will grab whatever
        /// is first available in all saved data.
        /// </summary>
        public bool LoadSaveAsCurrentSave(int slot)
        {
            if (slot == -1 && _allSavedData.Count > 0)
            {
                slot = _allSavedData.ContainsKey(0) ? 0 : _allSavedData.Keys.First();
            }

            if (!_allSavedData.TryGetValue(slot, out SaveDataInfo? data))
            {
                return false;
            }

            string saveDataPath = SaveDataInfo.GetFullPackedSavePath(slot);
            string saveDataAssetsPath = SaveDataInfo.GetFullPackedAssetsSavePath(slot);
            if (!File.Exists(saveDataPath) || !File.Exists(saveDataAssetsPath))
            {
                return false;
            }

            PackedSaveData? packedData = FileManager.UnpackContent<PackedSaveData>(saveDataPath);
            PackedSaveAssetsData? packedAssetsData = FileManager.UnpackContent<PackedSaveAssetsData>(saveDataAssetsPath);

            if (packedData is null || packedAssetsData is null)
            {
                return false;
            }

            _activeSaveData = packedData.Data;
            LoadAllAssetsForCurrentSave(packedAssetsData.Assets);

            return true;
        }

        public void SaveWorld(MonoWorld world)
        {
            ActiveSaveData.SaveAsync(world);
        }

        /// <summary>
        /// Retrieve a dynamic asset within the current save data based on a guid.
        /// </summary>
        public GameAsset? TryGetAssetForCurrentSave(Guid guid)
        {
            if (_currentSaveAssets.TryGetValue(guid, out GameAsset? asset))
            {
                return asset;
            }

            return default;
        }

        /// <summary>
        /// Create a new save data for the game.
        /// </summary>
        /// <param name="asset">
        /// Save data asset. This might have any custom user settings.
        /// </param>
        /// <returns>
        /// Whether the save succeeded.
        /// </returns>
        private bool TrackSaveAssetAsActiveSave(SaveData asset)
        {
            if (_allSavedData.ContainsKey(asset.SaveSlot))
            {
                GameLogger.Warning("Overwriting save slot...");
            }

            GameLogger.Verify(asset.Guid != Guid.Empty);
            GameLogger.Verify(!string.IsNullOrWhiteSpace(asset.Name));
            GameLogger.Verify(!string.IsNullOrWhiteSpace(asset.FilePath));

            SaveDataInfo info = _game?.CreateSaveDataInfo(asset.SaveVersion, asset.SaveName) ?? 
                new(asset.SaveVersion, asset.SaveName);

            _allSavedData[asset.SaveSlot] = info;
            _activeSaveData = asset;

            return true;
        }

        public bool AddAssetForCurrentSave(GameAsset asset)
        {
            if (_currentSaveAssets.ContainsKey(asset.Guid))
            {
                GameLogger.Fail("Adding duplicate save asset?");
                return false;
            }

            if (asset.Guid == Guid.Empty)
            {
                asset.MakeGuid();
            }

            if (string.IsNullOrWhiteSpace(asset.Name))
            {
                asset.Name = asset.Guid.ToString();
            }

            _currentSaveAssets.TryAdd(asset.Guid, asset);
            return true;
        }

        public ValueTask<bool>? PendingSave = null;

        /// <summary>
        /// Quickly serialize our save assets.
        /// </summary>
        public void QuickSave()
        {
            PendingSave = SerializeSaveAsync();
        }

        protected async ValueTask<bool> SerializeSaveAsync()
        {
            if (PendingSave is not null && !PendingSave.Value.IsCompleted)
            {
                // Save is already in progress.
                return false;
            }

            if (_activeSaveData is null)
            {
                return false;
            }

            try
            {
                _waitPendingSaveTrackerOperation = true;
                Game.Instance.SetWaitForSaveComplete();

                await Task.Yield();

                int slot = _activeSaveData.SaveSlot;

                PerfTimeRecorder saveToJsonRecorder = new("Serializing save");

                // make sure we do any last minute changes...
                _activeSaveData.OnBeforeSave();

                SaveDataTracker tracker = new(_allSavedData);
                string trackerJson = FileManager.SerializeToJson(tracker);

                // Wait for any pending operations.
                if (_activeSaveData.PendingOperation is not null)
                {
                    await _activeSaveData.PendingOperation;
                }

                PackedSaveData packedData = new(_activeSaveData);
                string packedDataJson = FileManager.SerializeToJson(packedData);

                _waitPendingSaveTrackerOperation = false;

                saveToJsonRecorder.Dispose();

                using PerfTimeRecorder recorderSerialize = new("Writing save to file");

                PackedSaveAssetsData packedAssetsData = new([.. _currentSaveAssets.Values]);
                string packedAssetsDataJson = FileManager.SerializeToJson(packedAssetsData);

                string packedSavePath = SaveDataInfo.GetFullPackedSavePath(slot);
                string packedSaveAssetsPath = SaveDataInfo.GetFullPackedAssetsSavePath(slot);

                // before doing anything, let's save a backup, shall we?
                if (File.Exists(packedSavePath) && File.Exists(packedSaveAssetsPath))
                {
                    string backupDirectory = SaveDataInfo.GetFullPackedSaveBackupDirectory(slot);
                    FileManager.GetOrCreateDirectory(backupDirectory);

                    string packedSavePathBackup = Path.Join(backupDirectory, PackedSaveData.Name);
                    File.Copy(packedSavePath, packedSavePathBackup, overwrite: true);

                    string packedSaveAssetsPathBackup = Path.Join(backupDirectory, PackedSaveAssetsData.Name);
                    File.Copy(packedSaveAssetsPath, packedSaveAssetsPathBackup, overwrite: true);
                }

                FileManager.CreateDirectoryPathIfNotExists(packedSavePath);

                await Task.WhenAll(
                    FileManager.PackContentAsync(trackerJson, path: Path.Join(Game.Data.SaveBasePath, SaveDataTracker.Name)),
                    FileManager.PackContentAsync(packedDataJson, path: packedSavePath),
                    FileManager.PackContentAsync(packedAssetsDataJson, packedSaveAssetsPath));

                return true;
            }
            catch (Exception e)
            {
                GameLogger.Error($"An unexpected error occurred while saving: {e.Message}");
                return false;
            }
        }

        public bool RemoveAssetForCurrentSave(Guid guid)
        {
            if (!_currentSaveAssets.TryGetValue(guid, out GameAsset? asset))
            {
                return false;
            }

            return _currentSaveAssets.TryRemove(asset.Guid, out _);
        }

        public bool LoadAllSaves()
        {
            if (_loadedSaveFiles)
            {
                // already loaded, bye.
                return true;
            }

            using PerfTimeRecorder recorder = new("Loading Saves");

            _allSavedData.Clear();
            _loadedSaveFiles = true;

            // Load all the save data assets.
            string trackerPath = Path.Join(SaveBasePath, SaveDataTracker.Name);

            if (!File.Exists(trackerPath))
            {
                return true;
            }

            SaveDataTracker? tracker = FileManager.UnpackContent<SaveDataTracker>(trackerPath);
            if (tracker is null)
            {
                return true;
            }

            foreach ((int slot, SaveDataInfo save) in tracker.Value.Info)
            {
                if (save.Version < _game?.Version)
                {
                    // Skip this info... for now.
                    continue;
                }

                string saveDataPath = SaveDataInfo.GetFullPackedSavePath(slot);
                if (!File.Exists(saveDataPath))
                {
                    continue;
                }

                _allSavedData[slot] = save;
            }

            return true;
        }

        private bool LoadAllAssetsForCurrentSave(List<GameAsset> assets)
        {
            _currentSaveAssets.Clear();

            foreach (GameAsset asset in assets)
            {
                _currentSaveAssets.TryAdd(asset.Guid, asset);
            }

            return true;
        }

        protected bool UnloadCurrentSave()
        {
            if (_activeSaveData is null)
            {
                return false;
            }

            _allSavedData.Remove(_activeSaveData.SaveSlot);
            _currentSaveAssets.Clear();

            _activeSaveData = null;

            _loadedSaveFiles = false;
            return true;
        }

        private bool UnloadSaveAt(int slot)
        {
            SaveData? current = _activeSaveData;
            if (current is not null && current.SaveSlot == slot)
            {
                return UnloadCurrentSave();
            }

            _loadedSaveFiles = false;
            _allSavedData.Remove(slot);

            return true;
        }

        /// <summary>
        /// Used to clear all saves files currently active.
        /// </summary>
        public void UnloadAllSaves()
        {
            _currentSaveAssets.Clear();
            _activeSaveData = null;

            _loadedSaveFiles = false;
        }

        public virtual void DeleteAllSaves()
        {
            UnloadAllSaves();

            _allSavedData.Clear();

            File.Delete(Path.Join(SaveBasePath, SaveDataTracker.Name));
            FileManager.DeleteContent(SaveBasePath, deleteRootFiles: false);
        }

        public virtual bool DeleteSaveAt(int slot)
        {
            if (!_loadedSaveFiles)
            {
                LoadAllSaves();
            }

            if (!_allSavedData.TryGetValue(slot, out SaveDataInfo? data))
            {
                return false;
            }

            string path = SaveDataInfo.GetFullPackedSaveDirectory(slot);
            if (string.IsNullOrEmpty(path))
            {
                FileManager.DeleteContent(SaveBasePath, deleteRootFiles: false);
            }
            else
            {
                FileManager.DeleteDirectoryIfExists(path);
            }

            UnloadSaveAt(slot);

            // Update tracker that a save has just been deleted.
            SaveDataTracker tracker = new(_allSavedData);
            FileManager.PackContent(tracker, path: Path.Join(Game.Data.SaveBasePath, SaveDataTracker.Name));

            return true;
        }
    }
}