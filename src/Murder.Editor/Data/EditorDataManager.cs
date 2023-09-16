﻿using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Data.Graphics;
using Murder.Editor.EditorCore;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Serialization;
using static Murder.Editor.Data.Graphics.FontLookup;

namespace Murder.Editor.Data
{
    public partial class EditorDataManager : GameDataManager
    {
        /// <summary>
        /// Initialized in <see cref="Init(string)"/>.
        /// </summary>
        public EditorSettingsAsset EditorSettings { get; private set; } = null!;

        public const string EditorSettingsFileName = "editor_config";

        private string AssetsDataPath => FileHelper.GetPath(Path.Join(EditorSettings.BinResourcesPath, GameProfile.AssetResourcesPath));

        private readonly Dictionary<Guid, GameAsset> _saveAssetsForEditor = new();

        public ImmutableArray<GameAsset> GetAllSaveAssets() => _saveAssetsForEditor.Values.ToImmutableArray();

        public ImmutableArray<string> HiResImages;

        private string _sourceResourcesDirectory = "resources";

        protected string? _assetsSourceDirectoryPath;

        public string AssetsSourceDirectoryPath => _assetsSourceDirectoryPath!;

        private string? _packedSourceDirectoryPath;

        public string PackedSourceDirectoryPath => _packedSourceDirectoryPath!;

        private CursorTextureManager? _cursorTextureManager = null;
        public CursorTextureManager? CursorTextureManager => _cursorTextureManager;

        private readonly ImGuiTextureManager _imGuiTextureManager = new();
        public ImGuiTextureManager ImGuiTextureManager => _imGuiTextureManager;

        public EditorDataManager(IMurderGame? game) : base(game) { }

        [MemberNotNull(
            nameof(_assetsSourceDirectoryPath),
            nameof(_packedSourceDirectoryPath))]
        public override void Init(string _ = "")
        {
            LoadEditorSettings();

            base.Init(EditorSettings.BinResourcesPath);

            _sourceResourcesDirectory = EditorSettings.SourceResourcesPath;

            _assetsSourceDirectoryPath = FileHelper.GetPath(_sourceResourcesDirectory, GameProfile.AssetResourcesPath);
            _packedSourceDirectoryPath = FileHelper.GetPath(EditorSettings.SourcePackedPath);

            EditorSettings.FilePath = EditorSettingsFileName;
            EditorSettings.Name = "Editor Settings";

            GameProfile.FilePath = GameProfileFileName;
            GameProfile.Name = "Game Profile";
        }

        /// <summary>
        /// List of contents which will be "automatically" updated once the windows enters foreground.
        /// This should be relatively lightweight operations that can check for a last modified cache, for example.
        /// </summary>
        public void ReloadOnWindowForeground()
        {
            if (Architect.Instance.ActiveScene is EditorScene scene)
            {
                scene.ReloadOnWindowForeground();
            }
        }

        public override void LoadContent()
        {
            // Convert TTF Fonts
            ConvertTTFToSpriteFont();

            // Pack assets (this will be pre-packed for the final game)
            PackAtlas();

            base.LoadContent();

            RefreshAfterSave();
        }

        internal void ConvertTTFToSpriteFont()
        {
            string ttfFontsPath = FileHelper.GetPath(EditorSettings.RawResourcesPath, Game.Profile.FontsPath);
            if (!Directory.Exists(ttfFontsPath))
            {
                // No font directory, so skip.
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
                    if (FontImporter.GenerateFontJsonAndPng(info.Index, ttfFile, info.Size, fontName))
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

        internal void PackAtlas()
        {
            DisposeAtlases();

            // Cleanup generated assets folder
            FileHelper.DeleteDirectoryIfExists(FileHelper.GetPath(Path.Join(Game.Profile.GenericAssetsPath, "Generated")));

            if (!Directory.Exists(FileHelper.GetPath(EditorSettings.GameSourcePath)))
            {
                GameLogger.Warning($"Please specify a valid \"Game Source Path\" in \"Editor Settings\". Unable to find the resources to build the atlas from.");
                return;
            }

            string sourcePackedTarget = FileHelper.GetPath(EditorSettings.SourcePackedPath);
            if (!Directory.Exists(sourcePackedTarget))
            {
                GameLogger.Warning($"Didn't find packed folder. Creating one.");
                FileHelper.GetOrCreateDirectory(sourcePackedTarget);
            }

            string binPackedTarget = FileHelper.GetPath(EditorSettings.BinResourcesPath);
            FileHelper.GetOrCreateDirectory(binPackedTarget);

            string editorImagesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath, "/editor/");
            Processor.Pack(editorImagesPath, sourcePackedTarget, binPackedTarget,
                AtlasId.Editor, !Architect.EditorSettings.OnlyReloadAtlasWithChanges);

            // Pack the regular pixel art atlasses
            string rawImagesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath, "/images/");
            Processor.Pack(rawImagesPath, sourcePackedTarget, binPackedTarget,
                AtlasId.Gameplay, !Architect.EditorSettings.OnlyReloadAtlasWithChanges);

            // Copy the lost textures to the no_atlas folder
            var noAtlasRawResourceDirecotry = FileHelper.GetPath(EditorSettings.RawResourcesPath, "no_atlas");
            if (!Directory.Exists(noAtlasRawResourceDirecotry))
            {
                return;
            }

            string sourceNoAtlasPath = FileHelper.GetPath(Path.Join(EditorSettings.SourceResourcesPath, "/images/"));
            FileHelper.DeleteContent(sourceNoAtlasPath, deleteRootFiles: true);
            FileHelper.GetOrCreateDirectory(sourceNoAtlasPath);

            foreach (var image in Directory.GetFiles(noAtlasRawResourceDirecotry))
            {
                var target = Path.Join(sourceNoAtlasPath, Path.GetRelativePath(noAtlasRawResourceDirecotry, image));
                File.Copy(image, target);

                // GameLogger.Log($"Copied {image} to {target}");
            }

            // Make sure we are sending this to the bin folder!
            string noAtlasImageBinPath = FileHelper.GetPath(Path.Join(EditorSettings.BinResourcesPath, "/images/"));
            FileHelper.DirectoryDeepCopy(sourceNoAtlasPath, noAtlasImageBinPath);
        }

