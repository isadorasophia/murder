// Based on https://github.com/lucas-miranda/Raccoon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Murder.Diagnostics;
using Murder.Services;
using System.Collections.Immutable;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Murder.Core.Graphics;
public class Batch2D : IDisposable
{
    public string Name;

    public const int StartBatchItemsCount = 128;
    public int TotalItemCount => _batchItems.Length;
    public int TotalTransparentItemCount => _transparencyBatchItems?.Length ?? 0;

    private VertexInfo[] _vertices = new VertexInfo[StartBatchItemsCount * 4];
    private int[] _indices = new int[StartBatchItemsCount * 4];

    private VertexInfo[] _vertexBuffer = new VertexInfo[StartBatchItemsCount * 4];
    private short[] _indexBuffer = new short[StartBatchItemsCount * 6];

    private SpriteBatchItem[] _batchItems = new SpriteBatchItem[StartBatchItemsCount];
    private SpriteBatchItem[]? _transparencyBatchItems;

    private int _nextItemIndex, _nextItemWithTransparencyIndex;

    public GraphicsDevice GraphicsDevice { get; set; }
    public readonly BatchMode BatchMode;
    private BlendState? _blendState;
    public readonly SamplerState SamplerState;
    public readonly DepthStencilState DepthStencilState;
    public readonly RasterizerState RasterizerState;

    public Batch2D(string name,
        GraphicsDevice graphicsDevice,
        Effect? effect,
        BatchMode batchMode,
        SamplerState samplerState,
        DepthStencilState? depthStencilState = null,
        RasterizerState? rasterizerState = null,
        bool autoHandleAlphaBlendedSprites = false)
        : this(
              name,
              graphicsDevice,
              false,
              effect,
              batchMode,
              samplerState,
              depthStencilState,
              rasterizerState,
              autoHandleAlphaBlendedSprites)
    { }

    public Batch2D(string name,
        GraphicsDevice graphicsDevice,
        bool followCamera,
        Effect? effect,
        BatchMode batchMode,
        SamplerState samplerState,
        DepthStencilState? depthStencilState = null,
        RasterizerState? rasterizerState = null,
        bool autoHandleAlphaBlendedSprites = false)
    {
        Name = name;

        GraphicsDevice = graphicsDevice;
        Effect = effect;
        BatchMode = batchMode;
        SamplerState = samplerState;
        DepthStencilState = depthStencilState ?? DepthStencilState.None;
        RasterizerState = rasterizerState ?? RasterizerState.CullNone;
        _followCamera = followCamera;

        AutoHandleAlphaBlendedSprites = autoHandleAlphaBlendedSprites;

        if (AutoHandleAlphaBlendedSprites)
        {
            _transparencyBatchItems = new SpriteBatchItem[_batchItems.Length / 2];
            InitializeTransparencyItemsBuffers();
        }

        Initialize();
    }

#if DEBUG

    /// <summary>
    /// Track number of draw calls.
    /// </summary>
    public static int TotalDrawCalls { get; private set; }

    /// <summary>
    /// Sprite count at current buffer.
    /// </summary>
    public static int SpriteCount { get; private set; }

#endif

    public bool IsBatching { get; private set; }
    public Effect? Effect { get; set; } = null;

    /// <summary>
    /// Auto handle any non-opaque (i.e. with some transparency; Opacity &lt; 1.0f) sprite rendering.
    /// By drawing first all opaque sprites, with depth write enabled, followed by non-opaque sprites, with only depth read enabled.
    /// </summary>
    public bool AutoHandleAlphaBlendedSprites { get; private set; }
    public bool AllowIBasicShaderEffectParameterClone { get; set; } = true;

    public Matrix Transform { get; private set; }
    public bool IsDisposed { get; private set; }

    private readonly bool _followCamera;

    public void Begin(Matrix cameraMatrix)
    {
        Transform = _followCamera ? cameraMatrix : Matrix.Identity;
        IsBatching = true;
    }

    /// <summary>
    /// Similar to <see cref="End"/> but without actually drawing the batch
    /// </summary>
    public void GiveUp()
    {
        _nextItemWithTransparencyIndex = 0;
        _nextItemIndex = 0;
    }

