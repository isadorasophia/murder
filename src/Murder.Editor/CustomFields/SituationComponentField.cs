using ImGuiNET;
using Murder.Components;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Editor.CustomEditors;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Interactions;
using System.Collections.Immutable;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(SituationComponent), priority: 10)]
    internal class SituationComponentField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            SituationComponent situation = (SituationComponent)fieldValue!;

            if (DrawSituationField(situation.Character, situation.Situation, out int result))
            {
                EditorMember situationField = typeof(SituationComponent).
                    TryGetFieldForEditor(nameof(SituationComponent.Situation))!;

                situationField.SetValue(situation, result);
                modified = true;
            }

            modified |= DrawValue(ref situation, nameof(SituationComponent.Character));

            return (modified, situation);
        }

        /// <summary>
        /// Draw field for <see cref="TalkToInteraction.Situation"/> in <see cref="TalkToInteraction"/>.
        /// </summary>
        public static bool DrawSituationField(Guid character, int situation, out int result)
        {
            result = 0;

            ImGui.TableNextRow();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            // -- Field --
            ImGui.Text("Situation:");

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            if (Game.Data.TryGetAsset<CharacterAsset>(character) is not CharacterAsset asset)
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
            if (asset.TryFetchSituation(situation) is Situation target)
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
    }
}

