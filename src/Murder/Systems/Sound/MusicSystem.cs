using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Watch(typeof(MusicComponent))]
    internal class MusicSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            StopAndPlay(entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            StopAndPlay(entities);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }

        private void StopAndPlay(ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                _ = SoundServices.Play(e.GetMusic().Id, SoundLayer.Music, SoundProperties.Persist | SoundProperties.SkipIfAlreadyPlaying | SoundProperties.StopOtherEventsInLayer);
            }
        }
    }
}