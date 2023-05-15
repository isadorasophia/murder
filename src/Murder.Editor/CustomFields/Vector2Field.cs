using ImGuiNET;
using Murder.Attributes;
using Murder.Editor.Reflection;
using Murder.Editor.ImGuiExtended;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Vector2))]
    internal class Vector2SysField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            Vector2 vector2 = (Vector2)fieldValue!;

            return ProcessInputImpl(member, vector2);
        }

        internal static (bool modified, Vector2 result) ProcessInputImpl(EditorMember member, Vector2 vector2)
        {
            bool modified;

            int availableWidth = (int)ImGui.GetContentRegionAvail().X;

            ImGui.PushItemWidth(availableWidth - 92);
            if (AttributeExtensions.TryGetAttribute(member, out SliderAttribute? slider))
            {
                modified = ImGui.SliderFloat2("", ref vector2, slider.Minimum, slider.Maximum);
            }
            else
            {
                modified = ImGui.InputFloat2("", ref vector2);
            }
            ImGui.PopItemWidth();

            ImGui.SameLine();
            if (ImGuiHelpers.IconButton('', "TopLeft"))
            {
                return (true, Vector2.Zero);
            }

            ImGui.SameLine();
            if (ImGuiHelpers.IconButton('', "Center"))
            {
                return (true, Vector2.One * 0.5f);
            }

            ImGui.SameLine();
            if (ImGuiHelpers.IconButton('', "BCenter"))
            {
                return (true, new Vector2(0.5f, 1));
            }

            return (modified, vector2);
        }
    }

    [CustomFieldOf(typeof(Microsoft.Xna.Framework.Vector2))]
    internal class Vector2XnaField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            var vector2 = (Microsoft.Xna.Framework.Vector2)fieldValue!;

            var (fileChanged, sysVector2) = Vector2SysField.ProcessInputImpl(member, vector2.ToSysVector2());

            return (fileChanged, sysVector2.ToXnaVector2());
        }
    }

    [CustomFieldOf(typeof(Murder.Core.Geometry.Vector2))]
    internal class Vector2CoreField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            var vector2 = (Murder.Core.Geometry.Vector2)fieldValue!;

            var (fileChanged, sysVector2) = Vector2SysField.ProcessInputImpl(member, vector2);

            return (fileChanged, sysVector2.ToXnaVector2());
        }
    }
}
