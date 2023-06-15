using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components.Cutscenes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    [StoryEditor]
    [Filter(ContextAccessorFilter.AllOf, ContextAccessorKind.Read, typeof(CutsceneAnchorsComponent))]
    internal class StoryEditorSystem : GenericSelectorSystem, IStartupSystem, IUpdateSystem, IMonoRenderSystem
    {
        private Type[]? _filter = null;

        private ImmutableArray<Entity> FetchEntities(World world) => _filter is null || _filter.Length == 0 ? 
            ImmutableArray<Entity>.Empty : world.GetEntitiesWith(ContextAccessorFilter.AnyOf, _filter);

        public void Start(Context context)
        {
            _filter = StageHelpers.FetchComponentsWithAttribute<StoryAttribute>();
        }

        public void Update(Context context)
        {
            ImmutableArray<Entity> entities = FetchEntities(context.World);
            if (entities.Length == 0)
            {
                return;
            }

            Update(context.World, context.Entities, clearOnlyWhenSelectedNewEntity: true);
        }

        public void Draw(RenderContext render, Context context)
        {
            World world = context.World;

            ImmutableArray<Entity> entities = FetchEntities(world);
            if (entities.Length == 0)
            {
                return;
            }

            DrawImpl(render, world, entities);

            EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;
            foreach (Entity e in entities)
            {
                if (!e.HasTransform()) continue;

                bool isSelected = hook.IsEntitySelected(e.EntityId);

                Vector2 position = e.GetGlobalTransform().Vector2;
                RenderSprite(render, position, isSelected);
            }
        }

        private void RenderSprite(RenderContext render, Vector2 position, bool isHighlighted)
        {
            RenderServices.DrawSprite(
                render.GameUiBatch,
                Game.Profile.EditorAssets.DialogueIconBaloon,
                position,
                new DrawInfo() { Origin = Vector2.Center, Sort = 0, Outline = isHighlighted ? Color.White : null },
                AnimationInfo.Default);
        }
    }
}
