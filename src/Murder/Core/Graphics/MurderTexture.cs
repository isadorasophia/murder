using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;

namespace Murder.Core.Graphics
{
    public readonly struct MurderTexture
    {
        private readonly AtlasTexture? _atlasTexture;
        private readonly string? _texture2D;

        public MurderTexture(AtlasTexture atlasTexture)
        {
            _atlasTexture = atlasTexture;
            _texture2D = null;
        }

        public MurderTexture(string texture)
        {
            _atlasTexture = null;
            _texture2D = texture;
        }

        /// <summary>
        /// Draws a texture with a clipping area.
        /// </summary>
        public void Draw(Batch2D batch2D, Vector2 position, Vector2 scale, Rectangle clip, Color color, ImageFlip flip, float sort, Microsoft.Xna.Framework.Vector3 blend)
        {
            if (_atlasTexture.HasValue)
            {
                _atlasTexture.Value.Draw(
                    batch2D,
                    position,
                    clip,
                    color,
                    sort,
                    blend
                );
            }
            else if (_texture2D is not null)
            {
                var texture = Game.Data.FetchTexture(_texture2D);
                
                batch2D.Draw(
                    texture,
                    position,
                    clip.Size,
                    clip,
                    sort,
                    0,
                    scale,
                    flip,
                    color,
                    Vector2.Zero,
                    blend);
            }
            else
            {
                throw new Exception("Texture doesn't have any information.");
            }
        }
    }
}
