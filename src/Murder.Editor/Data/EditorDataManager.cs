using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.CustomEditors;
using Murder.Editor.Data.Graphics;
using Murder.Editor.Core;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Importers;
using Murder.Editor.Systems.Debug;
using Murder.Editor.Systems;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Systems.Graphics;
using Murder.Systems;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using static Murder.Editor.Data.Graphics.FontLookup;
using Murder.Services;
using Murder.Assets.Graphics;
using Murder.Assets.Save;
using Bang.Diagnostics;
using Murder.Utilities;
using System.Collections.Generic;

namespace Murder.Editor.Data
{
    public partial class EditorDataManager : GameDataManager
    {
        /// <summary>
        /// Initialized in <see cref="Initialize(string)"/>.
        /// </summary>
        public EditorSettingsAsset EditorSettings { get; private set; } = null!;

        public const string EditorSettingsFileName = "editor_config";

        public override bool IgnoreSerializationErrors => true;

        public ImmutableArray<string> AvailableUniqueTextures = [];

        private readonly Dictionary<Guid, GameAsset> _saveAssetsForEditor = new();
        public ImmutableArray<GameAsset> GetAllSaveAssets() => _saveAssetsForEditor.Values.ToImmutableArray();

        private string? _packedSourceDirectoryPath;
        public string PackedSourceDirectoryPath => _packedSourceDirectoryPath!;

        private CursorTextureManager? _cursorTextureManager = null;
        public CursorTextureManager? CursorTextureManager => _cursorTextureManager;

        private readonly ImGuiTextureManager _imGuiTextureManager = new();
        public ImGuiTextureManager ImGuiTextureManager => _imGuiTextureManager;

        protected string? _assetsSourceDirectoryPath;
        public string AssetsSourceDirectoryPath => _assetsSourceDirectoryPath!;

        private string _sourceResourcesDirectory = "resources";

        /// <summary>
        /// A dictionary matching file extensions to their corresponding <see cref="ResourceImporter"/>s.
        /// </summary>
        internal ImmutableArray<ResourceImporter> AllImporters = ImmutableArray<ResourceImporter>.Empty;

        public EditorDataManager(IMurderGame? game) : base(game, new EditorFileManager()) 
        { }

        [MemberNotNull(
            nameof(_assetsSourceDirectoryPath),
            nameof(_packedSourceDirectoryPath))]
        public override void Initialize(string _ = "")
        {
            LoadEditorSettings();

            base.Initialize(EditorSettings.BinResourcesPath);

            _sourceResourcesDirectory = EditorSettings.SourceResourcesPath;

            _assetsSourceDirectoryPath = FileHelper.GetPath(_sourceResourcesDirectory, GameProfile.AssetResourcesPath);
            _packedSourceDirectoryPath = FileHelper.GetPath(EditorSettings.SourcePackedPath);

            EditorSettings.FilePath = EditorSettingsFileName;
            EditorSettings.Name = "Editor Settings";

            GameProfile.FilePath = GameProfileFileName;
            GameProfile.Name = "Game Profile";

            FetchResourceImporters();
        }

        protected override LocalizationAsset GetLocalization(LanguageId id)
        {
            LocalizationAsset? asset;

            if (!Game.Profile.LocalizationResources.TryGetValue(id, out Guid resourceGuid))
            {
                GameLogger.Log($"Creating a new resource for {id}");
                resourceGuid = Guid.Empty;
            }

            asset = Game.Data.TryGetAsset<LocalizationAsset>(resourceGuid);
            if (asset is null)
            {
                // Create a default one.
                asset = new();
                asset.Name = id == LanguageId.English ? "Resources" : $"Resources-{CurrentLocalization.Identifier}";

                Game.Data.AddAsset(asset);
                Game.Profile.LocalizationResources = Game.Profile.LocalizationResources.SetItem(CurrentLocalization.Id, asset.Guid);

                SaveAsset(Game.Profile);
            }

            return asset;
        }

        private void FetchResourceImporters()
        {
            if (AllImporters.Length > 0)
            {
                GameLogger.Error("Are we fetching resource importers more than once?");
                return;
            }

            var importers = ImmutableArray.CreateBuilder<ResourceImporter>();

            IEnumerable<Type> importerTypes = ReflectionHelper.GetAllImplementationsOf<ResourceImporter>();

            foreach (Type importerType in importerTypes)
            {
                ResourceImporter importer = (ResourceImporter)Activator.CreateInstance(importerType, args: EditorSettings)!;
                importers.Add(importer);
            }

            AllImporters = importers.ToImmutableArray();
        }

