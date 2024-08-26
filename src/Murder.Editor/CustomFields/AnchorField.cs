using ImGuiNET;
using Murder.Core.Cutscenes;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System.Numerics;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Anchor))]
    internal class AnchorField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            Anchor anchor = (Anchor)fieldValue!;

            Vector2 input = anchor.Position;

            bool modified = ImGui.InputFloat2("anchor", ref input);
            if (modified && StageHelpers.GetPositionForSelectedEntity() is Vector2 globalPosition)
            {
                if (anchor.Position.X != input.X)
                {
                    anchor = anchor.WithPosition(new Vector2(input.X - globalPosition.X, anchor.Position.Y));
                }

                if (anchor.Position.Y != input.Y)
                {
                    anchor = anchor.WithPosition(new Vector2(anchor.Position.X, input.Y - globalPosition.Y));
                }
            }

            return (modified, anchor);
        }
    }
}