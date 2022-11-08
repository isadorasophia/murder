using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Murder.Assets;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Utilities;
using Murder.Serialization;

namespace Murder.Editor.Data
{
    public class EditorDataManager : GameDataManager
    {
        /// <summary>
        /// Initialized in <see cref="Init(string)"/>.
        /// </summary>
        public EditorSettingsAsset EditorSettings { get; private set; } = null!;

        public const string EditorSettingsFileName = @"editor_config.json";

        private string AssetsDataPath => FileHelper.GetPath(Path.Join(EditorSettings.BinResourcesPath, GameProfile.AssetResourcesPath));

        private readonly Dictionary<Guid, GameAsset> _saveAssetsForEditor = new();

        public ImmutableArray<GameAsset> GetAllSaveAssets() => _saveAssetsForEditor.Values.ToImmutableArray();

        public ImmutableArray<string> HiResImages;

        private string _sourceResourcesDirectory = string.Empty;

        protected string? _assetsSourceDirectoryPath;

        public string AssetsSourceDirectoryPath => _assetsSourceDirectoryPath!;

        private string? _packedSourceDirectoryPath;

        public string PackedSourceDirectoryPath => _packedSourceDirectoryPath!;

        [MemberNotNull(
            nameof(_assetsSourceDirectoryPath),
            nameof(_packedSourceDirectoryPath))]
        public override void Init(string _ = "")
        {
            LoadEditorSettings();

            base.Init(EditorSettings.BinResourcesPath);

            _sourceResourcesDirectory = EditorSettings.SourceResourcesPath;

            _assetsSourceDirectoryPath = FileHelper.GetPath(_sourceResourcesDirectory, GameProfile.AssetResourcesPath);
            _packedSourceDirectoryPath = FileHelper.GetPath(_sourceResourcesDirectory, EditorSettings.SourcePackedPath);

            EditorSettings.FilePath = EditorSettingsFileName;
            EditorSettings.Name = "Editor Settings";

            GameProfile.FilePath = GameProfileFileName;
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
            if (!Directory.Exists(EditorSettings.RawResourcesPath))
            {
                GameLogger.Warning($"Please specify a valid \"Raw resources path\" in \"Editor Profile\". Unable to find the resources to scan the high resolution images.");
                return;
            }

            var builder = ImmutableArray.CreateBuilder<string>();
            foreach (var file in FileHelper.GetAllFilesInFolder(FileHelper.GetPath(EditorSettings.RawResourcesPath, "/hires_images/"), "*.png",true))
            {
                builder.Add(Path.GetRelativePath(FileHelper.GetPath(EditorSettings.RawResourcesPath) + "/hires_images/", FileHelper.GetPathWithoutExtension(file.FullName)));
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
            string editorSettingsPath = Path.Join(SaveBasePath, EditorSettingsFileName);

            if (FileHelper.Exists(editorSettingsPath))
            {
                EditorSettings = FileHelper.DeserializeAsset<EditorSettingsAsset>(editorSettingsPath)!;
                GameLogger.Log("Successfully loaded editor configurations.");
            }

            if (EditorSettings is null)
            {
                GameLogger.Warning($"Didn't find {EditorSettingsFileName} file. Creating one.");

                EditorSettings = new EditorSettingsAsset("<project-name>");
                EditorSettings.MakeGuid();
                SaveAsset(EditorSettings);
            }

            Architect.Instance.DPIScale = EditorSettings.DPI;

            string gameProfilePath = FileHelper.GetPath(Path.Join(EditorSettings.BinResourcesPath, GameProfileFileName));

            // Create a game profile, if none was provided from the base game.
            if (!FileHelper.Exists(gameProfilePath))
            {
                GameLogger.Warning($"Didn't find {GameProfileFileName} file. Creating one.");

                GameProfile = CreateGameProfile();
                GameProfile.MakeGuid();
                SaveAsset(GameProfile);
            }
        }

        /// <summary>
        /// Checks whether an asset has been loaded as part of the save path.
        /// </summary>
        internal bool IsLoadedSaveAsset(Guid assetGuid)
        {
            return _saveAssetsForEditor.ContainsKey(assetGuid);
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
                string? editorPath = EditorSettings.GetAssetPath();
                if (editorPath is not null)
                {
                    FileHelper.SaveSerialized(EditorSettings, editorPath);
                }
            }

            if (!Path.Exists(_sourceResourcesDirectory))
            {
                GameLogger.Error(
                    "Please select a valid Source Resources Path in \"Editor Profile\" in order to synchronize the game settings.");

                return;
            }

            // Saving game settings
            {
                GameLogger.Verify(GameProfile != null, "Cannot serialize a null GameSettings");

                // Manually create our path to source directory.
                string? profilePath = GameProfile.GetAssetPath();
                if (profilePath is not null)
                {
                    FileHelper.SaveSerialized(GameProfile, profilePath);
                }
            }
        }

