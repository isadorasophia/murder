using Bang.Components;
using Murder.Core.Sounds;

namespace Murder.Components.Sound
{
    /// <summary>
    /// This will track the spatial position of an event.
    /// </summary>
    public readonly struct SoundEventPositionTrackerComponent : IComponent
    {
        public readonly SoundEventId Sound = new();

        public SoundEventPositionTrackerComponent(SoundEventId sound) => Sound = sound;
    }
}
