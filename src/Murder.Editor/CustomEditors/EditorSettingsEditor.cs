using ImGuiNET;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(EditorSettingsAsset))]
    internal class EditorSettingsEditor : CustomEditor
    {
        public override object Target => _target;
        private EditorSettingsAsset _target = null!;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext _, object target, bool overwrite)
        {
            _target = (EditorSettingsAsset)target;
        }

        public override void DrawEditor()
        {
            bool fileChanged = false;

            if (ImGui.BeginTabBar("EditorBar"))
            {
                if (ImGui.BeginTabItem("Settings"))
                {
                    ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                    ImGui.BeginChild("editor_settings_child", ImGui.GetContentRegionAvail()
                        - new System.Numerics.Vector2(0, 5));

                    fileChanged = DrawSettingsTab();

                    ImGui.EndChild();
                    ImGui.PopStyleColor();

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Stage"))
                {
                    ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                    ImGui.BeginChild("editor_settings_child", ImGui.GetContentRegionAvail()
                        - new System.Numerics.Vector2(0, 5));

                    fileChanged = DrawStageTab();

                    ImGui.EndChild();
                    ImGui.PopStyleColor();

                    ImGui.EndTabItem();
                }
            }

            _target.FileChanged |= fileChanged;
        }

        private bool DrawSettingsTab()
        {
            using TableMultipleColumns table = new("editor_settings", flags: ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInnerH,
                (ImGuiTableColumnFlags.WidthFixed, -1), (ImGuiTableColumnFlags.WidthStretch, -1));

            return CustomComponent.DrawMembersForTarget(Target, FetchMembers());
        }

        private bool DrawStageTab()
        {
            if (FeatureAssetEditor.DrawSystemsEditor(StageHelpers.FetchEditorTypeSystems(), out var updatedSystems))
            {
                GameLogger.Warning("We didn't implement tweaking default systems settings... yet.");
                return true;
            }

            return false;
        }

        private IList<(string, EditorMember)>? _cachedMembers = null;

        /// <summary>
        /// Fetch all members which will go through normal editor serialization.
        /// </summary>
        /// <returns></returns>
        private IList<(string, EditorMember)> FetchMembers()
        {
            if (_cachedMembers is null)
            {
                HashSet<string> skipFields = new string[] { "_editorSystems" }.ToHashSet();

                _cachedMembers ??= typeof(EditorSettingsAsset).GetFieldsForEditor()
                    .Where(t => !skipFields.Contains(t.Name)).Select(f => (f.Name, f)).ToList();
            }

            return _cachedMembers;
        }
    }
}