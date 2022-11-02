using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Vector4  = System.Numerics.Vector4;
using Murder.Data;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Core.Graphics;

namespace Murder.ImGuiExtended
{
    public static class ImGuiHelpers
    {
        public static uint MakeColor32(byte r, byte g, byte b, byte a) { uint ret = a; ret <<= 8; ret += b; ret <<= 8; ret += g; ret <<= 8; ret += r; return ret; }
        public static uint MakeColor32(Vector4 vector) => MakeColor32((byte)(vector.X * 255), (byte)(vector.Y * 255), (byte)(vector.Z * 255), (byte)(vector.W * 255));
        
        private static IntPtr PreviewImage
        {
            get
            {
                _previewImage ??= Game.Instance.ImGuiRenderer.GetNextIntPtr();
                return _previewImage.Value;
            }
        }
        private static IntPtr? _previewImage;
        private static IntPtr MissingImage
        {
            get
            {
                using var texture = Game.Data.FetchAtlas(AtlasId.Gameplay).CreateTextureFromAtlas("missingImage");

                _missingImage ??= Game.Instance.ImGuiRenderer.BindTexture(texture);
                return _missingImage.Value;
            }
        }

        private static IntPtr? _missingImage;

        public static void HelpTooltip(string description)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
                ImGui.TextUnformatted(description);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public static bool SelectedButton(string label, Vector4? color = default)
        {
            color ??= Game.Profile.Theme.BgFaded;

            ImGui.PushStyleColor(ImGuiCol.Button, color.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color.Value);

            var pressed = ImGui.Button(label);

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();

            return pressed;
        }

