using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Data;
using Murder.Services;
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

        public static AtlasCoordinates Empty = new AtlasCoordinates();

        public AtlasCoordinates(string name, AtlasId atlasId, IntRectangle atlasRectangle, IntRectangle trimArea, Point size, int atlasIndex, int atlasWidth, int atlasHeight)
        {
            (Name, SourceRectangle, TrimArea, AtlasIndex) = (name, atlasRectangle, trimArea, atlasIndex);
            AtlasId = atlasId;
            Size = size;

            UV = new Rectangle((float)SourceRectangle.X / atlasWidth, (float)SourceRectangle.Y / atlasHeight, (float)SourceRectangle.Width / atlasWidth, (float)SourceRectangle.Height / atlasHeight);
        }

        /// <summary>
        /// Draws an image stored inside an atlas to the spritebatch.
        /// </summary>
        /// <param name="spriteBatch">The target <see cref="Batch2D"/></param>
        /// <param name="position">The pixel position to draw inside the <see cref="Batch2D"/></param>
        /// <param name="scale">The scale of the image (1 is the actual size).</param>
        /// <param name="origin">The pixel coordinate of the scale and rotation origin.</param>
        /// <param name="rotation">The rotation of the sprite, in radinans.</param>
        /// <param name="color">The color tint (or fill) to be applied to the image. The alpha is also applied to the image for transparency.</param>
        /// <param name="flip">If the image should be flipped horizontally, vertically, both or neither.</param>
        /// <param name="blendStyle">The blend style to be used by the shader. Use the constants in <see cref="RenderServices"/>.</param>
        /// <param name="depthLayer">A number from 0 to 1 that will be used to sort the images. 1 is behind, 0 is in front.</param>
        public void Draw(Batch2D spriteBatch, Vector2 position, Vector2 scale, Vector2 origin, float rotation, ImageFlip flip, Color color, Vector3 blendStyle, float depthLayer)
        {
            var flipH = flip == ImageFlip.Horizontal || flip == ImageFlip.Both;
            spriteBatch.Draw(
                texture: Atlas,
                position: position + new Vector2((flipH ? Size.X: TrimArea.X), TrimArea.Y).Rotate(rotation),
                targetSize: SourceRectangle.Size,
                sourceRectangle: SourceRectangle,
                rotation: rotation,
                scale: scale,
                flip: flip,
                color: color,
                origin: new Vector2((flipH ? TrimArea.Width + origin.X + TrimArea.X: origin.X), origin.Y),
                blendStyle: blendStyle,
                sort: depthLayer);
        }

        /// <summary>
        ///  Draws a partial image stored inside an atlas to the spritebatch.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        /// <param name="clip"></param>
        /// <param name="color"></param>
        /// <param name="depthLayer"></param>
        /// <param name="blend"></param>
        public void Draw(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, float depthLayer, Vector3 blend)
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
                depthLayer,
                0,
                Vector2.One,
                ImageFlip.None,
                color,
                Vector2.Zero,
                blend
                );
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

        /// <summary>
        /// Simpler draw method, just draws the image to the screen at a position. No fancy business.
        /// </summary>
        public void Draw(Batch2D batch, Vector2 position, float sort)
        {
            Draw(batch, position, Vector2.One, Vector2.Zero, 0, ImageFlip.None, Color.White, Vector3.One, sort);
        }

        //private Vector2 GetPosition(bool flipH) => new Vector2((flipH? Size.X + TrimArea.Width - TrimArea.X: TrimArea.X), TrimArea.Y);
        //private Vector2 GetPosition(Vector2 position) => new Vector2(position.X + TrimArea.X, position.Y + TrimArea.Y);
    }
}