    public void End()
    {
        if (!IsBatching)
        {
            throw new System.InvalidOperationException("Begin() must be called before End().");
        }

        Flush();

        IsBatching = false;
    }

    /// <summary>
    /// Draw a sprite to this sprite batch.
    /// </summary>
    /// <param name="texture">Texture to be drawn.</param>
    /// <param name="position">Position in the spritebatch (before camera).</param>
    /// <param name="targetSize">The pixel size of the texture to be drawn, before scaling.</param>
    /// <param name="sourceRectangle">The area of the original image to draw.</param>
    /// <param name="rotation">Rotation of the image, from the origin point, in radians.</param>
    /// <param name="scale">The scale applied to the image from the origin point. 1 is the actual scale.</param>
    /// <param name="flip">If the image should be flipped horizontally, vertically, both or neither.</param>
    /// <param name="color">The color tint (or fill) to be applied to the image. The alpha is also applied to the image for transparency.</param>
    /// <param name="offset">The origin point for scaling and rotating. In pixels, before scaling.</param>
    /// <param name="blendStyle">The blend style to be used by the shader. Use the constants in <see cref="RenderServices"/>.</param>
    /// <param name="blendState">The blend state which will be used.</param>
    /// <param name="sort">A number from 0 to 1 that will be used to sort the images. 0 is behind, 1 is in front.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Draw(
        Texture2D texture,
        Vector2 position,
        Vector2 targetSize,
        Rectangle sourceRectangle,
        float sort,
        float rotation,
        Vector2 scale,
        ImageFlip flip,
        XnaColor color,
        Vector2 offset,
        Vector3 blendStyle,
        MurderBlendState blendState
        )
    {
#if DEBUG
        if (!IsBatching)
        {
            GameLogger.Warning("Begin() must be called before any Draw() operation.");
            return;
        }
#endif

        ref SpriteBatchItem batchItem = ref GetBatchItem(AutoHandleAlphaBlendedSprites && color.A < byte.MaxValue);
        batchItem.Set(texture, position, targetSize, sourceRectangle, rotation, scale, flip, color, offset, blendStyle, blendState, sort);

        if (BatchMode == BatchMode.Immediate)
        {
            Flush();
        }
    }
    public void SetTransform(Vector2 position)
    {
        Transform = Matrix.CreateTranslation(position.X, position.Y, 0f);
    }

    public void SetTransform(Vector2 position, Vector2 scale)
    {
        Transform = Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateTranslation(position.X, position.Y, 0f);
    }

    public void DrawPolygon(Texture2D texture, System.Numerics.Vector2[] vertices, DrawInfo drawInfo)
    {
        if (!IsBatching)
        {
            throw new InvalidOperationException("Begin() must be called before any Draw() operation.");
        }

        ref SpriteBatchItem batchItem = ref GetBatchItem(AutoHandleAlphaBlendedSprites && drawInfo.Color.A < byte.MaxValue);
        batchItem.SetPolygon(texture, vertices, drawInfo);

        if (BatchMode == BatchMode.Immediate)
        {
            Flush();
        }
    }

    public void DrawPolygon(Texture2D texture, ImmutableArray<System.Numerics.Vector2> vertices, DrawInfo drawInfo)
    {
        if (!IsBatching)
        {
            throw new InvalidOperationException("Begin() must be called before any Draw() operation.");
        }

        ref SpriteBatchItem batchItem = ref GetBatchItem(AutoHandleAlphaBlendedSprites && drawInfo.Color.A < byte.MaxValue);

        batchItem.SetPolygon(texture, vertices.AsSpan(), drawInfo);

        if (BatchMode == BatchMode.Immediate)
        {
            Flush();
        }
    }

    /// <summary>
    /// Immediately releases the unmanaged resources used by this object.
    /// </summary>
    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Send all stored batches to rendering, but doesn't end batching.
    /// If auto handle alpha blended sprites is active, be careful! Since it can includes alpha blended sprites too.
    /// </summary>
    /// <param name="includeAlphaBlendedSprites">True, if flush can include stored alpha blended sprites (possibly breaking rendering order, unless you know what are doing), otherwise False.</param>
    public void Flush(bool includeAlphaBlendedSprites = true)
    {
        if (!IsBatching)
        {
            return;
        }

        Render(ref _batchItems, _nextItemIndex, DepthStencilState);

        if (AutoHandleAlphaBlendedSprites && includeAlphaBlendedSprites)
        {
            GameLogger.Verify(_transparencyBatchItems is not null);

            Render(ref _transparencyBatchItems, _nextItemWithTransparencyIndex, DepthStencilState.DepthRead);

#if DEBUG

            SpriteCount += _nextItemWithTransparencyIndex;

#endif

            _nextItemWithTransparencyIndex = 0;
        }

#if DEBUG

        SpriteCount += _nextItemIndex;
#endif

        _nextItemIndex = 0;
    }

    private ref SpriteBatchItem GetBatchItem(bool needsTransparency)
    {
        if (needsTransparency)
        {
            GameLogger.Verify(_transparencyBatchItems is not null);

            if (_nextItemWithTransparencyIndex >= _transparencyBatchItems.Length)
            {
                SetTransparencyBuffersCapacity(_transparencyBatchItems.Length + _transparencyBatchItems.Length / 2);
            }

            ref SpriteBatchItem transparencyBatchItem = ref _transparencyBatchItems[_nextItemWithTransparencyIndex];
            _nextItemWithTransparencyIndex++;

            return ref transparencyBatchItem;
        }

        if (_nextItemIndex >= _batchItems.Length)
        {
            SetBuffersCapacity(_batchItems.Length * 2);
        }

        ref SpriteBatchItem batchItem = ref _batchItems[_nextItemIndex];
        _nextItemIndex++;

        return ref batchItem;
    }

    private void SetBuffersCapacity(int newBatchItemsCapacity)
    {
        if (_batchItems.Length >= newBatchItemsCapacity)
        {
            return;
        }

        int previousSize = _batchItems.Length;
        System.Array.Resize(ref _batchItems, newBatchItemsCapacity);
        System.Array.Resize(ref _vertexBuffer, newBatchItemsCapacity * 4);
        System.Array.Resize(ref _indexBuffer, newBatchItemsCapacity * 6);

        Initialize(previousSize);
    }

    private void SetTransparencyBuffersCapacity(int newBatchItemsCapacity)
    {
        GameLogger.Verify(_transparencyBatchItems is not null);

        if (_transparencyBatchItems.Length >= newBatchItemsCapacity)
        {
            return;
        }

        int previousTransparencyItemsBatchSize = _transparencyBatchItems.Length;
        System.Array.Resize(ref _transparencyBatchItems, newBatchItemsCapacity);

        InitializeTransparencyItemsBuffers(previousTransparencyItemsBatchSize);
    }

    private void Initialize(int startIndex = 0)
    {
        for (int i = startIndex; i < _batchItems.Length; i++)
        {
            _batchItems[i] = new SpriteBatchItem();

            _indexBuffer[i * 6] = (short)(i * 4 + 3);
            _indexBuffer[i * 6 + 1] = (short)(i * 4);
            _indexBuffer[i * 6 + 2] = (short)(i * 4 + 2);

            _indexBuffer[i * 6 + 3] = (short)(i * 4 + 2);
            _indexBuffer[i * 6 + 4] = (short)(i * 4);
            _indexBuffer[i * 6 + 5] = (short)(i * 4 + 1);
        }
    }

    private void InitializeTransparencyItemsBuffers(int startIndex = 0)
    {
        GameLogger.Verify(_transparencyBatchItems is not null);

        for (int i = startIndex; i < _transparencyBatchItems.Length; i++)
        {
            _transparencyBatchItems[i] = new SpriteBatchItem();
        }
    }

    private void Render(ref SpriteBatchItem[] batchItems, int itemsCount, DepthStencilState depthStencilState)
    {
        if (itemsCount == 0)
        {
            return;
        }

        // pre-process batches, some modes demands it
        switch (BatchMode)
        {
            case BatchMode.DepthSortAscending:
                System.Array.Sort(batchItems, 0, itemsCount, new BatchModeComparer.DepthAscending());
                break;

            case BatchMode.DepthSortDescending:
                System.Array.Sort(batchItems, 0, itemsCount, new BatchModeComparer.DepthDescending());
                break;

            //case BatchMode.DepthBuffer:
            //    System.Array.Sort(batchItems, 0, itemsCount, new BatchModeComparer.DepthBuffer());
            //    break;

            case BatchMode.DrawOrder:
            case BatchMode.Immediate:
                break;

            default:
                throw new System.NotImplementedException($"SpriteBatch doesn't implements BatchMode '{BatchMode}'.");
        }

        SpriteBatchItem batchItem = batchItems[0];
        Texture? texture = batchItem.Texture != null ? batchItem.Texture : null;
        MurderBlendState blendState = batchItem.BlendState;
        SetBlendState(blendState);

        var matrix = Transform;

        var size = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        matrix *= Matrix.CreateScale((1f / size.X) * 2, -(1f / size.Y) * 2, 1f); // scale to relative points
        matrix *= Matrix.CreateTranslation(-1f, 1f, 0); // translate to relative points

        void Resize<T>(ref T[] array)
        {
            Array.Resize(ref array, array.Length * 2);
        }

        int verticesIndex = 0;
        int indicesIndex = 0;

        Effect?.Parameters["MatrixTransform"]?.SetValue(matrix);
        
        for (int i = 0; i < itemsCount; i++)
        {
            batchItem = batchItems[i];

            // If the texture is different, we need to flush the current batch and start a new one
            if (batchItem.Texture != null && batchItem.Texture != texture)
            {
                DrawQuads(_vertices, verticesIndex, _indices, indicesIndex, texture, depthStencilState, matrix);
                texture = batchItem.Texture;

                verticesIndex = 0;
                indicesIndex = 0;
            }

            if (blendState != batchItem.BlendState)
            {
                DrawQuads(_vertices, verticesIndex, _indices, indicesIndex, texture, depthStencilState, matrix);
                blendState = batchItem.BlendState;
                SetBlendState(blendState);
                verticesIndex = 0;
                indicesIndex = 0;
            }

            int vertexOffset = verticesIndex;
            for (int v = 0; v < batchItem.VertexCount; v++)
            {
                // Copy the vertex data to the vertex buffer
                _vertices[verticesIndex++] = batchItem.VertexData[v];

                if (verticesIndex >= _vertices.Length)
                {
                    // Resize the vertex buffer if we've run out of space
                    Resize(ref _vertices);
                }
            }

            for (int v = 0; v < (batchItem.VertexCount - 2) * 3; v++)
            {
                // Copy the index data to the index buffer
                _indices[indicesIndex++] = batchItem.IndexData[v] + vertexOffset;

                if (indicesIndex >= _indices.Length)
                {
                    // Resize the index buffer if we've run out of space
                    Resize(ref _indices);
                }
            }
        }

        Effect?.Parameters["MatrixTransform"]?.SetValue(matrix);
        DrawQuads(_vertices, verticesIndex, _indices, indicesIndex, texture, depthStencilState, matrix);
    }

    private void SetBlendState(MurderBlendState blendState)
    {
        switch (blendState)
        {
            default:
            case MurderBlendState.AlphaBlend:
                _blendState = BlendState.AlphaBlend;
                break;
            case MurderBlendState.Additive:
                _blendState = BlendState.Additive;
                break;
        }
    }

    private void DrawQuads(
    VertexInfo[] vertices,
    int verticesLength,
    int[] indices,
    int indicesLength,
    Texture? texture,
    DepthStencilState depthStencilState,
    Matrix matrix)
    {
        GraphicsDevice.BlendState = _blendState;
        GraphicsDevice.SamplerStates[0] = SamplerState;

        if (GraphicsDevice.DepthStencilState != depthStencilState)
        {
            GraphicsDevice.DepthStencilState = depthStencilState;
        }

        if (GraphicsDevice.RasterizerState != RasterizerState)
        {
            GraphicsDevice.RasterizerState = RasterizerState;
        }

        if (Effect is not null)
        {
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.Textures[0] = texture;

                // This is where we finally draw the vertices too the screen
                GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    vertexData: vertices,
                    vertexOffset: 0,
                    numVertices: verticesLength,
                    indexData: indices,
                    indexOffset: 0,
                    primitiveCount: indicesLength / 3
                );
            }
        }

#if DEBUG
        TotalDrawCalls++;
#endif
    }
}