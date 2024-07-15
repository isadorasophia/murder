using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Play Event On Interaction")]
    public readonly struct PlayEventInteraction : IInteraction
    {
        public readonly SoundEventId Event = new();

        public readonly SoundProperties Properties = SoundProperties.Persist;
        public readonly SoundLayer Layer = SoundLayer.Ambience;

        public PlayEventInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (Properties.HasFlag(SoundProperties.StopOtherEventsInLayer))
            {
                SoundServices.Stop(id: null, fadeOut: true);
            }

            SoundProperties properties = Properties;
            if (Properties.HasFlag(SoundProperties.Persist))
            {
                properties |= SoundProperties.SkipIfAlreadyPlaying;
            }

            _ = SoundServices.Play(Event, interactor, Layer, properties);
        }
    }
}
