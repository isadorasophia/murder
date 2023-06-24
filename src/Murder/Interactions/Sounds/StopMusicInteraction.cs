using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Services;

namespace Murder.Interactions
{
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
