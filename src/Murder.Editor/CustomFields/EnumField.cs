using ImGuiNET;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

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

            bool modified = false;
            if (Attribute.IsDefined(t, typeof(FlagsAttribute)))
            {
                modified |= ImGuiHelpers.DrawEnumFieldAsFlags(member.Name, t, ref intValue);
                return (modified, Enum.ToObject(t, intValue));
            }

            (modified, intValue) = ImGuiHelpers.DrawEnumField($"##{member.Name}", t, intValue);
            return (modified, Enum.ToObject(t, intValue));
        }

    }
}