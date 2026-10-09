using Bang.Components;
using Murder.Core.Sounds;

namespace Murder.Components.Agents;

public readonly struct OverrideAgentSpeedComponent : IComponent
{
    public readonly float MaxSpeed = 0;
    public readonly float Acceleration = -1;

    /// <summary>
    /// Whether if we allow buffs, for example, modifying the speed which was overriden.
    /// </summary>
    public readonly bool AllowAgentSpeedMultipliers { get; init; } = true;

    public OverrideAgentSpeedComponent() { }

    public OverrideAgentSpeedComponent(float maxSpeed)
    {
        MaxSpeed = maxSpeed;
    }

    public OverrideAgentSpeedComponent(float maxSpeed, bool allowMultiplier) : this(maxSpeed)
    {
        AllowAgentSpeedMultipliers = allowMultiplier;
    }

    public OverrideAgentSpeedComponent(float maxSpeed, float acceleration) : this(maxSpeed)
    {
        Acceleration = acceleration;
    }
}

public readonly struct AgentOnMoveTrackerComponent : IComponent
{
    public readonly SoundEventId OnWalk { get; init; } = new();

    public AgentOnMoveTrackerComponent() { }

    public AgentOnMoveTrackerComponent(SoundEventId onWalk) => OnWalk = onWalk;
}