        /// <summary>
        /// Save a generic asset into our database.
        /// </summary>
        public void SaveAsset<T>(T asset) where T : GameAsset
        {
            if (string.IsNullOrWhiteSpace(asset.FilePath))
            {
                // File has just been created and we need to name it.
                asset.FilePath = asset.Name + ".json";
            }

            string? sourcePath = asset.GetAssetPath();
            string? binPath = asset.GetAssetPath(useBinPath: true);
            if (sourcePath is null)
            {
                GameLogger.Error($"Unable to save asset of {typeof(T).Name}?");
                return;
            }

            // If the source is invalid and either the source resources or binaries path have not been initialized.
            if (!Directory.Exists(Path.GetDirectoryName(sourcePath)) &&
                (!Directory.Exists(_sourceResourcesDirectory) || !Directory.Exists(_binResourcesDirectory)))
            {
                GameLogger.Error($"Unable to save asset at path {_sourceResourcesDirectory}.");
                GameLogger.Error("Have you tried setting Game Source Path in \"Editor Profile\"?");
                return;
            }

            // File is about to be synchronized, so it's not changed.
            asset.FileChanged = false;

            if (asset.Rename || asset.TaggedForDeletion)
            {
                if (asset.CanBeDeleted)
                {
                    if (!FileHelper.DeleteFileIfExists(sourcePath))
                    {
                        GameLogger.Error($"Couldn't find file '{sourcePath}' to delete!");
                    }

                    if (binPath is not null)
                    {
                        _ = FileHelper.DeleteFileIfExists(binPath);
                    }
                }

                if (!asset.Rename)
                {
                    return;
                }
                else
                {
                    asset.Rename = false;
                    asset.FilePath = asset.Name + ".json";

                    sourcePath = asset.GetAssetPath()!;
                    binPath = asset.GetAssetPath(useBinPath: true);
                }
            }

            // Let's check if we are saving a file with a name that has already been taken.
            if (_database.TryGetValue(typeof(T), out HashSet<Guid>? guidAssetsOfType))
            {
                if (guidAssetsOfType.Count(
                    a => string.Equals(_allAssets[a].Name, asset.Name, StringComparison.OrdinalIgnoreCase) && _allAssets[a] != asset) > 0)
                {
                    // Since we already have an existing asset with the same name, create a new name for this.
                    asset.Name = GetNextName(asset.Name, EditorSettings.AssetNamePattern);
                    asset.FilePath = asset.Name + ".json";

                    sourcePath = asset.GetAssetPath()!;
                    binPath = asset.GetAssetPath(useBinPath: true);
                }
            }

            // Now that we know we have an actual valid path, create the relative path to this new file.
            // We save twice: one in source to persist and in bin to reflect in the executable.
            FileHelper.CreateDirectoryPathIfNotExists(sourcePath);
            FileHelper.SaveSerialized(asset, sourcePath);

            if (binPath is not null)
            {
                FileHelper.CreateDirectoryPathIfNotExists(binPath);
                FileHelper.SaveSerialized(asset, binPath);
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
            var targetBinPath = FileHelper.GetPath(Path.Join(GameProfile.AssetResourcesPath));
            
            var filesCopied = FileHelper.DirectoryCopy(
                AssetsDataPath,
                targetBinPath,
                true
                );
            GameLogger.Log($"Content updated from {AssetsDataPath} to {targetBinPath} (total files copied: {filesCopied})");
        }
    }
}