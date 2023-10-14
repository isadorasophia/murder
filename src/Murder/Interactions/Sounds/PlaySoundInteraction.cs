using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Play Sound On Interaction")]
    public readonly struct PlaySoundInteraction : IInteraction
    {
        public readonly SoundEventId Sound = new();

        public PlaySoundInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            _ = SoundServices.Play(Sound);
        }
    }
}