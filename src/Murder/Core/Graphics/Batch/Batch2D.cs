// Based on https://github.com/lucas-miranda/Raccoon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Murder.Diagnostics;
using Murder.Services;
using System.Collections.Immutable;
using System.Diagnostics;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Murder.Core.Graphics;
public class Batch2D
{
    public string Name;

    public const int StartBatchItemsCount = 128;
    public int TotalItemCount => _batchItems.Length;
    private int _lastQueue = 0;

    public int MaxTextureSwaps { get; private set; } = 0;
    private int _currentTextureSwitches = 0;
    public int ItemsQueued => _lastQueue;

    private VertexInfo[] _vertices = new VertexInfo[StartBatchItemsCount * 4];
    private int[] _indices = new int[StartBatchItemsCount * 4];

    private VertexInfo[] _vertexBuffer = new VertexInfo[StartBatchItemsCount * 4];
    private int[] _indexBuffer = new int[StartBatchItemsCount * 6];

    private SpriteBatchItem[] _batchItems = new SpriteBatchItem[StartBatchItemsCount];

    private int _nextItemIndex;

    private static readonly BatchModeComparer.DepthAscending _depthAscendingComparer = new();
    private static readonly BatchModeComparer.DepthDescending _depthDescendingComparer = new();

    public GraphicsDevice GraphicsDevice { get; set; }
    public readonly BatchMode BatchMode;
    private BlendState? _blendState;
    public readonly SamplerState SamplerState;
    public readonly DepthStencilState DepthStencilState;
    public readonly RasterizerState RasterizerState;

    private bool _matrixDirty = true;
    private Matrix _lastMatrix = Matrix.Identity;

    private readonly List<EffectPass> _cachedEffectPasses = new();
    private int _cachedEffectPassCount;

    public Batch2D(string name,
        GraphicsDevice graphicsDevice,
        Effect? effect,
        BatchMode batchMode,
        SamplerState samplerState,
        DepthStencilState? depthStencilState = null,
        RasterizerState? rasterizerState = null)
        : this(
              name,
              graphicsDevice,
              false,
              effect,
              batchMode,
              samplerState,
              depthStencilState,
              rasterizerState)
    { }

