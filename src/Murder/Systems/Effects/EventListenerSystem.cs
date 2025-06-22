using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Interactions;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Systems.Effects
{
    [DefaultEditorSystem(startActive: false)]
    [Filter(typeof(EventListenerComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(MuteEventsComponent))]
    [Messager(typeof(AnimationEventMessage))]
    public class EventListenerSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            if (message is not AnimationEventMessage animationEvent)
            {
                GameLogger.Error("How did we receive a message that is not AnimationEventMessage on EventListenerSystem?");
                return;
            }

            // Were we actually listening to this particular event?
            ImmutableDictionary<string, SpriteEventInfo> events = entity.GetEventListener().Events;

            bool alwaysPlay = entity.HasPlayEvenOffScreen() || entity.HasUi() || entity.HasCutsceneAnchors();
            bool canPlay = alwaysPlay || entity.IsInCamera(world);

            if (canPlay && events.TryGetValue(animationEvent.Event, out SpriteEventInfo info))
            {
                // Start doing event actions.
                if (info.Sound is SoundEventId sound)
                {
                    Entity? target = entity;
                    SoundProperties properties = SoundProperties.None;
                    SoundLayer layer = SoundLayer.Sfx;

                    // For now, infer that any sound event id fired from a sprite that persists
                    // is actually an ambience sound.
                    if (info.Persisted is SoundLayer specificLayer)
                    {
                        properties |= SoundProperties.Persist;
                        layer = specificLayer;

                        entity.SetPlayingPersistedEvent();
                    }
                    else if (alwaysPlay)
                    {
                        target = null;
                    }

                    _ = SoundServices.Play(sound, target, layer, properties);
                }

                if (info.Interactions is ImmutableArray<IInteractiveComponent> interactions)
                {
                    foreach (IInteractiveComponent interaction in interactions)
                    {
                        interaction.Interact(world, entity, entity);
                    }
                }
            }
        }
    }

    [DefaultEditorSystem(startActive: false)]
    [Filter(typeof(BroadcastEventMessageComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(MuteEventsComponent))]
    [Messager(typeof(AnimationEventMessage))]
    public class EventBroadcasterSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            if (message is not AnimationEventMessage animationEvent)
            {
                GameLogger.Error("How did we receive a message that is not AnimationEventMessage on EventListenerSystem?");
                return;
            }

            BroadcastEventMessageComponent broadcast = entity.GetBroadcastEventMessage();
            if (entity.TryFetchChild(broadcast.Target) is Entity target)
            {
                target.SendAnimationEventMessage(animationEvent);
            }
        }
    }

    [Filter(typeof(EventListenerComponent))]
    [Watch(typeof(PlayingPersistedEventComponent))]
    public class EventPersistedOnEntityListenerSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities) { }

        public void OnModified(World world, ImmutableArray<Entity> entities) { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                if (e.TryGetEventListener() is EventListenerComponent listener)
                {
                    foreach ((string _, SpriteEventInfo info) in listener.Events)
                    {
                        if (info.Sound is null)
                        {
                            continue;
                        }

                        SoundServices.Stop(info.Sound, fadeOut: true, entityId: e.EntityId);
                    }
                }
            }
        }

        public void OnDeactivated(World world, ImmutableArray<Entity> entities)
        {
            OnRemoved(world, entities);
        }
    }
}