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
                    Point smallerNativeResolition = nativeResolution * 0.87f;
                    Vector2 stretchedScale = new Vector2(viewportSize.X / (float)smallerNativeResolition.X, viewportSize.Y / (float)smallerNativeResolition.Y);
                    float targetScale = (Math.Max(Math.Min(stretchedScale.X, stretchedScale.Y), 1));
                    Vector2 outputSize = smallerNativeResolition.ToVector2() * targetScale;

                    Vector2 missingPixels = (viewportSize.ToVector2() - outputSize) / targetScale;
                    float allowance = 0.025f;
                    missingPixels.X = Calculator.Min(missingPixels.X, viewportSize.X * allowance);
                    missingPixels.Y = Calculator.Min(missingPixels.Y, viewportSize.Y * allowance);

                    Point newNativeResolution = new Point(
                        Calculator.CeilingToInt(smallerNativeResolition.X + missingPixels.X),
                        Calculator.CeilingToInt(smallerNativeResolition.Y + missingPixels.Y)
                        );

                    outputSize = newNativeResolution.ToVector2() * targetScale;

                    // Set the scale and output rectangle based on the new native resolution
                    Scale = new Vector2(targetScale);
                    OutputRectangle = CenterOutput(outputSize, viewportSize);
                    NativeResolution = newNativeResolution;
                    break;
                }
            case ScalingKind.Auto:
                {
                    float windowAspectRatio = viewportSize.X / (float)viewportSize.Y;
                    float nativeAspectRatio = nativeResolution.X / (float)nativeResolution.Y;

                    // Nudge aspect ratio slightly toward window's, within allowance
                    float allowance = 0.025f;
                    float targetAspectRatio = nativeAspectRatio;
                    if (windowAspectRatio > nativeAspectRatio)
                    {
                        targetAspectRatio = Calculator.Approach(nativeAspectRatio, windowAspectRatio, allowance);
                    }
                    else
                    {
                        // We allow less vertical expansion of the ratio
                        targetAspectRatio = Calculator.Approach(nativeAspectRatio, windowAspectRatio, allowance * 0.1f);
                    }

                    // Find the integer scale first, based on the minimum axis fit
                    float targetScale = Math.Max(
                        Math.Min(viewportSize.X / (nativeResolution.Y * targetAspectRatio),
                                 viewportSize.Y / (float)nativeResolution.Y),
                        1f);
                    float snapedScale = targetScale;// Calculator.CeilingToInt(targetScale - 0.2f);

                    // NOW derive the native resolution that fills the viewport at this integer scale
                    Point newNativeResolution = new Point(
                        Calculator.CeilingToInt(viewportSize.X / (float)snapedScale),
                        Calculator.CeilingToInt(viewportSize.Y / (float)snapedScale)
                    );

                    Point outputSize = new Point(
                        newNativeResolution.X * snapedScale,
                        newNativeResolution.Y * snapedScale
                    );

                    Scale = new Vector2(targetScale);
                    OutputRectangle = CenterOutput(outputSize, viewportSize);
                    NativeResolution = newNativeResolution;
                    break;
                }
            case ScalingKind.Snap:
                {
                    float windowAspectRatio = viewportSize.X / (float)viewportSize.Y;
                    float nativeAspectRatio = nativeResolution.X / (float)nativeResolution.Y;

                    // 1. What's the biggest integer scale that fits native inside the viewport?
                    float rawScale = Math.Min(
                        viewportSize.X / (float)nativeResolution.X,
                        viewportSize.Y / (float)nativeResolution.Y);
                    int snapedScale = Math.Max(Calculator.CeilingToInt(rawScale - 0.15f), 1);

                    // 2. What's our dream output?
                    Vector2 desiredOutput = new Point(
                        nativeResolution.X * snapedScale,
                        nativeResolution.Y * snapedScale
                    );

                    // 3. How different is that from our actual native aspect ratio?
                    float allowance = 0.28f;
                    float targetAspectRatio = nativeAspectRatio;
                    if (windowAspectRatio > nativeAspectRatio)
                    {
                        targetAspectRatio = Calculator.Approach(nativeAspectRatio, windowAspectRatio, allowance);
                    }
                    else
                    {
                        // We allow less vertical expansion of the ratio
                        targetAspectRatio = Calculator.Approach(nativeAspectRatio, windowAspectRatio, allowance * 0.1f);
                    }

                    float aspectDifference = targetAspectRatio - nativeAspectRatio;


                    Point newNativeResolution = new Point(
                        Calculator.CeilingToInt(viewportSize.X / snapedScale),
                        Calculator.CeilingToInt(viewportSize.Y / snapedScale)
                    );

                    // We will have to trim the native resolution to achieve the target aspect ratio
                    if (aspectDifference > 0)
                    {
                        // Window is wider than native, we need to trim width
                        newNativeResolution.X = Calculator.CeilingToInt(newNativeResolution.Y * targetAspectRatio);
                    }
                    else
                    {
                        // Window is taller than native, we need to trim height
                        newNativeResolution.Y = Calculator.CeilingToInt(newNativeResolution.X / targetAspectRatio);
                    }

                    // 5. Output — letterbox handles any remainder
                    Point outputSize = new Point(
                        newNativeResolution.X * snapedScale,
                        newNativeResolution.Y * snapedScale
                    );

                    Scale = new Vector2(snapedScale);
                    OutputRectangle = CenterOutput(outputSize, viewportSize);
                    NativeResolution = newNativeResolution;
                    break;
                }
            default:
                break;
        }

        Center = NativeResolution / 2f;
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
