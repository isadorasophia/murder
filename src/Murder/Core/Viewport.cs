using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Core;

public readonly struct Viewport
{

    /// <summary>
    /// The size of the viewport (tipically the game's window)
    /// </summary>
    public readonly Point Size;
    /// <summary>
    /// The resolution that the game is actually rendered
    /// </summary>
    public readonly Point NativeResolution;

    /// <summary>
    /// The scale that is applied to the native resolution before rendering
    /// </summary>
    public readonly Vector2 Scale;
    /// <summary>
    /// The offset point where the game should rendered on the screen. Tipically a result of the pillowbox or letterbox
    /// </summary>
    public readonly Vector2 Offset;
    /// <summary>
    /// The rectangle where the game should be rendered on the screen.
    /// </summary>
    public readonly IntRectangle OutputRectangle;

    public Viewport(Point viewportSize, Point nativeResolution, ViewportResizeStyle resizeStyle)
    {

        Size = viewportSize;
        NativeResolution = nativeResolution;

        switch (resizeStyle.ResizeMode)
        {
            case ViewportResizeMode.None:
                // Ignore the window size, use the game size from settings.
                Scale = Vector2.One;
                OutputRectangle = CenterOutput(nativeResolution.ToVector2(), viewportSize);
                Offset = OutputRectangle.TopLeft.ToVector2();
                break;

            case ViewportResizeMode.Stretch:
                // Stretch everything, ignoring aspect ratio.
                Scale = new Vector2(viewportSize.X / (float)nativeResolution.X, viewportSize.Y / (float)nativeResolution.Y);

                Scale = SnapToInt(Scale, resizeStyle.SnapToInteger, resizeStyle.RoundingMode);

                Offset = Vector2.Zero;
                OutputRectangle = new IntRectangle(0, 0, viewportSize.X, viewportSize.Y);
                break;

            case ViewportResizeMode.Letterbox:
                // Letterbox the game, keeping aspect ratio with some allowance.

                // Calculate the aspect ratios
                float windowAspectRatio = viewportSize.X / (float)viewportSize.Y;
                float unscaledAspectRatio = nativeResolution.X / (float)nativeResolution.Y;

                // Interpolate between the unscaled and window aspect ratios based on the allowance
                float targetAspectRatio;
                if (unscaledAspectRatio < windowAspectRatio)
                {
                    targetAspectRatio = Calculator.Approach(unscaledAspectRatio, windowAspectRatio, resizeStyle.PositiveApectRatioAllowance);
                }
                else
                {
                    targetAspectRatio = Calculator.Approach(unscaledAspectRatio, windowAspectRatio, resizeStyle.NegativeApectRatioAllowance);
                }

                // Calculate target size based on the interpolated aspect ratio
                int targetWidth, targetHeight;

                if (windowAspectRatio > targetAspectRatio)
                {
                    // Window is wider than target, adjust width to maintain aspect ratio
                    targetHeight = viewportSize.Y;
                    targetWidth = (int)(targetHeight * targetAspectRatio);
                }
                else
                {
                    // Window is taller than target, adjust height to maintain aspect ratio
                    targetWidth = viewportSize.X;
                    targetHeight = (int)(targetWidth / targetAspectRatio);
                }

                // Figure out the scale
                float scale = targetWidth / (float)nativeResolution.X;
                Scale = new Vector2(scale, scale);

                Scale = SnapToInt(Scale, resizeStyle.SnapToInteger, resizeStyle.RoundingMode);
                
                // For this we change the resolution to the target resolution divided by the scale
                // If no allowance is given, this will be the same as the native resolution
                NativeResolution = new Point(targetWidth / scale, targetHeight / scale);

                // Now from this native resolution we calculate the output rectangle by centering it in the window
                OutputRectangle = CenterOutput(viewportSize.ToVector2(), NativeResolution.ToVector2());
                Offset = OutputRectangle.TopLeft.ToVector2();
                break;

            case ViewportResizeMode.Crop:
                // Center the game in the window, keeping everything else;
                Scale = Vector2.One;
                OutputRectangle = CenterOutput(viewportSize.ToVector2(), nativeResolution.ToVector2());
                Offset = OutputRectangle.TopLeft.ToVector2();
                break;
            default:
                throw new Exception($"Invalid window resize mode ({resizeStyle.ResizeMode}).");
        }
    }

    private static Vector2 SnapToInt(Vector2 scale, float snapToIntegerThreshold, RoundingMode roundingMode)
    {
        Vector2 remaining = (scale - scale.Round());
        Vector2 remainingAbsolute = remaining.Abs();
        Vector2 finalScale = scale;
        if (remainingAbsolute.X < snapToIntegerThreshold || remainingAbsolute.Y < snapToIntegerThreshold)
        {
            switch (roundingMode)
            {
                case RoundingMode.Round:
                    finalScale = scale.Round();
                    break;
                case RoundingMode.Floor:
                    finalScale = scale.Floor();
                    break;
                case RoundingMode.Ceiling: 
                    finalScale = scale.Ceiling();
                    break;
                case RoundingMode.None:
                    break;
                default:
                    throw new Exception("Unknown rounding mode");
            }
        }

        return finalScale.Max(Vector2.One);
    }

    private static IntRectangle CenterOutput(Vector2 targetSize, Vector2 viewportSize)
        {
            return new IntRectangle(
                (int)(targetSize.X - viewportSize.X) / 2,
                (int)(targetSize.Y - viewportSize.Y) / 2,
                viewportSize.X,
                viewportSize.Y);
        }
    public bool HasChanges (Point size, Vector2 scale)
    {
        return Size != size || Scale != scale;
    }
}
