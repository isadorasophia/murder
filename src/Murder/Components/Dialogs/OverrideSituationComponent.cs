using Bang.Components;

namespace Murder.Components;

public readonly struct OverrideSituationComponent : IComponent
{
    public readonly SituationComponent Situation = new();

    public OverrideSituationComponent() { }

    public OverrideSituationComponent(SituationComponent situation)
    {
        Situation = situation;
    }
}
