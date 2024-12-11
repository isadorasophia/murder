using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Serialization;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Messages;
using Murder.Editor.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Systems;

[PathfindEditor]
[Watch(typeof(PathfindGridComponent))]
public class UpdatePathfindGridSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities) { }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        if (entities.Length == 0)
        {
            return;
        }

        Entity e = entities[0];
        e.SendMessage(new AssetUpdatedMessage(typeof(PathfindGridComponent)));
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities) { }
}