        public static bool LowButton(string label)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.5f, 0.5f, 0, 1));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.5f, 0, 1));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1, 1, 0.25f, 1));
            var pressed = ImGui.Button(label);
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();

            return pressed;
        }

        public static bool TreeNodeWithIcon(char icon, string label, ImGuiTreeNodeFlags flags)
        {
            ImGui.PushFont(FontAwesome.Solid);
            bool result = ImGui.TreeNodeEx($"{icon} {label}", flags);
            ImGui.PopFont();

            return result;
        }

        public static void ColorIcon(char icon, Vector4 color)
        {
            ImGui.PushFont(FontAwesome.Solid);
            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.Text(icon.ToString());
            ImGui.PopStyleColor();
            ImGui.PopFont();
        }

        public static bool ShowIcon(char icon, Vector4 selectedColor, Vector4 unselectedColor, bool selected = true)
        {
            ImGui.PushFont(selected ? FontAwesome.Solid : FontAwesome.Regular);
            ImGui.PushStyleColor(ImGuiCol.Text, selected ? selectedColor : unselectedColor);
            var clicked = ImGui.Selectable(icon.ToString());
            ImGui.PopStyleColor();
            ImGui.PopFont();
            return clicked;
        }

        public static bool FadedSelectableWithIcon(string text, char icon, bool selected)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded);
            bool clicked = SelectableWithIcon(text, icon, selected);
            ImGui.PopStyleColor();

            return clicked;
        }

        public static bool SelectableWithIcon(string text, char icon, bool selected) =>
            SelectableWithIconColor(text, icon, Game.Profile.Theme.White, Game.Profile.Theme.Faded, selected);

        public static bool SelectableWithIconColor(string text, char icon, Vector4 selectedColor, Vector4 unselectedColor, bool selected) 
        {
            var clicked = ShowIcon(icon, selectedColor, unselectedColor, selected);
            ImGui.SameLine();
            clicked = ImGui.Selectable(text, selected) || clicked;

            return clicked;
        }

        public static bool BeginPopupModalCentered(string name)
        {
            Vector2 center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));

            return BeginPopupModal(name, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.Modal);
        }

        public static bool BeginPopupModal(string name, ImGuiWindowFlags flags)
        {
            return ImGui.BeginPopupModal(name);
        }

        public static bool DeleteButton(string id)
        {
            var theme = Game.Profile.Theme;

            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, theme.Red);
            ImGui.PushStyleColor(ImGuiCol.Button, theme.Faded);
            var result = IconButton('\uf2ed', id);
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            return result;
        }

        public static bool IconButton(char icon, string id, Vector4? color = null, bool sameLine = false)
        {
            if (sameLine)
            {
                ImGui.SameLine();
            }

            if (color is not null)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, color.Value);
            }

            ImGui.PushFont(FontAwesome.Solid);
            ImGui.PushID(id);
            var result = ImGui.Button(icon.ToString());
            ImGui.PopID();
            ImGui.PopFont();

            if (color is not null)
            {
                ImGui.PopStyleColor();
            }

            return result;
        }

        public static bool ColoredIconButton(char icon, string id, bool isActive)
        {
            Vector4 textColor = Game.Profile.Theme.White;
            Vector4 selectedTextColor = Game.Profile.Theme.Faded;
            
            Vector4 backgroundColor = Game.Profile.Theme.Bg;

            ImGui.PushStyleColor(ImGuiCol.Text, isActive ? textColor : selectedTextColor);

            ImGui.PushStyleColor(ImGuiCol.Button, backgroundColor);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, backgroundColor);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, backgroundColor);

            ImGui.PushFont(FontAwesome.Solid);
            ImGui.PushID(id);
            var result = ImGui.Button(icon.ToString());
            ImGui.PopID();
            ImGui.PopFont();

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();

            ImGui.PopStyleColor();

            return result;
        }

        public static bool SelectedIconButton(char icon, Vector4? color = default)
        {
            color ??= Game.Profile.Theme.BgFaded;

            ImGui.PushFont(FontAwesome.Solid);
            ImGui.PushStyleColor(ImGuiCol.Button, color.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color.Value);

            var pressed = ImGui.Button(icon.ToString());

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopFont();

            return pressed;
        }

        public static bool Image(string id, float maxSize, TextureAtlas? atlas, float scale = 1)
        {
            if (Game.Instance.ImGuiRenderer.GetLoadedTexture(PreviewImage) is Texture2D previousTexture)
            {
                if (previousTexture.Name is not null && previousTexture.Name.Equals("preview", StringComparison.InvariantCultureIgnoreCase))
                {
                    previousTexture.Dispose();
                }
            }

            if (atlas is null)
            {
                var t = Game.Data.FetchTexture(id);
                Game.Instance.ImGuiRenderer.BindTexture(PreviewImage, t, false);
                var size = new Vector2(t.Width, t.Height) * Game.Instance.DPIScale / 100f;

                var factor = (t.Width > maxSize) ? maxSize / t.Width : 1;
                factor = (t.Height * factor > maxSize) ? maxSize / t.Height : factor;
                ImGui.Image(PreviewImage, size * factor * scale);
            }
            else if (atlas.TryCreateTexture(id, out var texture))
            {
                texture.Name = "preview";
                Game.Instance.ImGuiRenderer.BindTexture(PreviewImage, texture, false);
                var size = new Vector2(texture.Width, texture.Height) * Game.Instance.DPIScale / 100f;

                var factor = (texture.Width > maxSize) ? maxSize / texture.Width : 1;
                factor = (texture.Height * factor > maxSize) ? maxSize / texture.Height : factor;
                ImGui.Image(PreviewImage, size * factor * scale);
            } 
            else
            {
                // TODO: Add missing image.
                // GameLogger.Warning($"Missing image '{id}' on atlas '{atlas.Name}'")
                // ImGui.Image(MissingImage, Vector2.One * maxSize);

                return false;
            }

            return true;
        }

        public static void DisabledButton(string text)
            => DisabledButton(() => ImGui.Button(text));

        public static void DisabledButton(Action button)
        {
            var theme = Game.Profile.Theme;
            ImGui.PushStyleColor(ImGuiCol.Button, theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, theme.BgFaded);

            button();

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
        }

        public static bool DrawEnumField<T>(string id, ref T fieldValue) where T : Enum
        {
            var t = fieldValue.GetType();
            if (!t.IsEnum) throw new ArgumentException("T must be an enumerated type");
            
            var (mod, result) = DrawEnumField(id, t, (int)(object)fieldValue!);
            fieldValue = (T)(object)result;

            return mod;
        }

        public static (bool modified, int result) DrawEnumField(string id, Type enumType, int fieldValue)
        {
            int result = fieldValue;
            var fields = Enum.GetNames(enumType);

            bool modified = ImGui.Combo(id, ref result, fields, fields.Length);
            return (modified, result);
        }
    }
}