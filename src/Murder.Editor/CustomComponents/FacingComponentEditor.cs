using ImGuiNET;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Data.Graphics;
using Murder.Helpers;
using Murder.Utilities;
using System.Numerics;
namespace Murder.Editor.CustomComponents;

[CustomComponentOf(typeof(FacingComponent))]
public class FacingComponentEditor : CustomComponent
{
    bool _dragging = false;
    protected override bool DrawAllMembersWithTable(ref object target)
    {
        var facing = (FacingComponent)target;
        bool fileChanged = false;

        if (ImGui.BeginTable($"field_{target.GetType().Name}", 2,
            ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInnerH))
        {

            ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, -1, 0);
            ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1, 1);


            ImGui.TableNextColumn();

            ImGui.Dummy(new Vector2(40, 40));
            var min = ImGui.GetItemRectMin();
            var max = ImGui.GetItemRectMax();
            var mid = Vector2.Lerp(min, max, 0.5f);

            var draw = ImGui.GetForegroundDrawList();
            uint faded = Color.ToUint(Game.Profile.Theme.Faded);
            uint highlight = Color.ToUint(Game.Profile.Theme.Accent);

            draw.AddCircle(mid, 20, faded, 12, 2);
            draw.AddLine(mid, mid + Vector2Helper.Right.Rotate(facing.Angle) * 19, _dragging? highlight : faded, 2);

            var circleDelta = ImGui.GetMousePos() - mid;

            if (circleDelta.LengthSquared() < (20 * 20))
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    _dragging = true;
                }
                if (!_dragging)
                {
                    ImGui.SetTooltip("Hold Shift to snap");
                }
            }

            if (_dragging)
            {
                if (!ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    _dragging = false;
                }
                else
                {
                    fileChanged = true;
                    var finalAngle = circleDelta.Angle();
                    if (ImGui.IsKeyDown(ImGuiKey.ModShift))
                    {
                        finalAngle = Calculator.SnapAngle(finalAngle * Calculator.TO_DEG, 45) * Calculator.TO_RAD;
                    }

                    facing = new FacingComponent(finalAngle);
                }
            }

            ImGui.TableNextColumn();
            float angle = facing.Angle * Calculator.TO_DEG;

            if (ImGui.DragFloat("Angle", ref angle, 0.3f, -90, 360))
            {
                fileChanged = true;
                facing = new FacingComponent(angle * Calculator.TO_RAD);
            }

            int direction = (int)facing.Direction;
            var directions = Enum.GetNames<Direction>();
            if (ImGui.Combo("Direction", ref direction, directions, directions.Length))
            {
                fileChanged = true;
                facing = new FacingComponent((Direction)direction);
            }
            
            if (fileChanged)
            {
                target = facing;
            }

            ImGui.EndTable();
        }

        return fileChanged;
    }
}
