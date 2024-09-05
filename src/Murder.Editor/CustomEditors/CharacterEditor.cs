using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Core.Dialogs;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Stages;
using Murder.Editor.Systems;
using Murder.Editor.Utilities.Attributes;

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
                Stage stage = new(imGuiRenderer, renderContext, Stage.StageType.None);

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

            if (_script.Situations.Count != 0)
            {
                info.HelperId = stage.AddEntityWithoutAsset();
                info.ActiveSituation = _script.Situations.First().Key;

                stage.AddOrReplaceComponentOnEntity(info.HelperId, new DialogueNodeEditorComponent(_script.Situations.First().Value));
            }

            stage.ShowInfo = false;
            stage.EditorHook.OnNodeSelected += SelectNode;
        }

        public override void UpdateEditor()
        {
            if (_script is null || !ActiveEditors.TryGetValue(_script.Guid, out var info))
            {
                return;
            }

            Stage stage = info.Stage;
            stage.Update();
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

                float totalHeight = ImGui.GetContentRegionAvail().Y - 100;

                DrawSpeaker();
                DrawNotes();
                DrawSituations(info, 150);
                DrawCurrentDialog(info, totalHeight - 200);

                ImGui.EndTable();
            }
        }

        private void DrawSpeaker()
        {
            GameLogger.Verify(_script is not null);

            ImGui.Text("\uf508");
            ImGuiHelpers.HelpTooltip("Default speaker for this dialog");

            using (TableTwoColumns table = new($"fields_{_script.Guid}", ImGuiTableFlags.SizingFixedSame))
            {
                _script.FileChanged |= CustomComponent.DrawMembersForTarget(_script, MembersForCharacter.Value);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Portrait:");
                ImGuiHelpers.HelpTooltip("Default portrait chosen for this speaker");
                ImGui.TableNextColumn();

                if (Game.Data.TryGetAsset<SpeakerAsset>(_script.Owner) is SpeakerAsset speaker && 
                    MemberForPortrait.Value is EditorMember portraitMember)
                {
                    if (_script.Portrait is null)
                    {
                        portraitMember.SetValue(_script, speaker.DefaultPortrait);
                        _script.FileChanged = true;
                    }

                    int index = 0;

                    string[] keys = speaker.Portraits.Keys.ToArray();
                    foreach (string key in keys)
                    {
                        if (key == _script.Portrait)
                        {
                            break;
                        }

                        index++;
                    }

                    ImGui.PushItemWidth(-1);
                    bool modified = ImGui.Combo($"##speaker_portrait_{_script.Guid}", ref index, keys, keys.Length);
                    if (modified)
                    {
                        portraitMember.SetValue(_script, keys[index]);
                        _script.FileChanged = true;
                    }
                    ImGui.PopItemWidth();
                }
            }

            ImGui.Spacing();
        }

        private void DrawNotes()
        {
            GameLogger.Verify(_script is not null);

            // == Notes ==
            if (ImGuiHelpers.BlueIcon('\uf10d', "notes_toggle"))
            {
                ImGui.OpenPopup("notes_description");
            }

            string? notes = _script.LocalizationNotes;
            ImGuiHelpers.HelpTooltip("Update description");

            if (!string.IsNullOrEmpty(notes))
            {
                ImGui.SameLine();
                ImGui.TextWrapped(notes);
            }

            ImGui.Dummy(new System.Numerics.Vector2(0, 10));

            if (ImGui.BeginPopup("notes_description"))
            {
                string text = notes ?? string.Empty;

                if (ImGui.InputText("##notes_name", ref text, 1024, ImGuiInputTextFlags.AutoSelectAll))
                {
                    if (MemberForNotes.Value is EditorMember notesMember)
                    {
                        notesMember.SetValue(_script, text);
                        _script.FileChanged = true;
                    }
                }

                if (ImGui.Button("Ok!") || Game.Input.Pressed(MurderInputButtons.Submit))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        private void DrawSituations(ScriptInformation info, float height)
        {
            GameLogger.Verify(_script is not null);

            ImGui.Text("\uf02d");
            ImGuiHelpers.HelpTooltip("List of situations described in the character script");

            ImGui.BeginChild("situations_table", new System.Numerics.Vector2(-1, height));
            ImGui.TreePush("##label");

            foreach ((string id, Situation situation) in _script.Situations.OrderBy(s => s.Value.Id))
            {
                if (PrettySelectableWithIcon($"{situation.Name}", selectable: true, info.ActiveSituation == id))
                {
                    SwitchSituation(info, situation);
                }
            }

            ImGui.TreePop();
            ImGui.EndChild();
        }

        private void SwitchSituation(ScriptInformation info, Situation situation)
        {
            info.ActiveSituation = situation.Name;

            info.Stage.AddOrReplaceComponentOnEntity(info.HelperId, new DialogueNodeEditorComponent(situation));
            info.Stage.EditorHook.SelectedNode = info.ActiveDialog;
        }

        public override void ReloadEditor()
        {
            foreach ((Guid guid, ScriptInformation info) in ActiveEditors)
            {
                // we cannot guarantee or use any value of _script here;
                CharacterAsset? asset = Game.Data.TryGetAsset<CharacterAsset>(guid);
                if (asset is not null && !asset.Situations.ContainsKey(info.ActiveSituation) && asset.Situations.Count > 0)
                {
                    SwitchSituation(info, asset.Situations.First().Value);
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