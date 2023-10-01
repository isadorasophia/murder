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
        protected virtual SaveData CreateSaveData(string name = "_default") => 
            _game is not null ? _game.CreateSaveData(name) : new SaveData(name);

        /// <summary>
        /// Directory used for saving custom data.
        /// </summary>
        public virtual string GameDirectory => _game?.Name ?? "Murder";

        /// <summary>
        /// Save directory path used when serializing user data.
        /// </summary>
        public static string SaveBasePath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Game.Data.GameDirectory);

        /// <summary>
        /// This is the collection of save data.
        /// </summary>
        protected readonly Dictionary<Guid, SaveData> _allSavedData = new();

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

            Guid guid = _activeSaveData.Guid;
            string? path = _activeSaveData.GetGameAssetPath();

            UnloadCurrentSave();

            // Load the save from its directory.
            path = Path.GetDirectoryName(path);

            if (path is null || !Directory.Exists(path))
            {
                return null;
            }
            
            LoadSaveAtPath(path);
            LoadSaveAsCurrentSave(guid);
            
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
        public IEnumerable<SaveData> GetAllSaves() => _allSavedData.Values;

        /// <summary>
        /// Create a new save data based on a name.
        /// </summary>
        public virtual SaveData CreateSave(string name)
        {
            // We will actually wipe any previous saves at this point and create the new one.
            DeleteAllSaves();

            SaveData data = CreateSaveData();
            CreateSaveData(data);

            _activeSaveData = data;

            return data;
        }
            
        public bool CanLoadSaveData(Guid? guid = null)
        {
            if (guid is null)
            {
                return _allSavedData.Count > 0;
            }

            return _allSavedData.ContainsKey(guid.Value);
        }

        public bool LoadSaveAsCurrentSave(Guid? guid = null)
        {
            if (guid is null && _allSavedData.Count > 0)
            {
                guid = _allSavedData.Keys.First();
            }
            
            if (guid is null || guid == Guid.Empty || !_allSavedData.ContainsKey(guid.Value))
            {
                return false;
            }

            if (!_allSavedData.TryGetValue(guid.Value, out SaveData? data))
            {
                throw new InvalidOperationException("Loading invalid save?");
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
        private bool CreateSaveData(SaveData asset)
        {
            if (_allSavedData.ContainsKey(asset.Guid))
            {
                GameLogger.Fail("Adding duplicate save asset?");
                return false;
            }

            GameLogger.Verify(asset.Guid != Guid.Empty);
            GameLogger.Verify(!string.IsNullOrWhiteSpace(asset.Name));
            GameLogger.Verify(!string.IsNullOrWhiteSpace(asset.FilePath));

            _allSavedData.Add(asset.Guid, asset);
            
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

            GameAsset[] assets = _currentSaveAssets.Values.ToArray();
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

            return true;
        }

        public bool LoadSaveAtPath(string path)
        {
            foreach (var asset in FetchAssetsAtPath(path, recursive: false, stopOnFailure: true))
            {
                if (asset is SaveData saveData)
                {
                    _allSavedData.Add(saveData.Guid, saveData);
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

            _allSavedData.Remove(_activeSaveData.Guid);
            _currentSaveAssets.Clear();
            
            _activeSaveData = null;
            _pendingAssetsToDeleteOnSerialize.Clear();

            return true;
        }

        public virtual void DeleteAllSaves()
        {
            UnloadAllSaves();

            FileHelper.DeleteContent(SaveBasePath, deleteRootFiles: false);
            _pendingAssetsToDeleteOnSerialize.Clear(); 
        }

        /// <summary>
        /// Used to clear all saves files currently active.
        /// </summary>
        public void UnloadAllSaves()
        {
            _allSavedData.Clear();
            _currentSaveAssets.Clear();

            _activeSaveData = null;
        }
    }
}