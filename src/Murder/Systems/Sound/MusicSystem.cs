using Murder.Components;
using Murder.Services;
using Bang;
using Bang.Entities;
using Bang.Systems;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Watch(typeof(MusicComponent))]
    internal class MusicSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                _ = SoundServices.PlayMusic(e.GetMusic().MusicName);
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}
