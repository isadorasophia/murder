using ImGuiNET;
using Murder.Core.Dialogs;
using Murder.Editor.CustomEditors;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Threading.Channels;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(DialogAction))]
    internal class DialogActionField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            
            DialogAction action = (DialogAction)fieldValue!;
            
            using TableMultipleColumns table = new($"action", flags: ImGuiTableFlags.SizingStretchSame, 
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
            if (CharacterEditor.DrawActionCombo($"action_combo", ref action))
            {
                modified = true;
            }

            ImGui.PopItemWidth();
            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            // -- Select action value --
            if (action.Fact is not null)
            {
                string targetFieldName = CharacterEditor.GetTargetFieldForFact(action.Fact.Value.Kind);
                if (CustomField.DrawValue(ref action, targetFieldName))
                {
                    modified = true;
                }
            }

            ImGui.PopItemWidth();

            return (modified, action);
        }
    }
}
