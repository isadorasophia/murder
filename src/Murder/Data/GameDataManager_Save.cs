using Murder.Assets;
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
        protected virtual SaveData CreateSaveDataWithVersion(int slot) =>
            _game is not null ? _game.CreateSaveData(slot) : new SaveData(slot, _game?.Version ?? 0);

        /// <summary>
        /// Directory used for saving custom data.
        /// </summary>
        public virtual string GameDirectory => _game?.Name ?? "Murder";

        /// <summary>
        /// Save directory path used when serializing user data.
        /// </summary>
        public static string SaveBasePath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Game.Data.GameDirectory);

        /// <summary>
        /// This is the collection of save data according to the slots.
        /// </summary>
        protected readonly Dictionary<int, SaveData> _allSavedData = [];

        /// <summary>
        /// Stores all the save assets tied to the current <see cref="ActiveSaveData"/>.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, GameAsset> _currentSaveAssets = new();

        /// <summary>
        /// Stores all pending assets which will be removed on serializing the save.
        /// </summary>
        private readonly List<string> _pendingAssetsToDeleteOnSerialize = new();

        private SaveData? _activeSaveData;

        private GamePreferences? _preferences;
        public ImmutableArray<Color> CurrentPalette;

        private bool _loadedSaveFiles = false;

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

            LoadSaveAtPath(path);
            LoadSaveAsCurrentSave(slot);

            return _activeSaveData;
        }

        private string CurrentSaveDataDirectoryPath
        {
            get
            {
                return Path.Join(SaveBasePath, ActiveSaveData.SaveDataRelativeDirectoryPath);
            }
        }

        /// <summary>
        /// List all the available saves within the game.
        /// </summary>
        public Dictionary<int, SaveData> GetAllSaves() => _allSavedData;

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

            _activeSaveData = data;

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

            if (!_allSavedData.TryGetValue(slot, out SaveData? data))
            {
                return false;
            }

            _activeSaveData = data;
            LoadAllAssetsForCurrentSave();

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

            _allSavedData.Add(asset.SaveSlot, asset);

            return true;
        }

        private async ValueTask SaveGameDataAsync(GameAsset asset)
        {
            if (asset.GetGameAssetPath() is string path)
            {
                await FileHelper.SaveSerializedAsync(asset, path, isCompressed: true);
            }
            else
            {
                GameLogger.Error($"Unable to save {asset.Name} save data.");
            }
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

            await Task.Yield();

            // Wait for any pending operations.
            if (_activeSaveData.PendingOperation is not null)
            {
                await _activeSaveData.PendingOperation;
            }

            await SaveGameDataAsync(_activeSaveData);

            GameAsset[] assets = [.. _currentSaveAssets.Values];
            foreach (GameAsset asset in assets)
            {
                if (string.IsNullOrEmpty(asset.FilePath))
                {
                    asset.FilePath = Path.Join(CurrentSaveDataDirectoryPath, $"{asset.Name}.json");
                }

                await SaveGameDataAsync(asset);
            }

            foreach (string path in _pendingAssetsToDeleteOnSerialize)
            {
                if (!FileHelper.DeleteFileIfExists(path))
                {
                    GameLogger.Warning($"Unable to delete save asset at path: {path}");
                }
            }

            _pendingAssetsToDeleteOnSerialize.Clear();

            return true;
        }

        public bool RemoveAssetForCurrentSave(Guid guid)
        {
            if (!_currentSaveAssets.TryGetValue(guid, out GameAsset? asset))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(asset.FilePath))
            {
                _pendingAssetsToDeleteOnSerialize.Add(asset.FilePath);
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
            using PerfTimeRecorder recorder = new("Loading Saves");

            _allSavedData.Clear();

            // Load all the save data assets.
            foreach (string savePath in FileHelper.ListAllDirectories(SaveBasePath))
            {
                LoadSaveAtPath(savePath);
            }

            _loadedSaveFiles = true;
            return true;
        }

        private bool LoadSaveAtPath(string path)
        {
            foreach (GameAsset asset in FetchAssetsAtPath(path, recursive: false, skipFailures: false, stopOnFailure: true))
            {
                if (asset is SaveData saveData)
                {
                    if (saveData.SaveVersion < _game?.Version)
                    {
                        // Skip loading saves with incompatible version, for now.
                        continue;
                    }

                    _allSavedData.Add(saveData.SaveSlot, saveData);
                }
            }

            return true;
        }

        private bool LoadAllAssetsForCurrentSave()
        {
            _currentSaveAssets.Clear();

            foreach (GameAsset asset in FetchAssetsAtPath(CurrentSaveDataDirectoryPath))
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
            _pendingAssetsToDeleteOnSerialize.Clear();

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
            _allSavedData.Clear();
            _currentSaveAssets.Clear();

            _activeSaveData = null;

            _loadedSaveFiles = false;
        }

        public virtual void DeleteAllSaves()
        {
            UnloadAllSaves();

            FileHelper.DeleteContent(SaveBasePath, deleteRootFiles: false);
            _pendingAssetsToDeleteOnSerialize.Clear();
        }

        public virtual bool DeleteSaveAt(int slot)
        {
            if (!_loadedSaveFiles)
            {
                LoadAllSaves();
            }

            if (!_allSavedData.TryGetValue(slot, out SaveData? data))
            {
                return false;
            }

            if (string.IsNullOrEmpty(data.SaveRelativeDirectoryPath))
            {
                FileHelper.DeleteContent(SaveBasePath, deleteRootFiles: false);
            }
            else
            {
                string directory = Path.Join(SaveBasePath, data.SaveRelativeDirectoryPath);
                FileHelper.DeleteDirectoryIfExists(directory);
            }

            UnloadSaveAt(slot);

            return true;
        }
    }
}