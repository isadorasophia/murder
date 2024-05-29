using Bang.Components;

namespace Murder.Components;

public readonly struct BounceAmountComponent : IComponent
{
    public readonly float Bounciness = 1f;
    public readonly float GravityMod = 1f;
    public BounceAmountComponent(float bounciness, float gravity)
    {
        Bounciness = bounciness;
        GravityMod = gravity;
    }
}