using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Messages.Physics;

namespace Murder.Systems;

[Filter(typeof(MovementModAreaComponent))]
[Messager(typeof(OnCollisionMessage))]
public class AgentMovementModifierSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        var msg = (OnCollisionMessage)message;
        if (entity.TryGetMovementModArea() is not MovementModAreaComponent area)
            return;

        if (world.TryGetEntity(msg.EntityId) is not Entity actor)
        {
            // The other entity was destroyed
            return;
        }

        if (!area.AffectOnly.HasTags(actor.TryGetTags()))
        {
            return;
        }

        if (msg.Movement == Murder.Utilities.CollisionDirection.Enter)
        {
            OnEnter(actor, originId: entity.EntityId, area);
        }
        else // On exit
        {
            OnExit(actor, originId: entity.EntityId, area);
        }
    }

    public static void OnEnter(Entity interactor, int originId, MovementModAreaComponent area)
    {
        if (interactor.TryGetInsideMovementModArea() is InsideMovementModAreaComponent currentArea)
        {
            interactor.SetInsideMovementModArea(currentArea.AddArea(originId, area));
        }
        else
        {
            interactor.SetInsideMovementModArea(originId, area);
        }
    }

    public static void OnExit(Entity interactor, int originId, MovementModAreaComponent area)
    {
        if (interactor.TryGetInsideMovementModArea() is InsideMovementModAreaComponent currentArea)
        {
            if (currentArea.RemoveArea(originId, area) is InsideMovementModAreaComponent newAreaInfo)
            {
                interactor.SetInsideMovementModArea(newAreaInfo);
            }
            else
            {
                interactor.RemoveInsideMovementModArea();
            }
        }
    }
}