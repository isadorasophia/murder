using ImGuiNET;
using Murder.Editor.Reflection;
using Murder.Editor.ImGuiExtended;

namespace Murder.Editor.CustomFields
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
                ImGui.Text(Enum.GetName(fieldValue.GetType(), fieldValue));
                return (false, intValue);
            }

            return ImGuiHelpers.DrawEnumField($"##{member.Name}", fieldValue.GetType(), intValue);
        }
    }
}
