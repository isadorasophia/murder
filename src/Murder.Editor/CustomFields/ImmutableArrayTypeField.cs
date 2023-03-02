using Bang.Components;
using ImGuiNET;
using Murder.Attributes;
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
                ImGui.TextColored(Game.Profile.Theme.Red, "Type field not implemented (yet)!");
                return false;
            }

            //if (S)
            //{
            //    return true;
            //}

            return false;
        }
    }
}