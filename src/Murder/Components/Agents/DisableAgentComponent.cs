
using Bang.Components;
using Murder.Attributes;

namespace Murder.Components;

/// <summary>
/// Disables the agent from using the AgentMover and other agent related systems
/// </summary>
[DoNotPersistOnSave]
public readonly struct DisableAgentComponent : IComponent
{
    public DisableAgentComponent() { }
}