using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Helpers;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    [Filter(typeof(ITransformComponent))]
    [Filter(filter: ContextAccessorFilter.AnyOf, typeof(AsepriteComponent), typeof(AgentSpriteComponent))]
    internal class AsepriteRenderDebugSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                AsepriteComponent? aseprite = e.TryGetAseprite();
                AgentSpriteComponent? agentSprite = e.TryGetAgentSprite();
                IMurderTransformComponent transform = e.GetGlobalTransform();

                Vector2 offset = aseprite.HasValue ? aseprite.Value.Offset : Vector2.Zero;
                Batch2D batch = aseprite.HasValue ? render.GetSpriteBatch(aseprite.Value.TargetSpriteBatch) :
                    render.GameplayBatch;
                
                int ySortOffset = aseprite.HasValue ? aseprite.Value.YSortOffset : agentSprite!.Value.YSortOffset;
                if (e.HasComponent<ShowYSortComponent>())
                {
                    RenderServices.DrawHorizontalLine(
                    render.DebugSpriteBatch,
                    (int)render.Camera.Bounds.Left,
                    (int)(transform.Y + ySortOffset),
                    (int)render.Camera.Bounds.Width,
                    Color.BrightGray,
                    0.2f);
                }
                
                if (!render.Camera.SafeBounds.Contains(transform.Vector2))
                {
                    continue;
                }
                
                float rotation = transform.Angle;
                if (aseprite.HasValue && aseprite.Value.RotateWithFacing && e.TryGetFacing() is FacingComponent facing)
                {
                    rotation += DirectionHelper.Angle(facing.Direction);
                }

                float ySort = RenderServices.YSort(transform.Y + ySortOffset);

                string animationId;
                AsepriteAsset? asset;
                float start;
                bool flip;
                
                if (aseprite.HasValue)
                {
                    (animationId, asset, start, flip) = 
                        (aseprite.Value.AnimationId, Game.Data.TryGetAsset<AsepriteAsset>(aseprite.Value.AnimationGuid), aseprite.Value.AnimationStartedTime, false);
                }
                else
                {
                    (animationId, asset, start, flip)  = GetAgentAsepriteSettings(e);
                }

                Color baseColor = Color.White;
                if (e.HasComponent<IsPlacingComponent>())
                {
                    baseColor = baseColor.WithAlpha(.5f);
                }

                Vector2 renderPosition;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                {
                    renderPosition = transform.Vector2 + render.Camera.Position * (1 - parallax.Factor);
                }
                else
                {
                    renderPosition = transform.Vector2;
                }

                if (asset is not null)
                {
                    if (e.HasComponent<IsSelectedComponent>())
                    {
                        _ = RenderServices.RenderSpriteWithOutline(
                            batch,
                            render.Camera,
                            renderPosition,
                            animationId,
                            asset,
                            start,
                            -1,
                            offset,
                            flip,
                            rotation,
                            baseColor,
                            RenderServices.BLEND_NORMAL,
                            ySort);
                    }
                    else
                    {
                        _ = RenderServices.RenderSprite(
                            batch,
                            render.Camera,
                            renderPosition,
                            animationId,
                            asset,
                            start,
                            -1,
                            offset,
                            flip,
                            rotation,
                            Vector2.One,
                            baseColor,
                            RenderServices.BLEND_NORMAL,
                            ySort);
                    }
                }
            }

            return default;
        }
        
        private (string animationId, AsepriteAsset? asset, float start, bool flip) GetAgentAsepriteSettings(Entity e)
        {
            AgentSpriteComponent sprite = e.GetAgentSprite();
            FacingComponent facing = e.GetFacing();
            
            float start = NoiseHelper.Simple01(e.EntityId * 10) * 5f;
            var prefix = sprite.IdlePrefix;

            var suffix = facing.Direction.ToCardinal();
            bool flip = false;

            AsepriteAsset? asepriteAsset = Game.Data.TryGetAsset<AsepriteAsset>(sprite.AnimationGuid);
            if (asepriteAsset is not null && !asepriteAsset.Animations.ContainsKey(prefix + facing.Direction))
            {
                (suffix, flip) = facing.Direction.ToCardinalFlipped();
            }

            return (prefix + suffix, asepriteAsset, start, flip);
        }
    }
}
