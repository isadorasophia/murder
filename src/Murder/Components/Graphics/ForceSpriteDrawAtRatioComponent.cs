using Bang.Components;

namespace Murder.Components;

public readonly struct ForceSpriteDrawAtRatioComponent : IComponent
{
    public readonly float Ratio = 0;

    public ForceSpriteDrawAtRatioComponent() { }

    public ForceSpriteDrawAtRatioComponent(float ratio) => Ratio = ratio;
}
