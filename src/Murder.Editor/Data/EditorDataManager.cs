using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Murder.Assets;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Serialization;

namespace Murder.Editor.Data
{
    public class EditorDataManager : GameDataManager
    {
        /// <summary>
        /// Initialized in <see cref="Init(string)"/>.
        /// </summary>
        public EditorSettingsAsset EditorSettings { get; private set; } = null!;

        private string AssetsDataPath => FileHelper.GetPath(Path.Join(EditorSettings.AssetPathPrefix, GameProfile.GameAssetsResourcesPath));

        private readonly Dictionary<Guid, GameAsset> _saveAssetsForEditor = new();

        public ImmutableArray<GameAsset> GetAllSaveAssets() => _saveAssetsForEditor.Values.ToImmutableArray();

        public ImmutableArray<string> HiResImages;

        public override void Init(string _ = "")
        {
            LoadEditorSettings();

            // TODO: Fix so each client implement their own asset path prefix.
            base.Init(EditorSettings.AssetPathPrefix);

            EditorSettings.FilePath = EditorSettingsFileName;
            EditorSettings.Name = "Editor Settings";
            GameProfile.FilePath = GameDataManager.GameProfileFileName;
            GameProfile.Name = "Game Profile";
        }

        public override void LoadContent()
        {
            base.LoadContent();
            RefreshAfterSave();
        }

        public override void RefreshAtlas()
        {
            base.RefreshAtlas();
            ScanHighResImages();
        }

        private void ScanHighResImages()
        {
            var builder = ImmutableArray.CreateBuilder<string>();

            foreach (var file in FileHelper.GetAllFilesInFolder(FileHelper.GetPath(EditorSettings.ResourcesPath, "/hires_images/"), "*.png",true))
            {
                builder.Add(Path.GetRelativePath(FileHelper.GetPath(EditorSettings.ResourcesPath) + "/hires_images/", FileHelper.GetPathWithoutExtension(file.FullName)));
            }

            HiResImages = builder.ToImmutable();
        }

        public void RefreshAfterSave()
        {
            LoadAllSaveAssets();
        }

        private void LoadAllSaveAssets()
        {
            _saveAssetsForEditor.Clear();

            foreach (var (asset, filepath) in FetchAssetsAtPath(SaveBasePath, stopOnFailure: true))
            {
                asset.FilePath = filepath;
                _saveAssetsForEditor.Add(asset.Guid, asset);
            }
        }

        public override void DeleteAllSaves()
        {
            _saveAssetsForEditor.Clear();
            base.DeleteAllSaves();
        }

        private void LoadEditorSettings()
        {
            string editorSettingsPath = FileHelper.GetPath(EditorSettingsFileName);

            if (FileHelper.Exists(editorSettingsPath))
            {
                EditorSettings = FileHelper.DeserializeAsset<EditorSettingsAsset>(editorSettingsPath)!;
                GameLogger.Log("Successfully loaded editor configurations.");
            }

            if (EditorSettings is null)
            {
                GameLogger.Warning($"Didn't find {EditorSettingsFileName} file. Creating one.");
                EditorSettings = new EditorSettingsAsset();
                EditorSettings.MakeGuid();
            }

            Architect.Instance.DPIScale = EditorSettings.DPI;
        }

        protected override void RemoveAsset(Type t, Guid assetGuid)
        {
            if (TryGetAsset(assetGuid) is GameAsset asset)
            {
                asset.TaggedForDeletion = true;

                base.RemoveAsset(t, assetGuid);
                SaveAsset(asset);
            }
            else if (_saveAssetsForEditor.TryGetValue(assetGuid, out GameAsset? saveAsset))
            {
                FileHelper.DeleteFileIfExists(saveAsset.FilePath);
                _saveAssetsForEditor.Remove(assetGuid);
            }
        }

        internal GameAsset CreateNewAsset(Type type, string assetName)
        {
            var asset = Activator.CreateInstance(type) as GameAsset;
            GameLogger.Verify(asset != null);

            asset.Name = GetNextName(assetName, EditorSettings.AssetNamePattern);

            AddAsset(asset);
            return asset;
        }

