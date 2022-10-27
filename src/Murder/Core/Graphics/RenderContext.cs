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
        public readonly Batch2D SpriteBatch;
        public readonly Batch2D GameUiBatch;
        public readonly Batch2D UiBatch;
        public readonly Batch2D DebugSpriteBatch;

        protected RenderTarget2D? _floorBufferTarget;

        private RenderTarget2D? _uiGameplayBufferTarget;
        private RenderTarget2D? _uiBufferTarget;
        private RenderTarget2D? _gameBufferTarget;
        private RenderTarget2D? _preBufferTarget;
        private RenderTarget2D? _finalTarget;

        protected GraphicsDevice _graphicsDevice;

        public Point GameBufferSize;

        public RenderTarget2D? LastRenderTarget => _finalTarget;

        public RenderTarget2D UiRenderTarget => _uiBufferTarget ?? throw new InvalidOperationException("Tried to acquire UiRenderTarget uninitialized.");

        public readonly CacheDictionary<string, Texture2D> CachedTextTextures = new(32);

        public Batch2D GetSpriteBatch(TargetSpriteBatches targetSpriteBatch)
        {
            switch (targetSpriteBatch)
            {
                case TargetSpriteBatches.Gameplay:
                    return SpriteBatch;
                case TargetSpriteBatches.GameplayUi:
                    return GameUiBatch;
                case TargetSpriteBatches.Ui:
                    return UiBatch;
                default:
                    throw new Exception("Unknown spritebatch");
            }
        }

        public static readonly int CAMERA_BLEED = 4;

        public bool RenderToScreen = true;

        public Point ScreenSize;

        public enum RenderTargets
        {
            FloorBufferTarget,
            GameBufferTarget,
            LightBufferTarget,
            FloorLightBufferTarget,
            UiBufferTarget,
            UiGameplayBufferTarget,
            PreBufferTarget,
            Bloom,
            FinalTarget,
            DitherTexture,
        }

        public virtual Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
        {
            Texture2D? target = inspectingRenderTarget switch
            {
                RenderTargets.FloorBufferTarget => _floorBufferTarget,
                RenderTargets.GameBufferTarget => _gameBufferTarget,
                RenderTargets.PreBufferTarget => _preBufferTarget,
                RenderTargets.FinalTarget => _finalTarget,
                RenderTargets.DitherTexture => Game.Data.DitherTexture,
                RenderTargets.UiBufferTarget => _uiBufferTarget,
                RenderTargets.UiGameplayBufferTarget => _uiGameplayBufferTarget,
                _ => default
            };

            return target ?? throw new ArgumentException($"Unable to find a render target for {inspectingRenderTarget}.");
        }

        public RenderContext(GraphicsDevice graphicsDevice, Camera2D camera)
        {
            Camera = camera;

            _graphicsDevice = graphicsDevice;

            DebugSpriteBatch =          new(graphicsDevice);
            SpriteBatch =               new(graphicsDevice);
            FloorSpriteBatch =          new(graphicsDevice);

            UiBatch =                   new(graphicsDevice);
            GameUiBatch =               new(graphicsDevice);
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

            SpriteBatch.Begin(
                Game.Data.Shader2D,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.AlphaBlend,
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
                blendState: BlendState.AlphaBlend
            );
        }

        public void End()
        {
            GameLogger.Verify(
                _gameBufferTarget is not null &&
                _preBufferTarget is not null &&
                _finalTarget is not null &&
                _uiGameplayBufferTarget is not null &&
                _uiBufferTarget is not null,
                "Did not initialize buffer targets before calling RenderContext.End()?");
            
            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.Shader2D.SetTechnique("Alpha");


            // =======================================================>
            _graphicsDevice.SetRenderTarget(_floorBufferTarget);
            _graphicsDevice.Clear(Color.Black);

            // =======================================================>
            // Draw the floor tiles
            FloorSpriteBatch.End();

            // Then set our graphics device to be on the buffer.
            // =======================================================>
            _graphicsDevice.SetRenderTarget(_gameBufferTarget);
            _graphicsDevice.Clear(Color.Green);
            // =======================================================>

            DrawGameplayBatch();

            // Draw the main gameplay sprite batch!
            SpriteBatch.End();

            // Render the gameplay UI (This UI is influenced by the camera)
            _graphicsDevice.SetRenderTarget(_uiGameplayBufferTarget);
            _graphicsDevice.Clear(Color.Transparent);

            DebugSpriteBatch.End();
            GameUiBatch.End();

            // Render the static UI
            _graphicsDevice.SetRenderTarget(_uiBufferTarget);
            _graphicsDevice.Clear(Color.Transparent);
            UiBatch.End();

            // =======================================================>
            // >==== Draw the game buffer to the pre-final buffer ====>
            // =======================================================>
            _graphicsDevice.SetRenderTarget(_preBufferTarget);
            _graphicsDevice.Clear(Color.Transparent);
            // =======================================================>

            var (sourceRect, destRect) = PostProcessGameplayBatch(_preBufferTarget, _gameBufferTarget);

            // Place the gameplay into the final target
            // This is done because extracting the bloom clears the final target for some reason
            // =======================================================>
            _graphicsDevice.SetRenderTarget(_finalTarget);
            // =======================================================>

            RenderServices.DrawTextureQuad(_preBufferTarget,
                _preBufferTarget.Bounds,
                _finalTarget.Bounds,
                Matrix.Identity,
                Color.White, Game.Data.SimpleShader, BlendState.Opaque, false);

            if (RenderToScreen)
            {
                Game.Data.SimpleShader.SetTechnique("Saturation");
                Game.Data.SimpleShader.SetParameter("Saturation", 2f);
                // Place the bloom on top

                DrawFinalTarget(_finalTarget);
            }

            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.SimpleShader.SetParameter("Saturation", 1f);
            //=== Draw the Ui and Debug ===
            
            RenderServices.DrawTextureQuad(_uiGameplayBufferTarget,
                sourceRect,
                destRect,
                Matrix.Identity,
                Color.White, Game.Data.Shader2D, BlendState.AlphaBlend, true);
            RenderServices.DrawTextureQuad(_uiBufferTarget,
                _uiBufferTarget.Bounds,
                _finalTarget.Bounds,
                Matrix.Identity,
                Color.White, Game.Data.SimpleShader, BlendState.AlphaBlend, true);

            // ==============================================================>
            // >====== Finally, finally draw everything to the monitor ======>
            // ==============================================================>
            _graphicsDevice.SetRenderTarget(null);
            // =======================================================>

            if (RenderToScreen)
            {
                RenderServices.DrawTextureQuad(_finalTarget,
                    _finalTarget.Bounds, _graphicsDevice.Viewport.Bounds,
                    Matrix.Identity, Color.White, BlendState.Opaque);
            }
            
            Camera.Unlock();
        }

        protected virtual void DrawGameplayBatch() { }

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
            nameof(_gameBufferTarget),
            nameof(_floorBufferTarget),
            nameof(_finalTarget),
            nameof(_uiGameplayBufferTarget),
            nameof(_uiBufferTarget),
            nameof(_preBufferTarget))]
        public void UpdateBufferTarget(int scale, float downsample)
        {
            ScreenSize = new Vector2(Camera.Width, Camera.Height) * scale * downsample;

            _gameBufferTarget?.Dispose();
            _gameBufferTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.DiscardContents
                );
            _graphicsDevice.SetRenderTarget(_gameBufferTarget);
            _graphicsDevice.Clear(Color.Black);

            _floorBufferTarget?.Dispose();
            _floorBufferTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.DiscardContents
                );
            _graphicsDevice.SetRenderTarget(_floorBufferTarget);
            _graphicsDevice.Clear(Color.Black);

            _uiGameplayBufferTarget?.Dispose();
            _uiGameplayBufferTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.DiscardContents
                );
            _graphicsDevice.SetRenderTarget(_uiGameplayBufferTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _uiBufferTarget?.Dispose();
            _uiBufferTarget = new RenderTarget2D(
                _graphicsDevice,
                ScreenSize.X,
                ScreenSize.Y,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.DiscardContents
                );
            _graphicsDevice.SetRenderTarget(_uiBufferTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _preBufferTarget?.Dispose();
            _preBufferTarget = new RenderTarget2D(
                _graphicsDevice,
                ScreenSize.X,
                ScreenSize.Y,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.DiscardContents
                );
            _graphicsDevice.SetRenderTarget(_preBufferTarget);
            _graphicsDevice.Clear(Color.Transparent);

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
            SpriteBatch?.Dispose();
            GameUiBatch?.Dispose();
            UiBatch?.Dispose();
            DebugSpriteBatch?.Dispose();

            _floorBufferTarget?.Dispose();
            _gameBufferTarget?.Dispose();
            _uiGameplayBufferTarget?.Dispose();
            _uiBufferTarget?.Dispose();
            _preBufferTarget?.Dispose();

            CachedTextTextures.Dispose();
        }
    }
}
