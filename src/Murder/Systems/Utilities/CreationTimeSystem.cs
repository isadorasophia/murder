using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems.Utilities
{
    [Filter(Bang.Contexts.ContextAccessorFilter.NoneOf, typeof(CreatedAtComponent))]
    [Watch(typeof(DestroyAfterSecondsComponent))]
    internal class CreationTimeSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].SetCreatedAt(Game.Now);
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
        }
    }
}
