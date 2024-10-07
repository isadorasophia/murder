using ImGuiNET;
using Murder.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<string>))]
    internal class ImmutableArrayStringField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            ImmutableArray<string> current = (ImmutableArray<string>)fieldValue!;

            if (AttributeExtensions.IsDefined(member, typeof(ChildIdAttribute)))
            {
                return ProcessChildName(current);
            }

            if (AttributeExtensions.IsDefined(member, typeof(AnchorAttribute)))
            {
                return ProcessAnchorName(current);
            }

            if (AttributeExtensions.IsDefined(member, typeof(TargetAttribute)))
            {
                return ProcessTargetName(current);
            }

            if (AttributeExtensions.IsDefined(member, typeof(AtlasNameAttribute)))
            {
                return ProcessAtlasName(current);
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

                    if (StringField.ProcessStringCombo($"replace_childname_{i}", ref text, names))
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
                string element = "Select a child name";
                if (StringField.ProcessStringCombo("array_childname_new", ref element, names))
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

        private (bool modified, object? result) ProcessAnchorName(ImmutableArray<string> current)
        {
            bool modified = false;

            var builder = ImmutableArray.CreateBuilder<string>();
            builder.AddRange(current);

            HashSet<int> missingNames = new();

            HashSet<string>? names = StageHelpers.GetAnchorNamesForSelectedEntity();
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

                if (ImGuiHelpers.DeleteButton($"Delete anchor name##{i}"))
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

                    if (StringField.ProcessStringCombo($"replace_anchor_{i}", ref text, names))
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
                string element = "Select an anchor";
                if (StringField.ProcessStringCombo("array_anchorname_new", ref element, names))
                {
                    modified = true;
                    builder.Add(element);
                }
            }
            else if (current.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "No anchors :-(");
            }

            return (modified, result: builder.ToImmutable());
        }

        private (bool modified, object? result) ProcessTargetName(ImmutableArray<string> current)
        {
            bool modified = false;

            var builder = ImmutableArray.CreateBuilder<string>();
            builder.AddRange(current);

            HashSet<int> missingNames = new();

            HashSet<string>? names = StageHelpers.GetTargetNamesForSelectedEntity();
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

                if (ImGuiHelpers.DeleteButton($"Delete target name##{i}"))
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

                    if (StringField.ProcessStringCombo($"replace_target_{i}", ref text, names))
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
                string element = "Select a target";
                if (StringField.ProcessStringCombo("array_targetname_new", ref element, names))
                {
                    modified = true;
                    builder.Add(element);
                }
            }
            else if (current.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "No targets :-(");
            }

            return (modified, result: builder.ToImmutable());
        }

        private (bool modified, object? result) ProcessAtlasName(ImmutableArray<string> current)
        {
            bool modified = false;

            var builder = ImmutableArray.CreateBuilder<string>();
            builder.AddRange(current);

            HashSet<int> missingNames = new();

            HashSet<string>? names = [.. Game.Data.LoadedAtlasses.Keys];
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

                if (ImGuiHelpers.DeleteButton($"Delete target name##{i}"))
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

                    if (StringField.ProcessStringCombo($"replace_target_{i}", ref text, names))
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
                string element = "Select an atlas";
                if (StringField.ProcessStringCombo("array_targetname_new", ref element, names))
                {
                    modified = true;
                    builder.Add(element);
                }
            }
            else if (current.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "No atlas :-(");
            }

            return (modified, result: builder.ToImmutable());
        }
    }
}