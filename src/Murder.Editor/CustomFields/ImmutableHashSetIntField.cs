using ImGuiNET;
using Murder.Editor.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableHashSet<int>))]
    public class ImmutableHashSetIntField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            ImmutableHashSet<int> current = (ImmutableHashSet<int>)fieldValue!;

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
                var builder = ImmutableHashSet.CreateBuilder<int>();
                foreach (var item in parsed)
                {
                    if (int.TryParse(item, out var value))
                    {
                        builder.Add(value);
                    }
                }
                current = builder.ToImmutableHashSet();
            }

            return (modified, current);
        }

    }
}
