using Bang.Components;
using Murder.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Agents;

/// <summary>
/// Updated by AgentCleanupSystem when the agent is moving.
/// This component is not to be added or remove or modified in any way except by the AgentCleanupSystem
/// For performance reasons we disabled watching this component
/// </summary>
public readonly struct AgentWalkingComponent : IComponent, IDoNotCheckOnReplaceTag
{
    public readonly Vector2 Impulse;
    public readonly Direction Direction;
    public bool SkipNotificationsOnComponent => true;

    public AgentWalkingComponent(Vector2 impulse)
    {
        Impulse = impulse;
        Direction = DirectionHelper.FromVector(impulse);
    }

}
