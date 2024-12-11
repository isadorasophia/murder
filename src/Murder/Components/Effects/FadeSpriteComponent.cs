using Bang.Components;

namespace Murder.Components.Effects;

public readonly struct FadeSpriteComponent : IComponent
{
    public readonly float FadeStart;
    public readonly float FadeEnd;
    public readonly float MaxAlpha;
    public readonly bool DestroyOnEnd;

    public FadeSpriteComponent(float start, float end, float maxAlpha = 1, bool destroyOnEnd = false)
    {
        FadeStart = start;
        FadeEnd = end;
        MaxAlpha = maxAlpha;
        DestroyOnEnd = destroyOnEnd;
    }
}