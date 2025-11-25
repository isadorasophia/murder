using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components;

/// <summary>
/// Component that will disable effects.
/// </summary>
[RuntimeOnly]
[Unique]
public readonly struct ImmediatelyApplyEffectsComponent : IComponent
{
    public readonly float AddedAt = 0;

    public ImmediatelyApplyEffectsComponent() =>
        AddedAt = Game.NowUnscaled;
}