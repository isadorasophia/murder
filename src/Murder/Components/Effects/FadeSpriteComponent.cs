using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components;

public enum FadeSpriteFlags
{
    None = 0,
    Alpha = 1 << 0,
    Scale = 1 << 1,
    DestroyOnEnd = 1 << 2,
    DeactivateOnEnd = 1 << 3,
    Quantize8 = 1 << 4,
    Quantize16 = 1 << 5,
}

/// <summary>
/// Fades a sprite from a range.
/// By default, uses scaled time when taking a duration.
/// </summary>
[RuntimeOnly]
[DoNotPersistOnSave]
public readonly struct FadeSpriteComponent : IComponent
{
    public readonly float FadeStartTime;
    public readonly float FadeEndTime;

    public readonly float StartAlpha;
    public readonly float EndAlpha;

    public readonly FadeSpriteFlags Flags = FadeSpriteFlags.Alpha;

    /// <summary>
    /// Fades a sprite from one alpha to zero alpha.
    /// </summary>
    /// <param name="duration">Duration using scaled time (Game.Now).</param>
    public FadeSpriteComponent(float duration) : this(duration, FadeSpriteFlags.Alpha) { }

    public FadeSpriteComponent(float duration, FadeSpriteFlags flags) : this(Game.Now, Game.Now + duration, flags) { }

    public FadeSpriteComponent(float startTime, float endTime) : this(startTime, endTime, FadeSpriteFlags.Alpha) { }

    public FadeSpriteComponent(float startTime, float endTime, FadeSpriteFlags flags) : this(startTime, endTime, startAlpha: 1, endAlpha: 0, flags) { }

    public FadeSpriteComponent(float startTime, float endTime, float startAlpha, float endAlpha) : this(startTime, endTime, startAlpha, endAlpha, FadeSpriteFlags.Alpha) { }

    public FadeSpriteComponent(float startTime, float endTime, float startAlpha, float endAlpha, FadeSpriteFlags flags)
    {
        FadeStartTime = startTime;
        FadeEndTime = endTime;

        StartAlpha = startAlpha;
        EndAlpha = endAlpha;

        Flags = flags;
    }
}