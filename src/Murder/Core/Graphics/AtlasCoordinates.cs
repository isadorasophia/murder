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

        /// <summary>
        /// The real size of the image, without trimming
        /// </summary>
        public readonly Point Size;
        public readonly IntRectangle SourceRectangle;
        public readonly Rectangle UV;
        public readonly IntRectangle TrimArea;
        public readonly string AtlasId;

        public static AtlasCoordinates Empty = new();

        public AtlasCoordinates(string name, string atlasId, IntRectangle atlasRectangle, IntRectangle trimArea, Point size, int atlasIndex, int atlasWidth, int atlasHeight)
        {
            (Name, SourceRectangle, TrimArea, AtlasIndex) = (name, atlasRectangle, trimArea, atlasIndex);
            AtlasId = atlasId;
            Size = size;

            UV = new Rectangle((float)SourceRectangle.X / atlasWidth, (float)SourceRectangle.Y / atlasHeight, (float)SourceRectangle.Width / atlasWidth, (float)SourceRectangle.Height / atlasHeight);
        }

        /// <summary>
        ///  Draws a partial image stored inside an atlas to the spritebatch.
        /// </summary>
        public void Draw(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, Vector2 scale, float rotation, Vector2 offset, ImageFlip imageFlip, Vector3 blend, MurderBlendState blendState, float sort)
        {
            bool flipH = imageFlip == ImageFlip.Horizontal || imageFlip == ImageFlip.Both;
            bool flipV = imageFlip == ImageFlip.Vertical || imageFlip == ImageFlip.Both;

            
            Vector2 rotationOffsetAdjustment = (new Vector2(
                flipH ? Size.X * scale.X : 0,
                flipV ? Size.Y * scale.Y : 0)).Rotate(rotation);

            // Adjust position for rotation and flip offsets
            Vector2 adjustedPosition = position + rotationOffsetAdjustment;

            if (clip.IsEmpty)
            {
                // Adjust offset for trimming and user-defined offset, considering flips
                Vector2 finalOffset = new Vector2(
                        flipH ? TrimArea.Right + offset.X : offset.X - TrimArea.Left,
                        flipV ? TrimArea.Bottom + offset.Y : offset.Y - TrimArea.Top);

                spriteBatch.Draw(
                    texture: Atlas,
                    position: adjustedPosition.ToXnaVector2(),
                    targetSize: SourceRectangle.Size,
                    sourceRectangle: SourceRectangle,
                    rotation: rotation,
                    scale: scale.ToXnaVector2(),
                    flip: imageFlip,
                    color: color,
                    offset: finalOffset.ToXnaVector2(),
                    blendStyle: blend,
                    blendState: blendState,
                    sort: sort);
            }
            else
            {
                // Gets the intersection between the clip and the trimmed image
                var intersection = Rectangle.Intersection(clip, TrimArea);
                
                
                adjustedPosition -= clip.TopLeft;

                Vector2 finalOffset = new Vector2(
                    flipH ? offset.X + intersection.Right : offset.X - intersection.Left,
                    flipV ? offset.Y + intersection.Bottom : offset.Y - intersection.Top);

                spriteBatch.Draw(
                    Atlas,
                    adjustedPosition.ToXnaVector2(),
                    intersection.Size.ToXnaVector2(),
                    new Rectangle(
                        SourceRectangle.X - TrimArea.X + intersection.X,
                        SourceRectangle.Y - TrimArea.Y + intersection.Y,
                        intersection.Width,
                        intersection.Height),
                    sort,
                    rotation,
                    scale.ToXnaVector2(),
                    imageFlip,
                    color,
                    finalOffset.ToXnaVector2(),
                    blend,
                    blendState
                    );
            }
        }

#if false
        /// <summary>
        ///  Here only for legacy and reference purposes. Use the other Draw method instead
        /// </summary>
        private void DrawLegacy(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, Vector2 scale, float rotation, Vector2 offset, ImageFlip imageFlip, Vector3 blend, float sort)
        {
            if (clip.IsEmpty)
            {
                var flipH = imageFlip == ImageFlip.Horizontal || imageFlip == ImageFlip.Both;
                spriteBatch.Draw(
                    texture: Atlas,
                    position: position + new Vector2((flipH ? Size.X * scale.X : 0), 0).Rotate(rotation),
                    targetSize: SourceRectangle.Size,
                    sourceRectangle: SourceRectangle,
                    rotation: rotation,
                    scale: scale,
                    flip: imageFlip,
                    color: color,
                    offset: new Vector2((flipH ? TrimArea.Right + offset.X : -TrimArea.X + offset.X), offset.Y - TrimArea.Y),
                    blendStyle: blend,
                    sort: sort);
            }
            else
            {
                var intersection = Rectangle.Intersection(clip, TrimArea);

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
#endif

        /// <summary>
        ///  Draws a partial image stored inside an atlas to the spritebatch to a specific rect
        /// </summary>
        public void Draw(Batch2D spriteBatch, Rectangle clip, Rectangle target, Color color, float depthLayer, Vector3 blend)
        {
            var intersection = Rectangle.Intersection(clip, TrimArea);
            var scale = target.Size / clip.Size;

            var adjustPosition = new Vector2(intersection.X - clip.X, intersection.Y - clip.Y);
            spriteBatch.Draw(
                Atlas,
                (target.TopLeft + adjustPosition).ToXnaVector2(),
                (intersection.Size * scale).ToXnaVector2(),
                new Rectangle(
                    SourceRectangle.X - TrimArea.X + intersection.X,
                    SourceRectangle.Y - TrimArea.Y + intersection.Y,
                    intersection.Width,
                    intersection.Height),
                depthLayer,
                0,
                Microsoft.Xna.Framework.Vector2.One,
                ImageFlip.None,
                color,
                Microsoft.Xna.Framework.Vector2.Zero,
                blend,
                MurderBlendState.AlphaBlend // Default blend state
                );
        }
    }
}