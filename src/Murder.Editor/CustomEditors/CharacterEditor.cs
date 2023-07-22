using ImGuiNET;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Core.Graphics;
using Murder.Assets;
using Murder.Editor.Stages;
using Murder.Diagnostics;
using Murder.Editor.Utilities.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Systems;
using Murder.Editor.CustomComponents;
using Murder.Core.Dialogs;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(CharacterAsset))]
    public partial class CharacterEditor : CustomEditor
    {
        /// <summary>
        /// Tracks the dialog system editors across different guids.
        /// </summary>
        protected Dictionary<Guid, ScriptInformation> ActiveEditors { get; private set; } = new();

        private CharacterAsset? _script;

        public override object Target => _script!;
        
        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext renderContext, object target, bool overwrite)
        {
            _script = (CharacterAsset)target;

            if (!ActiveEditors.ContainsKey(_script.Guid))
            {
                Stage stage = new(imGuiRenderer, renderContext);

                ScriptInformation info = new(stage);
                ActiveEditors[_script.Guid] = info;

                InitializeStage(stage, info);
            }
        }

        private void InitializeStage(Stage stage, ScriptInformation info)
        {
            GameLogger.Verify(_script is not null);

            // Activate dialog system here:
            stage.ActivateSystemsWith(enable: true, typeof(DialogueEditorAttribute));
            stage.ToggleSystem(typeof(EntitiesSelectorSystem), false);

            if (_script.Situations.Length != 0)
            {
                info.HelperId = stage.AddEntityWithoutAsset();
                info.ActiveSituation = 0;

                stage.AddOrReplaceComponentOnEntity(info.HelperId, new DialogueNodeEditorComponent(_script.Situations[0]));
            }

            stage.ShowInfo = false;
            stage.EditorHook.OnNodeSelected += SelectNode;
        }

        public override void DrawEditor()
        {
            GameLogger.Verify(_script is not null);

            if (!ActiveEditors.TryGetValue(_script.Guid, out var info))
            {
                GameLogger.Warning("Unitialized stage for particle editor?");
                return;
            }

            Stage stage = info.Stage;

            if (ImGui.BeginTable("script_table", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthStretch, -1f, 1);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthFixed, 600, 0);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                if (ActiveEditors.ContainsKey(_script.Guid))
                {
                    stage.EditorHook.DrawSelection = false;
                    stage.Draw();
                }

                ImGui.TableNextColumn();

                float totalHeight = ImGui.GetWindowContentRegionMax().Y - 100;

                DrawSpeaker();
                DrawSituations(info, 150);
                DrawCurrentDialog(info, totalHeight  - 150);

                ImGui.EndTable();
            }
        }

        private void DrawSpeaker()
        {
            GameLogger.Verify(_script is not null);

            ImGui.Text("\uf508");
            ImGuiHelpers.HelpTooltip("Default speaker for this dialog");

            using (TableTwoColumns table = new($"fields_{_script.Guid}"))
            {
                _script.FileChanged |= CustomComponent.DrawMembersForTarget(_script, MembersForCharacter.Value);
            }

            ImGui.Spacing();
        }
        
        private void DrawSituations(ScriptInformation info, float height)
        {
            GameLogger.Verify(_script is not null);

            ImGui.Text("\uf02d");
            ImGuiHelpers.HelpTooltip("List of situations described in the character script");

            ImGui.BeginChild("situations_table", new System.Numerics.Vector2(-1, height));
            ImGui.TreePush("##label");

            for (int i = 0; i < _script.Situations.Length; i++)
            {
                var situation = _script.Situations[i];
                if (PrettySelectableWithIcon($"{situation.Name}", info.ActiveSituation == i))
                {
                    SwitchSituation(info, situation);
                }
            }

            ImGui.TreePop();
            ImGui.EndChild();
        }

        private void SwitchSituation(ScriptInformation info, Situation situation)
        {
            info.ActiveSituation = situation.Id;

            info.Stage.AddOrReplaceComponentOnEntity(info.HelperId, new DialogueNodeEditorComponent(situation));
            info.Stage.EditorHook.SelectedNode = info.ActiveDialog;
        }

        public override void ReloadEditor()
        {
            foreach ((Guid guid, ScriptInformation info) in ActiveEditors)
            {
                // we cannot guarantee or use any value of _script here;
                CharacterAsset? asset = Game.Data.TryGetAsset<CharacterAsset>(guid);
                if (asset is not null)
                {
                    SwitchSituation(info, asset.Situations[info.ActiveSituation]);
                }
            }
        }

        public override void CloseEditor(Guid target)
        {
            if (ActiveEditors.TryGetValue(target, out ScriptInformation? info))
            {
                info.Stage.Dispose();
            }

            ActiveEditors.Remove(target);
        }
    }
}
