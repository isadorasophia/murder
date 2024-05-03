using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using System.Numerics;

namespace Murder.Services
{
    public static class MurderUiServices
    {
        public struct BoxUiInfo
        {
            public Guid NineSliceGuid { get; init; }

            public Color TextColor { get; init; }
            public Color BorderColor { get; init; }

            /* Optional */

            /// <summary>
            /// Fade applied on the box when transitioning.
            /// </summary>
            public float BoxFade { get; init; } = 1;

            /// <summary>
            /// Fade applied on the text when transitioning.
            /// Used when only the text fades.
            /// </summary>
            public float TextFade { get; init; } = 1;

            public Color? BackgroundFadeColor { get; init; }

            public int Width { get; init; } = 160;

            /// <summary>
            /// Value used when an extra height is required after the text when drawing the box.
            /// </summary>
            public int ExtraHeight { get; init; } = 5;

            public Color WhiteFadeColor => Color.White * BoxFade;

            public Vector2 TextAlignment { get; init; } = new Vector2(.5f, 0);

            public BoxUiInfo() { }
        }

        public struct ButtonInfo
        {
            public Guid Sprite { get; init; }

            public IList<string> ButtonsText { get; init; } = Array.Empty<string>();

            public Color ButtonTextColor { get; init; }
            public Color ButtonBorderColor { get; init; }

            /* Optional */
            /// <summary>
            /// This is used when multiple buttons are displayed.
            /// </summary>
            public MenuInfo? MenuInfo { get; init; } = null;

            public bool IsButtonValid { get; init; } = false;

            public ButtonInfo() { }
        }
    }
}