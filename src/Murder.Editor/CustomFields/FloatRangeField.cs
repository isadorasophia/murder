using ImGuiNET;
using Murder.Attributes;
using Murder.Components;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(FloatRange))]
    internal class FloatRangeField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            FloatRange range = (FloatRange)fieldValue!;

            modified |= DrawRange(member, ref range);

            return (modified, range);
        }

        public static bool DrawRange(EditorMember member, ref FloatRange value)
        {
            bool modified = false;

            ImGui.Text("From");
            ImGui.SameLine();

            ImGui.PushItemWidth(100);

            if (value.GetType().TryGetFieldForEditor(nameof(FloatRange.Start)) is EditorMember startMember)
            {
                AttributeExtensions.TryGetAttribute(startMember, out SliderAttribute? slider);

                modified |= DrawPrimitiveValueWithSlider(id: $"{member.Name}_start", ref value, nameof(FloatRange.Start), slider);

                // Manually override the value if it surpasses the limit.
                if (value.Start > value.End)
                {
                    startMember.SetValue(ref value, value.End);
                    modified = true;
                }
            }

            ImGui.SameLine();

            ImGui.PopItemWidth();

            ImGui.Text("To");
            ImGui.SameLine();

            ImGui.PushItemWidth(100);

            if (value.GetType().TryGetFieldForEditor(nameof(FloatRange.End)) is EditorMember endMember)
            {
                AttributeExtensions.TryGetAttribute(endMember, out SliderAttribute? slider);

                modified |= DrawPrimitiveValueWithSlider(id: $"{member.Name}_end", ref value, nameof(FloatRange.End), slider);

                // Manually override the value if it surpasses the limit.
                if (value.End < value.Start)
                {
                    endMember.SetValue(ref value, value.Start);
                    modified = true;
                }
            }

            ImGui.PopItemWidth();

            return modified;
        }
    }
}