        /// <summary>
        /// List of contents which will be "automatically" updated once the windows enters foreground.
        /// This should be relatively lightweight operations that can check for a last modified cache, for example.
        /// </summary>
        public void ReloadOnWindowForeground()
        {
            if (CallAfterLoadContent)
            {
                // we are still loading, skip any foreground actions.
                return;
            }

            EditorScene? editorScene = Architect.Instance.ActiveScene as EditorScene;

            if (ReloadDialogs())
            {
                editorScene?.ReloadEditorsOfType<CharacterEditor>();
            }

            // Reload sprites regardless of the active scene.
            _ = ReloadSprites();
        }

        public override void LoadContent()
        {
            // Convert TTF Fonts
            ConvertTTFToSpriteFont();

            bool skipIfNoChangesFound = !EditorSettings.AlwaysBuildAtlasOnStartup;

            FetchResourcesForImporters(reload: false, skipIfNoChangesFound);
            LoadResourceImporters(reload: false, skipIfNoChangesFound);

            // Load content (from bin folder), as usual
            base.LoadContent();

            RefreshAfterSave();
        }

        protected override async Task LoadContentAsyncImpl()
        {
            string hiddenFolderPath = FileHelper.GetPath(
                _binResourcesDirectory, GameProfile.AssetResourcesPath, GameProfile.GenericAssetsPath, HiddenAssetsRelativePath);

            // Make sure we load the manager assets first.
            LoadAssetsAtPath(hiddenFolderPath, hasEditorPath: true);
            SkipLoadingAssetsAt(hiddenFolderPath);

            await LoadResourceImportersAsync(reload: false, skipIfNoChangesFound: !EditorSettings.AlwaysBuildAtlasOnStartup);
        }

        private ImmutableArray<(Type, bool)>? _cachedDiagnosticsSystems = null;

        private readonly CacheDictionary<Type, ImmutableDictionary<Guid, GameAsset>> _cachedFilteredAssetsWithImplementation = new(6);

        /// <summary>
        /// Filter all the assets and any types that implement those types.
        /// Cautious: this may be slow or just imply extra allocations.
        /// </summary>
        public ImmutableDictionary<Guid, GameAsset> FilterAllAssetsWithImplementation(Type type)
        {
            if (_cachedFilteredAssetsWithImplementation.TryGetValue(type, out var result))
            {
                return result;
            }

            var builder = ImmutableDictionary.CreateBuilder<Guid, GameAsset>();

            builder.AddRange(FilterAllAssets(type));

            // If the type is abstract, also gather all the assets that implement it.
            foreach (Type assetType in _database.Keys)
            {
                if (type.IsAssignableFrom(assetType))
                {
                    builder.AddRange(FilterAllAssets(assetType));
                }
            }

            result = builder.ToImmutableDictionary();

            _cachedFilteredAssetsWithImplementation.TryAdd(type, result);
            return result;
        }

        /// <inheritdoc/>
        protected override ImmutableArray<(Type, bool)> FetchSystemsToStartWith()
        {
            if (_cachedDiagnosticsSystems is null)
            {
                var builder = ImmutableArray.CreateBuilder<(Type, bool)>();

                ImmutableDictionary<Guid, GameAsset> assets = Architect.EditorData.FilterAllAssetsWithImplementation(typeof(FeatureAsset));
                foreach ((_, GameAsset g) in assets)
                {
                    if (g is not FeatureAsset f || !f.IsDiagnostics)
                    {
                        continue;
                    }

                    builder.AddRange(f.FetchAllSystems(enabled: true));

                    // TODO: Also pull diagnostic systems?
                }

                _cachedDiagnosticsSystems = builder.ToImmutableArray();
            }

            return _cachedDiagnosticsSystems.Value;
        }

        /// <summary>
        /// Always loads all the assets in the editor. Except! When already loaded when generating assets.
        /// </summary>
        protected override bool ShouldSkipAsset(string fullFilename)
        {
            return IsPathOnSkipLoading(fullFilename);
        }

