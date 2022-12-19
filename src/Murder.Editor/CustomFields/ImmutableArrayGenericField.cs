using ImGuiNET;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<>), priority: -1)]
    public class ImmutableArrayGenericField<T> : ImmutableArrayField<T> 
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out T? element)
        {
            element = default!;

            if (ImGui.Button("Add Item"))
            {
                element = default!;
                return true;
            }

            return false;
        }
    }
}
