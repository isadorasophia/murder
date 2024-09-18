using ImGuiNET;
using Murder.Editor.Reflection;
using System.Collections.Immutable;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<int>))]
    internal class ImmutableArrayIntField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            ImmutableArray<int> current = (ImmutableArray<int>)fieldValue!;

            if (current.IsDefaultOrEmpty)
            {
                return (false, current);
            }

            if (member.IsReadOnly)
            {
                // Read only, do not modify enum value.
                ImGui.Text(String.Join(',', current));
                return (false, current);
            }

            var cache = String.Join(',', current);
            var modified = ImGui.InputText($"##{member.Name}_value", ref cache, 256);

            if (modified)
            {
                var parsed = cache.Trim('\t', '\n', ' ', ',').Split(',');
                var builder = ImmutableArray.CreateBuilder<int>();
                foreach (var item in parsed)
                {
                    if (int.TryParse(item, out var value))
                    {
                        builder.Add(value);
                    }
                }
                current = builder.ToImmutableArray();
            }

            return (modified, current);
        }


    }
}