using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Messages.Physics;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Systems.Sound;

[Filter(typeof(AmbienceComponent))]
[Messager(typeof(OnCollisionMessage))]
public class AmbienceTrackerSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity interactedEntity, IMessage msg)
    {
        OnCollisionMessage message = (OnCollisionMessage)msg;
        if (world.TryGetEntity(message.EntityId) is not Entity interactorEntity)
        {
            return;
        }

        if (interactorEntity.IsDestroyed ||
            (!interactorEntity.HasAgent() && (!interactorEntity.TryFetchParent()?.HasAgent() ?? true)) ||
            interactorEntity.HasIgnoreUntil())
        {
            return;
        }

        if (interactedEntity.IsDestroyed || interactorEntity.HasIgnoreUntil())
        {
            return;
        }

        if (!IsInteractAllowed(interactorEntity))
        {
            return;
        }

        AmbienceComponent ambience = interactedEntity.GetAmbience();
        if (message.Movement == CollisionDirection.Enter)
        {
            foreach (SoundEventIdInfo info in ambience.Events)
            {
                _ = SoundServices.Play(info.Id, info.Layer, SoundProperties.Persist);
            }
        }
        else if (message.Movement == CollisionDirection.Exit)
        {
            foreach (SoundEventIdInfo info in ambience.Events)
            {
                SoundServices.Stop(info.Id, fadeOut: true);
            }
        }
    }

    protected virtual bool IsInteractAllowed(Entity interactor)
    {
        return true;
    }
}
