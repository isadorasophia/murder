using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics.CodeAnalysis;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Murder.Core.Graphics
{
    public enum TargetSpriteBatches
    {
        Gameplay,
        Floor,
        GameplayUi,
        Ui
    }

    public class RenderContext : IDisposable
    {
        public readonly Camera2D Camera;

        public readonly Batch2D FloorSpriteBatch;
        public readonly Batch2D GameplayBatch;
        public readonly Batch2D GameUiBatch;
        public readonly Batch2D UiBatch;
        public readonly Batch2D DebugFxSpriteBatch;
        public readonly Batch2D DebugSpriteBatch;

        protected RenderTarget2D? _floorBufferTarget;

        private RenderTarget2D? _mainTarget;
        private RenderTarget2D? _finalTarget;

        protected GraphicsDevice _graphicsDevice;

        public Point GameBufferSize;

        public RenderTarget2D? LastRenderTarget => _finalTarget;

        public readonly CacheDictionary<string, Texture2D> CachedTextTextures = new(32);

        public Batch2D GetSpriteBatch(TargetSpriteBatches targetSpriteBatch)
        {
            switch (targetSpriteBatch)
            {
                case TargetSpriteBatches.Gameplay:
                    return GameplayBatch;
                case TargetSpriteBatches.GameplayUi:
                    return GameUiBatch;
                case TargetSpriteBatches.Ui:
                    return UiBatch;
                case TargetSpriteBatches.Floor:
                    return FloorSpriteBatch;
                default:
                    throw new Exception("Unknown spritebatch");
            }
        }

        public static readonly int CAMERA_BLEED = 4;

        public bool RenderToScreen = true;

        public Point ScreenSize;
        public Color BackColor => Game.Data.GameProfile.BackColor;

        public enum RenderTargets
        {
            MainBufferTarget,
            FinalTarget,
        }

        public virtual Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
        {
            Texture2D? target = inspectingRenderTarget switch
            {
                RenderTargets.MainBufferTarget => _mainTarget,
                RenderTargets.FinalTarget => _finalTarget,
                _ => default
            };

            return target ?? throw new ArgumentException($"Unable to find a render target for {inspectingRenderTarget}.");
        }

        public RenderContext(GraphicsDevice graphicsDevice, Camera2D camera)
        {
            Camera = camera;

            _graphicsDevice = graphicsDevice;

            DebugFxSpriteBatch =    new(graphicsDevice);
            DebugSpriteBatch =      new(graphicsDevice);
            GameplayBatch =         new(graphicsDevice);
            FloorSpriteBatch =      new(graphicsDevice);
            UiBatch =               new(graphicsDevice);
            GameUiBatch =           new(graphicsDevice);
        }

        /// <summary>
        /// Refresh the window size with <paramref name="size"/> with width and height information,
        /// respectively.
        /// </summary>
        /// <returns>
        /// Whether the window actually required a refresh.
        /// </returns>
        public bool RefreshWindow(Point size, int scale) =>
            RefreshWindow(_graphicsDevice, size, scale);

        internal bool RefreshWindow(GraphicsDevice graphicsDevice, Point size, int scale)
        {
            if (_graphicsDevice == graphicsDevice && 
                size.X == Camera.Width && 
                size.Y == Camera.Height && 
                ScreenSize != Point.Zero)
            {
                return false;
            }

            _graphicsDevice = graphicsDevice;

            Camera.UpdateSize(size.X, size.Y);
            UpdateBufferTarget(scale, Game.Instance.Downsample);

            return true;
        }

        public void Begin()
        {
             // no one should interfere with camera settings at this point.
            Camera.Lock();

            FloorSpriteBatch.Begin(
                Game.Data.Shader2D,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            GameplayBatch.Begin(
                Game.Data.Shader2D,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            DebugFxSpriteBatch.Begin(
                Game.Data.Shader2D,
                blendState: BlendState.NonPremultiplied,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );
            
            DebugSpriteBatch.Begin(
                Game.Data.Shader2D,
                blendState: BlendState.NonPremultiplied,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            GameUiBatch.Begin(
                effect: Game.Data.Shader2D,
                batchMode: BatchMode.DrawOrder,
                sampler: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transform: Camera.WorldViewProjection
            );

            UiBatch.Begin(
                Game.Data.Shader2D,
                batchMode: BatchMode.DepthSortDescending,
                depthStencil: DepthStencilState.None,
                sampler: SamplerState.AnisotropicWrap,
                blendState: BlendState.AlphaBlend,
                transform: Matrix.Identity
            );
        }

        public void End()
        {
            GameLogger.Verify(
                _mainTarget is not null &&
                _finalTarget is not null,
                "Did not initialize buffer targets before calling RenderContext.End()?");
            
            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.Shader2D.SetTechnique("Alpha");


            // =======================================================>
            _graphicsDevice.SetRenderTarget(_mainTarget);
            _graphicsDevice.Clear(BackColor);
            // =======================================================>

            // Draw the first round of sprite batches
            FloorSpriteBatch.End();     // <=== Floor batch
            GameplayBatch.End();        // <=== Gameplay batch

            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.Shader2D.SetTechnique("DiagonalLines");
            Game.Data.Shader2D.SetParameter("inputTime", Game.Now);
            DebugFxSpriteBatch.End();

            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.Shader2D.SetTechnique("Alpha");

            DebugSpriteBatch.End();
            GameUiBatch.End();
            
            _graphicsDevice.SetRenderTarget(_finalTarget);

            var scale = _finalTarget.Bounds.Size.ToVector2() / _mainTarget.Bounds.Size.ToVector2();
            scale = Vector2.One * 6;
            var cameraAdjust = - new Vector2(
                Camera.Position.X - Camera.Position.Point.X,
                Camera.Position.Y - Camera.Position.Point.Y) * 
                scale;

            RenderServices.DrawTextureQuad(_mainTarget,
                _mainTarget.Bounds,
                new Rectangle(cameraAdjust , _finalTarget.Bounds.Size.ToVector2()),
                Matrix.Identity,
                Color.White, Game.Data.SimpleShader, BlendState.Opaque, false);

            // Render the static UI
            _graphicsDevice.SetRenderTarget(_finalTarget);
            UiBatch.End();

            //var (sourceRect, destRect) = PostProcessGameplayBatch(_preBufferTarget, _gameBufferTarget);

            // =======================================================>
            // =======================================================>


            if (RenderToScreen)
            {
                Game.Data.SimpleShader.SetTechnique("Saturation");
                Game.Data.SimpleShader.SetParameter("Saturation", 2f);

                DrawFinalTarget(_finalTarget);
            }

            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.SimpleShader.SetParameter("Saturation", 1f);
            
            _graphicsDevice.SetRenderTarget(null);

            if (RenderToScreen)
            {
                RenderServices.DrawTextureQuad(_finalTarget,
                    _finalTarget.Bounds, _graphicsDevice.Viewport.Bounds,
                    Matrix.Identity, Color.White, BlendState.Opaque);
            }
            
            Camera.Unlock();
        }

        protected virtual (Rectangle SourceRect, Rectangle DestinationRect) PostProcessGameplayBatch(
            RenderTarget2D preBufferTarget, RenderTarget2D gameBufferTarget)
        {
            // Calculate stuff
            Vector2 scale = new((float)Game.GraphicsDevice.Viewport.Width / Camera.Width,
                (float)Game.GraphicsDevice.Viewport.Height / Camera.Height);

            // Do the camara snapping and draw the game buffer to the screen
            Vector2 pixelOffset = new(Calculator.RoundedDecimals(Camera.Position.X), Calculator.RoundedDecimals(Camera.Position.Y));
            Point pixelSnap = new(Calculator.RoundToInt(pixelOffset.X), Calculator.RoundToInt(pixelOffset.Y));

            Rectangle sourceRect = new Rectangle(
                -pixelSnap.X + CAMERA_BLEED, -pixelSnap.Y + CAMERA_BLEED,
                Camera.Width - Calculator.RoundToInt(pixelOffset.X) + CAMERA_BLEED,
                Camera.Height - Calculator.RoundToInt(pixelOffset.Y) + CAMERA_BLEED);

            Rectangle destinationRect = new Rectangle(
                Calculator.RoundToInt(-pixelOffset.X * scale.X),
                Calculator.RoundToInt(-pixelOffset.Y * scale.Y),
                sourceRect.Width * scale.X,
                sourceRect.Height * scale.Y);

            return (sourceRect, destinationRect);
        }

        protected virtual void DrawFinalTarget(RenderTarget2D target) { }

        protected virtual void UpdateBufferTargetImpl() { }

        [MemberNotNull(
            nameof(_mainTarget),
            nameof(_finalTarget))]
        public void UpdateBufferTarget(int scale, float downsample)
        {
            ScreenSize = new Point(Camera.Width, Camera.Height) * scale * downsample;

            _mainTarget?.Dispose();
            _mainTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.DiscardContents
                );
            _graphicsDevice.SetRenderTarget(_mainTarget);
            _graphicsDevice.Clear(BackColor);

            _finalTarget?.Dispose();
            _finalTarget = new RenderTarget2D(
                _graphicsDevice,
                ScreenSize.X,
                ScreenSize.Y,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.DiscardContents
                );
            _graphicsDevice.SetRenderTarget(_finalTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _graphicsDevice.SetRenderTarget(null);

            GameBufferSize = new Point(Camera.Width + CAMERA_BLEED * 2, Camera.Height + CAMERA_BLEED * 2);
        }

        /// <summary>
        /// Unload the render context.
        /// Called when the render context is no longer being actively displayed.
        /// </summary>
        public void Unload()
        {
            Dispose();

            Camera.Reset();

            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.SimpleShader.SetParameter("Saturation", 1f);

            UnloadImpl();

            _graphicsDevice.SetRenderTarget(null);

            Game.GraphicsDevice.Reset();
        }

        protected virtual void UnloadImpl() { }

        public virtual void Dispose()
        {
            FloorSpriteBatch?.Dispose();
            GameplayBatch?.Dispose();
            GameUiBatch?.Dispose();
            UiBatch?.Dispose();
            DebugFxSpriteBatch?.Dispose();
            DebugSpriteBatch?.Dispose();

            _floorBufferTarget?.Dispose();

            CachedTextTextures.Dispose();
        }
    }
}
