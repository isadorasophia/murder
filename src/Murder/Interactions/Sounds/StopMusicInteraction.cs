using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Stop Event On Interaction")]
    public readonly struct StopMusicInteraction : IInteraction
    {
        public readonly bool FadeOut = true;

        [Default("Stop specific...")]
        public readonly SoundEventId? Music = null;

        public StopMusicInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            SoundServices.Stop(Music, FadeOut);
        }
    }
}
