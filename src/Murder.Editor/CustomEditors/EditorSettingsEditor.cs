using ImGuiNET;
using Murder.Editor.Assets;
using Murder.ImGuiExtended;

namespace Murder.Editor.CustomEditors
{
    //[CustomEditorOf(typeof(EditorSettings))]
    internal class EditorSettingsEditor : CustomEditor
    {
        public override object Target => _target;
        private EditorSettingsAsset _target = null!;

        public override ValueTask DrawEditor()
        {

            ImGui.ShowStyleEditor();

            return default;
        }

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _target = (EditorSettingsAsset)target;
        }
    }
}
