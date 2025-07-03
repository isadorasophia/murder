
using Murder;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Importers;
using Murder.Utilities;

[ImporterSettings(FilterType.OnlyTheseFolders, [RelativeDirectory], [".aseprite", ".ase"])]
internal class CutsceneAsepriteImporter(EditorSettingsAsset editorSettings) : ResourceImporter(editorSettings)
{
    private const string RelativeDirectory = "cutscene_images";

    public override string RelativeSourcePath => RelativeDirectory;

    public override string RelativeOutputPath => string.Empty;

    public override string RelativeDataOutputPath => string.Empty;

    public override bool SupportsAsyncLoading => true;

    public override string GetSourcePackedAtlasDescriptorPath() => 
        GetSourcePackedAtlasDescriptorPath(Atlas.GetDescription());

    protected AtlasId Atlas => AtlasId.Cutscenes;

    /// <summary>
    /// List of all sub atlas directories.
    /// </summary>
    private readonly Dictionary<string, AsepriteImporter> _importers = [];

    public override async ValueTask LoadStagedContentAsync(bool reload, bool skipIfNoChangesFound)
    {
        if (AllFiles.Count == 0)
        {
            return;
        }

        foreach (AsepriteImporter importer in _importers.Values)
        {
            if (skipIfNoChangesFound && !importer.HasChanges)
            {
                continue;
            }

            await importer.LoadStagedContentAsync(reload, skipIfNoChangesFound);
        }

        AllFiles.Clear();
        ChangedFiles.Clear();
    }

    protected override void StageFileImpl(string file, bool changed) 
    {
        if (Path.GetDirectoryName(file) is not string directory)
        {
            GameLogger.Error($"Invalid directory for {file}.");
            return;
        }

        string name = new DirectoryInfo(directory).Name;
        if (name is null)
        {
            return;
        }

        if (!_importers.TryGetValue(name, out AsepriteImporter? importer))
        {
            importer = new SubCutsceneAsepriteImporter(_editorSettings) { SubAtlasId = name };
            _importers[name] = importer;
        }

        importer.StageFile(file, changed);
    }

    internal override void Flush()
    {
        foreach (AsepriteImporter importer in _importers.Values)
        {
            importer.Flush();
        }
    }

    private string GetSourcePackedAtlasDescriptorPath(string atlasName)
    {
        // this is okay for the descriptor!
        string atlasSourceDirectoryPath = Path.Join(GetSourcePackedPath(), Game.Profile.AtlasFolderName);
        return Path.Join(atlasSourceDirectoryPath, $"{atlasName}.json");
    }
}