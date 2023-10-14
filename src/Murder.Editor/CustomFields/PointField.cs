using ImGuiNET;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Point))]
    internal class PointField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            Point point = (Point)fieldValue!;

            return ProcessInputImpl(member, point);
        }

        internal static (bool modified, Point result) ProcessInputImpl(EditorMember member, Point point)
        {
            bool modified = false;

            int availableWidth = (int)ImGui.GetContentRegionAvail().X;
            using TableMultipleColumns table = new($"{member.Name}_point", flags: ImGuiTableFlags.SizingFixedFit, availableWidth / 2, availableWidth / 2);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImGui.PushItemWidth(-1);

            int x = point.X;
            modified |= ImGui.InputInt($"##{member.Name}_x", ref x);
            ImGuiHelpers.HelpTooltip("x");

            ImGui.PopItemWidth();

            ImGui.TableNextColumn();

            ImGui.PushItemWidth(-1);

            int y = point.Y;
            modified |= ImGui.InputInt($"##{member.Name}_y", ref y);
            ImGuiHelpers.HelpTooltip("y");

            ImGui.PopItemWidth();

            if (modified)
            {
                point.X = x;
                point.Y = y;
            }

            return (modified, point);
        }
    }
}