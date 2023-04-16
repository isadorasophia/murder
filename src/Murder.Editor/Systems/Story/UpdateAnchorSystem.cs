using Bang;
using Bang.Entities;
using Bang.Systems;
using System.Collections.Immutable;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Editor.Attributes;
using Murder.Components.Cutscenes;

namespace Murder.Editor.Systems
{
    [StoryEditor]
    [Watch(typeof(CutsceneAnchorsComponent))]
    public class UpdateAnchorSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        { }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUnique<EditorComponent>() is EditorComponent editor)
            {
                EditorHook hook = editor.EditorHook;
                foreach (Entity e in entities)
                {
                    hook.OnComponentModified?.Invoke(e.EntityId, e.GetComponent<CutsceneAnchorsComponent>());
                }
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}
