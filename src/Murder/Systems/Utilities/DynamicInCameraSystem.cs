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
[Filter(ContextAccessorFilter.AnyOf, typeof(SpriteComponent), typeof(ColliderComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(StaticComponent))]
public class DynamicInCameraSystem : IMonoPreRenderSystem
{
    public void BeforeDraw(Context context)
    {
        var camera = ((MonoWorld)context.World).Camera;

        Rectangle cameraBounds = camera.Bounds;
        Vector2 cameraPosition = camera.Position;

        if (context.World.TryGetUnique<DisableSceneTransitionEffectsComponent>()?.OverrideCameraPosition is Vector2 overridePosition)
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

                // This is as early as we can to check for out of bounds
                if (sprite.TargetSpriteBatch == TargetSpriteBatches.Ui ||
                    cameraBounds.TouchesWithMaxRotationCheck(renderPosition - ase.Origin, ase.Size, sprite.Offset))
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
                ColliderComponent collider = e.GetCollider();

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
        }
    }

}