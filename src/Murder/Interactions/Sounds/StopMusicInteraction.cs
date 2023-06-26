using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Stop Event On Interaction")]
    public readonly struct StopMusicInteraction : Interaction
    {
        public readonly bool FadeOut = true;

        public StopMusicInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            SoundServices.StopAll(FadeOut);
        }
    }
}
