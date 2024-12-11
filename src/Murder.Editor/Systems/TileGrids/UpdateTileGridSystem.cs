using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Editor.Components;
using Murder.Editor.Messages;
using Murder.Editor.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    [TileEditor]
    [Watch(typeof(TileGridComponent))]
    public class UpdateTileGridSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        { }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                e.SendMessage(new AssetUpdatedMessage(typeof(TileGridComponent)));
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}