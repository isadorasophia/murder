using Bang.Contexts;
using Bang.Entities;
using Bang;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using System.Collections.Immutable;
using Murder.Utilities;
using Murder.Core.Geometry;

namespace Murder.Editor.Systems.Sounds
{
    [SoundEditor]
    [Filter(ContextAccessorFilter.AnyOf, typeof(SoundComponent), typeof(MusicComponent), typeof(SoundParameterComponent))]
    internal class SoundEditorSystem : GenericSelectorSystem, IUpdateSystem, IMonoRenderSystem
    {
        public void Update(Context context)
        {
            Update(context.World, context.Entities, clearOnlyWhenSelectedNewEntity: false);
        }

        public void Draw(RenderContext render, Context context)
        {
            World world = context.World;

            ImmutableArray<Entity> entities = context.Entities;
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

                Guid asset = Game.Profile.EditorAssets.SoundImage;

                Vector2 position = e.GetGlobalTransform().Vector2;
                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    position = collider.GetBoundingBox(position).CenterPoint;
                }

                if (e.HasMusic() || e.HasSoundParameter())
                {
                    asset = Game.Profile.EditorAssets.MusicImage;
                }

                RenderSprite(render, asset, position, isSelected);
            }
        }

        private void RenderSprite(RenderContext render, Guid asset, Vector2 position, bool isHighlighted)
        {
            RenderServices.DrawSprite(
                render.GameUiBatch,
                asset,
                position,
                new DrawInfo() { Origin = Vector2.Center, Sort = 0, Outline = isHighlighted ? Color.White : null },
                AnimationInfo.Default);
        }
    }
}
