using Bang.Components;

namespace Murder.Components;

public readonly struct AgentPauseComponent : IComponent
{
    public readonly int Count = 1;

    public AgentPauseComponent() { }

    public AgentPauseComponent(int count) => Count = count;

    public AgentPauseComponent Add() => new(Count + 1);
    public AgentPauseComponent Remove() => new(Count - 1);
}
