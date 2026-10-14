using Bang.Components;
using Murder.Helpers;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Components.Agents;

/// <summary>
/// Updated by AgentCleanupSystem when the agent is moving.
/// This component is not to be added or remove or modified in any way except by the AgentCleanupSystem
/// </summary>
[RuntimeOnly]
public readonly struct AgentWalkingComponent : IComponent//, IDoNotCheckOnReplaceTag//, IDoNotTrackMeTag
{
    public readonly Vector2 Impulse;
    public readonly Direction Direction;

    public AgentWalkingComponent(Vector2 impulse)
    {
        Impulse = impulse;
        Direction = DirectionHelper.FromVector(impulse);
    }
}
