using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Start Event On Interaction")]
    public readonly struct PlayMusicInteraction : IInteraction
    {
        public readonly SoundEventId Music = new();

        [Tooltip("Whether it should stop playing the last music with fade-out.")]
        public readonly bool StopPrevious = false;

        [Default("Stop specific...")]
        public readonly SoundEventId? PreviousMusic = null;

        public PlayMusicInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (StopPrevious)
            {
                SoundServices.Stop(PreviousMusic, fadeOut: true);
            }

            if (world.TryGetUniqueEntity<MusicComponent>() is not Entity e)
            {
                e = world.AddEntity();
            }

            _ = SoundServices.Play(Music, SoundProperties.Persist | SoundProperties.SkipIfAlreadyPlaying);
        }
    }
}
