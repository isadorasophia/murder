using Murder.Data;
using Murder.Editor.Assets;

namespace Murder.Editor.Importers
{
    [ImporterSettings(FilterType.OnlyTheseFolders, new[] { RelativeDirectory }, new[] { ".aseprite", ".ase" })]
    internal class GameplayAsepriteImporter(EditorSettingsAsset editorSettings) : AsepriteImporter(editorSettings)
    {
        protected override AtlasId Atlas => AtlasId.Gameplay;

        private const string RelativeDirectory = "custcene_images";

        public override string RelativeSourcePath => RelativeDirectory;

        public override string RelativeOutputPath => string.Empty;

        public override string RelativeDataOutputPath => string.Empty;
    }
}