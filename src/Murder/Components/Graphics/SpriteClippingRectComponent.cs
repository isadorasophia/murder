using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components;

public readonly struct SpriteClippingRectComponent : IComponent
{
    /// <summary>
    /// When growing from center -1 extends to the end.
    /// </summary>
    public enum ClippingStyle
    {
        GrowFromCenter,
        CutFromBorders
    }

    public readonly float Left;
    public readonly float Right;
    public readonly float Top;
    public readonly float Down;
    public readonly ClippingStyle Style;

    public SpriteClippingRectComponent(float left, float right, float top, float down, ClippingStyle clippingStyle)
    {
        Left = left;
        Right = right;
        Top = top;
        Down = down;
        Style = clippingStyle;
    }

    public Rectangle GetClippingRect(Point spriteSize)
    {
        switch (Style)
        {
            case ClippingStyle.GrowFromCenter:
                {
                    var centerX = spriteSize.X / 2f;
                    var centerY = spriteSize.Y / 2f;

                    // Assuming -1 indicates full extension in the respective direction
                    var left = (Left == -1 ? 0 : centerX - Left);
                    var right = (Right == -1 ? 0 : spriteSize.X - centerX - Right);
                    var top = (Top == -1 ? 0 : centerY - Top);
                    var down = (Down == -1 ? 0 : spriteSize.Y - centerY - Down);

                    var rectWidth = spriteSize.X - left - right;
                    var rectHeight = spriteSize.Y - top - down;

                    return new Rectangle((int)left, (int)top, (int)rectWidth, (int)rectHeight);
                }

            case ClippingStyle.CutFromBorders:
                {
                    // Correct the calculation for width and height. Subtract the left/right (or top/down) from the total size to get width/height.
                    var rectWidth = spriteSize.X - Left - Right;
                    var rectHeight = spriteSize.Y - Top - Down;

                    return new Rectangle((int)Left, (int)Top, (int)rectWidth, (int)rectHeight);
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(Style), $"Unsupported clipping style: {Style}");
        }
    }
}
