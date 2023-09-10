using ImGuiNET;
using Murder.Editor.Reflection;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Immutable;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Enum), priority: 10)]
    internal class EnumField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            if (fieldValue is null)
            {
                return (true, Activator.CreateInstance(member.Type));
            }

            int intValue = Convert.ToInt32(fieldValue);

            Type t = fieldValue.GetType();
            if (!t.IsEnum)
            {
                t = member.Type;
            }

            if (member.IsReadOnly)
            {
                // Read only, do not modify enum value.
                ImGui.Text(Enum.GetName(t, fieldValue));
                return (false, intValue);
            }

            if (Attribute.IsDefined(t, typeof(FlagsAttribute)))
            {
                bool modified = false;

                using TableMultipleColumns table = new($"##{member.Name}-{member.Member.Name}-col-table",
                    flags: ImGuiTableFlags.SizingFixedFit, -1, -1, -1);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                Array values = Enum.GetValues(t);
                string[] prettyNames = Enum.GetNames(t);

                int tableIndex = 0;

                for (int i = 0; i < values.Length; i++)
                {
                    if (values.GetValue(i) is not object objValue)
                    {
                        continue;
                    }

                    int value = (int)objValue;
                    if (value == 0)
                    {
                        continue;
                    }

                    bool isChecked = (value & intValue) != 0;

                    if (ImGui.Checkbox($"##{member.Name}-{i}-col-layer", ref isChecked))
                    {
                        if (isChecked)
                        {
                            intValue |= value;
                        }
                        else
                        {
                            intValue &= ~value;
                        }

                        modified = true;
                    }

                    ImGui.SameLine();
                    ImGui.Text(prettyNames[i]);

                    ImGui.TableNextColumn();

                    if ((tableIndex + 1) % 3 == 0)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                    }

                    tableIndex++;
                }

                return (modified, intValue);
            }

            return ImGuiHelpers.DrawEnumField($"##{member.Name}", t, intValue);
        }
    }
}
