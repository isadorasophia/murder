using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Messages.Physics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems.Sound;

[Filter(typeof(AmbienceComponent))]
[Messager(typeof(OnCollisionMessage))]
[Watch(typeof(AmbienceComponent))]
public class AmbienceTrackerSystem : IMessagerSystem, IReactiveSystem
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
                _ = SoundServices.Play(info.Id, info.Layer, SoundProperties.Persist, entityId: interactedEntity.EntityId);
            }
        }
        else if (message.Movement == CollisionDirection.Exit)
        {
            foreach (SoundEventIdInfo info in ambience.Events)
            {
                SoundServices.Stop(info.Id, fadeOut: true, entityId: interactedEntity.EntityId);
            }
        }
    }

    protected virtual bool IsInteractAllowed(Entity interactor)
    {
        return true;
    }

    public void OnAdded(World world, ImmutableArray<Entity> entities) { }

    public void OnModified(World world, ImmutableArray<Entity> entities) { }

    public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

    public void OnActivated(World world, ImmutableArray<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            AmbienceComponent ambience = e.GetAmbience();
            foreach (SoundEventIdInfo info in ambience.Events)
            {
                _ = SoundServices.Play(info.Id, info.Layer, SoundProperties.Persist, entityId: e.EntityId);
            }
        }
    }

    public void OnDeactivated(World world, ImmutableArray<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            AmbienceComponent ambience = e.GetAmbience();
            foreach (SoundEventIdInfo info in ambience.Events)
            {
                SoundServices.Stop(info.Id, fadeOut: true, entityId: e.EntityId);
            }
        }
    }
}
