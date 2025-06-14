using ImGuiNET;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Editor.CustomEditors;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(DialogAction))]
    public class DialogActionField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            return DrawActionEditor(fieldValue);
        }

        public static (bool modified, DialogAction result) DrawActionEditor(object? fieldValue)
        {
            bool modified = false;

            DialogAction action = (DialogAction)fieldValue!;

            ImGui.SetNextItemWidth(-1);

            using TableMultipleColumns table = new($"action", flags: ImGuiTableFlags.SizingStretchProp,
                -1, 70, 140);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            // -- Facts across all blackboards --
            if (SearchBox.SearchFacts($"action_fact", action.Fact) is Fact newFact)
            {
                action = action.WithFact(newFact);
                modified = true;
            }

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            // -- Select action kind --
            if (DrawActionCombo($"action_combo", ref action))
            {
                modified = true;
            }

            ImGui.PopItemWidth();
            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            // -- Select action value --
            if (!string.IsNullOrEmpty(action.Fact.Name) && action.Kind != BlackboardActionKind.Toggle)
            {
                string targetFieldName = GetTargetFieldForFact(action.Fact.Kind);
                if (DrawValue(ref action, targetFieldName))
                {
                    modified = true;
                }
            }

            ImGui.PopItemWidth();

            return (modified, action);
        }

        internal static string GetTargetFieldForFact(FactKind kind)
        {
            switch (kind)
            {
                case FactKind.Enum:
                case FactKind.Int:
                    return nameof(Criterion.IntValue);

                case FactKind.Float:
                    return nameof(Criterion.FloatValue);

                case FactKind.String:
                    return nameof(Criterion.StrValue);

                case FactKind.Weight:
                    return nameof(Criterion.IntValue);

                case FactKind.Bool:
                    return nameof(Criterion.BoolValue);

                default:
                    GameLogger.Warning("Invalid fact?");
                    return nameof(Criterion.IntValue);
            }
        }

        private static bool DrawActionCombo(string id, ref DialogAction action)
        {
            BlackboardActionKind[] allKinds = FetchValidActionKind(action);
            if (allKinds.Length == 0)
            {
                // This may be empty if the action has not been initialized yet.
                ImGui.TextColored(Game.Profile.Theme.Warning, "\uf059");
                return false;
            }

            int index = Array.IndexOf(allKinds, action.Kind);
            if (index == -1)
            {
                action = action.WithKind(allKinds[0]);
                return true;
            }

            string[] kindToString = allKinds.Select(k => k.ToString()).ToArray();

            if (ImGui.Combo($"##{id}", ref index, kindToString, kindToString.Length))
            {
                action = action.WithKind(allKinds[index]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// This returns a list of all the valid <see cref="BlackboardActionKind"/> for the <see cref="Fact"/>.
        /// </summary>
        private static BlackboardActionKind[] FetchValidActionKind(DialogAction action)
        {
            switch (action.Fact.Kind)
            {
                case FactKind.Bool:
                    return [BlackboardActionKind.Set, BlackboardActionKind.Toggle];

                case FactKind.Int:
                case FactKind.Float:
                    return [BlackboardActionKind.Set, BlackboardActionKind.SetMax, BlackboardActionKind.SetMin, BlackboardActionKind.Add, BlackboardActionKind.Minus];

                case FactKind.String:
                    return [BlackboardActionKind.Set];
            }

            return new BlackboardActionKind[] { };
        }
    }
}