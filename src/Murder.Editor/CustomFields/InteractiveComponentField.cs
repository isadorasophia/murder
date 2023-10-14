using Bang.Interactions;
using ImGuiNET;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(IInteractiveComponent), priority: 10)]
    internal class InteractiveComponentField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            IInteractiveComponent? component = fieldValue as IInteractiveComponent;
            if (fieldValue is null || member.Type.IsInterface)
            {
                if (SearchBox.SearchInteractions(fieldValue?.GetType().GetGenericArguments().FirstOrDefault()) is Type chosenInteractive)
                {
                    component = (IInteractiveComponent)Activator.CreateInstance(chosenInteractive)!;
                    modified = true;
                }
            }
            else
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, component?.GetType().GetGenericArguments()[0].Name);
            }

            if (fieldValue is not null)
            {
                modified |= CustomComponent.ShowEditorOf(component);
            }

            return (modified, component);
        }
    }
}