using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Stop Event On Interaction")]
    public readonly struct StopMusicInteraction : IInteraction
    {
        public readonly bool FadeOut = true;

        [Default("Stop specific...")]
        public readonly SoundEventId? Music = null;

        /// <summary>
        /// TODO: We might replace this with a "category" of sounds we want to exclude/include in the stop.
        /// I will wait until we need that first.
        /// </summary>
        [Tooltip("Ignore stop for the following events")]
        [Default("Except for...")]
        public readonly ImmutableArray<SoundEventId>? ExceptFor = null;

        public StopMusicInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (ExceptFor is not null && ExceptFor.Value.Length > 0)
            {
                SoundServices.StopAll(FadeOut, ExceptFor.Value.ToHashSet());
            }
            else
            {
                SoundServices.Stop(Music, FadeOut);
            }
        }
    }
}