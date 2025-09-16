using Bang.Components;
using Murder.Attributes;

namespace Murder.Components.Effects;

[DoNotPersistOnSave]
public readonly struct FadeSpriteComponent : IComponent
{
    public readonly float FadeStartTime;
    public readonly float FadeEndTime;
    public readonly bool DestroyOnEnd;
    
    public readonly float StartAlpha;
    public readonly float EndAlpha;

    public FadeSpriteComponent(float start, float end, float startAlpha = 1, bool destroyOnEnd = false)
    {
        FadeStartTime = start;
        FadeEndTime = end;
        StartAlpha = startAlpha;
        EndAlpha = 0;
        DestroyOnEnd = destroyOnEnd;
    }

    public FadeSpriteComponent(float startTime, float endTime, float startAlpha, float endAlpha, bool destroyOnEnd = false)
    {
        FadeStartTime = startTime;
        FadeEndTime = endTime;
        StartAlpha = startAlpha;
        EndAlpha = endAlpha;
        DestroyOnEnd = destroyOnEnd;
    }
}