using Bang.Components;
using Murder.Attributes;
using Murder.Core;

namespace Murder.Components.Effects;

public enum FadeSpriteFlags
{
    None = 0,
    Alpha = 1 << 0,
    Scale = 1 << 1,
}

[DoNotPersistOnSave]
public readonly struct FadeSpriteComponent : IComponent
{
    public readonly float FadeStartTime;
    public readonly float FadeEndTime;
    public readonly bool DestroyOnEnd;

    public readonly float StartAlpha;
    public readonly float EndAlpha;

    public readonly FadeSpriteFlags FadeSpriteFlags;

    public FadeSpriteComponent(float start, float end, float startAlpha = 1, bool destroyOnEnd = false)
    {
        FadeStartTime = start;
        FadeEndTime = end;
        StartAlpha = startAlpha;
        EndAlpha = 0;
        DestroyOnEnd = destroyOnEnd;
        FadeSpriteFlags = FadeSpriteFlags.Alpha;
    }

    public FadeSpriteComponent(float startTime, float endTime, float startAlpha, float endAlpha, bool destroyOnEnd = false)
    {
        FadeStartTime = startTime;
        FadeEndTime = endTime;
        StartAlpha = startAlpha;
        EndAlpha = endAlpha;
        DestroyOnEnd = destroyOnEnd;
        FadeSpriteFlags = FadeSpriteFlags.Alpha;
    }
    public FadeSpriteComponent(float startTime, float endTime, float startAlpha, float endAlpha, bool destroyOnEnd = false, FadeSpriteFlags flags = FadeSpriteFlags.Alpha)
    {
        FadeStartTime = startTime;
        FadeEndTime = endTime;
        StartAlpha = startAlpha;
        EndAlpha = endAlpha;
        DestroyOnEnd = destroyOnEnd;
        FadeSpriteFlags = flags;
    }
}