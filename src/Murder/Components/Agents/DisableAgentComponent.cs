
using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components;

/// <summary>
/// This will disable any conversion from impulse to velocity,
/// or stopping velocity with friction.
/// Effectively, this will pretend the entity doesn't have a <see cref="AgentComponent"/>.
/// If you want to control the entity to stop with friction, use <see cref="AgentPauseRuntimeComponent"/>.
/// </summary>
[RuntimeOnly]
public readonly struct DisableAgentComponent : IComponent
{
    public readonly int Count = 1;

    public DisableAgentComponent() { }

    public DisableAgentComponent(int count) => Count = count;

    public DisableAgentComponent Add() => new(Count + 1);
    public DisableAgentComponent Remove() => new(Count - 1);
}