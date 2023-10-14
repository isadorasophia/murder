using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Core.Graphics
{
    public readonly struct MurderTexture
    {
        private readonly AtlasCoordinates? _AtlasCoordinates;
        private readonly string? _texture2D;

        public MurderTexture(AtlasCoordinates AtlasCoordinates)
        {
            _AtlasCoordinates = AtlasCoordinates;
            _texture2D = null;
        }

        public MurderTexture(string texture)
        {
            _AtlasCoordinates = null;
            _texture2D = texture;
        }

        /// <summary>
        /// Draws a texture with a clipping area.
        /// </summary>
        public void Draw(Batch2D batch2D, Vector2 position, Vector2 scale, Rectangle clip, Color color, ImageFlip flip, float sort, Microsoft.Xna.Framework.Vector3 blend)
        {
            if (_AtlasCoordinates.HasValue)
            {
                _AtlasCoordinates.Value.Draw(
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