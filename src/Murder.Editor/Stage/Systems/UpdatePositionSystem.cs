using InstallWizard.Components;
using InstallWizard.Components.Editor;
using InstallWizard.DebugUtilities;
using Bang;
using Bang.Entities;
using Bang.Systems;
using System.Collections.Immutable;

namespace Editor.Systems
{
    [Watch(typeof(PositionComponent))]
    public class UpdatePositionSystem : IReactiveSystem
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
                    hook.OnComponentModified?.Invoke(e.EntityId, e.GetComponent<PositionComponent>());
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
