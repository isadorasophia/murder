using Murder.Assets;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Save;
using Murder.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Data
{
    public partial class GameDataManager
    {
        /// <summary>
        /// Directory used for saving custom data.
        /// </summary>
        private const string GameDirectory = "InstallWizard";

        /// <summary>
        /// Save directory path used when serializing user data.
        /// </summary>
        public static string SaveBasePath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), GameDirectory);

        /// <summary>
        /// This is the collection of save data.
        /// </summary>
        protected readonly Dictionary<Guid, SaveData> _allSavedData = new();

        /// <summary>
        /// Stores all the save assets tied to the current <see cref="ActiveSaveData"/>.
        /// </summary>
        private readonly Dictionary<Guid, GameAsset> _currentSaveAssets = new();

        private SaveData? _activeSaveData;
        
        private GamePreferences? _preferences;

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
                _preferences ??= GamePreferences.FetchOrCreate();

                return _preferences;
            }
        }

        /// <summary>
        /// Active saved run in the game.
        /// </summary>
        public SaveData? TryGetActiveSaveData() => _activeSaveData;
        
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
        public virtual Guid CreateSave(string name)
        {
            // We will actually wipe any previous saves at this point and create the new one.
            DeleteAllSaves();

            SaveData data = new();
            CreateSaveData(data);

            return data.Guid;
        }

        public void LoadSave(Guid guid)
        {
            if (!_allSavedData.TryGetValue(guid, out SaveData? data))
            {
                throw new InvalidOperationException("Loading invalid save?");
            }

            _activeSaveData = data;
            LoadAllAssetsForCurrentSave();
        }

        internal void SaveWorld(Guid worldGuid, MonoWorld world)
        {
            ActiveSaveData.SynchronizeWorld(worldGuid, world);

            // Save all the dynamic assets tied to this world.
            SerializeSave();
        }

        internal bool TryGetDynamicAsset<T>([NotNullWhen(true)] out T? asset) where T : DynamicAsset
        {
            asset = _activeSaveData?.TryGetDynamicAsset<T>();

            return asset != null;
        }

        /// <summary>
        /// Retrieve a dynamic asset within the current save data.
        /// If no dynamic asset is found, it creates a new one to the save data.
        /// </summary>
        internal T GetDynamicAsset<T>() where T : DynamicAsset, new()
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
        internal GameAsset? TryGetAssetForCurrentSave(Guid guid)
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

            // Immediately serialize the save after creating it.
            FileHelper.SaveSerialized(asset, asset.FilePath, isCompressed: true);

            return true;
        }

        internal bool AddAssetForCurrentSave(GameAsset asset)
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

            _currentSaveAssets.Add(asset.Guid, asset);
            SerializeSave();
            
            return true;
        }

        /// <summary>
        /// Quickly serialize our save assets.
        /// </summary>
        public void QuickSave()
        {
            SerializeSave();
        }

        internal bool SerializeSave()
        {
            if (_activeSaveData is null)
            {
                return false;
            }

            FileHelper.SaveSerialized(_activeSaveData, _activeSaveData.FilePath, isCompressed: true);

            foreach (GameAsset asset in _currentSaveAssets.Values)
            {
                if (string.IsNullOrEmpty(asset.FilePath))
                {
                    asset.FilePath = Path.Join(CurrentSaveDataDirectoryPath, $"{asset.Name}.json");
                }

                FileHelper.SaveSerialized(asset, asset.FilePath, isCompressed: true);
            }

            return true;
        }

        internal bool RemoveAssetForCurrentSave(Guid guid)
        {
            if (!_currentSaveAssets.TryGetValue(guid, out GameAsset? asset))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(asset.FilePath))
            {
                if (!FileHelper.DeleteFileIfExists(asset.FilePath))
                {
                    GameLogger.Warning($"Unable to delete save asset at path: {asset.FilePath}");
                }
            }

            bool removed = _currentSaveAssets.Remove(asset.Guid);
            
            if (asset is DynamicAsset)
            {
                _activeSaveData?.RemoveDynamicAsset(asset.GetType());
            }

            return removed;
        }

        public bool LoadAllSaves()
        {
            _allSavedData.Clear();
            
            // Load all the save data assets.
            foreach (string savePath in FileHelper.ListAllDirectories(SaveBasePath))
            {
                foreach (var (asset, filepath) in FetchAssetsAtPath(savePath, recursive: false, stopOnFailure: true))
                {
                    if (asset is SaveData saveData)
                    {
                        saveData.FilePath = filepath;
                        
                        _allSavedData.Add(saveData.Guid, saveData);
                    }
                }
            }

            return true;
        }

        private bool LoadAllAssetsForCurrentSave()
        {
            _currentSaveAssets.Clear();

            foreach (var (asset, filepath) in FetchAssetsAtPath(CurrentSaveDataDirectoryPath))
            {
                asset.FilePath = filepath;

                _currentSaveAssets.Add(asset.Guid, asset);
            }

            return true;
        }

        public virtual void DeleteAllSaves()
        {
            UnloadAllSaves();

            FileHelper.DeleteContent(SaveBasePath, deleteRootFiles: false);
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