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

        internal override ValueTask DrawEditor()
        {

            ImGui.ShowStyleEditor();

            return default;
        }

        internal override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _target = (EditorSettingsAsset)target;
        }
    }
}
