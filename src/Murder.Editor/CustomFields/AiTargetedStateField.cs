using InstallWizard.Util;
using InstallWizard.StateMachines.AiStates;
using Editor.CustomComponents;
using Editor.Gui;
using Editor.Reflection;
using ImGuiNET;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(IAiStateBase))]
    internal class AiStateField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            IAiStateBase? field = (IAiStateBase)fieldValue!;

            if (field is null)
            {
                if (SearchBox.SearchInterfaces(member.Type) is Type t)
                {
                    modified = true;
                    if (Activator.CreateInstance(t) is IAiStateBase v)
                    {
                        field = v;
                    }
                }
            }
            else if (fieldValue is IAiTargetedState state)
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
            }

            return (modified, field);
        }
    }
}

