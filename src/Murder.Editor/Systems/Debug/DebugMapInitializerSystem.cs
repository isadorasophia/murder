using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Editor.Attributes;
using Murder.Systems;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    [WorldEditor(startActive: true)]
    [PathfindEditor]
    [StoryEditor]
    [Watch(typeof(TileGridComponent))]
    internal class DebugMapInitializerSystem : MapInitializerSystem, IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            GridCacheRenderSystem.OnTileGridModified(world);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
        }
    }
}