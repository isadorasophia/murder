using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Core.Graphics
{
    public readonly struct MurderTexture
    {
        private readonly AtlasCoordinates? _atlasCoordinates;
        private readonly string? _texture2D;

        public MurderTexture(AtlasCoordinates AtlasCoordinates)
        {
            _atlasCoordinates = AtlasCoordinates;
            _texture2D = null;
        }

        public MurderTexture(string texture)
        {
            _atlasCoordinates = null;
            _texture2D = texture;
        }

        public void Preload()
        {
            if (_texture2D is not null)
            {
                Game.Data.FetchTexture(_texture2D);
            }
        }

        /// <summary>
        /// Draws a texture with a clipping area.
        /// </summary>
        public void Draw(Batch2D batch2D, Vector2 position, Vector2 scale, Rectangle clip, Color color, ImageFlip flip, float sort, Microsoft.Xna.Framework.Vector3 blend)
        {
            if (_atlasCoordinates.HasValue)
            {
                _atlasCoordinates.Value.Draw(
                    batch2D,
                    position,
                    clip,
                    color,
                    scale,
                    0,
                    Vector2.Zero,
                    flip,
                    blend,
                    sort
                );
            }
            else if (_texture2D is not null)
            {
                Texture2D texture = Game.Data.FetchTexture(_texture2D);

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