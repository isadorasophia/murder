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

    public Viewport(Point viewportSize, Point nativeResolution, ViewportResizeStyle resizeStyle, ScalingKind scaling)
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
                    int snapedScale = Calculator.CeilingToInt(targetScale - 0.2f);

                    // NOW derive the native resolution that fills the viewport at this integer scale
                    Point newNativeResolution = new Point(
                        Calculator.CeilingToInt(viewportSize.X / (float)snapedScale),
                        Calculator.CeilingToInt(viewportSize.Y / (float)snapedScale)
                    );

                    Point outputSize = new Point(
                        newNativeResolution.X * snapedScale,
                        newNativeResolution.Y * snapedScale
                    );

                    Scale = new Vector2(snapedScale);
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
        return;

        switch (resizeStyle.ResizeMode)
        {
            case ViewportResizeMode.None:
                // Ignore the window size, use the game size from settings.
                Scale = Vector2.One;
                OutputRectangle = CenterOutput(nativeResolution.ToVector2(), viewportSize);
                break;

            case ViewportResizeMode.Stretch:
                // Stretch everything, ignoring aspect ratio.
                Scale = new Vector2(viewportSize.X / (float)nativeResolution.X, viewportSize.Y / (float)nativeResolution.Y);
                Scale = new(SnapToInt(Scale.X, resizeStyle.SnapToInteger, resizeStyle.RoundingMode), SnapToInt(Scale.Y, resizeStyle.SnapToInteger, resizeStyle.RoundingMode));

                OutputRectangle = new IntRectangle(0, 0, viewportSize.X, viewportSize.Y);
                break;

            case ViewportResizeMode.KeepRatio:
                {
                    //Scale the game to fit the window, keeping aspect ratio.
                    Vector2 stretchScale = new Vector2(viewportSize.X / (float)nativeResolution.X, viewportSize.Y / (float)nativeResolution.Y);
                    float minScale = Math.Min(stretchScale.X, stretchScale.Y);

                    Vector2 originalScale = new Vector2(minScale, minScale);
                    // Set the output rectangle to center the game in the window with the calculated scale to keep aspect ratio
                    OutputRectangle = CenterOutput(NativeResolution * originalScale, viewportSize);

                    Vector2 snappedScale = new(SnapToInt(originalScale.X, resizeStyle.SnapToInteger, resizeStyle.RoundingMode), SnapToInt(originalScale.Y, resizeStyle.SnapToInteger, resizeStyle.RoundingMode));
                    Scale = snappedScale;

                    // Now change trim the native resolution to account for the possible scale ceiling to integer
                    NativeResolution = new Point(Math.Min(nativeResolution.X, Calculator.RoundToInt(OutputRectangle.Width / snappedScale.X)),
                                           Math.Min(nativeResolution.Y, Calculator.RoundToInt(OutputRectangle.Height / snappedScale.Y)));
                }
                break;

            case ViewportResizeMode.AdaptiveLetterbox:
                {
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

                    // Adjust the native resolution to match the target aspect ratio
                    Point adjustedNativeResolution = new Point(
                        Math.Min(nativeResolution.X, Calculator.RoundToInt(nativeResolution.Y * targetAspectRatio)),
                        Math.Min(nativeResolution.Y, Calculator.RoundToInt(nativeResolution.X / targetAspectRatio))
                        );

                    //Scale the game to fit the window, keeping aspect ratio.
                    Vector2 stretchScale = new Vector2(viewportSize.X / (float)adjustedNativeResolution.X, viewportSize.Y / (float)adjustedNativeResolution.Y);
                    float minScale = Math.Min(stretchScale.X, stretchScale.Y);
                    float snappedScale = SnapToInt(minScale, resizeStyle.SnapToInteger, resizeStyle.RoundingMode);
                    if (snappedScale - (Math.Truncate(snappedScale)) > 0.5f)
                    {
                        snappedScale = MathF.Ceiling(snappedScale);
                    }

                    float ceilingScale = Calculator.CeilToInt(minScale);

                    Vector2 originalScale = new Vector2(minScale, minScale);
                    // Set the output rectangle to center the game in the window with the calculated scale to keep aspect ratio
                    OutputRectangle = CenterOutput(adjustedNativeResolution * originalScale, viewportSize);
                    Scale = new Vector2(snappedScale);

                    // Now change trim the native resolution to account for the possible scale ceiling to integer
                    NativeResolution = new Point(
                        Calculator.CeilToInt(OutputRectangle.Width / snappedScale),
                        Calculator.CeilToInt(OutputRectangle.Height / snappedScale)
                        );
                }
                break;

            case ViewportResizeMode.Grow:
                {
                    Vector2 stretchedScale = new Vector2(viewportSize.X / (float)nativeResolution.X, viewportSize.Y / (float)nativeResolution.Y);
                    float ceiledYScale;
                    ceiledYScale = MathF.Ceiling(Math.Max(stretchedScale.Y - 0.1f, 1));

                    Vector2 outputSize = nativeResolution.ToVector2() * ceiledYScale;

                    // Now we see how many pixels are missing from the viewport and adjust the native resolution to fill the gap
                    Vector2 missingPixels = (viewportSize.ToVector2() - outputSize) / ceiledYScale;

                    NativeResolution = new Point(
                        Calculator.RoundToInt(nativeResolution.X + missingPixels.X),
                        Calculator.RoundToInt(nativeResolution.Y + missingPixels.Y)
                        );

                    OutputRectangle = CenterOutput(NativeResolution * ceiledYScale, viewportSize);
                    Scale = new Vector2(ceiledYScale);
                }
                break;
            case ViewportResizeMode.ConstrainedGrow:
                {
                    // Get min / max constraints, defaulting to sensible values if not provided
                    Point minRes = resizeStyle.MinNativeResolution ?? new Point(
                        Calculator.RoundToInt(nativeResolution.X * 0.8f),
                        Calculator.RoundToInt(nativeResolution.Y * 0.8f)
                    );
                    Point maxRes = resizeStyle.MaxNativeResolution ?? new Point(
                        Calculator.RoundToInt(nativeResolution.X * 1.2f),
                        Calculator.RoundToInt(nativeResolution.Y * 1.2f)
                    );

                    Vector2 stretchedScale = new Vector2(viewportSize.X / (float)nativeResolution.X, viewportSize.Y / (float)nativeResolution.Y);
                    int targetScale = (int)Math.Ceiling(Math.Max(stretchedScale.Y - 0.05f, 1));
                    Vector2 outputSize = nativeResolution.ToVector2() * targetScale;

                    // Now we see how many pixels are missing from the viewport and adjust the native resolution to fill the gap
                    Vector2 missingPixels = (viewportSize.ToVector2() - outputSize) / targetScale;

                    // Calculate the new native resolution based on the target scale
                    Point newNativeResolution = new Point(
                        Calculator.CeilingToInt(nativeResolution.X + missingPixels.X),
                        Calculator.CeilingToInt(nativeResolution.Y + missingPixels.Y)
                        );

                    if (newNativeResolution.X < minRes.X || newNativeResolution.Y < minRes.Y)
                    {
                        targetScale--;
                        if (targetScale < 1)
                        {
                            targetScale = 1;
                            FailedConstraints = true;
                        }

                        outputSize = nativeResolution.ToVector2() * targetScale;
                        missingPixels = (viewportSize.ToVector2() - outputSize) / targetScale;

                        // Calculate the new native resolution based on the target scale
                        newNativeResolution = new Point(
                            Calculator.CeilingToInt(nativeResolution.X + missingPixels.X),
                            Calculator.CeilingToInt(nativeResolution.Y + missingPixels.Y)
                            );
                    }

                    if (newNativeResolution.X > maxRes.X || newNativeResolution.Y > maxRes.Y)
                    {
                        newNativeResolution = new Point(
                            Math.Min(maxRes.X, newNativeResolution.X),
                            Math.Min(maxRes.Y, newNativeResolution.Y));
                    }

                    // Set the scale and output rectangle based on the new native resolution
                    Scale = new Vector2(targetScale);
                    OutputRectangle = CenterOutput(newNativeResolution.ToVector2() * Scale, viewportSize);
                    NativeResolution = newNativeResolution;
                    break;
                }
            case ViewportResizeMode.Crop:
                // Center the game in the window, keeping everything else;
                Scale = Vector2.One;
                OutputRectangle = CenterOutput(viewportSize, nativeResolution);
                break;
            case ViewportResizeMode.AbsoluteScale:
                // Ignore the window size, use the game size from settings.
                Scale = Vector2.One * (resizeStyle.AbsoluteScale ?? 1);
                NativeResolution = (viewportSize / Scale).Point();
                OutputRectangle = new IntRectangle(0, 0, viewportSize.X, viewportSize.Y);
                break;
            default:
                throw new Exception($"Invalid window resize mode ({resizeStyle.ResizeMode}).");
        }

        // Cache
        Center = NativeResolution / 2f;
    }

    private static float SnapToInt(float scale, float snapToIntegerThreshold, RoundingMode roundingMode)
    {
        float remaining = scale - MathF.Round(scale);

        if (remaining > 0 && remaining < snapToIntegerThreshold)
        {
            switch (roundingMode)
            {
                case RoundingMode.Round:
                    return MathF.Round(scale);
                case RoundingMode.Floor:
                    return MathF.Floor(scale);
                case RoundingMode.Ceiling:
                    return scale;
                case RoundingMode.None:
                    return scale;
                default:
                    throw new Exception("Unknown rounding mode");
            }
        }
        else if (remaining < 0 && remaining > -snapToIntegerThreshold)
        {
            switch (roundingMode)
            {
                case RoundingMode.Round:
                    return MathF.Round(scale);
                case RoundingMode.Floor:
                    return scale;
                case RoundingMode.Ceiling:
                    return MathF.Ceiling(scale);
                case RoundingMode.None:
                    return scale;
                default:
                    throw new Exception("Unknown rounding mode");
            }
        }

        return scale;
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
