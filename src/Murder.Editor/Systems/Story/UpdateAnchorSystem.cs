using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components.Serialization;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Messages;
using Murder.Editor.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    [StoryEditor]
    [Watch(typeof(CutsceneAnchorsEditorComponent))]
    public class UpdateAnchorSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        { }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                e.SendMessage(new AssetUpdatedMessage(typeof(CutsceneAnchorsEditorComponent)));
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}