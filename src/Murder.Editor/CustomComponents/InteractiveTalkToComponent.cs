using InstallWizard;
using InstallWizard.Data.Dialogs;
using InstallWizard.DebugUtilities;
using InstallWizard.Interactions;
using Editor.CustomEditors;
using Editor.Gui;
using Editor.Reflection;
using Editor.Util;
using ImGuiNET;
using Bang.Interactions;
using Bang.StateMachines;
using System.Collections.Immutable;

namespace Editor.CustomComponents
{
    [CustomComponentOf(typeof(InteractiveComponent<TalkToInteraction>))]
    public class InteractiveTalkToComponent : InteractiveComponent
    {
        protected override bool DrawInteraction(object? interaction)
        {
            if (interaction is not TalkToInteraction talkToInteraction)
            {
                throw new ArgumentException(nameof(interaction));
            }

            using TableMultipleColumns table = new($"talk_to_interaction", 
                flags: ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter, 
                (-1, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));
            
            if (DrawSituationField(talkToInteraction, out int result))
            {
                EditorMember situationField = typeof(TalkToInteraction).
                    TryGetFieldForEditor(nameof(TalkToInteraction.Situation))!;

                situationField.SetValue(interaction, result);

                return true;
            }
            
            return DrawMembersForTarget(interaction, TalkToMembers());
        }

        /// <summary>
        /// Draw field for <see cref="TalkToInteraction.Situation"/> in <see cref="TalkToInteraction"/>.
        /// </summary>
        private bool DrawSituationField(in TalkToInteraction interaction, out int result)
        {
            result = 0;

            ImGui.TableNextRow();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            
            // -- Field --
            ImGui.Text("Situation:");

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            if (Game.Data.TryGetAsset<CharacterAsset>(interaction.Character) is not CharacterAsset asset)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "Choose a valid character first!");
                return false;
            }
            
            ImmutableArray<(string name, int id)> situations = CharacterEditor.FetchAllSituations(asset);
            string[] situationNames = situations.Select(s => s.name).ToArray();
            
            if (situationNames.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "No situation to go next?");
                return false;
            }

            int item = 0;
            if (asset.TryFetchSituation(interaction.Situation) is Situation target)
            {
                item = situations.IndexOf((target.Name, target.Id));
            }
            else
            {
                // Set a initial value for this.
                result = situations[item].id;

                return true;
            }

            if (ImGui.Combo($"##talto_situation", ref item, situationNames, situationNames.Length))
            {
                result = situations[item].id;
                return true;
            }

            ImGui.PopItemWidth();
            return false;
        }

        private IList<(string, EditorMember)>? _cachedMembers = null;

        private IList<(string, EditorMember)> TalkToMembers()
        {
            if (_cachedMembers is null)
            {
                HashSet<string> skipFields = new string[] { "Situation" }.ToHashSet();
                
                _cachedMembers ??= typeof(TalkToInteraction).GetFieldsForEditor()
                    .Where(t => !skipFields.Contains(t.Name)).Select(f => (f.Name, f)).ToList();
            }

            return _cachedMembers;
        }
    }
}
