using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components;

public readonly struct AgentPauseComponent : IComponent
{
    public AgentPauseComponent() { }
}

[RuntimeOnly]
public readonly struct AgentPauseRuntimeComponent : IComponent
{
    public readonly int Count = 1;

    public AgentPauseRuntimeComponent() { }

    public AgentPauseRuntimeComponent(int count) => Count = count;

    public AgentPauseRuntimeComponent Add() => new(Count + 1);
    public AgentPauseRuntimeComponent Remove() => new(Count - 1);
}
