using Murder.Data;
using Murder.Editor.Assets;

namespace Murder.Editor.Importers;

/// <summary>
/// This will be manually imported by <see cref="CutsceneAsepriteImporter"/>.
/// </summary>
[ImporterSettings(FilterType.Ignore)]
internal class SubCutsceneAsepriteImporter(EditorSettingsAsset editorSettings) : AsepriteImporter(editorSettings)
{
    protected override AtlasId Atlas => AtlasId.Cutscenes;

    private const string RelativeDirectory = "cutscene_images";

    public override string RelativeSourcePath => RelativeDirectory;

    public override string RelativeOutputPath => string.Empty;

    public override string RelativeDataOutputPath => string.Empty;
}