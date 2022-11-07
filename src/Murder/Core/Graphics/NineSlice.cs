using Microsoft.Xna.Framework;
using Murder.Data;
using Murder.Services;

namespace Murder.Core.Graphics
{
    public struct NineSlice
    {
        readonly AtlasTexture _textureCoord;

        /// <summary>
        /// The size of the cut in pixels
        /// </summary>
        public readonly int SliceSize;
        public Color Color;
        public NineSlice(string texturePath, AtlasId atlasId, int sliceSize) : this(texturePath, atlasId, sliceSize, Color.White) { }
        public NineSlice(string texturePath, AtlasId atlasId, int sliceSize, Color color)
        {
            _textureCoord = Game.Data.FetchAtlas(atlasId).Get(texturePath);
            SliceSize = sliceSize;
            Color = color;
        }

        public void Draw(Batch2D spriteBatch, Rectangle drawArea, float layerDepth)
        {
            // RenderServices.DrawRectangleOutline(spriteBatch, rectangle, Color.Black);

            var rectInterior = new Rectangle(drawArea.X + SliceSize, drawArea.Y + SliceSize, drawArea.Width - SliceSize * 2, drawArea.Height - SliceSize * 2);

            // Top Left
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(drawArea.X, drawArea.Y),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X, _textureCoord.SourceRectangle.Y, SliceSize, SliceSize),
                layerDepth,
                0,
                Vector2.One,
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL          
                );

            // Top
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(drawArea.X + SliceSize, drawArea.Y),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X + SliceSize, _textureCoord.SourceRectangle.Y, _textureCoord.SourceRectangle.Width - SliceSize * 2, SliceSize),
                layerDepth,
                0,
                scale: new Vector2(rectInterior.Width / (_textureCoord.SourceRectangle.Width - SliceSize * 2f), 1),
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL
                );

            // Top Right
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(drawArea.X + drawArea.Width - SliceSize, drawArea.Y),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X + _textureCoord.SourceRectangle.Width - SliceSize, _textureCoord.SourceRectangle.Y, SliceSize, SliceSize),
                layerDepth,
                0,
                scale: Vector2.One,
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL
                );

            // Left
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(drawArea.X, drawArea.Y + SliceSize),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X, _textureCoord.SourceRectangle.Y + SliceSize, SliceSize, _textureCoord.SourceRectangle.Height - SliceSize * 2),
                layerDepth,
                0,
                scale: new Vector2(1, rectInterior.Height / (_textureCoord.SourceRectangle.Height - SliceSize * 2f)),
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL
                );

            // Right
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(drawArea.X + drawArea.Width - SliceSize, drawArea.Y + SliceSize),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X + _textureCoord.SourceRectangle.Width - SliceSize, _textureCoord.SourceRectangle.Y + SliceSize, SliceSize, _textureCoord.SourceRectangle.Height - SliceSize * 2),
                layerDepth,
                0,
                scale: new Vector2(1, rectInterior.Height / (_textureCoord.SourceRectangle.Height - SliceSize * 2f)),
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL
                );

            // Bottom Left
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(drawArea.X, drawArea.Y + drawArea.Height - SliceSize),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X, _textureCoord.SourceRectangle.Y + _textureCoord.SourceRectangle.Height - SliceSize, SliceSize, SliceSize),
                layerDepth,
                0,
                scale: Vector2.One,
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL
                );


            // Bottom
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(drawArea.X + SliceSize, drawArea.Y + drawArea.Height - SliceSize),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X + SliceSize, _textureCoord.SourceRectangle.Y + _textureCoord.SourceRectangle.Height - SliceSize, _textureCoord.SourceRectangle.Width - SliceSize * 2, SliceSize),
                layerDepth,
                0,
                scale: new Vector2(rectInterior.Width / (_textureCoord.SourceRectangle.Width - SliceSize * 2f), 1),
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL
                );

            // Bottom Right
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(drawArea.X + drawArea.Width - SliceSize, drawArea.Y + drawArea.Height - SliceSize),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X + _textureCoord.SourceRectangle.Width - SliceSize, _textureCoord.SourceRectangle.Y + _textureCoord.SourceRectangle.Height - SliceSize, SliceSize, SliceSize),
                layerDepth,
                0,
                scale: Vector2.One,
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL
                );

            // Draw the interior piece
            spriteBatch.Draw(_textureCoord.Atlas,
                new Vector2(rectInterior.X, rectInterior.Y),
                new Vector2(SliceSize, SliceSize),
                new Rectangle(_textureCoord.SourceRectangle.X + SliceSize, _textureCoord.SourceRectangle.Y + SliceSize, _textureCoord.SourceRectangle.Width - SliceSize * 2, _textureCoord.SourceRectangle.Height - SliceSize * 2),
                layerDepth,
                0,
                scale: new Vector2(rectInterior.Width / (_textureCoord.SourceRectangle.Width - SliceSize * 2f), rectInterior.Height / (_textureCoord.SourceRectangle.Height - SliceSize * 2f)),
                ImageFlip.None,
                Color,
                Vector2.Zero,
                RenderServices.BLEND_NORMAL
                );

            // RenderServices.DrawRectangleOutline(spriteBatch, rectInterior, Color.Red);
        }
    }
}