        internal void ConvertTTFToSpriteFont()
        {
            string ttfFontsPath = FileHelper.GetPath(EditorSettings.RawResourcesPath, Game.Profile.FontsPath);
            if (!Directory.Exists(ttfFontsPath))
            {
                // No font directory, so skip.
                return;
            }

            if (!FileLoadHelpers.ShouldRecalculate(ttfFontsPath, FontImporter.SourcePackedPath))
            {
                return;
            }

            // Load the "config" file with all the fonts settings.
            FontLookup lookup = new(ttfFontsPath + "fonts.murder");

            string[] ttfFiles = Directory.GetFiles(ttfFontsPath, "*.ttf", SearchOption.AllDirectories);
            foreach (string ttfFile in ttfFiles)
            {
                string fontName = Path.GetFileNameWithoutExtension(ttfFile);

                if (lookup.GetInfo(fontName + ".ttf") is FontInfo info)
                {
                    if (FontImporter.GenerateFontJsonAndPng(info.Index, ttfFile, info.Size, info.Offset, info.Padding, fontName, info.Chars))
                    {
                        GameLogger.Log($"Converting {ttfFile}...");
                    }
                }
                else
                {
                    GameLogger.Error($"File {ttfFile} doesn't having a matching name in fonts.murder. Maybe there's a typo?");
                }
            }
        }

        public void RefreshAfterSave()
        {
            _ = LoadAllSaveAssets();
        }

        private async Task LoadAllSaveAssets()
        {
            _saveAssetsForEditor.Clear();

            await Task.Yield();

            using PerfTimeRecorder recorder = new("Loading Saves (for editor)");

            string trackerPath = Path.Join(SaveBasePath, SaveDataTracker.Name);
            if (!File.Exists(trackerPath))
            {
                return;
            }

            SaveDataTracker tracker = FileManager.UnpackContent<SaveDataTracker>(trackerPath);
            foreach ((int slot, SaveDataInfo save) in tracker.Info)
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

                _saveAssetsForEditor[packedData.Data.Guid] = packedData.Data;

                foreach (GameAsset asset in packedAssetsData.Assets)
                {
                    _saveAssetsForEditor[asset.Guid] = asset;
                }
            }
        }

        public override void DeleteAllSaves()
        {
            _saveAssetsForEditor.Clear();

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

            string trackerPath = Path.Join(SaveBasePath, SaveDataTracker.Name);
            if (!File.Exists(trackerPath))
            {
                return;
            }

            SaveDataTracker tracker = FileManager.UnpackContent<SaveDataTracker>(trackerPath);
            foreach ((int slot, SaveDataInfo save) in tracker.Info)
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
                _allSavedData[slot] = save;

                if (!_saveAssetsForEditor.TryGetValue(packedData.Data.Guid, out GameAsset? packedSaveData))
                {
                    GameLogger.Error($"Why has save slot {slot} not loaded in the editor?");
                    continue;
                }

                _activeSaveData = (SaveData)packedSaveData;
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

                    _currentSaveAssets.TryAdd(packedAsset.Guid, _saveAssetsForEditor[packedAsset.Guid]);
                }

