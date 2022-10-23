using InstallWizard.Util;
using InstallWizard.StateMachines.AiStates;
using Editor.CustomComponents;
using Editor.Gui;
using Editor.Reflection;
using ImGuiNET;
using Bang.Interactions;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(IInteractiveComponent), priority: 10)]
    internal class InteractiveComponentField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            IInteractiveComponent? component = fieldValue as IInteractiveComponent;
            if (fieldValue is null)
            {
                if (SearchBox.SearchInteractions() is Type chosenInteractive)
                {
                    component = (IInteractiveComponent)Activator.CreateInstance(chosenInteractive)!;
                    modified = true;
                }
            }
            else
            {
                modified = CustomComponent.ShowEditorOf(component);
            }

            return (modified, component);
        }
    }
}

