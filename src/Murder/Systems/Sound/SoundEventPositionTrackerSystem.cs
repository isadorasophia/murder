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
                SoundServices.TrackEventSourcePosition(tracker.Sound, e);
            }
        }
    }
}
