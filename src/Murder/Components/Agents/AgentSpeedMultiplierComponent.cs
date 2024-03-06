using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

[RuntimeOnly,DoNotPersistOnSave]
public readonly struct AgentSpeedMultiplierComponent : IComponent
{
    public static readonly ImmutableArray<float> _emptyTemplate = ImmutableArray.Create(1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);

    /// <summary>
    /// Array of speed multiplayers, Currentlty with 8 slots
    /// </summary>
    public readonly ImmutableArray<float> SpeedMultiplier { get; init; }

    public AgentSpeedMultiplierComponent(int slot, float speedMultiplier)
    {
        SpeedMultiplier = _emptyTemplate.SetItem(slot, speedMultiplier);
    }
}