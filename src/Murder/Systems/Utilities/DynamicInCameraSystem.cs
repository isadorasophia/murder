using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System;
using System.Numerics;

namespace Murder.Systems;

[Filter(typeof(IMurderTransformComponent))]
[Filter(ContextAccessorFilter.AnyOf, typeof(SpriteComponent), typeof(AgentSpriteComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(StaticComponent))]
public sealed class DynamicInCameraSystem : IMonoPreRenderSystem
{
    /// <summary>
    /// Returns a conservative axis-aligned bounding box that fully contains
    /// the sprite after scale, flip, origin offset and rotation.
    /// </summary>
    public static Rectangle CalculateBounds(
        Vector2 position,
        Vector2 origin,
        Point size,
        Vector2 scale,
        ImageFlip flip = ImageFlip.None)
    {
        // ----- Apply flipping to scale -------------------------------------------------
        if ((flip & ImageFlip.Horizontal) != 0)
            scale.X = -scale.X;

        if ((flip & ImageFlip.Vertical) != 0)
            scale.Y = -scale.Y;

        // ----- World-space half-extents (after scale) ----------------------------------
        float absScaleX = Math.Abs(scale.X);
        float absScaleY = Math.Abs(scale.Y);

        float halfW = size.X * absScaleX * 0.5f;
        float halfH = size.Y * absScaleY * 0.5f;

        // ----- Bounding circle radius --------------------------------------------------
        float radius = MathF.Sqrt(halfW * halfW + halfH * halfH);

        // Position is typically centre; offset origin first
        Vector2 worldPos = position - Vector2.Multiply(origin, scale);

        Vector2 min = new(worldPos.X - radius, worldPos.Y - radius);
        int wh = (int)(radius * 2);

        return new Rectangle((int)min.X, (int)min.Y, wh, wh);
    }

    public void BeforeDraw(Context context)
    {
        var world = (MonoWorld)context.World;
        var camera = world.Camera;

        Rectangle cameraBounds = camera.SafeBounds;
        Vector2 cameraPos = camera.Position;

        // Respect scene-override camera
        if (context.World.TryGetUniqueDisableSceneTransitionEffects()?.OverrideCameraPosition
            is Vector2 overridePos)
        {
            cameraPos = overridePos;
            cameraBounds = cameraBounds.SetPosition(cameraPos);
        }

        foreach (Entity e in context.Entities)
        {
            var transform = e.GetGlobalTransform();
            Vector2 pos = transform.Vector2;

            // ------------  cached bounds -------------------------------------
            if (e.TryGetEntityBoundsCache() is EntityBoundsCacheComponent cache)
            {
                Rectangle worldBounds = cache.Bounds.AddPosition(pos);
                if (cameraBounds.Touches(worldBounds))
                    e.SetInCamera();
                else
                    e.RemoveInCamera();

                continue;
            }

            // ------------ Sprite  ------------------------------------------------
            if (e.TryGetSprite() is SpriteComponent sprite &&
                Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid) is SpriteAsset asset)
            {
                // Parallax adjustment
                Vector2 renderPos = pos;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                    renderPos += cameraPos * (1f - parallax.Factor);

                // Scale / rotation
                Vector2 scale = (e.TryGetScale() is ScaleComponent sc) ? sc.Scale : Vector2.One;
                var flip = (e.TryGetFlipSprite() is FlipSpriteComponent fc) ? fc.Orientation : ImageFlip.None;

                // ----- circle vs camera early-out -------------------------------
                float absScaleX = Math.Abs(scale.X);
                float absScaleY = Math.Abs(scale.Y);
                float radius = 0.5f * MathF.Sqrt(
                    asset.Size.X * asset.Size.X * absScaleX * absScaleX +
                    asset.Size.Y * asset.Size.Y * absScaleY * absScaleY);

                if (!cameraBounds.Expand(radius).Contains(renderPos))
                {
                    e.RemoveInCamera();
                    continue;
                }

                // ----- Full AABB only if needed ---------------------------------------
                Rectangle aabb = CalculateBounds(
                    renderPos,
                    sprite.Offset + asset.Origin,
                    asset.Size,
                    scale,
                    flip);

                // Cache bounds for future frames if entity is static
                if (e.HasStatic())
                    e.SetEntityBoundsCache(aabb.AddPosition(-pos));

                if (sprite.TargetSpriteBatch == Batches2D.UiBatchId || cameraBounds.Touches(aabb))
                    e.SetInCamera();
                else
                    e.RemoveInCamera();

                continue;
            }

            // ------------ Collider ---------------------------------------------
            if (e.TryGetCollider() is ColliderComponent col)
            {
                Rectangle worldBox = col.GetBoundingBox(pos);

                // Cache once for static colliders
                if (e.HasStatic())
                    e.SetEntityBoundsCache(worldBox.AddPosition(-pos));

                if (cameraBounds.Touches(worldBox))
                    e.SetInCamera();
                else
                    e.RemoveInCamera();

                continue;
            }

            // ------------ Fallback: 1-pixel -------------------------------------
            if (cameraBounds.Contains(pos.ToPoint()))
                e.SetInCamera();
            else
                e.RemoveInCamera();

            // Cache a 1-pixel box if it will never change
            if (e.HasStatic())
                e.SetEntityBoundsCache(Rectangle.One);
        }
    }
}
