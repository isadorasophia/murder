using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;
namespace Murder.Systems;

[Filter(typeof(TextureComponent), typeof(ITransformComponent))]
[Watch(typeof(TextureComponent))]
public class TextureRenderSystem : IMonoRenderSystem, IReactiveSystem, IExitSystem
{
    public void Draw(RenderContext render, Context context)
    {
        foreach (var e in context.Entities)
        {
            var texture = e.GetTexture();

            var batch = render.GetSpriteBatch(texture.TargetSpriteBatch);

            float alpha = e.TryGetAlpha()?.Alpha ?? 1;

            // Lots of hardcoded stuff here
            // Will update this if the need arrives
            batch.Draw(
                texture.Texture,
                e.GetGlobalTransform().Point + render.Camera.Position,
                texture.Texture.Bounds.Size.ToVector2(),
                texture.Texture.Bounds,
                0,
                0,
                Vector2.One,
                ImageFlip.None,
                Color.White * alpha,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL);
        }
    }

    public void Exit(Context context)
    {
        foreach (var e in context.Entities)
        {
            if (e.TryGetTexture() is not TextureComponent texture)
                return;

            if (texture.AutoDispose)
                texture.Texture.Dispose();
        }
    }

    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        foreach (var e in entities)
        {
            if (e.TryGetTexture() is not TextureComponent texture)
                return;

            if (texture.AutoDispose)
                texture.Texture.Dispose();
        }
    }
}
