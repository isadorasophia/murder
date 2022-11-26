// Based on https://github.com/lucas-miranda/Raccoon

using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Murder.Core.Graphics
{
    public class SpriteBatchItem : IBatchItem
    {
        #region Public Properties
        
        public Texture2D? Texture { get; private set; }
        public VertexInfo[] VertexData { get; private set; } = new VertexInfo[4];
        public int[] IndexData { get; private set; } = new int[6] { 3, 0, 2, 2, 0, 1 };

        #endregion Public Properties

        #region Public Methods

        public void Set(Texture2D texture, Vector2[] vertices, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 origin, Vector3 colorBlend,float layerDepth = 1f)
        {
            Texture = texture;

            if (!sourceRectangle.HasValue)
            {
                sourceRectangle = new Rectangle(0,0, texture.Width, texture.Height);
            }

            Vector2 topLeft = (-origin + vertices[0]) * scale,
                    topRight = (-origin + vertices[1]) * scale,
                    bottomRight = (-origin + vertices[2]) * scale,
                    bottomLeft = (-origin + vertices[3]) * scale;

            if (rotation != 0)
            {
                float cos = MathF.Cos(rotation),
                      sin = MathF.Sin(rotation);

                topLeft = new Vector2(topLeft.X * cos - topLeft.Y * sin, topLeft.X * sin + topLeft.Y * cos);
                topRight = new Vector2(topRight.X * cos - topRight.Y * sin, topRight.X * sin + topRight.Y * cos);
                bottomRight = new Vector2(bottomRight.X * cos - bottomRight.Y * sin, bottomRight.X * sin + bottomRight.Y * cos);
                bottomLeft = new Vector2(bottomLeft.X * cos - bottomLeft.Y * sin, bottomLeft.X * sin + bottomLeft.Y * cos);
            }

            VertexData[0] = new VertexInfo(
                new Vector3(position + topLeft, layerDepth),
                color,
                new Microsoft.Xna.Framework.Vector2(sourceRectangle.Value.Left / texture.Width, sourceRectangle.Value.Top / texture.Height),
                colorBlend
            );

            VertexData[1] = new VertexInfo(
                new Vector3(position + topRight, layerDepth),
                color,
                new Microsoft.Xna.Framework.Vector2(sourceRectangle.Value.Right / texture.Width, sourceRectangle.Value.Top / texture.Height),
                colorBlend
            );

            VertexData[2] = new VertexInfo(
                new Vector3(position + bottomRight, layerDepth),
                color,
                new Microsoft.Xna.Framework.Vector2(sourceRectangle.Value.Right / texture.Width, sourceRectangle.Value.Bottom / texture.Height),
                colorBlend
            );

            VertexData[3] = new VertexInfo(
                new Vector3(position + bottomLeft, layerDepth),
                color,
                new Microsoft.Xna.Framework.Vector2(sourceRectangle.Value.Left / texture.Width, sourceRectangle.Value.Bottom / texture.Height),
                colorBlend
            );

            if ((flip & ImageFlip.Horizontal) != ImageFlip.None)
            {
                Microsoft.Xna.Framework.Vector2 texCoord = VertexData[1].TextureCoordinate;
                VertexData[1].TextureCoordinate = VertexData[0].TextureCoordinate;
                VertexData[0].TextureCoordinate = texCoord;

                texCoord = VertexData[2].TextureCoordinate;
                VertexData[2].TextureCoordinate = VertexData[3].TextureCoordinate;
                VertexData[3].TextureCoordinate = texCoord;
            }

            if ((flip & ImageFlip.Vertical) != ImageFlip.None)
            {
                Microsoft.Xna.Framework.Vector2 texCoord = VertexData[2].TextureCoordinate;
                VertexData[2].TextureCoordinate = VertexData[1].TextureCoordinate;
                VertexData[1].TextureCoordinate = texCoord;

                texCoord = VertexData[3].TextureCoordinate;
                VertexData[3].TextureCoordinate = VertexData[0].TextureCoordinate;
                VertexData[0].TextureCoordinate = texCoord;
            }
        }

        public void Set(Texture2D texture, Vector2 position, Vector2 destinationSize, Rectangle? sourceRectangle, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 origin, Vector3 colorBlend, float layerDepth = 1f)
        {
            Texture = texture;

            if (!sourceRectangle.HasValue)
            {
                sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            }

            Vector2 topLeft = -origin * scale,
                    topRight = (-origin + new Vector2(destinationSize.Width, 0f)) * scale,
                    bottomRight = (-origin + destinationSize) * scale,
                    bottomLeft = (-origin + new Vector2(0f, destinationSize.Height)) * scale;

            if (rotation != 0)
            {
                float cos = MathF.Cos(rotation),
                      sin = MathF.Sin(rotation);

                topLeft = new Vector2(topLeft.X * cos - topLeft.Y * sin, topLeft.X * sin + topLeft.Y * cos);
                topRight = new Vector2(topRight.X * cos - topRight.Y * sin, topRight.X * sin + topRight.Y * cos);
                bottomRight = new Vector2(bottomRight.X * cos - bottomRight.Y * sin, bottomRight.X * sin + bottomRight.Y * cos);
                bottomLeft = new Vector2(bottomLeft.X * cos - bottomLeft.Y * sin, bottomLeft.X * sin + bottomLeft.Y * cos);
            }

            VertexData[0] = new VertexInfo(
                new Vector3(position + topLeft, layerDepth),
                color,
                new Microsoft.Xna.Framework.Vector2((float)sourceRectangle.Value.Left / texture.Width, (float)sourceRectangle.Value.Top / texture.Height),
                colorBlend
            );

            VertexData[1] = new VertexInfo(
                new Vector3(position + topRight, layerDepth),
                color,
                new Microsoft.Xna.Framework.Vector2((float)sourceRectangle.Value.Right / texture.Width, (float)sourceRectangle.Value.Top / texture.Height),
                colorBlend
            );

            VertexData[2] = new VertexInfo(
                new Vector3(position + bottomRight, layerDepth),
                color,
                new Microsoft.Xna.Framework.Vector2((float)sourceRectangle.Value.Right / texture.Width, (float)sourceRectangle.Value.Bottom / texture.Height),
                colorBlend
            );

            VertexData[3] = new VertexInfo(
                new Vector3(position + bottomLeft, layerDepth),
                color,
                new Microsoft.Xna.Framework.Vector2((float)sourceRectangle.Value.Left / texture.Width, (float)sourceRectangle.Value.Bottom / texture.Height),
                colorBlend
            );

            if ((flip & ImageFlip.Horizontal) != ImageFlip.None)
            {
                Microsoft.Xna.Framework.Vector2 texCoord = VertexData[1].TextureCoordinate;
                VertexData[1].TextureCoordinate = VertexData[0].TextureCoordinate;
                VertexData[0].TextureCoordinate = texCoord;

                texCoord = VertexData[2].TextureCoordinate;
                VertexData[2].TextureCoordinate = VertexData[3].TextureCoordinate;
                VertexData[3].TextureCoordinate = texCoord;
            }

            if ((flip & ImageFlip.Vertical) != ImageFlip.None)
            {
                Microsoft.Xna.Framework.Vector2 texCoord = VertexData[2].TextureCoordinate;
                VertexData[2].TextureCoordinate = VertexData[1].TextureCoordinate;
                VertexData[1].TextureCoordinate = texCoord;

                texCoord = VertexData[3].TextureCoordinate;
                VertexData[3].TextureCoordinate = VertexData[0].TextureCoordinate;
                VertexData[0].TextureCoordinate = texCoord;
            }
        }

        //public void Set(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 origin, Vector3 colorBlend, float layerDepth = 1f)
        //{
        //    if (!sourceRectangle.HasValue)
        //    {
        //        sourceRectangle = new Rectangle(0,0, texture.Width, texture.Height);
        //    }

        //    Set(texture, position, sourceRectangle, rotation, scale, flip, color, origin, colorBlend, layerDepth);
        //}

        //public void Set(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 origin, Vector2 scroll, float layerDepth = 1f)
        //{
        //    Set(texture, position, sourceRectangle, rotation, scale, flip, color, origin, scroll, layerDepth);
        //}

        public void Clear()
        {
            Texture = null;
            //VertexData = null;
        }

        void IBatchItem.Clear()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}