using ImGuiNET;
using Murder.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;
using Newtonsoft.Json.Linq;
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

            if (AttributeExtensions.IsDefined(member, typeof(ChildIdAttribute)))
            {
                return ProcessChildName(current);
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

        private (bool modified, object? result) ProcessChildName(ImmutableArray<string> current)
        {
            bool modified = false;

            var builder = ImmutableArray.CreateBuilder<string>();
            builder.AddRange(current);

            HashSet<int> missingNames = new();

            HashSet<string>? names = StageHelpers.GetChildNamesForSelectedEntity();
            if (names is not null)
            {
                for (int i = 0; i < current.Count(); i++)
                {
                    string text = current[i];

                    bool removed = names.Remove(text);
                    if (!removed)
                    {
                        missingNames.Add(i);
                    }
                }
            }

            for (int i = 0; i < current.Count(); i++)
            {
                string text = current[i];

                if (ImGuiHelpers.DeleteButton($"Delete child name##{i}"))
                {
                    modified = true;
                    builder.RemoveAt(i);
                }

                ImGui.SameLine();

                if (names is null)
                {
                    ImGui.Text(text);
                }
                else
                {
                    bool isMissing = missingNames.Contains(i);
                    if (isMissing)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Red);
                    }

                    if (StringField.ProcessChildName($"replace_childname_{i}", ref text, names))
                    {
                        modified = true;
                        builder[i] = text;
                    }

                    if (isMissing)
                    {
                        ImGui.PopStyleColor();
                    }
                }

                // Remove element that has been added.
                names?.Remove(text);
            }

            if (names is not null && names.Any())
            {
                string element = string.Empty;
                if (StringField.ProcessChildName("array_childname_new", ref element, names))
                {
                    modified = true;
                    builder.Add(element);
                }
            }
            else if (current.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "No children :-(");
            }

            return (modified, result: builder.ToImmutable());
        }
    }
}
