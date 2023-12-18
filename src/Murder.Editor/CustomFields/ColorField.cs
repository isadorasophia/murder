using ImGuiNET;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Numerics;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Vector4))]
    internal class Vector4Field : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            Vector4 color = (Vector4)fieldValue!;

            return ProcessInputImpl(member, color);
        }

        internal static (bool modified, Vector4 result) ProcessInputImpl(EditorMember _, Vector4 color)
        {
            bool modified = false;

            if (ImGui.ColorButton("color", color))
            {
                ImGui.OpenPopup("colorPicker");
            }
            if (DisplayMiniAlphaWarning(color.W))
            {
                // Manually convert the alpha.
                color.W = 1f;
                modified = true;
            }

            if (ImGui.BeginPopup("colorPicker"))
            {
                modified = ImGui.ColorPicker4("", ref color);

                if (DisplayAlphaWarning(color.W))
                {
                    // Manually convert the alpha.
                    color.W = 1f;
                    modified = true;
                }

                ImGui.EndPopup();
            }

            return (modified, color);
        }
        private static bool DisplayMiniAlphaWarning(float alpha)
        {
            if (alpha == 0f)
            {
                ImGui.SameLine();
                ImGuiHelpers.ColorIcon('', Architect.Profile.Theme.Red);
            }
            else if (alpha < 1f)
            {
                ImGui.SameLine();
                var warningColor = Architect.Profile.Theme.Warning;
                ImGuiHelpers.ColorIcon('', new Vector4(warningColor.X, warningColor.Y, warningColor.Z, Calculator.Remap(alpha, 0, 1, 1f, 0.2f)));
            }

            return false;
        }

        private static bool DisplayAlphaWarning(float alpha)
        {
            if (alpha < 1f)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "Low alpha detected!");
                ImGui.SameLine();

                if (ImGui.Button("Set Alpha to 1"))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [CustomFieldOf(typeof(XnaColor))]
    internal class XnaColorField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            // ImGui only knows how to process Vector4 color input, so we'll convert to that.
            XnaColor color = (XnaColor)fieldValue!;

            var (modified, vector4Color) = Vector4Field.ProcessInputImpl(member, color.ToSysVector4());

            return (modified, vector4Color.ToXnaColor());
        }
    }

    [CustomFieldOf(typeof(Murder.Core.Graphics.Color))]
    internal class CoreColorField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            // ImGui only knows how to process Vector4 color input, so we'll convert to that.
            var color = ((Murder.Core.Graphics.Color)fieldValue!);

            bool modified = false;
            Vector4 vector4Color = color.ToSysVector4();

            if (AttributeExtensions.TryGetAttribute<PaletteColorAttribute>(member, out var paletteAttribute))
            {
                DrawPalettePicker(member, color, ref modified, ref vector4Color);
            }
            else
            {

                (modified, vector4Color) = Vector4Field.ProcessInputImpl(member, new(color.R, color.G, color.B, color.A));
            }

            return (modified, new Murder.Core.Graphics.Color(vector4Color.X, vector4Color.Y, vector4Color.Z, vector4Color.W));
        }

        internal static void DrawPalettePicker(EditorMember member, Murder.Core.Graphics.Color color, ref bool modified, ref Vector4 vector4Color)
        {
            if (ImGui.BeginChild(member.Name + "_frame", new Vector2(-1, 20)))
            {
                (modified, vector4Color) = Vector4Field.ProcessInputImpl(member, new(color.R, color.G, color.B, color.A));
                ImGui.SameLine();
                if (ImGui.BeginCombo($"##{member.Name}", color.ToString()))
                {

                    var i = 0;
                    foreach (var c in Game.Data.CurrentPalette)
                    {
                        if (ImGuiHelpers.SelectableColor($"pal_{color}_{i++}", c.ToSysVector4()))
                        {
                            modified = true;
                            vector4Color = c.ToSysVector4();
                        }
                    }
                    ImGui.EndCombo();
                }
                else
                {
                    var p_min = ImGui.GetItemRectMin();
                    var p_max = ImGui.GetItemRectMax();
                    ImGui.GetWindowDrawList().AddRectFilled(p_min, p_max, ImGuiHelpers.MakeColor32(vector4Color));
                }
            }
            ImGui.EndChild();
        }
    }
}