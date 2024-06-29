using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Play Sound On Interaction")]
    public readonly struct PlaySoundInteraction : IInteraction
    {
        public readonly SoundEventId Sound = new();

        [Tooltip("Whether this sound should persist. For example, ambience sounds.")]
        public readonly bool Persist = new();

        public PlaySoundInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            SoundProperties properties = SoundProperties.None;
            if (Persist)
            {
                properties = SoundProperties.Persist;
            }

            _ = SoundServices.Play(Sound, interactor, properties);
        }
    }
}