    public Batch2D(string name,
        GraphicsDevice graphicsDevice,
        bool followCamera,
        Effect? effect,
        BatchMode batchMode,
        SamplerState samplerState,
        DepthStencilState? depthStencilState = null,
        RasterizerState? rasterizerState = null)
    {
        Name = name;

        GraphicsDevice = graphicsDevice;
        Effect = effect;
        BatchMode = batchMode;
        SamplerState = samplerState;
        DepthStencilState = depthStencilState ?? DepthStencilState.None;
        RasterizerState = rasterizerState ?? RasterizerState.CullNone;
        _followCamera = followCamera;

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

    public Matrix Transform { get; private set; }

    private readonly bool _followCamera;

    public void Begin(Matrix cameraMatrix)
    {
        Transform = _followCamera ? cameraMatrix : Matrix.Identity;
        IsBatching = true;
        _matrixDirty = true;
    }

    /// <summary>
    /// Similar to <see cref="End"/> but without actually drawing the batch
    /// </summary>
    public void GiveUp()
    {
        _lastQueue = _nextItemIndex;
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

        ref SpriteBatchItem batchItem = ref GetBatchItem();
        batchItem.Set(texture, position, targetSize, sourceRectangle, rotation, scale, flip, color, offset, blendStyle, blendState, sort);

        if (BatchMode == BatchMode.Immediate)
        {
            Flush();
        }
    }
    public void SetTransform(Vector2 position)
    {
        Transform = Matrix.CreateTranslation(position.X, position.Y, 0f);
        _matrixDirty = true;
    }

    public void SetTransform(Vector2 position, Vector2 scale)
    {
        Transform = Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateTranslation(position.X, position.Y, 0f);
        _matrixDirty = true;
    }
    public void DrawPolygon(Texture2D texture, System.Numerics.Vector2 position, ReadOnlySpan<System.Numerics.Vector2> vertices, DrawInfo drawInfo)
    {
        if (!IsBatching)
        {
            throw new InvalidOperationException("Begin() must be called before any Draw() operation.");
        }

        ref SpriteBatchItem batchItem = ref GetBatchItem();

        batchItem.SetPolygon(texture, position, vertices, drawInfo);

        if (BatchMode == BatchMode.Immediate)
        {
            Flush();
        }
    }
    public void DrawPolygon(Texture2D texture, System.Numerics.Vector2 position, ImmutableArray<System.Numerics.Vector2> vertices, DrawInfo drawInfo)
    {
        if (!IsBatching)
        {
            throw new InvalidOperationException("Begin() must be called before any Draw() operation.");
        }

        ref SpriteBatchItem batchItem = ref GetBatchItem();

        batchItem.SetPolygon(texture, position, vertices.AsSpan(), drawInfo);

        if (BatchMode == BatchMode.Immediate)
        {
            Flush();
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

#if DEBUG
        SpriteCount += _nextItemIndex;
        _lastQueue = _nextItemIndex;
#endif

        _nextItemIndex = 0;
    }

    private ref SpriteBatchItem GetBatchItem()
    {
        if (_nextItemIndex >= _batchItems.Length)
        {
            // Ideally this is very rare, and in practice shouldn't happen more than a few times after loading a scene.
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


    private void Initialize(int startIndex = 0)
    {
        for (int i = startIndex; i < _batchItems.Length; i++)
        {
            _batchItems[i] = new SpriteBatchItem();

            _indexBuffer[i * 6] = (i * 4 + 3);
            _indexBuffer[i * 6 + 1] = (i * 4);
            _indexBuffer[i * 6 + 2] = (i * 4 + 2);

            _indexBuffer[i * 6 + 3] = (i * 4 + 2);
            _indexBuffer[i * 6 + 4] = (i * 4);
            _indexBuffer[i * 6 + 5] = (i * 4 + 1);
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
                System.Array.Sort(batchItems, 0, itemsCount, _depthAscendingComparer);
                break;

            case BatchMode.DepthSortDescending:
                System.Array.Sort(batchItems, 0, itemsCount, _depthDescendingComparer);
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

        _currentTextureSwitches = 0;
        SpriteBatchItem batchItem = batchItems[0];
        Texture? texture = batchItem.Texture != null ? batchItem.Texture : null;
        MurderBlendState blendState = batchItem.BlendState;
        SetBlendState(blendState);

        var matrix = Transform;

        var size = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        matrix *= Matrix.CreateScale((1f / size.X) * 2, -(1f / size.Y) * 2, 1f); // scale to relative points
        matrix *= Matrix.CreateTranslation(-1f, 1f, 0); // translate to relative points

        int verticesIndex = 0;
        int indicesIndex = 0;

        // Cache the matrix to avoid setting it multiple times
        if (_matrixDirty || matrix != _lastMatrix)
        {
            Effect?.Parameters["MatrixTransform"]?.SetValue(matrix);
            _lastMatrix = matrix;
            _matrixDirty = false;
        }

        // Cache effect passes to avoid allocating them multiple times
        if (Effect != null)
        {
            _cachedEffectPassCount = 0;
            var passes = Effect.CurrentTechnique.Passes;
            for (int p = 0; p < passes.Count; p++)
            {
                EffectPass pass = passes[p];
                if (_cachedEffectPasses.Count <= _cachedEffectPassCount)
                {
                    _cachedEffectPasses.Add(pass);
                }
                else
                {
                    _cachedEffectPasses[_cachedEffectPassCount] = pass;
                }
                _cachedEffectPassCount++;
            }
        }

        for (int i = 0; i < itemsCount; i++)
        {
            batchItem = batchItems[i];

#if DEBUG
            if (texture == null)
            {
                GameLogger.Error($"Batch2D '{Name}' has a null texture at index {i}. This is likely a bug.");
                Debugger.Break();
            }
#endif

            // If the texture is different, we need to flush the current batch and start a new one
            if (batchItem.Texture != null && batchItem.Texture != texture)
            {
                DrawQuads(_vertices, verticesIndex, _indices, indicesIndex, texture, depthStencilState, matrix);
                texture = batchItem.Texture;

                verticesIndex = 0;
                indicesIndex = 0;
                _currentTextureSwitches++;
            }

            if (blendState != batchItem.BlendState)
            {
                DrawQuads(_vertices, verticesIndex, _indices, indicesIndex, texture, depthStencilState, matrix);
                blendState = batchItem.BlendState;
                SetBlendState(blendState);
                verticesIndex = 0;
                indicesIndex = 0;
            }

            WriteBatchItemToBuffers(batchItem, ref verticesIndex, ref indicesIndex);
        }

        //Effect?.Parameters["MatrixTransform"]?.SetValue(matrix); // Redundant due to caching above
        DrawQuads(_vertices, verticesIndex, _indices, indicesIndex, texture, depthStencilState, matrix);

        MaxTextureSwaps = Math.Max(MaxTextureSwaps, _currentTextureSwitches);

        void WriteBatchItemToBuffers(SpriteBatchItem batchItem, ref int verticesIndex, ref int indicesIndex)
        {
            int vertexOffset = verticesIndex;

            // Pre-check and resize
            int requiredVertexCapacity = verticesIndex + batchItem.VertexCount;
            if (requiredVertexCapacity > _vertices.Length)
            {
                int newSize = Math.Max(_vertices.Length * 2, requiredVertexCapacity);
                Array.Resize(ref _vertices, newSize);
            }

            // Use Span to avoid extra allocations
            Span<VertexInfo> verticesSource = batchItem.VertexData.AsSpan(0, batchItem.VertexCount);
            Span<VertexInfo> verticesDestination = _vertices.AsSpan(verticesIndex, batchItem.VertexCount);

            verticesSource.CopyTo(verticesDestination);
            verticesIndex += batchItem.VertexCount;


            // Indices
            int indexCount = (batchItem.VertexCount - 2) * 3;
            int requiredIndexCapacity = indicesIndex + indexCount;
            if (requiredIndexCapacity > _indices.Length)
            {
                int newSize = Math.Max(_indices.Length * 2, requiredIndexCapacity);
                Array.Resize(ref _indices, _indices.Length * 2);
            }

            // Optimized index copy
            var sourceIndices = batchItem.IndexData.AsSpan(0, indexCount);
            var destIndices = _indices.AsSpan(indicesIndex, indexCount);
            for (int v = 0; v < indexCount; v++)
            {
                destIndices[v] = sourceIndices[v] + vertexOffset;
            }
            indicesIndex += indexCount;
        }

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
            int primitiveCount = indicesLength / 3;
            for (int i = 0; i < _cachedEffectPassCount; i++)
            {
                GraphicsDevice.Textures[0] = texture;
                _cachedEffectPasses[i].Apply();

                // This is where we finally draw the vertices too the screen
                GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    vertexData: vertices,
                    vertexOffset: 0,
                    numVertices: verticesLength,
                    indexData: indices,
                    indexOffset: 0,
                    primitiveCount: primitiveCount
                );
            }
        }

#if DEBUG
        TotalDrawCalls++;
#endif
    }
}