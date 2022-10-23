using InstallWizard.Util;
using InstallWizard.StateMachines.AiStates;
using Editor.CustomComponents;
using Editor.Gui;
using Editor.Reflection;
using ImGuiNET;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(IAiDecision))]
    internal class AiDecisionField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            IAiDecision? field = (IAiDecision)fieldValue!;

            if (field is null)
            {
                if (SearchBox.SearchInterfaces(member.Type) is Type t)
                {
                    modified = true;
                    if (Activator.CreateInstance(t) is IAiDecision v)
                    {
                        field = v;
                    }
                }
            }
            else if (fieldValue is IAiDecision state)
            {
                if (ImGuiExtended.DeleteButton($"{member.Name}_del"))
                {
                    field = null;
                    modified = true;
                }
                ImGui.SameLine();

                var stateName = state.GetType().Name;
                ImGui.Text(stateName);
                if (CustomComponent.ShowEditorOf(state))
                {
                    return (true, field);
                }
                //TODO: Draw the normal editor for this
            }

            return (modified, field);
        }
    }
}

