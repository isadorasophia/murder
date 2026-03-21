using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components;

public readonly struct AgentPauseComponent : IComponent
{
    public AgentPauseComponent() { }
}

/// <summary>
/// This will disable any conversion from impulse to velocity, but it will
/// apply friction every frame according to <see cref="FrictionComponent"/>
/// set on the entity.
/// If you want to control the velocity of the agent without friction, use <see cref="DisableAgentComponent"/>.
/// </summary>
[RuntimeOnly]
public readonly struct AgentPauseRuntimeComponent : IComponent
{
    public readonly int Count = 1;

    public AgentPauseRuntimeComponent() { }

    public AgentPauseRuntimeComponent(int count) => Count = count;

    public AgentPauseRuntimeComponent Add() => new(Count + 1);
    public AgentPauseRuntimeComponent Remove() => new(Count - 1);
}
