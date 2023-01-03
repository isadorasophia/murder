using ImGuiNET;
using Murder.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using static System.Net.Mime.MediaTypeNames;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<string>))]
    internal class ImmutableArrayStringField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            ImmutableArray<string> current = (ImmutableArray<string>)fieldValue!;

            if (AttributeExtensions.IsDefined(member, typeof(SoundAttribute)))
            {
                return ProcessSound(current);
            }

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


        private (bool modified, object? result) ProcessSound(ImmutableArray<string> current)
        {
            bool modified = false;
            List<string> result = new(current);
            
            for (int i = 0; i < current.Count(); i++)
            {
                string text = current[i];

                if (ImGuiHelpers.DeleteButton($"No Sound##{i}"))
                {
                    modified = true;
                    result.RemoveAt(i);
                }
                ImGui.SameLine();
                if (SearchBox.SearchSounds(text, i.ToString()) is string sound)
                {
                    modified = true;
                    result[i] = sound;
                }
            }

            if (SearchBox.SearchSounds(string.Empty, "") is string newSound)
            {
                modified = true;
                result.Add(newSound);
            }

            return (modified: modified, result: result.ToImmutableArray());
        }
    }
}
