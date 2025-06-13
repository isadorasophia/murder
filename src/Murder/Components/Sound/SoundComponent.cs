using Bang.Components;
using Murder.Core.Sounds;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// Event component which will be immediately played and destroyed.
    /// </summary>
    [Sound]
    public readonly struct SoundComponent : IComponent
    {
        public readonly SoundEventId? Sound = default;

        public readonly bool DestroyEntity = false;

        public SoundComponent() { }

        public SoundComponent(SoundEventId sound, bool destroyEntity)
        {
            Sound = sound;
            DestroyEntity = destroyEntity;
        }
    }
}