        internal void SaveSettings()
        {
            // Saving editor settings
            {
                GameLogger.Verify(EditorSettings != null, "Cannot serialize a null EditorSettings");
                var json = FileHelper.SaveSerializedFromRelativePath(EditorSettings, EditorSettingsFileName);

                FileHelper.SaveTextFromRelativePath("../../../" + EditorSettingsFileName, json);
            }

            // Saving game settings
            {
                GameLogger.Verify(GameProfile != null, "Cannot serialize a null GameSettings");
                var json = FileHelper.SaveSerializedFromRelativePath(GameProfile, GameProfileFileName);

                FileHelper.SaveTextFromRelativePath("../../../" + GameProfileFileName, json);
            }
        }

        /// <summary>
        /// Save a generic asset into our database.
        /// </summary>
        public void SaveAsset<T>(T asset) where T : GameAsset
        {
            asset.FileChanged = false;

            // If this is actually a save data asset, just save it from the file path directly.
            if (_saveAssetsForEditor.ContainsKey(asset.Guid))
            {
                FileHelper.SaveSerialized(asset, asset.FilePath);
                return;
            }

            // Otherwise, it's an asset, and deal with it accordingly.
            var pathPrefix = Path.Join(EditorSettings.AssetPathPrefix, Game.Profile.GameAssetsResourcesPath, asset.SaveLocation);
            if (!string.IsNullOrWhiteSpace(asset.FilePath) && asset.CanBeDeleted)
            {
                var pathToDelete = FileHelper.GetPath(asset.CustomPath ?? Path.Join(pathPrefix, asset.FilePath));

                if (!FileHelper.DeleteFileIfExists(pathToDelete))
                {
                    GameLogger.Error($"Couldn't find file '{pathToDelete}' to delete!");
                }
            }

            if (asset.TaggedForDeletion) return;

            if (_database.TryGetValue(typeof(T), out HashSet<Guid>? guidAssetsOfType))
            {
                if (guidAssetsOfType.Count(
                    a => string.Equals(_allAssets[a].Name, asset.Name, StringComparison.OrdinalIgnoreCase) && _allAssets[a] != asset) > 0)
                {
                    // Since we already have an existing asset with the same name, create a new name for this.
                    asset.Name = GetNextName(asset.Name, EditorSettings.AssetNamePattern);
                }
            }

            if (asset.CustomPath != null)
            {
                FileHelper.SaveSerializedFromRelativePath(asset, asset.CustomPath);
            }
            else
            {
                asset.FilePath = asset.Name + ".json";
                FileHelper.SaveSerializedFromRelativePath(asset, Path.Join(pathPrefix, asset.Name + ".json"));
            }
        }

        private GameAsset? GetAssetByName(string name)
        {
            foreach (GameAsset other in _allAssets.Values)
            {
                if (string.Equals(other.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return other;
                }
            }

            return null;
        }

        public string GetNextName(string name, string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
            {
                throw new ArgumentException("The pattern must include an index place-holder like '{0}'", "pattern");
            }
            
            if (GetAssetByName(name) is null)
            {
                return name;
            }

            string candidate;
            int min = 1, max = 1024; // min is inclusive, max is exclusive/untested

            Regex r = new Regex("[0-9]+");
            Match m = r.Match(name);
            if (m.Success)
            {
                // Name already had a number, start from its number then.
                min = int.Parse(m.Groups[0].Value);

                candidate = name;
            }
            else
            {
                // First time instantiating a candidate, use the format.
                candidate = name + string.Format(pattern, min);
            }

            for (int i = min; i < max; i++)
            {
                candidate = r.Replace(candidate, $"{i}");
                if (GetAssetByName(candidate) == null)
                    return candidate;
            }

            GameLogger.Error("Couldn't find a valid asset name!");
            return "ERROR!!!";
        }

        public void BuildBinContentFolder()
        {
            var targetBinPath = FileHelper.GetPath(Path.Join(GameProfile.GameAssetsResourcesPath));
            
            var filesCopied = FileHelper.DirectoryCopy(
                AssetsDataPath,
                targetBinPath,
                true
                );
            GameLogger.Log($"Content updated from {AssetsDataPath} to {targetBinPath} (total files copied: {filesCopied})");
        }
    }
}