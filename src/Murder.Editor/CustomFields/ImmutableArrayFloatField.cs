using Editor.Reflection;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<float>))]
    internal class ImmutableArrayFloatField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            ImmutableArray<float> current = (ImmutableArray<float>)fieldValue!;

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
                var builder = ImmutableArray.CreateBuilder<float>();
                foreach (var item in parsed)
                {
                    if (float.TryParse(item, out var value))
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
