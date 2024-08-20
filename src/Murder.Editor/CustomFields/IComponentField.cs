using Bang.Components;
using Bang.Interactions;
using Bang.StateMachines;
using Murder.Attributes;
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
                Type? result;

                if (member.Type == typeof(IStateMachineComponent))
                {
                    Type? subtypeOf = null;
                    if (AttributeExtensions.TryGetAttribute(member.Member, out TypeOfAttribute? attribute))
                    {
                        subtypeOf = attribute.Type;
                    }

                    SearchBox.SearchStateMachines(initialValue: component?.GetType(), out result, subtypeOf);
                }
                else if (member.Type == typeof(IInteractiveComponent))
                {
                    result = SearchBox.SearchInteractions(initialValue: null);
                }
                else
                {
                    result = SearchBox.SearchComponent(initialValue: component);
                }

                if (result is not null)
                {
                    modified = true;
                    component = (IComponent)Activator.CreateInstance(result)!;
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