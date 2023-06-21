using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Systems.Effects
{
    [Filter(typeof(EventListenerComponent))]
    [Messager(typeof(AnimationEventMessage))]
    internal class EventListenerSystem : IMessagerSystem
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
            if (events.TryGetValue(animationEvent.Event, out SpriteEventInfo info))
            {
                // Start doing event actions.
                if (info.Sound is SoundEventId sound)
                {
                    _ = SoundServices.Play(sound);
                }
            }
        }
    }
}
