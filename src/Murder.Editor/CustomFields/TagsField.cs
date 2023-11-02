using ImGuiNET;
using Murder.Core;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Tags))]
    internal class TagsField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            Tags tags = (Tags)fieldValue!;
            int number = tags.Flags;

            bool all = tags.All;
            if (ImGui.Checkbox($"All##{member.Name}-all-tag", ref all))
            {
                modified = true;
            }
            ImGui.Separator();
            if (!all)
            {
                using TableMultipleColumns table = new($"##{member.Name}-{member.Member.Name}-col-table",
                                    flags: ImGuiTableFlags.SizingFixedFit,
                                    -1, -1, -1);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImmutableArray<(string Name, int Id)> list = AssetsFilter.Tags;
                string[] prettyNames = AssetsFilter.TagsNames;

                if (list.Length == 0)
                {
                    ImGui.Text("No tags available");
                }

                for (int i = 0; i < list.Length; i++)
                {
                    bool isChecked = (list[i].Id & number) != 0;

                    if (ImGui.Checkbox($"##{member.Name}-{i}-tag", ref isChecked))
                    {
                        if (isChecked)
                        {
                            number |= list[i].Id;
                        }
                        else
                        {
                            number &= ~list[i].Id;
                        }

                        modified = true;
                    }

                    ImGui.SameLine();
                    ImGui.Text(prettyNames[i]);

                    ImGui.TableNextColumn();

                    if ((i + 1) % 3 == 0)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                    }
                }
            }
            return (modified, new Tags(number, all));
        }
    }
}
