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

            return ImGuiHelpers.DrawEnumField($"##{member.Name}", t, intValue);
        }
    }
}
