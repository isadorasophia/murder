﻿using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Utilities;
using Murder.Core;
using Murder.Messages.Physics;

namespace Murder.Systems;

[Filter(typeof(MovementModAreaComponent))]
[Messager(typeof(OnActorEnteredOrExitedMessage))]
public class AgentMovementModifierSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        var msg = (OnActorEnteredOrExitedMessage)message;
        if (entity.TryGetMovementModArea() is not MovementModAreaComponent area)
            return;

        if (world.TryGetEntity(msg.EntityId) is not Entity actor)
            return;

        if (!area.AffectOnly.HasTags(actor.TryGetTags()))
        {
            return;
        }

        if (msg.Movement == Murder.Utilities.CollisionDirection.Enter)
        {
            if (actor.TryGetInsideMovementModArea() is InsideMovementModAreaComponent currentArea)
            {
                actor.SetInsideMovementModArea(currentArea.AddArea(area));
            }
            else
            {
                actor.SetInsideMovementModArea(area);
            }
        }
        else
        {
            if (actor.TryGetInsideMovementModArea() is InsideMovementModAreaComponent currentArea)
            {
                if (currentArea.RemoveArea(area) is InsideMovementModAreaComponent newAreaInfo)
                {
                    actor.SetInsideMovementModArea(newAreaInfo);
                }
            }
        }
    }
}