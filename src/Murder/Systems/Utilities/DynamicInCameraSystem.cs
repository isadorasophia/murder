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
using System.Numerics;

namespace Murder.Systems;

[Filter(typeof(IMurderTransformComponent))]
[Filter(ContextAccessorFilter.AnyOf, typeof(SpriteComponent), typeof(AgentSpriteComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(StaticComponent))]
public class DynamicInCameraSystem : IMonoPreRenderSystem
{
    public static Rectangle CalculateBounds(Vector2 position, Vector2 origin, Point size, Vector2 scale)
    {
        // Determine if the sprite is flipped
        bool isFlippedHorizontally = scale.X < 0;
        bool isFlippedVertically = scale.Y < 0;

        // Adjust corners based on the flip
        Vector2 topLeft = new Vector2(isFlippedHorizontally ? size.X - origin.X : -origin.X,
                                      isFlippedVertically ? size.Y - origin.Y : -origin.Y);
        Vector2 bottomRight = new Vector2(size.X - topLeft.X, size.Y - topLeft.Y);

        // Apply absolute scale
        topLeft *= new Vector2(Math.Abs(scale.X), Math.Abs(scale.Y));
        bottomRight *= new Vector2(Math.Abs(scale.X), Math.Abs(scale.Y));

        // Adjust for rotation by using the maximum extent
        float maxExtent = Math.Max(topLeft.Length(), bottomRight.Length());

        // Calculate the AABB
        Vector2 min = position - new Vector2(maxExtent, maxExtent);
        Vector2 max = position + new Vector2(maxExtent, maxExtent);

        return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
    }
    public void BeforeDraw(Context context)
    {
        var camera = ((MonoWorld)context.World).Camera;

        Rectangle cameraBounds = camera.SafeBounds;
        Vector2 cameraPosition = camera.Position;

        if (context.World.TryGetUniqueDisableSceneTransitionEffects()?.OverrideCameraPosition is Vector2 overridePosition)
        {
            cameraPosition = overridePosition;
            cameraBounds = cameraBounds.SetPosition(cameraPosition);
        }

        foreach (Entity e in context.Entities)
        {
            Vector2 position = e.GetGlobalTransform().Vector2;

            if (e.TryGetSprite() is SpriteComponent sprite)
            {
                if (Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid) is not SpriteAsset ase)
                {
                    continue;
                }

                Vector2 renderPosition;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                {
                    renderPosition = position + cameraPosition * (1 - parallax.Factor);
                }
                else
                {
                    renderPosition = position;
                }

                Vector2 scale = Vector2.One;
                if (e.TryGetScale() is ScaleComponent scaleCompoennt)
                {
                    scale = scaleCompoennt.Scale;
                }


                Rectangle spriteRect = CalculateBounds(renderPosition, sprite.Offset + ase.Origin, ase.Size, scale);
                // This is as early as we can to check for out of bounds
                if (sprite.TargetSpriteBatch == Batches2D.UiBatchId ||
                    cameraBounds.Touches(spriteRect))
                {
                    e.SetInCamera();
                }
                else
                {
                    e.RemoveInCamera();
                }
            }
            else
            {
                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    Rectangle edges = collider.GetBoundingBox(position);
                    if (cameraBounds.Touches(edges))
                    {
                        e.SetInCamera();
                    }
                    else
                    {
                        e.RemoveInCamera();
                    }
                }
                else
                {
                    // Assume a 1px point
                    if (cameraBounds.Contains(position.ToPoint()))
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