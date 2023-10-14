using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Watch(typeof(SoundComponent))]
    internal class SoundsSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var sound = e.GetSound();
                if (sound.Sound.HasValue)
                {
                    _ = SoundServices.Play(sound.Sound.Value);
                }

                if (sound.DestroyEntity)
                {
                    e.Destroy();
                }
                else
                {
                    e.RemoveSound();
                }
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}