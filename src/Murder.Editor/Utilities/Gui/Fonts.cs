using ImGuiNET;
using Murder.Core.Geometry;
using System.Collections.Immutable;

namespace Murder.Editor.ImGuiExtended
{
    public static class Fonts
    {
        public const int FontAwesomeIconRangeStart = 0xe005;
        public const int FontAwesomeIconRangeEnd = 0xf8ff;

        public static ushort[] IconRanges = new ushort[]
        {
            0xe005, 0xf8ff,
            0
        };

        public static ushort[] BasicLatin = new ushort[] { 0x0020, 0x00FF, 0 };

        public static ImFontPtr TitleFont;
        public static ImFontPtr LargeIcons;
    }
}