using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Diagnostics;

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

            public BoxUiInfo() { }
        }

        public struct ButtonInfo
        {
            public NineSliceInfo ButtonNineSliceInfo { get; init; }

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

        public static Rectangle DrawBoxInCenter(RenderContext render, ReadOnlySpan<string> text, BoxUiInfo boxInfo, ButtonInfo? buttonInfo = null)
        {
            int width = boxInfo.Width;
            float maxCharactersPerLine = width / 4f;

            int linePadding = 4;

            if (boxInfo.BackgroundFadeColor is Color fadeColor)
            {
                // Background fade...
                RenderServices.DrawRectangle(render.UiBatch, new(0, 0, render.Camera.Width, render.Camera.Height), fadeColor, 1f);
            }

            int totalOfLines = 0;
            foreach (string line in text)
            {
                totalOfLines += Calculator.CeilToInt(PixelFont.Escape(line).Length / maxCharactersPerLine);
            }

            Rectangle targetRectangle;
            {
                Vector2 position = render.Camera.Size / 2f - new Vector2(0, (1 - boxInfo.BoxFade) * 4);

                int height = 30 + boxInfo.ExtraHeight + (totalOfLines - 1) * 9;
                targetRectangle = Rectangle.CenterRectangle(position.Point, width, height);
            }

            RenderServices.Draw9Slice(render.UiBatch, boxInfo.NineSliceGuid, targetRectangle, new DrawInfo() { Color = boxInfo.WhiteFadeColor, Sort = 0.7f });

            int currentLine = 1;
            foreach (string line in text)
            {
                // Draw the dialogue line
                RenderServices.DrawText(render.UiBatch, MurderFonts.PixelFont, line, 
                targetRectangle.TopCenter + new Vector2(0, 12 + 6 * currentLine + (currentLine - 1) * linePadding - text.Length * linePadding * 0.5f), width - 30,
                    new DrawInfo(0.6f)
                    {
                        Origin = new Vector2(.5f, 0),
                        Color = boxInfo.TextColor,
                        Outline = boxInfo.BorderColor
                    });

                currentLine += Calculator.CeilToInt(line.Length / maxCharactersPerLine);
            }

            if (buttonInfo is ButtonInfo button && button.ButtonsText.Count != 0)
            {
                int buttonPaddingWidth = width - 40;

                if (button.ButtonsText.Count == 1)
                {
                    IntRectangle doneBox = new(targetRectangle.BottomCenter.Point - new Point(buttonPaddingWidth / 2, 4), buttonPaddingWidth, 18);

                    button.ButtonNineSliceInfo.Draw(render.UiBatch, doneBox, button.IsButtonValid ? "on" : "off", boxInfo.WhiteFadeColor, 0.6f);

                    RenderServices.DrawText(render.UiBatch, MurderFonts.PixelFont, button.ButtonsText[0], doneBox.Center - new Point(0, -(1 - boxInfo.TextFade) * 1), width - 12,
                        new DrawInfo(0.56f)
                        {
                            Origin = Vector2.Center,
                            Color = button.ButtonTextColor,
                            Outline = boxInfo.BorderColor
                        });
                }
                else if (button.MenuInfo is MenuInfo menuInfo)
                {
                    int total = button.ButtonsText.Count;
                    if (total != 2)
                    {
                        GameLogger.Error("I still don't know how to do more than two buttons.");
                        return targetRectangle;
                    }

                    int buttonPadding = 5;
                    int boxesWidth = (width - 40 - buttonPadding) / total;

                    for (int i = 0; i < total; ++i)
                    {
                        bool isLeftHalf = i < total / 2;
                        IntRectangle box = new(targetRectangle.BottomCenter.Point - new Point((isLeftHalf ? boxesWidth + buttonPadding : -buttonPadding), 4), boxesWidth, 18);

                        button.ButtonNineSliceInfo.Draw(render.UiBatch, box, menuInfo.Selection == i ? "on" : "off", boxInfo.WhiteFadeColor, 0.6f);

                        RenderServices.DrawText(
                            render.UiBatch, MurderFonts.PixelFont, button.ButtonsText[i], box.Center - new Point(0, -(1 - boxInfo.TextFade) * 1), width - 12, new DrawInfo(0.56f)
                            {
                                Origin = Vector2.Center,
                                Color = button.ButtonTextColor,
                                Outline = button.ButtonBorderColor
                            });
                    }
                }
            }

            return targetRectangle;
        }
    }
}
