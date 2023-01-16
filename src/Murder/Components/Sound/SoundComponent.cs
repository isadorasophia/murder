using Murder.Attributes;
using Bang.Components;
using Murder.Assets;
using Murder.Core.Sounds;

namespace Murder.Components
{
    /// <summary>
    /// Sound component which will be immediately played and destroyed.
    /// </summary>
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
