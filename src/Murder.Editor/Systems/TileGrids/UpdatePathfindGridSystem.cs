using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
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
        if (world.TryGetUnique<EditorComponent>() is not EditorComponent editor || entities.Length == 0)
        {
            return;
        }

        EditorHook hook = editor.EditorHook;
        Entity e = entities[0];

        PathfindGridComponent pathfindGrid = e.GetPathfindGrid();
        hook.OnComponentModified?.Invoke(e.EntityId, pathfindGrid);
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities) { }
}