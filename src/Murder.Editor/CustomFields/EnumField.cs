using InstallWizard.Util;
using Editor.Reflection;
using ImGuiNET;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(Enum), priority: 10)]
    internal class EnumField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            int intValue = (int)fieldValue!;

            if (member.IsReadOnly)
            {
                // Read only, do not modify enum value.
                ImGui.Text(Enum.GetName(member.Type, fieldValue));
                return (false, intValue);
            }

            return ImGuiExtended.DrawEnumField($"##{member.Name}", member.Type, intValue);
        }
    }

}
