using Bang.Components;
using ImGuiNET;
using Murder.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<Type>))]
    internal class ImmutableArrayTypeField : ImmutableArrayField<Type>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out Type? element)
        {
            element = default;

            if (AttributeExtensions.TryGetAttribute(member.Member, out TypeOfAttribute? attribute) &&
                attribute.Type != typeof(IComponent))
            {
                ImGui.TextColored(Game.Profile.Theme.Red, $"Type {attribute.Type.Name} not supported yet.");
                return false;
            }

            if (SearchBox.SearchComponent() is Type t)
            {
                element = t;

                return true;
            }

            return false;
        }

        protected override bool DrawElement(ref Type? element, EditorMember member, int index)
        {
            if (element is null)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "Empty");
                return false;
            }

            ImGui.TextColored(Game.Profile.Theme.Accent, element.Name);

            return false;
        }
    }
}