using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Data;
using Murder.Utilities;
using System.Numerics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// An image coordinate inside an atlas
    /// </summary>
    public readonly struct AtlasCoordinates
    {
        public Texture2D Atlas => Game.Data.FetchAtlas(AtlasId).Textures[AtlasIndex];
        public Point AtlasSize => new(Atlas.Width, Atlas.Height);
        public int Width => SourceRectangle.Width;
        public int Height => SourceRectangle.Height;

        public readonly int AtlasIndex;
        public readonly string Name;
        public readonly Point Size;
        public readonly IntRectangle SourceRectangle;
        public readonly Rectangle UV;
        public readonly IntRectangle TrimArea;
        public readonly AtlasId AtlasId;

        public static AtlasCoordinates Empty = new();

        public AtlasCoordinates(string name, AtlasId atlasId, IntRectangle atlasRectangle, IntRectangle trimArea, Point size, int atlasIndex, int atlasWidth, int atlasHeight)
        {
            (Name, SourceRectangle, TrimArea, AtlasIndex) = (name, atlasRectangle, trimArea, atlasIndex);
            AtlasId = atlasId;
            Size = size;

            UV = new Rectangle((float)SourceRectangle.X / atlasWidth, (float)SourceRectangle.Y / atlasHeight, (float)SourceRectangle.Width / atlasWidth, (float)SourceRectangle.Height / atlasHeight);
        }

        /// <summary>
        ///  Draws a partial image stored inside an atlas to the spritebatch.
        /// </summary>
        public void Draw(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, Vector2 scale, float rotation, Vector2 offset, ImageFlip imageFlip, Vector3 blend, float sort)
        {
            if (clip.IsEmpty)
            {
                var flipH = imageFlip == ImageFlip.Horizontal || imageFlip == ImageFlip.Both;
                spriteBatch.Draw(
                    texture: Atlas,
                    position: position + new Vector2((flipH ? Size.X : 0), 0).Rotate(rotation),
                    targetSize: SourceRectangle.Size,
                    sourceRectangle: SourceRectangle,
                    rotation: rotation,
                    scale: scale,
                    flip: imageFlip,
                    color: color,
                    offset: new Vector2((flipH ? TrimArea.Width + offset.X + TrimArea.X : -TrimArea.X + offset.X), offset.Y - TrimArea.Y),
                    blendStyle: blend,
                    sort: sort);
            }
            else
            {
                var intersection = Rectangle.GetIntersection(clip, TrimArea);
                var adjustPosition = new Vector2(intersection.X - clip.X, intersection.Y - clip.Y);
                spriteBatch.Draw(
                    Atlas,
                    position + adjustPosition,
                    intersection.Size,
                    new Rectangle(
                        SourceRectangle.X - TrimArea.X + intersection.X,
                        SourceRectangle.Y - TrimArea.Y + intersection.Y,
                        intersection.Width,
                        intersection.Height),
                    sort,
                    rotation,
                    scale,
                    imageFlip,
                    color,
                    offset,
                    blend
                    );
            }
        }

        /// <summary>
        ///  Draws a partial image stored inside an atlas to the spritebatch to a specific rect
        /// </summary>
        public void Draw(Batch2D spriteBatch, Rectangle clip, Rectangle target, Color color, float depthLayer, Vector3 blend)
        {
            var intersection = Rectangle.GetIntersection(clip, TrimArea);
            var scale = target.Size / clip.Size;

            var adjustPosition = new Vector2(intersection.X - clip.X, intersection.Y - clip.Y);
            spriteBatch.Draw(
                Atlas,
                target.TopLeft + adjustPosition,
                intersection.Size * scale,
                new Rectangle(
                    SourceRectangle.X - TrimArea.X + intersection.X,
                    SourceRectangle.Y - TrimArea.Y + intersection.Y,
                    intersection.Width,
                    intersection.Height),
                depthLayer,
                0,
                Vector2.One,
                ImageFlip.None,
                color,
                Vector2.Zero,
                blend
                );
        }
    }
}