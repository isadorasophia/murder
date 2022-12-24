// Based on https://github.com/lucas-miranda/Raccoon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Murder.Diagnostics;
using Murder.Services;
using System.Diagnostics.CodeAnalysis;

using XnaColor = Microsoft.Xna.Framework.Color;

namespace Murder.Core.Graphics
{
    public class Batch2D : IDisposable
    {
        #region Public Members

        public const int StartBatchItemsCount = 100;

        #endregion Public Members

        #region Private Members

        private VertexInfo[] _vertexBuffer = new VertexInfo[StartBatchItemsCount * 4];
        private short[] _indexBuffer = new short[StartBatchItemsCount * 6];

        private SpriteBatchItem[] _batchItems = new SpriteBatchItem[StartBatchItemsCount];
        private SpriteBatchItem[]? _transparencyBatchItems;

        private int _nextItemIndex, _nextItemWithTransparencyIndex;

        #endregion Private Members

        #region Constructors

        public Batch2D(GraphicsDevice graphicsDevice, bool autoHandleAlphaBlendedSprites = false)
        {
            GraphicsDevice = graphicsDevice;
            Effect = Game.Data.ShaderSprite;

            AutoHandleAlphaBlendedSprites = autoHandleAlphaBlendedSprites;

            if (AutoHandleAlphaBlendedSprites)
            {
                _transparencyBatchItems = new SpriteBatchItem[_batchItems.Length / 2];
                InitializeTransparencyItemsBuffers();
            }

            Initialize();
        }

        #endregion Constructors

        #region Public Properties

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

        public GraphicsDevice GraphicsDevice { get; set; }
        public bool IsBatching { get; private set; }
        public Effect Effect { get; set; }

        /// <summary>
        /// Auto handle any non-opaque (i.e. with some transparency; Opacity &lt; 1.0f) sprite rendering.
        /// By drawing first all opaque sprites, with depth write enabled, followed by non-opaque sprites, with only depth read enabled.
        /// </summary>
        public bool AutoHandleAlphaBlendedSprites { get; private set; }
        public bool AllowIBasicShaderEffectParameterClone { get; set; } = true;

        /** Initialized in Begin() **/
        public BatchMode BatchMode { get; private set; }
        public BlendState BlendState { get; private set; } = null!;
        public SamplerState SamplerState { get; private set; } = null!;
        public DepthStencilState DepthStencilState { get; private set; } = null!;
        public RasterizerState RasterizerState { get; private set; } = null!;
        public Matrix Transform { get; private set; }

        public bool IsDisposed { get; private set; }

        #endregion Public Properties

        #region Public Methods

        [MemberNotNull(nameof(BatchMode), nameof(BlendState), nameof(SamplerState), nameof(DepthStencilState), nameof(RasterizerState), nameof(Transform), nameof(Effect))]
        public void Begin(Effect? effect = null, BatchMode batchMode = BatchMode.DrawOrder, BlendState? blendState = null, SamplerState? sampler = null, DepthStencilState? depthStencil = null, RasterizerState? rasterizer = null, Matrix? transform = null)
        {
            BatchMode = batchMode;
            BlendState = blendState ?? BlendState.AlphaBlend;
            SamplerState = sampler ?? SamplerState.PointClamp;
            DepthStencilState = depthStencil ?? DepthStencilState.None;
            RasterizerState = rasterizer ?? RasterizerState.CullNone;
            Transform = transform ?? Matrix.Identity;
            Effect = effect ?? new BasicEffect(GraphicsDevice);

            IsBatching = true;
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
        /// <param name="origin">The origin point for scaling and rotating.</param>
        /// <param name="blendStyle">The blend style to be used by the shader. Use the constants in <see cref="RenderServices"/>.</param>
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
        Vector2 origin,
        Vector3 blendStyle)
        {
            if (!IsBatching)
            {
                throw new InvalidOperationException("Begin() must be called before any Draw() operation.");
            }

            ref SpriteBatchItem batchItem = ref GetBatchItem(AutoHandleAlphaBlendedSprites && color.A < byte.MaxValue);
            batchItem.Set(texture, position, targetSize, sourceRectangle, rotation, scale, flip, color, origin, blendStyle, sort);

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
                //GraphicsDevice = null!;
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

        #endregion Public Methods

        #region Private Methods

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
                SetBuffersCapacity(_batchItems.Length + _batchItems.Length / 2);
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
            Texture? texture = batchItem.Texture!=null ? batchItem.Texture : null;
            Effect effect = Effect;

            int startIndex = 0,
                endIndex = 0;


            var matrix = Transform;

            var size = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            matrix *= Matrix.CreateScale((1f / size.X) * 2, -(1f / size.Y) * 2, 1f); // scale to relative points
            matrix *= Matrix.CreateTranslation(-1f, 1f, 0); // translate to relative points

            for (int i = 0; i < itemsCount; i++)
            {
                batchItem = batchItems[i];

                if (batchItem.Texture != null && batchItem.Texture != texture)
                {
                    DrawQuads(startIndex, endIndex - 1, texture, depthStencilState, matrix);
                    texture = batchItem.Texture;
                    startIndex = endIndex;
                }

                batchItem.VertexData.CopyTo(_vertexBuffer, endIndex * 4);
                batchItem.Clear();
                endIndex++;
            }

            DrawQuads(startIndex, endIndex - 1, texture, depthStencilState, matrix);
        }

        private void DrawQuads(int startBatchIndex, int endBatchIndex, Texture? texture, DepthStencilState depthStencilState, Matrix matrix)
        {
            int batchCount = endBatchIndex - startBatchIndex + 1;

            // prepare device
            GraphicsDevice.BlendState = BlendState;
            GraphicsDevice.SamplerStates[0] = SamplerState;

            GraphicsDevice.DepthStencilState = depthStencilState;
            GraphicsDevice.RasterizerState = RasterizerState;


            if (Effect.Parameters["MatrixTransform"] != null)
                Effect.Parameters["MatrixTransform"].SetValue(matrix);

            if (texture != null)
            {
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.Textures[0] = texture;

                    DrawUserIndexedPrimitives(startBatchIndex, batchCount);
                }
            }
            else // Saving that 1 check for performance
            {
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    DrawUserIndexedPrimitives(startBatchIndex, batchCount);
                }
            }

#if DEBUG
            TotalDrawCalls++;
#endif
        }

        private void DrawUserIndexedPrimitives(int startBatchIndex, int batchCount)
        {
            GraphicsDevice.DrawUserIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                _vertexBuffer,
                                startBatchIndex * 4,
                                batchCount * 4,
                                _indexBuffer,
                                0,
                                batchCount * 2
                            );
        }

        #endregion Private Methods

    }
}