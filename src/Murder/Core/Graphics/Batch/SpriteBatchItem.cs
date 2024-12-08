// Based on https://github.com/lucas-miranda/Raccoon

using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Utilities;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Murder.Core.Graphics;

public class SpriteBatchItem
{
    public Texture2D? Texture;
    public VertexInfo[] VertexData = new VertexInfo[4];
    public int[] IndexData = new int[6];
    public int VertexCount = 4;
    public SpriteBatchItem() { }

    private readonly int[] _defaultIndexData = new int[6] { 3, 0, 2, 2, 0, 1 };


    /// <summary>
    /// Sets a Texture to be drawn to the batch
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="position"></param>
    /// <param name="destinationSize"></param>
    /// <param name="sourceRectangle"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    /// <param name="flip"></param>
    /// <param name="color"></param>
    /// <param name="origin">Origin coordinates 0 is top left, 1 is bottom right</param>
    /// <param name="colorBlend"></param>
    /// <param name="layerDepth"></param>
    public void Set(Texture2D texture, Vector2 position, Vector2 destinationSize, Rectangle? sourceRectangle, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 origin, Vector3 colorBlend, float layerDepth = 1f)
    {
        Texture = texture;
        VertexCount = 4;
        IndexData[0] = 3;
        IndexData[1] = 0;
        IndexData[2] = 2;
        IndexData[3] = 2;
        IndexData[4] = 0;
        IndexData[5] = 1;

        if (!sourceRectangle.HasValue)
        {
            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        Vector2 topLeft = -origin * scale,
                topRight = (-origin + new Vector2(destinationSize.X, 0f)) * scale,
                bottomRight = (-origin + destinationSize) * scale,
                bottomLeft = (-origin + new Vector2(0f, destinationSize.Y)) * scale;

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
    public void SetPolygon(Texture2D texture, ReadOnlySpan<System.Numerics.Vector2> vertices, DrawInfo drawInfo)
    {
        Texture = texture;
        VertexCount = vertices.Length;
        int triangleCount = VertexCount - 2;

        // Make sure we have space
        if (VertexData.Length < VertexCount)
            VertexData = new VertexInfo[VertexCount];

        if (IndexData.Length < triangleCount * 3)
            IndexData = new int[triangleCount * 3];

        // Calculate the transformed origin
        System.Numerics.Vector2 origin = new(drawInfo.Origin.X * drawInfo.Scale.X, drawInfo.Origin.Y * drawInfo.Scale.Y);

        // Set vertex data
        for (int i = 0; i < VertexCount; i++)
        {
            // Apply scale and subtract the origin
            Vector2 transformedVertex = ((vertices[i] * drawInfo.Scale) - origin).ToXnaVector2();

            // Apply rotation if necessary
            if (drawInfo.Rotation != 0)
            {
                float cos = MathF.Cos(drawInfo.Rotation);
                float sin = MathF.Sin(drawInfo.Rotation);

                transformedVertex = new Vector2(
                    transformedVertex.X * cos - transformedVertex.Y * sin,
                    transformedVertex.X * sin + transformedVertex.Y * cos
                );
            }

            // Apply offset and flipping
            transformedVertex += drawInfo.Offset.ToXnaVector2();
            if (drawInfo.ImageFlip.HasFlag(ImageFlip.Horizontal))
            {
                transformedVertex.X = -transformedVertex.X;
            }

            if (drawInfo.ImageFlip.HasFlag(ImageFlip.Vertical))
            {
                transformedVertex.Y = -transformedVertex.Y;
            }

            VertexData[i] = new VertexInfo(
                new Vector3(transformedVertex, drawInfo.Sort),
                drawInfo.Color,
                new Microsoft.Xna.Framework.Vector2(0, 0), // Texture coordinates can be updated if necessary
                drawInfo.GetBlendMode()
            );
        }

        // Set index data
        for (int i = 0; i < triangleCount; i++)
        {
            IndexData[i * 3] = 0;
            IndexData[i * 3 + 1] = i + 1;
            IndexData[i * 3 + 2] = i + 2;
        }
    }


}