using Bang;
using Bang.Entities;
using Bang.Systems;
using System.Collections.Immutable;
using Murder.Editor.Components;
using Murder.Components;
using Murder.Editor.Utilities;

namespace Murder.Editor.Systems
{
    [Watch(typeof(ColliderComponent))]
    public class UpdateColliderSystem : IReactiveSystem
    {
        public ValueTask OnAdded(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }

        public ValueTask OnModified(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUnique<EditorComponent>() is EditorComponent editor)
            {
                EditorHook hook = editor.EditorHook;
                foreach (Entity e in entities)
                {
                    hook.OnComponentModified?.Invoke(e.EntityId, e.GetComponent<ColliderComponent>());
                }
            }

            return default;
        }

        public ValueTask OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }
    }
}
