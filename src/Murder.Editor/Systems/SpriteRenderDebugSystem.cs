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
    [Filter(filter: ContextAccessorFilter.AnyOf, typeof(SpriteComponent), typeof(AgentSpriteComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(ThreeSliceComponent))]
    internal class SpriteRenderDebugSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                SpriteComponent? aseprite = e.TryGetSprite();
                AgentSpriteComponent? agentSprite = e.TryGetAgentSprite();
                IMurderTransformComponent transform = e.GetGlobalTransform();

                string animationId;
                SpriteAsset? asset;
                float start;
                bool flip = false;

                Vector2 boundsOffset = Vector2.Zero;
                if (aseprite.HasValue)
                {
                    (animationId, asset, start) =
                        (aseprite.Value.CurrentAnimation, Game.Data.TryGetAsset<SpriteAsset>(aseprite.Value.AnimationGuid), aseprite.Value.AnimationStartedTime);
                    boundsOffset = aseprite.Value.Offset;
                }
                else
                {
                    (animationId, asset, start, flip) = GetAgentAsepriteSettings(e);
                }

                if (asset is null)
                    continue;

                Vector2 renderPosition;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                {
                    renderPosition = transform.Vector2 + render.Camera.Position * (1 - parallax.Factor);
                }
                else
                {
                    renderPosition = transform.Vector2;
                }

                // This is as early as we can to check for out of bounds
                if (!render.Camera.Bounds.Touches(new Rectangle(renderPosition - asset.Size * boundsOffset - asset.Origin, asset.Size)))
                    continue;

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

                float rotation = transform.Angle;
                if (aseprite.HasValue && e.TryGetFacing() is FacingComponent facing)
                {
                    if (aseprite.Value.RotateWithFacing)
                        rotation += DirectionHelper.Angle(facing.Direction);

                    if (aseprite.Value.FlipWithFacing)
                    {
                        flip = facing.Direction.GetFlipped() == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                    }
                }


                float ySort = RenderServices.YSort(transform.Y + ySortOffset);

                Color baseColor = Color.White;
                if (e.HasComponent<IsPlacingComponent>())
                {
                    baseColor = baseColor * .5f;
                }

                RenderServices.DrawSprite(
                    batch,
                    asset.Guid,
                    renderPosition,
                    animationId,
                    start,
                    new DrawInfo()
                    {
                        Origin = offset,
                        FlippedHorizontal = flip,
                        Rotation = rotation,
                        Sort = ySort,
                        UseScaledTime = true,
                        Color = baseColor,
                        Outline = e.HasComponent<IsSelectedComponent>()? Color.White.FadeAlpha(0.65f): null,
                    });
            }
        }
        
        private (string animationId, SpriteAsset? asset, float start, bool flip) GetAgentAsepriteSettings(Entity e)
        {
            AgentSpriteComponent sprite = e.GetAgentSprite();
            FacingComponent facing = e.GetFacing();
            
            float start = NoiseHelper.Simple01(e.EntityId * 10) * 5f;
            var prefix = sprite.IdlePrefix;

            var angle = facing.Direction.Angle() / (MathF.PI * 2); // Gives us an angle from 0 to 1, with 0 being right and 0.5 being left
            (string suffix, bool flip) = DirectionHelper.GetSuffixFromAngle(sprite, angle);

            SpriteAsset? SpriteAsset = Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid);
            
            return (prefix + suffix, SpriteAsset, start, flip);
        }
    }
}
