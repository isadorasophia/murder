using ImGuiNET;
using Murder.Editor.Reflection;
using System.Collections.Immutable;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<string>))]
    internal class ImmutableArrayStringField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            ImmutableArray<string> current = (ImmutableArray<string>)fieldValue!;

            if (member.IsReadOnly)
            {
                // Read only, do not modify enum value.
                ImGui.Text(String.Join(',', current));
                return (false, current);
            }

            var cache = String.Join(',', current);
            var modified = ImGui.InputText($"##{member.Name}_value", ref cache, 256);

            if (!ImGui.IsItemFocused())
            {
                cache = String.Join(',', current);
            }
            if (modified)
            {
                var parsed = cache.Trim('\t', ',').Split(',');
                var builder = ImmutableArray.CreateBuilder<string>();
                foreach (var item in parsed)
                {
                    builder.Add(item);
                }
                current = builder.ToImmutableArray();
            }

            return (modified, current);
        }


    }
}