                if (match)
                {
                    PendingSave = SerializeSaveAsync();
                    await PendingSave.Value;

                    asset.FileChanged = false;
                    break;
                }
            }

            _activeSaveData = null;
            _currentSaveAssets.Clear();
        }

        private void LoadEditorSettings()
        {
            string editorSettingsPath = Path.Join(SaveBasePath, EditorSettingsFileName);

            if (FileManager.Exists(editorSettingsPath))
            {
                EditorSettings = (EditorSettingsAsset)FileManager.DeserializeAsset<GameAsset>(editorSettingsPath)!;
            }

            // TODO: Is there a better way to verify if the settings match?
            EditorSettingsAsset settings = CreateEditorSettings();
            if (EditorSettings is null || EditorSettings.GetType() != settings.GetType())
            {
                GameLogger.Warning($"Didn't find {EditorSettingsFileName} file. Creating one.");

                EditorSettings = settings;
                EditorSettings.MakeGuid();
                SaveAsset(EditorSettings);
            }

            PopulateEditorSettings(EditorSettings);

            string gameProfilePath = FileHelper.GetPath(Path.Join(EditorSettings.SourceResourcesPath, GameProfileFileName));

            if (FileManager.Exists(gameProfilePath))
            {
                _gameProfile = (GameProfile)FileManager.DeserializeAsset<GameAsset>(gameProfilePath)!;
            }

            // Create a game profile, if none was provided from the base game or if the game
            // provides a different one.
            // TODO: Is there a better way to verify if the profile match?
            GameProfile profile = CreateGameProfile();
            if (_gameProfile is null || _gameProfile.GetType() != profile.GetType())
            {
                GameLogger.Warning($"Didn't find {GameProfileFileName} file. Creating one.");

                GameProfile = profile;
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
                saveAsset.TaggedForDeletion = true;

                SaveAsset(saveAsset);
            }

            _cachedFilteredAssetsWithImplementation.Clear();
        }

        internal GameAsset CreateNewAsset(Type type, string assetName)
        {
            GameAsset? asset = Activator.CreateInstance(type) as GameAsset;
            GameLogger.Verify(asset != null);

            asset.Name = GetNextName(type, assetName, EditorSettings.AssetNamePattern);

            AddAsset(asset);

            _cachedFilteredAssetsWithImplementation.Clear();
            return asset;
        }

        internal void SaveSettings()
        {
            // Saving editor settings
            {
                GameLogger.Verify(EditorSettings != null, "Cannot serialize a null EditorSettings");
                string? editorPath = EditorSettings.GetEditorAssetPath();
                if (editorPath is not null)
                {
                    FileManager.SaveSerialized<GameAsset>(EditorSettings, editorPath);
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
                string? profilePath = GameProfile.GetEditorAssetPath();
                if (profilePath is not null)
                {
                    FileManager.SaveSerialized<GameAsset>(GameProfile, profilePath);
                }
            }
        }

        /// <summary>
        /// Save a generic asset into our database.
        /// </summary>
        public void SaveAsset<T>(T asset) where T : GameAsset
        {
            if (asset.IsSavePacked)
            {
                // Asset is actually stored in the save data. In that case, save it through a different path.
                _ = SerializeSaveForAssetAsync(asset);

                return;
            }

            if (string.IsNullOrWhiteSpace(asset.FilePath))
            {
                // File has just been created and we need to name it.
                asset.FilePath = asset.Name + ".json";
            }

            string? sourcePath = asset.GetEditorAssetPath();
            string? binPath = asset.GetEditorAssetPath(useBinPath: true);
            if (sourcePath is null)
            {
                GameLogger.Error($"Unable to save asset of {typeof(T).Name}?");
                return;
            }

            // If the source resources or binaries path have not been initialized.
            if (!Directory.Exists(FileHelper.GetPath(_sourceResourcesDirectory)) || !Directory.Exists(FileHelper.GetPath(_binResourcesDirectory ?? "")))
            {
                GameLogger.Error($"Unable to save asset at path {_sourceResourcesDirectory}.");
                GameLogger.Error("Have you tried setting Game Source Path in \"Editor Profile\"?");
                return;
            }

            OnAssetRenamedOrAddedOrDeleted();

            // File is about to be synchronized, so it's not changed.
            asset.FileChanged = false;

            if (asset.Rename || asset.TaggedForDeletion)
            {
                if (asset.CanBeDeleted)
                {
                    if (!FileManager.DeleteFileIfExists(sourcePath))
                    {
                        // Right now, we will throw this on a rename or deleting without saving a file.
                        // TODO: Do we need to reenable this?
                        // GameLogger.Error($"Couldn't find file '{sourcePath}' to delete!");
                    }

                    if (binPath is not null)
                    {
                        _ = FileManager.DeleteFileIfExists(binPath);
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

                    sourcePath = asset.GetEditorAssetPath()!;
                    binPath = asset.GetEditorAssetPath(useBinPath: true);
                }
            }

            // Let's check if we are saving a file with a name that has already been taken.
            if (_database.TryGetValue(typeof(T), out HashSet<Guid>? guidAssetsOfType))
            {
                if (guidAssetsOfType.Count(
                    a => string.Equals(_allAssets[a].Name, asset.Name, StringComparison.OrdinalIgnoreCase) && _allAssets[a] != asset) > 0)
                {
                    // Since we already have an existing asset with the same name, create a new name for this.
                    asset.Name = GetNextName(typeof(T), asset.Name, EditorSettings.AssetNamePattern);
                    asset.FilePath = asset.Name + ".json";

                    sourcePath = asset.GetEditorAssetPath()!;
                    binPath = asset.GetEditorAssetPath(useBinPath: true);
                }
            }

            // Now that we know we have an actual valid path, create the relative path to this new file.
            // We save twice: one in source to persist and in bin to reflect in the executable.
            FileManager.CreateDirectoryPathIfNotExists(sourcePath);
            FileManager.SaveSerialized<GameAsset>(asset, sourcePath);

            if (binPath is not null)
            {
                FileManager.CreateDirectoryPathIfNotExists(binPath);
                FileManager.SaveSerialized<GameAsset>(asset, binPath);
            }

            // Also save any extra assets at this point.
            List<Guid>? saveAssetsOnSave = asset.AssetsToBeSaved();
            if (saveAssetsOnSave is not null)
            {
                foreach (Guid g in saveAssetsOnSave)
                {
                    GameAsset? a = TryGetAsset(g);
                    if (a is null)
                    {
                        continue;
                    }

                    SaveAsset(a);
                }
            }
        }

        private bool HasAssetOfName(Type type, string name)
        {
            if (!_database.ContainsKey(type))
            {
                return false;
            }
            HashSet<Guid> assets = _database[type];

            foreach (Guid g in assets)
            {
                string otherName = _allAssets[g].Name;
                if (string.Equals(otherName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public string GetNextName(Type type, string name, string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
            {
                throw new ArgumentException("The pattern must include an index place-holder like '{0}'", "pattern");
            }

            if (!HasAssetOfName(type, name))
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
                if (!HasAssetOfName(type, candidate))
                    return candidate;
            }

            GameLogger.Error("Couldn't find a valid asset name!");
            return "ERROR!!!";
        }

        public void SaveAllAssets()
        {
            foreach (var asset in _allAssets)
            {
                SaveAsset(asset.Value);
            }
        }

        protected override bool TryCompileShader(string path, [NotNullWhen(true)] out Effect? result)
        {
            result = null;

            string? assemblyPath = AppContext.BaseDirectory;
            if (assemblyPath is null)
            {
                // When publishing the game, this assembly won't be available as part of a path.
                return false;
            }

            string? fxcPath = ShaderHelpers.ProbeFxcPath();
            if (fxcPath is null)
            {
                GameLogger.Warning(
                    $$"""
                    Unable to find a valid shader path for fxc.exe. You have a couple of options:
                        - You may download DirectX 9: https://www.microsoft.com/en-us/download/details.aspx?id=6812 
                        - Install a Windows SDK version with Visual Studio
                        - Provide your custom path for fxc.exe at Editor Settings -> FxcPath (recommended for non-Windows OS)
                    This is not mandatory but the shaders won't compile until this is set!
                    """);
                return false;
            }

            if (!File.Exists(fxcPath))
            {
                GameLogger.Warning("How did we return an invalid fxc.exe path from our probe?");
                return false;
            }

            if (!Directory.Exists(EditorSettings.RawResourcesPath) || !Directory.Exists(EditorSettings.GameSourcePath))
            {
                GameLogger.Log($"Skipped compiling shader '{path}', no directory found at {FileHelper.GetPath(EditorSettings.RawResourcesPath)}.");
                return false;
            }

            string sourceFile = Path.GetFullPath(Path.Join(EditorSettings.RawResourcesPath, GameProfile.ShadersPath, "src", $"{path}.fx"));
            if (!File.Exists(sourceFile))
            {
                GameLogger.Log($"Skipped compiling shader '{path}', no source shader found at {sourceFile}.");
                return false;
            }

            string binOutputFilePath = FileHelper.GetPath(PackedBinDirectoryPath, string.Format(ShaderRelativePath, path));
            
            // NOTE: In Unix systems, absolute paths usually start with a slash:/
            // which causes the fxc compiler to mistakenly recognize the path parameters as fxc options,
            // so I replaced the slash at the beginning of the directory with a backslash
            string paramBinOutputFilePath = binOutputFilePath;
            if (paramBinOutputFilePath.StartsWith('/'))
            {
                paramBinOutputFilePath = $"\\{paramBinOutputFilePath[1..]}";
            }

            string paramSourceFile = sourceFile;
            if (paramSourceFile.StartsWith('/'))
            {
                paramSourceFile = $"\\{paramSourceFile[1..]}";
            }
            
            string arguments = $"/nologo /T fx_2_0 {paramSourceFile} /Fo {paramBinOutputFilePath}";

            // The tool needs that the output directory exists.
            FileManager.CreateDirectoryPathIfNotExists(binOutputFilePath);

            bool success;
            string stderr;

            try
            {
                if (OperatingSystem.IsWindows())
                {
                    success = ExternalTool.Run(fxcPath, arguments, out string _, out stderr) == 0;
                }
                else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
                {
                    string? cmdWine64 = ExternalTool.FindCommand("wine64");
                    if (cmdWine64 is not null)
                    {
                        success = ExternalTool.Run(cmdWine64, $"{fxcPath} {arguments}", out string _, out stderr) == 0;
                    }
                    else
                    {
                        GameLogger.Warning(
                            $$"""
                            Unable to find a valid Wine installation path. 
                            Please install Wine to run the fxc compiler and make sure wine is added to PATH!
                            """);

                        return false;
                    }
                }
                else
                {
                    GameLogger.Error("Unsupported operating system for compiling the shaders! We should implement this...?");
                    return false;
                }
            }
            catch (Exception ex)
            {
                GameLogger.Error($"Error running shader command: {ex.Message}");
                return false;
            }

            if (!success)
            {
                GameLogger.Error(stderr);
                Debugger.Log(2, "Shader Compile Error", stderr);

                return false;
            }

            // Copy the output to the source directory as well.
            string sourceOutputFilePath = Path.Join(PackedSourceDirectoryPath, string.Format(ShaderRelativePath, path));

            FileManager.CreateDirectoryPathIfNotExists(sourceOutputFilePath);
            File.Copy(binOutputFilePath, sourceOutputFilePath, true);

            result = new Effect(Game.GraphicsDevice, File.ReadAllBytes(binOutputFilePath));
            return true;
        }

        protected override void OnAfterPreloadLoaded()
        {
            // Load editor assets so the editor is *clean*.
            string editorPath = FileHelper.GetPath(_binResourcesDirectory, GameProfile.AssetResourcesPath, GameProfile.GenericAssetsPath, "Generated", "editor");

            LoadAssetsAtPath(editorPath, hasEditorPath: true);
            SkipLoadingAssetsAt(editorPath);

            LoadTextureManagers();
        }

        /// <summary>
        /// Called once all the content has been loaded.
        /// This is separate since it has to be called on the main thread.
        /// </summary>
        public void AfterContentLoaded()
        {
            ApplyEventManagerChangesIfNeeded();

            ReloadDialogs();
            FlushResourceImporters();

            InitializeShaderFileSystemWather();

            _cachedFilteredAssetsWithImplementation.Clear();
            CallAfterLoadContent = false;
        }

        /// <summary>
        /// Called after the content was loaded back from the main thread.
        /// </summary>
        public override void AfterContentLoadedFromMainThread()
        {
            if (_gameProfile?.PreloadTextures is not true)
            {
                return;
            }

            base.AfterContentLoadedFromMainThread();
        }

        private void LoadTextureManagers()
        {
            _cursorTextureManager ??= new(Game.Profile.EditorAssets);
        }

        public override void Dispose()
        {
            base.Dispose();

            _cursorTextureManager?.Dispose();
            _imGuiTextureManager?.Dispose();
        }

        protected override void OnAssetLoadError(GameAsset asset)
        {
            if (EditorSettings.SaveDeserializedAssetOnError)
            {
                GameLogger.Log($"Saving asset '{asset.Name}' after loading error.");
                SaveAsset(asset);

                return;
            }

            GameLogger.Warning($"Set EditorSettings.SaveDeserializedAssetOnError to change how asset errors are handled.");
        }

        private static EditorSettingsAsset CreateEditorSettings()
        {
            string name = EditorSettingsFileName;
            string gameSourcePath = $"../../../../{Game.Data.GameDirectory}";

            return Architect.Game?.CreateEditorSettings(name, gameSourcePath) ??
                    new EditorSettingsAsset(name, gameSourcePath);
        }

        private static void PopulateEditorSettings(EditorSettingsAsset settings)
        {
            settings.FilePath = EditorSettingsFileName;
        }

        public override void OnAssetRenamedOrAddedOrDeleted()
        {
            // Only apply this if the active scene is the editor scene (probably).
            if (Architect.Instance.ActiveScene is EditorScene scene)
            {
                scene.OnAssetRenamedOrAddedOrDeleted();
            }

            _cachedFilteredAssetsWithImplementation.Clear();
        }
    }
}
