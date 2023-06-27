using Bang.Components;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(IComponent), priority: -10)]
    internal class IComponentField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            IComponent? component = (IComponent?)fieldValue;
            if (member.Type.IsInterface)
            {
                if (SearchBox.SearchComponent(initialValue: component) is Type t)
                {
                    modified = true;
                    component = (IComponent)Activator.CreateInstance(t)!;
                }
            }

            if (component is not null)
            {
                modified |= CustomComponent.ShowEditorOf(ref component);
            }

            return (modified, component);
        }
    }
}
