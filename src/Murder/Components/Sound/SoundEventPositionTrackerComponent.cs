using Bang.Components;
using Murder.Core.Sounds;
using System.Text.Json.Serialization;

namespace Murder.Components.Sound
{
    public enum SoundEventPositionTrackerProperties
    {
        None = 0,
        FollowListenerPosition = 1
    }

    /// <summary>
    /// This will track the spatial position of an event.
    /// </summary>
    public readonly struct SoundEventPositionTrackerComponent : IComponent
    {
        public readonly SoundEventId Sound = new();
        public readonly SoundEventPositionTrackerProperties Properties { get; init; } = SoundEventPositionTrackerProperties.None;

        public SoundEventPositionTrackerComponent() { }

        public SoundEventPositionTrackerComponent(SoundEventId sound) => Sound = sound;

        [JsonConstructor]
        public SoundEventPositionTrackerComponent(SoundEventId sound, SoundEventPositionTrackerProperties properties)
        {
            Sound = sound;
            Properties = properties;
        }
    }
}
