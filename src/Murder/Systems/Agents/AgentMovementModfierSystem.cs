using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Utilities;
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

        if (actor.TryGetTags() is TagsComponent tags && !area.AffectOnly.HasTags(tags.Tags))
        {
            return;
        }

        if (msg.Movement == Murder.Utilities.CollisionDirection.Enter)
        {
            actor.SetInsideMovementModArea(area);
        }
        else
        {
            actor.RemoveInsideMovementModArea();
        }
    }
}