﻿using Murder.Assets;
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

                _saveBasePath = FileHelper.GetSaveBasePath(GameDirectory);
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
        private readonly ConcurrentDictionary<Guid, GameAsset> _currentSaveAssets = new();

        private SaveData? _activeSaveData;

        private GamePreferences? _preferences;
        public ImmutableArray<Color> CurrentPalette;

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
        public SaveData? ResetActiveSave()
        {
            if (_activeSaveData is null)
            {
                // No active save data, just dismiss.
                return null;
            }

            int slot = _activeSaveData.SaveSlot;
            string? path = _activeSaveData.GetGameAssetPath();

            UnloadCurrentSave();

            // Load the save from its directory.
            path = Path.GetDirectoryName(path);

            if (path is null || !Directory.Exists(path))
            {
                return null;
            }

            LoadSaveAsCurrentSave(slot);

            return _activeSaveData;
        }

        /// <summary>
        /// List all the available saves within the game.
        /// </summary>
        public Dictionary<int, SaveDataInfo> GetAllSaves() => _allSavedData;

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
                slot = _allSavedData.Count;
            }

            SaveData data = CreateSaveDataWithVersion(slot);
            TrackSaveAsset(data);

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

            string saveDataPath = data.GetFullPackedSavePath(slot);
            string saveDataAssetsPath = data.GetFullPackedAssetsSavePath(slot);
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

        public bool TryGetDynamicAsset<T>([NotNullWhen(true)] out T? asset) where T : DynamicAsset
        {
            asset = _activeSaveData?.TryGetDynamicAsset<T>();

            return asset != null;
        }

        /// <summary>
        /// Retrieve a dynamic asset within the current save data.
        /// If no dynamic asset is found, it creates a new one to the save data.
        /// </summary>
        public T GetDynamicAsset<T>() where T : DynamicAsset, new()
        {
            if (!TryGetDynamicAsset(out T? asset))
            {
                // Create the dynamic asset and add it to the save data.
                asset = new();

                AddAssetForCurrentSave(asset);
                ActiveSaveData.SaveDynamicAsset<T>(asset.Guid);
            }

            return asset;
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
        private bool TrackSaveAsset(SaveData asset)
        {
            if (_allSavedData.ContainsKey(asset.SaveSlot))
            {
                GameLogger.Fail("Adding duplicate save asset?");
                return false;
            }

            GameLogger.Verify(asset.Guid != Guid.Empty);
            GameLogger.Verify(!string.IsNullOrWhiteSpace(asset.Name));
            GameLogger.Verify(!string.IsNullOrWhiteSpace(asset.FilePath));

            _allSavedData.Add(asset.SaveSlot, new(asset.SaveVersion, asset.SaveName));
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

        public async ValueTask<bool> SerializeSaveAsync()
        {
            if (_activeSaveData is null)
            {
                return false;
            }

            _waitPendingSaveTrackerOperation = true;
            Game.Instance.SetWaitForSaveComplete();

            await Task.Yield();

            int slot = _activeSaveData.SaveSlot;
            SaveDataInfo info = _allSavedData[slot];

            using PerfTimeRecorder recorder = new("Serializing save");

            SaveDataTracker tracker = new(_allSavedData);
            string trackerJson = FileManager.SerializeToJson(tracker);

            PackedSaveData packedData = new(_activeSaveData);
            string packedDataJson = FileManager.SerializeToJson(packedData);

            _waitPendingSaveTrackerOperation = false;

            // Wait for any pending operations.
            if (_activeSaveData.PendingOperation is not null)
            {
                await _activeSaveData.PendingOperation;
            }

            using PerfTimeRecorder recorderSerialize = new("Writing save to file");

            PackedSaveAssetsData packedAssetsData = new([.. _currentSaveAssets.Values]);
            string packedAssetsDataJson = FileManager.SerializeToJson(packedAssetsData);

            string packedSavePath = Path.Join(info.GetFullPackedSavePath(slot));
            string packedSaveAssetsPath = Path.Join(info.GetFullPackedAssetsSavePath(slot));

            FileManager.CreateDirectoryPathIfNotExists(packedSavePath);

            await Task.WhenAll(
                FileManager.PackContentAsync(trackerJson, path: Path.Join(Game.Data.SaveBasePath, SaveDataTracker.Name)),
                FileManager.PackContentAsync(packedDataJson, path: packedSavePath),
                FileManager.PackContentAsync(packedAssetsDataJson, packedSaveAssetsPath));

            return true;
        }

        public bool RemoveAssetForCurrentSave(Guid guid)
        {
            if (!_currentSaveAssets.TryGetValue(guid, out GameAsset? asset))
            {
                return false;
            }

            bool removed = _currentSaveAssets.TryRemove(asset.Guid, out _);

            if (asset is DynamicAsset)
            {
                _activeSaveData?.RemoveDynamicAsset(asset.GetType());
            }

            return removed;
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

                string saveDataPath = save.GetFullPackedSavePath(slot);
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

        private bool UnloadCurrentSave()
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

            string path = data.GetFullPackedSaveDirectory(slot);
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