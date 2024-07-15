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
    public readonly struct StopEventInteraction : IInteraction
    {
        public readonly bool FadeOut = true;

        [Default("Stop specific...")]
        public readonly SoundEventId? Event = null;

        [Tooltip("Only used if event is not specified and it should be applied to the layer instead")]
        public readonly SoundLayer TargetLayer = SoundLayer.Any;

        public StopEventInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (Event is not null)
            {
                SoundServices.Stop(Event, FadeOut);
            }
            else
            {
                SoundServices.Stop(TargetLayer, FadeOut);
            }
        }
    }
}