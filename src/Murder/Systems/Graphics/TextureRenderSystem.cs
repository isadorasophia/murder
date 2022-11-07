using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Data;
using Murder.Services;
using Microsoft.Xna.Framework;

namespace Murder.Systems.Graphics
{
    [Filter(kind: ContextAccessorKind.Read, typeof(TextureComponent), typeof(PositionComponent)), ShowInEditor]
    public class TextureRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                PositionComponent position = e.GetGlobalPosition();
                TextureComponent texture = e.GetTexture();

                // update position...
                if (Game.Data.FetchAtlas(AtlasId.Gameplay).TryGet(texture.Texture, out var textureCoord))
                {
                    textureCoord.Draw(
                        spriteBatch: render.GameplayBatch, 
                        position: position.ToVector2() - textureCoord.SourceRectangle.Size.ToVector2() * texture.Offset,
                        scale: Vector2.One,
                        origin: Vector2.Zero,
                        rotation: 0f,
                        ImageFlip.None,
                        color: Microsoft.Xna.Framework.Color.White,
                        blendStyle: RenderServices.BLEND_NORMAL,
                        depthLayer: RenderServices.YSort(position.Y));
                }
            }

            return default;
        }
    }
}
