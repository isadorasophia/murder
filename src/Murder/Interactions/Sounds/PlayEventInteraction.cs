using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions
{
    [Sound]
    [CustomName("\uf2a2 Play Event On Interaction")]
    public readonly struct PlayEventInteraction : IInteraction
    {
        public readonly SoundEventId? Event = null;

        public readonly SoundProperties Properties = SoundProperties.Persist;
        public readonly SoundLayer Layer = SoundLayer.Ambience;

        public PlayEventInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (Event is null)
            {
                return;
            }

            if (Properties.HasFlag(SoundProperties.StopOtherEventsInLayer))
            {
                SoundServices.Stop(id: null, fadeOut: true);
            }

            SoundProperties properties = Properties;
            if (Properties.HasFlag(SoundProperties.Persist))
            {
                properties |= SoundProperties.SkipIfAlreadyPlaying;
            }

            if (interactor.HasOnExitMessage())
            {
                SoundServices.Stop(Event.Value, fadeOut: true, interactor.EntityId);
            }
            else
            {
                _ = SoundServices.Play(Event.Value, interactor, Layer, properties);
            }
        }
    }
}
