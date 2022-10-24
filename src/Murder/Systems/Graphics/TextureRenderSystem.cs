using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Entities;
using Murder.Data;
using Murder.Services;

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
                    textureCoord.Draw(render.SpriteBatch, position.ToVector2() - textureCoord.SourceRectangle.Size.ToVector2() * texture.Offset, 0f,
                        Microsoft.Xna.Framework.Color.White, RenderServices.YSort(position.Y));
                }
            }

            return default;
        }
    }
}
