﻿using ImGuiNET;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Editor.CustomEditors;
using Murder.Editor.Reflection;
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
                situation = situation.WithSituation(result);
                modified = true;
            }

            modified |= DrawValue(ref situation, nameof(SituationComponent.Character));

            return (modified, situation);
        }

        public static bool DrawSituationField(Guid character, int situation, out int result)
        {
            result = 0;

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

            return false;
        }

        /// <summary>
        /// Draw field for <see cref="TalkToInteraction.Situation"/> in <see cref="TalkToInteraction"/>.
        /// </summary>
        public static bool DrawSituationFieldWithTable(Guid character, int situation, out int result)
        {
            ImGui.TableNextRow();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            // -- Field --
            ImGui.Text("Situation:");

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            bool modified = DrawSituationField(character, situation, out result);

            ImGui.PopItemWidth();
            return modified;
        }
    }
}