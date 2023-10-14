using Murder.Data;
using Murder.Editor.Assets;

namespace Murder.Editor.Importers
{
    [ImporterSettings(FilterType.OnlyTheseFolders, new[] { RelativeDirectory }, new[] { ".aseprite", ".ase" })]
    internal class EditorAsepriteImporter(EditorSettingsAsset editorSettings) : AsepriteImporter(editorSettings)
    {
        protected override AtlasId Atlas => AtlasId.Editor;

        private const string RelativeDirectory = "editor";

        public override string RelativeSourcePath => RelativeDirectory;

        public override string RelativeOutputPath => string.Empty;

        public override string RelativeDataOutputPath => string.Empty;
    }
}