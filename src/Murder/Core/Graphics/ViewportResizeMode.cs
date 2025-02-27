using Murder.Attributes;

namespace Murder.Core.Graphics;
public enum ViewportResizeMode
{
    None,
    Stretch,
    KeepRatio,
    AdaptiveLetterbox,
    Grow,
    Crop,
    AbsoluteScale
}
public readonly struct ViewportResizeStyle
{
    public readonly ViewportResizeMode ResizeMode = ViewportResizeMode.Stretch;


    [Tooltip("Used on letterbox and stretch modes only")]
    public readonly float SnapToInteger = 0.25f;

    public readonly RoundingMode RoundingMode;

    [Tooltip("Used on letterbox and stretch modes only")]
    public readonly float PositiveApectRatioAllowance = 0.5f;

    [Tooltip("Used on letterbox and stretch modes only")]
    public readonly float NegativeApectRatioAllowance = 0.1f;

    public readonly float? AbsoluteScale { get; init; } = null;
    public ViewportResizeStyle()
    {
        
    }
    public ViewportResizeStyle(ViewportResizeMode resizeMode) : this(resizeMode, 0, RoundingMode.None, 0, 0) { }
    public ViewportResizeStyle(ViewportResizeMode resizeMode, float snapToInteger, RoundingMode roundingMode, float positiveApectRatioAllowance, float negativeApectRatioAllowance)
    {
        ResizeMode = resizeMode;
        SnapToInteger = snapToInteger;
        RoundingMode = roundingMode;
        PositiveApectRatioAllowance = positiveApectRatioAllowance;
        NegativeApectRatioAllowance = negativeApectRatioAllowance;
    }
}
