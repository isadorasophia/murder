using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Save;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Core;

public readonly struct Viewport
{
    /// <summary>
    /// The size of the viewport (typically the game's window)
    /// </summary>
    public readonly Point Size;

    /// <summary>
    /// The resolution that the game is actually rendered
    /// </summary>
    public readonly Point NativeResolution;

    /// <summary>
    /// The scale resuling in viewportSize/nativeResolution without any snapping.
    /// </summary>
    public readonly Vector2 OriginalScale;

    /// <summary>
    /// The scale that is applied to the native resolution before rendering
    /// </summary>
    public readonly Vector2 Scale;

    /// <summary>
    /// The rectangle where the game should be rendered on the screen.
    /// </summary>
    public readonly IntRectangle OutputRectangle;

    public readonly Vector2 Center;

    public readonly bool FailedConstraints;

    public Viewport(Point viewportSize, Point nativeResolution, ScalingKind scaling)
    {
        Size = viewportSize;
        NativeResolution = nativeResolution;
        OriginalScale = viewportSize.ToVector2() / nativeResolution.ToVector2();

        switch (scaling)
        {
            case ScalingKind.OneX:
                Scale = Vector2.One;
                OutputRectangle = CenterOutput(nativeResolution.ToVector2(), viewportSize);
                break;
            case ScalingKind.TwoX:
                Scale = new Vector2(2, 2);
                OutputRectangle = CenterOutput(nativeResolution.ToVector2() * 2, viewportSize);
                break;
            case ScalingKind.ThreeX:
                Scale = new Vector2(3, 3);
                OutputRectangle = CenterOutput(nativeResolution.ToVector2() * 3, viewportSize);
                break;
            case ScalingKind.Large:
                {
                    Point scaledNativeResolution = nativeResolution * 0.81f;
                    if (viewportSize.Y < (1080 - 20))
                    {
                        scaledNativeResolution = nativeResolution * 0.79f;
                    }
                    if (viewportSize.Y <= (810))
                    {
                        scaledNativeResolution = nativeResolution * 0.74f;
                    }

                    AutoScale(viewportSize, scaledNativeResolution, out Point newNativeResolution, out float targetScale, out Vector2 outputSize);

                    // Set the scale and output rectangle based on the new native resolution
                    Scale = new Vector2(targetScale);
                    OutputRectangle = CenterOutput(outputSize, viewportSize);
                    NativeResolution = newNativeResolution;
                    break;
                }
            case ScalingKind.Auto:
                {
                    Point scaledNativeResolution = nativeResolution;

                    if (viewportSize.Y < (1440 + 20))
                    {
                        scaledNativeResolution = nativeResolution * 0.95f;
                    }
                    if (viewportSize.Y < (1080 - 20))
                    {
                        scaledNativeResolution = nativeResolution * 0.85f;
                    }
                    if (viewportSize.Y <= (810))
                    {
                        scaledNativeResolution = nativeResolution * 0.8f;
                    }

                    AutoScale(viewportSize, scaledNativeResolution, out Point newNativeResolution, out float targetScale, out Vector2 outputSize);

                    // Set the scale and output rectangle based on the new native resolution
                    Scale = new Vector2(targetScale);
                    OutputRectangle = CenterOutput(outputSize, viewportSize);
                    NativeResolution = newNativeResolution;
                    break;
                }
            case ScalingKind.Snap:
                {
                    const float MinAspectRatio = 4f / 3f;
                    const float MaxAspectRatio = 18f / 9f;

                    // clamp nativeResolution's aspect ratio to [MinAspectRatio, MaxAspectRatio],
                    // preserving whichever dimension is larger.
                    float viewportAspect = (float)viewportSize.X / viewportSize.Y;
                    float clampedAspect = Math.Clamp(viewportAspect, MinAspectRatio, MaxAspectRatio);
                    Point newNativeResolution = nativeResolution;
                    if (nativeResolution.X >= nativeResolution.Y)
                    {
                        // Wider or square — fix width, adjust height.
                        int newHeight = (int)Math.Round(nativeResolution.X / clampedAspect);
                        newNativeResolution = new Point(nativeResolution.X, newHeight);
                    }
                    else
                    {
                        // Taller — fix height, adjust width.
                        int newWidth = (int)Math.Round(nativeResolution.Y * clampedAspect);
                        newNativeResolution = new Point(newWidth, nativeResolution.Y);
                    }

                    Vector2 stretchedScale = viewportSize.ToVector2() / newNativeResolution.ToVector2();
                    float targetScale = Math.Max(Math.Min(stretchedScale.X, stretchedScale.Y), 1);
                    Vector2 outputSize = newNativeResolution.ToVector2() * targetScale;

                    // Snap to integer scale if close enough.
                    float snappedScale = MathF.Round(targetScale + 0.32f);
                    targetScale = snappedScale;
                    newNativeResolution = (outputSize / targetScale).ToPoint();

                    outputSize = newNativeResolution.ToVector2() * targetScale;
                    Scale = new Vector2(targetScale);
                    OutputRectangle = CenterOutput(outputSize, viewportSize);
                    NativeResolution = newNativeResolution;
                    break;
                }
            default:
                break;
        }

        Center = NativeResolution / 2f;
    }
    private static void AutoScale(Point viewportSize, Point nativeResolution, out Point newNativeResolution, out float targetScale, out Vector2 outputSize)
    {
        const float MinAspectRatio = 4f / 3f;
        const float MaxAspectRatio = 18f / 9f;

        // clamp nativeResolution's aspect ratio to [MinAspectRatio, MaxAspectRatio],
        // preserving whichever dimension is larger.
        float viewportAspect = (float)viewportSize.X / viewportSize.Y;
        float clampedAspect = Math.Clamp(viewportAspect, MinAspectRatio, MaxAspectRatio);

        if (nativeResolution.X >= nativeResolution.Y)
        {
            // Wider or square — fix width, adjust height.
            int newHeight = (int)Math.Round(nativeResolution.X / clampedAspect);
            newNativeResolution = new Point(nativeResolution.X, newHeight);
        }
        else
        {
            // Taller — fix height, adjust width.
            int newWidth = (int)Math.Round(nativeResolution.Y * clampedAspect);
            newNativeResolution = new Point(newWidth, nativeResolution.Y);
        }

        Vector2 stretchedScale = viewportSize.ToVector2() / newNativeResolution.ToVector2();
        targetScale = Math.Max(Math.Min(stretchedScale.X, stretchedScale.Y), 1);
        outputSize = newNativeResolution.ToVector2() * targetScale;

        // Snap to integer scale if close enough.
        float snappedScale = MathF.Round(targetScale);
        float diff = targetScale - snappedScale; // positive = above integer, negative = below

        if (diff >= 0 && diff < 0.22f)
        {
            // e.g. 3.12 -> 3: snap down, outputSize stays, native gets larger.
            targetScale = snappedScale;
            newNativeResolution = (outputSize / targetScale).ToPoint();
        }
        else if (diff < 0 && diff > -0.15f)
        {
            // e.g. 0.75 -> 1: snap up by shrinking native so outputSize still fits.
            targetScale = snappedScale;
            newNativeResolution = (outputSize / targetScale).ToPoint();
        }
    }

    private static IntRectangle CenterOutput(Vector2 targetSize, Vector2 viewportSize)
    {
        return new IntRectangle(
            Calculator.RoundToInt((viewportSize.X - targetSize.X) / 2f),
            Calculator.RoundToInt((viewportSize.Y - targetSize.Y) / 2f),
            targetSize.X,
            targetSize.Y);
    }
}
