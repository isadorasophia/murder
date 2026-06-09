using Bang;
using Bang.Components;
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

[Filter(typeof(PositionComponent))]
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

        float width = size.X * absScaleX;
        float height = size.Y * absScaleY;

        float maxOriginFactorX = Math.Max(origin.X, 1 - origin.X);
        float maxOriginFactorY = Math.Max(origin.Y, 1 - origin.Y);

        float halfW = (width * maxOriginFactorX);
        float halfH = (height * maxOriginFactorY);

        // ----- Bounding circle radius --------------------------------------------------
        float radius = MathF.Sqrt(halfW * halfW + halfH * halfH);

        Vector2 min = new(position.X - radius, position.Y - radius);
        float wh = (int)(radius * 2);

        return new Rectangle(min.X, min.Y, wh, wh);
    }

    public void BeforeDraw(Context context)
    {
        var world = (MonoWorld)context.World;
        var camera = world.Camera;

        Rectangle cameraBounds = camera.SafeBounds;
        Vector2 cameraPos = camera.Position; // This avoids multiple property accesses

        // Respect scene-override camera
        if (context.World.TryGetUniqueDisableSceneTransitionEffects()?.ForceCameraPosition
            is Vector2 overridePos)
        {
            cameraPos = overridePos;
            cameraBounds = cameraBounds.SetPosition(cameraPos);
        }

        foreach (Entity e in context.Entities)
        {
            Vector2 pos = e.GetGlobalPosition();

            // ------------ Sprite  ------------------------------------------------
            if (e.TryGetSprite() is SpriteComponent sprite &&
                Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid) is SpriteAsset asset)
            {
                // Parallax adjustment
                Vector2 adjustedPosition = pos;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                    adjustedPosition += cameraPos * (1f - parallax.Factor);

                // Scale / rotation
                Vector2 scale = (e.TryGetScale() is ScaleComponent sc) ? sc.Scale : Vector2.One;
                var flip = (e.TryGetFlipSprite() is FlipSpriteComponent fc) ? fc.Orientation : ImageFlip.None;


                // ----- Full AABB ---------------------------------------
                Rectangle aabb = CalculateBounds(
                    adjustedPosition,
                    (asset.Origin + sprite.Offset) / asset.Size,
                    asset.Size,
                    scale,
                    flip);

                // Cache bounds for future frames if entity is static
                bool inCamera = (sprite.TargetSpriteBatch == Batches2D.UiBatchId || cameraBounds.Touches(aabb));

                if (inCamera != e.HasInCamera())
                {
                    if (inCamera)
                    {
                        e.SetInCamera();
                    }
                    else
                    {
                        e.RemoveInCamera();
                    }
                }

                continue;
            }

            // ------------  cached bounds -------------------------------------
            if (e.TryGetEntityBoundsCache() is EntityBoundsCacheComponent cache)
            {
                Rectangle worldBounds = cache.Bounds;
                bool inCamera = cameraBounds.Touches(worldBounds);
                if (inCamera != e.HasInCamera())
                {
                    if (inCamera)
                    {
                        e.SetInCamera();
                    }
                    else
                    {
                        e.RemoveInCamera();
                    }
                }

                continue;
            }

            // ------------ Collider ---------------------------------------------
            if (e.TryGetCollider() is ColliderComponent col)
            {
                Rectangle worldBox = col.GetBoundingBox(pos, e.FetchScale());

                bool inCamera = (cameraBounds.Touches(worldBox));
                if (inCamera != e.HasInCamera())
                {
                    if (inCamera)
                    {
                        e.SetInCamera();
                    }
                    else
                    {
                        e.RemoveInCamera();
                    }
                }

                continue;
            }

            // ------------ Fallback: 1-pixel -------------------------------------
            {
                bool inCamera = cameraBounds.Contains(pos.ToPoint());
                if (inCamera != e.HasInCamera())
                {
                    if (inCamera)
                    {
                        e.SetInCamera();
                    }
                    else
                    {
                        e.RemoveInCamera();
                    }
                }
            }
        }
    }
}
