using Bang.Components;
using Bang.Interactions;
using ImGuiNET;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<IComponent>), priority: -1)]
    internal class ImmutableArrayComponentField : ImmutableArrayField<IComponent>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out IComponent element)
        {
            element = default!;
            
            if (SearchBox.SearchComponent() is Type t)
            {
                element = (IComponent)Activator.CreateInstance(t)!;
                return true;
            }

            return false;
        }

        protected override bool DrawElement(ref IComponent? element, EditorMember member, int _)
        {
            Type t = element?.GetType()!;

            ImGui.TextColored(Game.Profile.Theme.Faded, t.IsGenericType ? t.GetGenericArguments()[0].Name : t.Name);
            return CustomComponent.ShowEditorOf(ref element);
        }
    }
}
