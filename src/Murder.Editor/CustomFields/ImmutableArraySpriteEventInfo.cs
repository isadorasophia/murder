using ImGuiNET;
using Murder.Components;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<SpriteEventInfo>))]
    internal class ImmutableArraySpriteEventInfo : ImmutableArrayField<SpriteEventInfo>
    {
        protected override bool AllowReorder => true;

        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out SpriteEventInfo element)
        {
            if (ImGui.Button("New event"))
            {
                element = new();
                return true;
            }

            element = default;
            return false;
        }

        protected override void Reorder(ref ImmutableArray<SpriteEventInfo> elements)
        {
            elements = [.. elements.OrderBy(p => p.Id)];
        }

        protected override bool DrawElement(ref SpriteEventInfo element, EditorMember member, int index)
        {
            using TableMultipleColumns table = new($"sprite_{index}",
                flags: ImGuiTableFlags.SizingFixedSame,
                (-1, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));

            object oElement = (object)element;
            bool modified = CustomComponent.DrawAllMembers(
                oElement, 
                exceptForMembers: [nameof(SpriteEventInfo.Persisted), nameof(SpriteEventInfo.Interactions)]);

            if (modified)
            {
                element = (SpriteEventInfo)oElement;
            }

            return modified;
        }
    }
}