
using Bang.Components;

namespace Murder.Component;

public readonly struct BounceAmountComponent : IComponent
{
    public readonly float Bounciness = 1f;

    public BounceAmountComponent(float bounciness)
    {
        Bounciness = bounciness;
    }
}
