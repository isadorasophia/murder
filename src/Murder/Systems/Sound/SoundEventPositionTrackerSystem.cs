using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components.Sound;
using Murder.Services;

namespace Murder.Systems.Sound
{
    [Filter(typeof(SoundEventPositionTrackerComponent))]
    internal class SoundEventPositionTrackerSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                SoundEventPositionTrackerComponent tracker = e.GetSoundEventPositionTracker();
                if (tracker.Properties == SoundEventPositionTrackerProperties.FollowListenerPosition)
                {
                    Game.Sound.UpdateEvent(tracker.Sound, e.EntityId, Game.Sound.LastListenerPosition);
                }
                else
                {
                    SoundServices.TrackEventSourcePosition(tracker.Sound, e);
                }
            }
        }
    }
}
