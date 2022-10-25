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
        public readonly Batch2D ShadowFloorSpriteBatch;
        public readonly Batch2D SpriteBatch;
        public readonly Batch2D GameUiBatch;
        public readonly Batch2D UiBatch;
        public readonly Batch2D DebugSpriteBatch;

        private RenderTarget2D? _uiGameplayBufferTarget;
        private RenderTarget2D? _uiBufferTarget;
        private RenderTarget2D? _gameBufferTarget;
        private RenderTarget2D? _floorBufferTarget;
        private RenderTarget2D? _shadowBufferTarget;
        private RenderTarget2D? _lightBufferTarget;
        private RenderTarget2D? _floorLightBufferTarget;
        private RenderTarget2D? _preBufferTarget;
        private RenderTarget2D? _finalTarget;
        private Texture2D _bloom = null!;

        private RenderTarget2D? _lightSpotsTempTarget;
        private BloomFilter? _bloomFilter;
        private GraphicsDevice _graphicsDevice;

        public Point GameBufferSize;
        public Point UiReferenceScale = new(1920, 1080);

        public float UiMinScale
        {
            get
            {
                if (_uiMinScale is null)
                {
                    _uiMinScale = Math.Min(
                        (float)UiRenderTarget.Bounds.Width / UiReferenceScale.X,
                        (float)UiRenderTarget.Bounds.Height / UiReferenceScale.Y);
                }
                return _uiMinScale.Value;
            }
        }

        private float? _uiMinScale;

        public Vector2 UiScale
        {
            get 
            {
                if (_uiScale is null)
                    _uiScale = new Vector2(
                        (float)UiRenderTarget.Bounds.Width / UiReferenceScale.X,
                        (float)UiRenderTarget.Bounds.Height / UiReferenceScale.Y);
                
                return _uiScale.Value;
            }
        }

        private Vector2? _uiScale;
        
        public RenderTarget2D? LastRenderTarget => _finalTarget;

        public RenderTarget2D UiRenderTarget => _uiBufferTarget ?? throw new InvalidOperationException("Tried to acquire UiRenderTarget uninitialized.");
        public RenderTarget2D LightRenderTarget => _lightBufferTarget ?? throw new InvalidOperationException("Tried to acquire LightRenderTarget uninitialized.");
        public RenderTarget2D FloorLightRenderTarget => _floorLightBufferTarget ?? throw new InvalidOperationException("Tried to acquire FloorLightRenderTarget uninitialized.");

        public readonly CacheDictionary<string, Texture2D> CachedTextTextures = new(32);

        public Batch2D GetSpriteBatch(TargetSpriteBatches targetSpriteBatch)
        {
            switch (targetSpriteBatch)
            {
                case TargetSpriteBatches.Gameplay:
                    return SpriteBatch;
                case TargetSpriteBatches.Floor:
                    return FloorSpriteBatch;
                case TargetSpriteBatches.GameplayUi:
                    return GameUiBatch;
                case TargetSpriteBatches.Ui:
                    return UiBatch;
                default:
                    throw new Exception("Unknown spritebatch");
            }
        }

        public RenderTarget2D SpotsRenderTarget => _lightSpotsTempTarget ?? throw new InvalidOperationException("Tried to acquire SpotsRenderTarget uninitialized.");

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
        public Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
        {
            Texture2D? target = inspectingRenderTarget switch
            {
                RenderTargets.FloorBufferTarget => _floorBufferTarget,
                RenderTargets.GameBufferTarget => _gameBufferTarget,
                RenderTargets.LightBufferTarget => _lightSpotsTempTarget,
                RenderTargets.FloorLightBufferTarget => _lightSpotsTempTarget,
                RenderTargets.PreBufferTarget => _preBufferTarget,
                RenderTargets.FinalTarget => _finalTarget,
                RenderTargets.DitherTexture => Game.Data.DitherTexture,
                RenderTargets.UiBufferTarget => _uiBufferTarget,
                RenderTargets.UiGameplayBufferTarget => _uiGameplayBufferTarget,
                RenderTargets.Bloom => _bloom,
                _ => default
            };

            return target ?? throw new ArgumentException($"Unable to find a render target for {inspectingRenderTarget}.");
        }

        public RenderContext(GraphicsDevice graphicsDevice, Camera2D camera)
        {
            Camera = camera;

            _graphicsDevice = graphicsDevice;

            DebugSpriteBatch =          new(graphicsDevice);
            FloorSpriteBatch =          new(graphicsDevice);
            ShadowFloorSpriteBatch =    new(graphicsDevice);
            SpriteBatch =               new(graphicsDevice);

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
            _uiScale = null;
            _uiMinScale = null;
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
                Game.Data.SimpleShader,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.NonPremultiplied,
                sampler: SamplerState.PointClamp,
                transform: Camera.WorldViewProjection
            );

            ShadowFloorSpriteBatch.Begin( 
                Game.Data.SimpleShader,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            SpriteBatch.Begin(
                Game.Data.BasicShader,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            DebugSpriteBatch.Begin(
                Game.Data.BasicShader,
                blendState: BlendState.NonPremultiplied,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            GameUiBatch.Begin(
                effect: Game.Data.BasicShader,
                batchMode: BatchMode.DrawOrder,
                sampler: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transform: Camera.WorldViewProjection
            );

            UiBatch.Begin(
                Game.Data.BasicShader,
                batchMode: BatchMode.DepthSortDescending,
                depthStencil: DepthStencilState.None,
                sampler: SamplerState.AnisotropicWrap,
                blendState: BlendState.AlphaBlend
            );
        }

        public void End()
        {
            GameLogger.Verify(
                _shadowBufferTarget is not null &&
                _floorBufferTarget is not null &&
                _lightBufferTarget is not null &&
                _floorLightBufferTarget is not null &&
                _gameBufferTarget is not null &&
                _bloomFilter is not null &&
                _preBufferTarget is not null &&
                _finalTarget is not null &&
                _uiGameplayBufferTarget is not null &&
                _uiBufferTarget is not null,
                "Did not initialize buffer targets before calling RenderContext.End()?");
            
            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.BasicShader.SetTechnique("Alpha");

            if (RenderToScreen)
            {
                // Place the drop shadows in the shadow buffer target
                // (Little blobs under the characters)
                // Shadows have their own buffer because they need to be all added with a single transparency
                // =======================================================>
                _graphicsDevice.SetRenderTarget(_shadowBufferTarget);
                _graphicsDevice.Clear(Color.Transparent);
                // =======================================================>

                ShadowFloorSpriteBatch.End();
            }

            // =======================================================>
            _graphicsDevice.SetRenderTarget(_floorBufferTarget);
            _graphicsDevice.Clear(Color.Black);
            // =======================================================>
            // Draw the floor tiles
            FloorSpriteBatch.End();
            // Apply the blob shadows buffer into the floor buffer
            RenderServices.DrawTextureQuad(
                _shadowBufferTarget,
                new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                RenderServices.BlendNormal,
                Color.White.WithAlpha(0.75f),
                Game.Data.BasicShader,
                false
                );

            // Then set our graphics device to be on the buffer.
            // =======================================================>
            _graphicsDevice.SetRenderTarget(_gameBufferTarget);
            _graphicsDevice.Clear(Color.Green);
            // =======================================================>

            // Apply the floor to the gameplay batch
            Game.Data.MainShader.SetTechnique("Shade");
            Game.Data.MainShader.SetParameter("lightMapSampler", _floorLightBufferTarget);
            Game.Data.MainShader.SetParameter("shadeAmount", 0.35f);

            RenderServices.DrawTextureQuad(
                _floorBufferTarget,
                new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                RenderServices.BlendNormal,
                Color.White,
                Game.Data.MainShader,
                false
                );


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
            _graphicsDevice.Clear(Color.Red);
            // =======================================================>

            // Calculate stuff
            Vector2 scale = new((float)Game.GraphicsDevice.Viewport.Width / Camera.Width, (float)Game.GraphicsDevice.Viewport.Height / Camera.Height);
            // Do the camara snapping and draw the game buffer to the screen
            Vector2 pixelOffset = new(Calculator.RoundedDecimals(Camera.Position.X), Calculator.RoundedDecimals(Camera.Position.Y));
            Point pixelSnap = new(Calculator.RoundToInt(pixelOffset.X), Calculator.RoundToInt(pixelOffset.Y));

            // Warm up that shader
            Game.Data.MainShader.SetParameter("cameraPosition", Camera.Position.Point());
            Game.Data.MainShader.SetParameter("pixelScale", scale);
            Game.Data.MainShader.SetParameter("lightMapSampler", _lightBufferTarget);
            Game.Data.MainShader.SetParameter("time", Time.Elapsed);
            
            // Draw the main gameplay images to the buffer
            if (RenderToScreen)
                Game.Data.MainShader.SetTechnique("Monitor");
            else
                Game.Data.MainShader.SetTechnique("Dither"); // Simpler version for the editor

            var sourceRect = new Rectangle(
                    -pixelSnap.X + CAMERA_BLEED, -pixelSnap.Y + CAMERA_BLEED,
                    Camera.Width - Calculator.RoundToInt(pixelOffset.X) + CAMERA_BLEED,
                    Camera.Height - Calculator.RoundToInt(pixelOffset.Y) + CAMERA_BLEED);
            var destinationRect = new Rectangle(
                    Calculator.RoundToInt(-pixelOffset.X * scale.X),
                    Calculator.RoundToInt(-pixelOffset.Y * scale.Y),
                    sourceRect.Width * scale.X,
                    sourceRect.Height * scale.Y);

            RenderServices.DrawTextureQuad(_gameBufferTarget,
                source: sourceRect,
                destination: destinationRect,
                Matrix.Identity, Color.White, Game.Data.MainShader, BlendState.Opaque, false);

            if (RenderToScreen)
            {
                // Grab the bloom data before the UI;
                _bloomFilter.BloomThreshold = 0.58f;
                _bloomFilter.BloomStrengthMultiplier = 0.8f;
                //_bloom?.Dispose();
                _bloom = _bloomFilter.Draw(_preBufferTarget, _preBufferTarget.Width, _preBufferTarget.Height);
            }
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

                RenderServices.DrawTextureQuad(_bloom,
                    _bloom.Bounds, _finalTarget.Bounds,
                    Matrix.Identity, Color.White, BlendState.Additive);
            }

            Game.Data.SimpleShader.SetTechnique("Simple");
            Game.Data.SimpleShader.SetParameter("Saturation", 1f);
            //=== Draw the Ui and Debug ===
            
            RenderServices.DrawTextureQuad(_uiGameplayBufferTarget,
                sourceRect,
                destinationRect,
                Matrix.Identity,
                Color.White, Game.Data.BasicShader, BlendState.AlphaBlend, true);
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

        [MemberNotNull(
            nameof(_gameBufferTarget),
            nameof(_shadowBufferTarget),
            nameof(_lightBufferTarget),
            nameof(_finalTarget),
            nameof(_uiGameplayBufferTarget),
            nameof(_uiBufferTarget),
            nameof(_preBufferTarget),
            nameof(_lightSpotsTempTarget),
            nameof(_floorBufferTarget),
            nameof(_floorLightBufferTarget))]
        public void UpdateBufferTarget(int scale, float downsample)
        {
            ScreenSize = new Vector2(Camera.Width, Camera.Height) * scale * downsample;

            _shadowBufferTarget?.Dispose();
            _shadowBufferTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_shadowBufferTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _lightBufferTarget?.Dispose();
            _lightBufferTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_lightBufferTarget);
            _graphicsDevice.Clear(Color.Black);

            _floorLightBufferTarget?.Dispose();
            _floorLightBufferTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_floorLightBufferTarget);
            _graphicsDevice.Clear(Color.Black);

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

            _lightSpotsTempTarget?.Dispose();
            _lightSpotsTempTarget = new RenderTarget2D(
                _graphicsDevice,
                1024,
                1024,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_lightSpotsTempTarget);
            _graphicsDevice.Clear(Color.Black);

            _graphicsDevice.SetRenderTarget(null);

            // TODO: Pedro fix shaders! Since those are in InstallWizard now.
            //_bloomFilter?.Dispose();
            //_bloomFilter = new BloomFilter(_graphicsDevice, Game.Data.BloomShader, Camera.Width + CAMERA_BLEED * 2, Camera.Height + CAMERA_BLEED * 2);
            //_bloomFilter.BloomPreset = BloomFilter.BloomPresets.Wide;
            //_bloomFilter.BloomThreshold = 0.45f;
            //_bloomFilter.BloomStrengthMultiplier = 0.7f;

            GameBufferSize = new Point(Camera.Width + CAMERA_BLEED * 2, Camera.Height + CAMERA_BLEED * 2);

            Game.Data.MainShader?.SetParameter("screenSize", new Vector2(
                (Camera.Width + CAMERA_BLEED * 2),
                (Camera.Height + CAMERA_BLEED * 2)));
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

            Game.Data.MainShader.SetParameter("cameraPosition", Camera.Position.Point());
            Game.Data.MainShader.SetParameter("pixelScale", Vector2.One);
            Game.Data.MainShader.SetParameter("time", Time.Elapsed);

            _graphicsDevice.SetRenderTarget(null);

            Game.GraphicsDevice.Reset();
        }

        public void Dispose()
        {
            FloorSpriteBatch?.Dispose();
            ShadowFloorSpriteBatch?.Dispose();
            SpriteBatch?.Dispose();
            GameUiBatch?.Dispose();
            UiBatch?.Dispose();
            DebugSpriteBatch?.Dispose();

            _shadowBufferTarget?.Dispose();
            _lightBufferTarget?.Dispose();
            _floorLightBufferTarget?.Dispose();
            _gameBufferTarget?.Dispose();
            _floorBufferTarget?.Dispose();
            _uiGameplayBufferTarget?.Dispose();
            _uiBufferTarget?.Dispose();
            _preBufferTarget?.Dispose();
            _lightSpotsTempTarget?.Dispose();
            _bloom?.Dispose();
            _bloomFilter?.Dispose();

            CachedTextTextures.Dispose();
        }
    }
}
