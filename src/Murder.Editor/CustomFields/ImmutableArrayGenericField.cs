using ImGuiNET;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<>), priority: -1)]
    public class ImmutableArrayGenericField<T> : ImmutableArrayField<T> where T : new()
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out T? element)
        {
            element = default!;

            if (ImGui.Button("Add item"))
            {
                element = new();
                return true;
            }

            return false;
        }
    }
}