        public override void RefreshAtlas()
        {
            base.RefreshAtlas();
            ScanHighResImages();
        }

        private void ScanHighResImages()
        {
            string fullRawResourcesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath);
            if (!Directory.Exists(fullRawResourcesPath))
            {
                GameLogger.Log($"Unable to find raw resources path at {FileHelper.GetPath(fullRawResourcesPath)}. " +
                    $"Use this directory for images that will be built into the atlas.");

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

            foreach (GameAsset asset in FetchAssetsAtPath(SaveBasePath, stopOnFailure: true))
            {
                _saveAssetsForEditor[asset.Guid] = asset;
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

                EditorSettings = new EditorSettingsAsset(Game.Data.GameDirectory);
                EditorSettings.MakeGuid();
                SaveAsset(EditorSettings);
            }

            string gameProfilePath = FileHelper.GetPath(Path.Join(EditorSettings.SourceResourcesPath, GameProfileFileName));

            if (FileHelper.Exists(gameProfilePath))
            {
                _gameProfile = FileHelper.DeserializeAsset<GameProfile>(gameProfilePath)!;
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
                string? editorPath = EditorSettings.GetEditorAssetPath();
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
                string? profilePath = GameProfile.GetEditorAssetPath();
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

            string? sourcePath = asset.GetEditorAssetPath();
            string? binPath = asset.GetEditorAssetPath(useBinPath: true);
            if (sourcePath is null)
            {
                GameLogger.Error($"Unable to save asset of {typeof(T).Name}?");
                return;
            }

            FileHelper.CreateDirectoryPathIfNotExists(sourcePath);
            if (binPath != null && !string.IsNullOrEmpty(_binResourcesDirectory))
            {
                FileHelper.CreateDirectoryPathIfNotExists(FileHelper.GetPath(_binResourcesDirectory));
            }

            // If the source is invalid and either the source resources or binaries path have not been initialized.
            string fullResourcesPath = FileHelper.GetPath(_sourceResourcesDirectory);
            if (binPath != null && !Directory.Exists(fullResourcesPath))
            {
                GameLogger.Error($"Unable to save asset to resources directory at '{fullResourcesPath}'.");
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
                        // Right now, we will throw this on a rename or deleting without saving a file.
                        // TODO: Do we need to reenable this?
                        // GameLogger.Error($"Couldn't find file '{sourcePath}' to delete!");
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
                    asset.Name = GetNextName(asset.Name, EditorSettings.AssetNamePattern);
                    asset.FilePath = asset.Name + ".json";

                    sourcePath = asset.GetEditorAssetPath()!;
                    binPath = asset.GetEditorAssetPath(useBinPath: true);
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

            var filesCopied = FileHelper.DirectoryDeepCopy(
                AssetsDataPath,
                targetBinPath);
            GameLogger.Log($"Content updated from {AssetsDataPath} to {targetBinPath} (total files copied: {filesCopied})");
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

            string mgfxcPath = Path.Combine(assemblyPath, "mgfxc.dll");
            if (!File.Exists(mgfxcPath))
            {
                GameLogger.Log($"Couldn't find mgfxc.dll to compile shader.");
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
            string arguments = "\"" + mgfxcPath + "\" \"" + sourceFile + "\" \"" + binOutputFilePath + "\" /Profile:OpenGL /Debug";

            bool success;
            string stderr;

            try
            {
                success = ExternalTool.Run("dotnet", arguments, out string _, out stderr) == 0;
            }
            catch (Exception ex)
            {
                GameLogger.Error($"Error running dotnet shader command: {ex.Message}");
                return false;
            }

            if (success)
            {
                // Copy the output to the source directory as well.
                string sourceOutputFilePath = Path.Join(PackedSourceDirectoryPath, string.Format(ShaderRelativePath, path));

                FileHelper.CreateDirectoryPathIfNotExists(sourceOutputFilePath);
                File.Copy(binOutputFilePath, sourceOutputFilePath, true);

                // GameLogger.Log($"Sucessfully compiled {name}.fx");
            }
            else
            {
                GameLogger.Error(stderr);
                Debugger.Log(2,"Shader Compile Error", stderr);
            }

            CompiledEffectContent compiledEffect = new CompiledEffectContent(File.ReadAllBytes(binOutputFilePath));
            result = new Effect(Game.GraphicsDevice, compiledEffect.GetEffectCode());

            return true;
        }

        /// <summary>
        /// Called once all the content has been loaded.
        /// This is separate since it has to be called on the main thread.
        /// </summary>
        public void AfterContentLoaded()
        {
            LoadTextureManagers();
            ReloadDialogs();

            CallAfterLoadContent = false;
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

    }
}
