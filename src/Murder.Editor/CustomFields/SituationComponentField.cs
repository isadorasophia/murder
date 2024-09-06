using ImGuiNET;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Editor.CustomEditors;
using Murder.Editor.Reflection;
using Murder.Interactions;
using Murder.Services;
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

            bool showFirstLinePreview = AttributeExtensions.IsDefined(member, typeof(ShowFirstLineAttribute));

            if (DrawSituationField(situation.Character, situation.Situation, showFirstLinePreview, out string result))
            {
                situation = situation.WithSituation(result);
                modified = true;
            }

            modified |= DrawValue(ref situation, nameof(SituationComponent.Character));

            return (modified, situation);
        }

        public static bool DrawSituationField(Guid character, string? situation, bool showFirstLinePreview, out string result)
        {
            result = string.Empty;

            if (Game.Data.TryGetAsset<CharacterAsset>(character) is not CharacterAsset asset)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "Choose a valid character first!");
                return false;
            }

            if (showFirstLinePreview && situation is not null)
            {
                string line = DialogueServices.FetchFirstLine(world: null, target: null, new(character, situation));
                ImGui.Text(line);
            }

            ImmutableDictionary<string, Situation> situations = asset.Situations;
            string[] situationNames = situations.Select(s => s.Key).ToArray();

            if (situationNames.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "No situation to go next?");
                return false;
            }

            int item = 0;
            if (situation is not null && asset.TryFetchSituation(situation) is Situation target)
            {
                item = Array.IndexOf(situationNames, target.Name);
            }
            else
            {
                // Set a initial value for this.
                result = situations.First().Key;

                return true;
            }

            if (ImGui.Combo($"##talto_situation", ref item, situationNames, situationNames.Length))
            {
                result = situationNames[item];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Draw field for <see cref="TalkToInteraction.Situation"/> in <see cref="TalkToInteraction"/>.
        /// </summary>
        public static bool DrawSituationFieldWithTable(Guid character, string situation, out string result)
        {
            ImGui.TableNextRow();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            // -- Field --
            ImGui.Text("Situation:");

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            bool modified = DrawSituationField(character, situation, showFirstLinePreview: false, out result);

            ImGui.PopItemWidth();
            return modified;
        }
    }
}