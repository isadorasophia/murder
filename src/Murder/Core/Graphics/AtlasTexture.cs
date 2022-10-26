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
    public readonly struct AtlasTexture
    {
        public Texture2D Atlas => Game.Data.FetchAtlas(AtlasId).Textures[AtlasIndex];
        public Vector2 AtlasSize => new(Atlas.Width, Atlas.Height);
        public int Width => SourceRectangle.Width;
        public int Height => SourceRectangle.Height;

        public Vector2 OriginalSize => new Vector2(Size.X - TrimArea.X + TrimArea.Width, Size.Y + TrimArea.Y + TrimArea.Height);

        public readonly int AtlasIndex;
        public readonly string Name;
        public readonly Point Size;
        public readonly IntRectangle SourceRectangle;
        public readonly Rectangle UV;
        public readonly IntRectangle TrimArea;
        public readonly AtlasId AtlasId;

        public static AtlasTexture Empty = new AtlasTexture();

        public AtlasTexture(string name, AtlasId atlasId, IntRectangle atlasRectangle, IntRectangle trimArea, Point originalSize, int atlasIndex, int atlasWidth, int atlasHeight)
        {
            (Name, SourceRectangle, TrimArea, AtlasIndex) = (name, atlasRectangle, trimArea, atlasIndex);
            AtlasId = atlasId;
            Size = originalSize;

            UV = new Rectangle((float)SourceRectangle.X / atlasWidth, (float)SourceRectangle.Y / atlasHeight, (float)SourceRectangle.Width / atlasWidth, (float)SourceRectangle.Height / atlasHeight);
        }

        /// <summary>
        /// Draws a sprite to the spritebatch.
        /// </summary>
        public void Draw(Batch2D spriteBatch, Vector2 position, float rotation, Color color, float depthLayer)
        {
            spriteBatch.Draw(Atlas, GetPosition(position), SourceRectangle, rotation, Vector2.One, ImageFlip.None, color, Vector2.Zero, RenderServices.BlendNormal, depthLayer);
        }

        public void Draw(Batch2D spriteBatch, Vector2 position, float rotation, Color color, float depthLayer, Vector3 blendMode)
        {
            spriteBatch.Draw(Atlas, GetPosition(position), SourceRectangle, rotation, Vector2.One, ImageFlip.None, color, Vector2.Zero, blendMode, depthLayer);
        }

        public void Draw(Batch2D spriteBatch, Vector2 position, Vector2 size, float rotation, Color color, float depthLayer)
        {
            spriteBatch.Draw(Atlas, GetPosition(position), SourceRectangle, rotation, size/Size, ImageFlip.None, color, Vector2.Zero, RenderServices.BlendNormal, depthLayer);
        }

        public void Draw(Batch2D spriteBatch, Vector2 position, float rotation, Color color, ImageFlip flip, float depthLayer)
        {
            spriteBatch.Draw(Atlas, GetPosition(position, flip == ImageFlip.Horizontal), SourceRectangle, rotation, Vector2.One, flip, color, Vector2.Zero, RenderServices.BlendNormal, depthLayer);
        }

        public void Draw(Batch2D spriteBatch, Vector2 position, float rotation, Color color, ImageFlip flip, float depthLayer, Vector3 colorBlend)
        {
            spriteBatch.Draw(Atlas, GetPosition(position, flip == ImageFlip.Horizontal), SourceRectangle, rotation, Vector2.One, flip, color, Vector2.Zero, colorBlend, depthLayer);
        }

        public void Draw(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, float depthLayer)
        {
            spriteBatch.Draw(Atlas, GetPosition(position), new Rectangle(SourceRectangle.X + clip.X, SourceRectangle.Y + clip.Y, clip.Width, clip.Height), 0, Vector2.One, ImageFlip.None, color, Vector2.Zero, RenderServices.BlendNormal, depthLayer);
        }

        public void Draw(Batch2D spriteBatch, Rectangle destination, Rectangle clip, Color color, float depthLayer)
        {
            var source = new Rectangle(SourceRectangle.X + clip.X, SourceRectangle.Y + clip.Y, clip.Width, clip.Height);
            Vector2 scale = destination.Size / source.Size;
            spriteBatch.Draw(Atlas, destination.TopLeft, source, 0, scale, ImageFlip.None, color, Vector2.Zero, RenderServices.BlendNormal, depthLayer);
        }

        public void Draw(Batch2D spriteBatch, Rectangle destination, Color color, float depthLayer)
        {
            spriteBatch.Draw(
                texture: Atlas,
                position: destination.TopLeft,
                targetSize: Size,
                sourceRectangle: SourceRectangle,
                rotation: 0,
                scale: destination.Size / Size,
                flip: ImageFlip.None,
                color: color,
                origin: Vector2.Zero,
                blendColor: RenderServices.BlendNormal,
                layerDepth: depthLayer);
        }

        public void Draw(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, float depthLayer, Vector3 blend)
        {
            var intersection = Rectangle.GetIntersection(clip, TrimArea);
            var adjustPosition = new Vector2(intersection.X - clip.X, intersection.Y - clip.Y);
            spriteBatch.Draw(
                Atlas,
                position + adjustPosition,
                new Rectangle(
                    SourceRectangle.X - TrimArea.X  + intersection.X,
                    SourceRectangle.Y - TrimArea.Y  + intersection.Y,
                    intersection.Width,
                    intersection.Height),
                0,
                Vector2.One,
                ImageFlip.None,
                color,
                Vector2.Zero,
                blend,
                depthLayer);
        }

        private Vector2 GetPosition(Vector2 position, bool flipH) => new Vector2(position.X + (flipH? Size.X - TrimArea.Width - TrimArea.X: TrimArea.X), position.Y + TrimArea.Y);
        private Vector2 GetPosition(Vector2 position) => new Vector2(position.X + TrimArea.X, position.Y + TrimArea.Y);

        public void Draw(Batch2D spriteBatch, Vector2 position, Vector2 size, float rotation, Color color, Vector2 origin)
        {
            spriteBatch.Draw(
                texture: Atlas,
                position: GetPosition(position, false),
                targetSize: size,
                sourceRectangle: SourceRectangle,
                rotation: rotation,
                scale: Vector2.One,
                flip: ImageFlip.None,
                color: color,
                origin: origin * SourceRectangle.Size,
                blendColor: RenderServices.BlendNormal,
                layerDepth: 1f);
        }

    }
}