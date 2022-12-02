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
    [CustomFieldOf(typeof(ImmutableArray<IInteractiveComponent>))]
    internal class ImmutableArrayInteractivesField : ImmutableArrayField<IInteractiveComponent>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out IInteractiveComponent element)
        {
            element = default!;
            
            if (SearchBox.SearchInteractions() is Type t)
            {
                element = (IInteractiveComponent)Activator.CreateInstance(t)!;
                return true;
            }

            return false;
        }

        protected override bool DrawElement(ref IInteractiveComponent? element, EditorMember member, int _)
        {
            ImGui.TextColored(Game.Profile.Theme.Faded, element?.GetType().GetGenericArguments()[0].Name);
            return CustomComponent.ShowEditorOf(ref element);
        }
    }
}
