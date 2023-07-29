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
using Murder.Utilities;
using System.Diagnostics;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.AllOf, typeof(SpriteComponent), typeof(ITransformComponent))]
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

        foreach (var e in context.Entities)
        {
            var sprite = e.GetSprite();
            var transform = e.GetGlobalTransform().Vector2;
            Vector2 renderPosition;

            if (Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid) is not SpriteAsset ase)
            {
                continue;
            }

            if (e.TryGetParallax() is ParallaxComponent parallax)
            {
                renderPosition = transform + cameraPosition * (1 - parallax.Factor);
            }
            else
            {
                renderPosition = transform;
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
    }

}