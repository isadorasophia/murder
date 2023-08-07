using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using static System.Formats.Asn1.AsnWriter;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Murder.Core.Graphics
{
    public enum TargetSpriteBatches
    {
        Gameplay,
        Floor,
        GameplayUi,
        Ui,
        Light,
        ReflectionArea,
        Reflected
    }

    public class RenderContext : IDisposable
    {
        public readonly Camera2D Camera;

        public readonly Batch2D FloorSpriteBatch;
        public readonly Batch2D GameplayBatch;
        public readonly Batch2D LightBatch;
        public readonly Batch2D GameUiBatch;
        public readonly Batch2D ReflectionAreaBatch;
        public readonly Batch2D ReflectedBatch;
        public readonly Batch2D UiBatch;
        public readonly Batch2D DebugFxSpriteBatch;
        public readonly Batch2D DebugSpriteBatch;

        protected RenderTarget2D? _floorBufferTarget;

        private RenderTarget2D? _uiTarget;
        private RenderTarget2D? _mainTarget;
        private RenderTarget2D? _reflectionTarget;
        private RenderTarget2D? _reflectedTarget;

        public BatchPreviewState PreviewState;

        public enum BatchPreviewState
        {
            None,
            Step1,
            Step2,
            Gameplay,
            Ui,
            Lights,
            Final,
            Reflected,
            Reflection
        }
        private RenderTarget2D? _debugBatchPreview;

        private RenderTarget2D? _debugTarget;
        /// <summary>
        /// Temporary buffer with the camera size. Used so we can apply effects
        /// such as limited palette and bloom on a smaller screen before applying
        /// it to the final target
        /// </summary>
        private RenderTarget2D? _tempTarget;
        /// <summary>
        /// The final screen target, has the real screen size.
        /// </summary>
        private RenderTarget2D? _finalTarget;

        private readonly RenderTarget2D? _bloomTarget = null;
        /// <summary>
        /// Bloom temporary render target (for bright pass)
        /// </summary>
        private readonly RenderTarget2D? _bloomBrightRenderTarget = null;
        /// <summary>
        /// Bloom temporary render target (for blur pass)
        /// </summary>
        private readonly RenderTarget2D? _bloomBlurRenderTarget = null;

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
                case TargetSpriteBatches.Light:
                    return LightBatch;
                case TargetSpriteBatches.Ui:
                    return UiBatch;
                case TargetSpriteBatches.Floor:
                    return FloorSpriteBatch;
                case TargetSpriteBatches.ReflectionArea:
                    return ReflectionAreaBatch;
                case TargetSpriteBatches.Reflected:
                    return ReflectedBatch;
                default:
                    throw new Exception("Unknown or forbidden Sprite Batch");
            }
        }

        public static readonly int CAMERA_BLEED = 4;

        public bool RenderToScreen = true;

        public Point ScreenSize;
        public Color BackColor => Game.Data.GameProfile.BackColor;

        public Texture2D? ColorGrade;
        private bool _useCustomShader;

        public void SwitchCustomShader(bool enable)
        {
            _useCustomShader = enable;
        }

        // Use the bloom shader before applying the final result to the screen
        public float Bloom = 0;

        public enum RenderTargets
        {
            MainBufferTarget,
            BloomTarget1,
            BloomTarget2,
            FinalTarget,
            UiTarget
        }

        public virtual Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
        {
            Texture2D? target = inspectingRenderTarget switch
            {
                RenderTargets.MainBufferTarget => _mainTarget,
                RenderTargets.BloomTarget1 => _bloomBrightRenderTarget,
                RenderTargets.BloomTarget2 => _bloomBlurRenderTarget,
                RenderTargets.FinalTarget => _finalTarget,
                RenderTargets.UiTarget => _uiTarget,
                _ => default
            };

            return target ?? throw new ArgumentException($"Unable to find a render target for {inspectingRenderTarget}.");
        }

        public RenderContext(GraphicsDevice graphicsDevice, Camera2D camera, bool useCustomShader)
        {
            Camera = camera;

            _useCustomShader = useCustomShader && Game.Data.CustomGameShader.Length > 0;
            _graphicsDevice = graphicsDevice;

            DebugFxSpriteBatch = new(graphicsDevice);
            DebugSpriteBatch = new(graphicsDevice);
            GameplayBatch = new(graphicsDevice);
            LightBatch = new(graphicsDevice);
            FloorSpriteBatch = new(graphicsDevice);
            UiBatch = new(graphicsDevice);
            GameUiBatch = new(graphicsDevice);
            ReflectedBatch = new(graphicsDevice);
            ReflectionAreaBatch = new(graphicsDevice);
        }

        /// <summary>
        /// Refresh the window size with <paramref name="size"/> with width and height information,
        /// respectively.
        /// </summary>
        /// <returns>
        /// Whether the window actually required a refresh.
        /// </returns>
        public bool RefreshWindow(Point size, float scale) =>
            RefreshWindow(_graphicsDevice, size, scale);

        internal bool RefreshWindow(GraphicsDevice graphicsDevice, Point size, float scale)
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
            UpdateBufferTarget(scale);

            return true;
        }

        public void Begin()
        {
            // no one should interfere with camera settings at this point.
            Camera.Lock();

            FloorSpriteBatch.Begin(
                Game.Data.ShaderSprite,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            GameplayBatch.Begin(
                Game.Data.ShaderSprite,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            LightBatch.Begin(
                Game.Data.ShaderSprite,
                batchMode: BatchMode.DrawOrder,
                blendState: BlendState.Additive,
                sampler: SamplerState.AnisotropicClamp,
                depthStencil: DepthStencilState.None,
                transform: Camera.WorldViewProjection
            );

#if DEBUG
            DebugFxSpriteBatch.Begin(
                Game.Data.ShaderSprite,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );

            DebugSpriteBatch.Begin(
                Game.Data.ShaderSprite,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead,
                transform: Camera.WorldViewProjection
            );
#endif

            GameUiBatch.Begin(
                effect: Game.Data.ShaderSprite,
                batchMode: BatchMode.DepthSortDescending,
                sampler: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transform: Camera.WorldViewProjection
            );
            
            ReflectionAreaBatch.Begin(
                effect: Game.Data.ShaderSprite,
                batchMode: BatchMode.DepthSortDescending,
                sampler: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transform: Camera.WorldViewProjection
            );

            ReflectedBatch.Begin(
                effect: Game.Data.ShaderSprite,
                batchMode: BatchMode.DepthSortDescending,
                sampler: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transform: Camera.WorldViewProjection
            );

            UiBatch.Begin(
                Game.Data.ShaderSprite,
                batchMode: BatchMode.DepthSortDescending,
                depthStencil: DepthStencilState.None,
                sampler: SamplerState.PointClamp,
                transform: Matrix.Identity,
                blendState: BlendState.AlphaBlend
            );
        }

        public void End()
        {
            GameLogger.Verify(
                _uiTarget is not null &&
                _mainTarget is not null &&
                _tempTarget is not null &&
                _finalTarget is not null,
                "Did not initialize buffer targets before calling RenderContext.End()?");

            Game.Data.ShaderSimple.SetTechnique("Simple");
            Game.Data.ShaderSprite.SetTechnique("Alpha");

            // =======================================================>
            // Draw the floor to a temp batch
            _graphicsDevice.SetRenderTarget(_tempTarget);
            _graphicsDevice.Clear(BackColor);
            FloorSpriteBatch.End();     // <=== Floor batch
            // =======================================================>

            // Handle reflections
            if (RenderToScreen && _reflectionTarget != null && _reflectedTarget != null)
            {
                _graphicsDevice.SetRenderTarget(_reflectedTarget);
                _graphicsDevice.Clear(Color.Transparent);
                RenderServices.DrawTextureQuad(_tempTarget, _tempTarget.Bounds, _mainTarget.Bounds, Matrix.Identity, Color.White, BlendState.Opaque, Game.Data.ShaderSimple);
                ReflectedBatch.End();
                CreateDebugPreviewIfNecessary(BatchPreviewState.Reflected, _reflectedTarget);

                _graphicsDevice.SetRenderTarget(_reflectionTarget);
                _graphicsDevice.Clear(Color.Transparent);
                ReflectionAreaBatch.End();
                CreateDebugPreviewIfNecessary(BatchPreviewState.Reflection, _reflectionTarget);

                _graphicsDevice.SetRenderTarget(_mainTarget);

                Game.Data.MaskShader.Parameters["maskSampler"]?.SetValue(_reflectionTarget);
                Game.Data.MaskShader.Parameters["Time"]?.SetValue(Game.Now);
                Game.Data.MaskShader.Parameters["RippleAmplitude"]?.SetValue(10);
                Game.Data.MaskShader.Parameters["RippleFrequency"]?.SetValue(0.01f);
                Game.Data.MaskShader.Parameters["TextureSize"]?.SetValue(_reflectedTarget.Bounds.Size.ToVector2());
                Game.Data.MaskShader.Parameters["CameraOffset"]?.SetValue(Matrix.Invert(Camera.WorldViewProjection));

                RenderServices.DrawTextureQuad(_tempTarget, _tempTarget.Bounds, _mainTarget.Bounds, Matrix.Identity, Color.White, BlendState.AlphaBlend, Game.Data.ShaderSimple);
                RenderServices.DrawTextureQuad(_reflectedTarget, _reflectedTarget.Bounds, _mainTarget.Bounds, Matrix.Identity, Color.White, BlendState.AlphaBlend, Game.Data.MaskShader);

                CreateDebugPreviewIfNecessary(BatchPreviewState.Step1, _mainTarget);
            }
            else
            {
                ReflectedBatch.GiveUp();
                ReflectionAreaBatch.GiveUp();
                
                _graphicsDevice.SetRenderTarget(_mainTarget);
                RenderServices.DrawTextureQuad(_tempTarget, _tempTarget.Bounds, _mainTarget.Bounds, Matrix.Identity, Color.White, BlendState.Opaque, Game.Data.ShaderSimple);
                CreateDebugPreviewIfNecessary(BatchPreviewState.Step1, _mainTarget);
            }

            // Draw all the gameplay graphics to _mainTarget
            GameplayBatch.End();        // <=== Gameplay batch

            GameUiBatch.End();          // <=== Ui that follows the camera

            CreateDebugPreviewIfNecessary(BatchPreviewState.Gameplay, _mainTarget);

            _graphicsDevice.SetRenderTarget(_uiTarget);
            _graphicsDevice.Clear(new Color(0, 0, 0, 0));

            UiBatch.End();              // <=== Static Ui

            _graphicsDevice.SetRenderTarget(_finalTarget);

            var scale = (_finalTarget.Bounds.Size.ToVector2() / _mainTarget.Bounds.Size.ToVector2());
            scale.Ceiling();

            var cameraAdjust = (new Vector2(
                Camera.Position.Point.X - Camera.Position.X - CAMERA_BLEED / 2,
                Camera.Position.Point.Y - Camera.Position.Y - CAMERA_BLEED / 2) *
                scale).Point;

            if (_useCustomShader)
            {
                Game.Data.CustomGameShader[0]?.TrySetParameter("gameTime", Game.Now);
            }

            Effect? gameShader = default;
            if (_useCustomShader)
            {
                gameShader = Game.Data.CustomGameShader[0];
                gameShader.SetTechnique("FixedPalette");
            }
            gameShader ??= Game.Data.ShaderSimple;
            Game.Data.PosterizerShader.SetParameter("aberrationStrength", 0.01f);

            _graphicsDevice.SetRenderTarget(_tempTarget);
            _graphicsDevice.Clear(BackColor);
            RenderServices.DrawTextureQuad(_mainTarget,     // <=== Draws the game buffer to a temp buffer with the fancy shader
                _mainTarget.Bounds,
                new Rectangle(Vector2.Zero, _mainTarget.Bounds.Size.ToVector2()),
                Matrix.Identity,
                Color.White, gameShader, BlendState.Opaque, false);

            _graphicsDevice.SetRenderTarget(_finalTarget);
            RenderServices.DrawTextureQuad(_tempTarget,     // <=== Draws the game buffer to the final buffer using a cheap shader
                _tempTarget.Bounds,
                new Rectangle(cameraAdjust, _tempTarget.Bounds.Size.ToVector2() * scale),
                Matrix.Identity,
                Color.White, Game.Data.ShaderSimple, BlendState.Opaque, false);

            _graphicsDevice.SetRenderTarget(_tempTarget);
            _graphicsDevice.Clear(Color.Black);
            LightBatch.End();
            _graphicsDevice.SetRenderTarget(_finalTarget);
            Game.Data.PosterizerShader.SetParameter("levels", 16f);
            Game.Data.PosterizerShader.SetParameter("aberrationStrength", 0.04f);

            RenderServices.DrawTextureQuad(_tempTarget,     // <=== Draws the light buffer to the final buffer using an additive blend
                _tempTarget.Bounds,
                new Rectangle(cameraAdjust, _tempTarget.Bounds.Size.ToVector2() * scale),
                Matrix.Identity,
                Color.White * 0.75f, Game.Data.PosterizerShader, BlendState.Additive, false);


#if false
            if (Game.Preferences.Bloom && Bloom > 0)
            {
                var finalTarget = _finalTarget;
                finalTarget = ApplyBloom(_finalTarget, 0.75f, 2f);

                _graphicsDevice.SetRenderTarget(_finalTarget);
                RenderServices.DrawTextureQuad(finalTarget,     // <=== Apply that sweet sweet bloom
                    finalTarget.Bounds,
                    _finalTarget.Bounds,
                    Matrix.Identity,
                    Color.White * Bloom, Game.Data.ShaderSimple, BlendState.Additive, false);
            }
#endif
            _graphicsDevice.SetRenderTarget(_tempTarget);
            _graphicsDevice.Clear(Color.Transparent);
            RenderServices.DrawTextureQuad(_uiTarget,     // <=== Draws the ui buffer to a temp buffer with the fancy shader
                _uiTarget.Bounds,
                new Rectangle(Vector2.Zero, _uiTarget.Bounds.Size.ToVector2()), // Since the UI doesn't move a lot, we will force it to the output size
                Matrix.Identity,                                                          // This WILL break pixels, another solution is to add a small bleed area
                Color.White, gameShader, BlendState.Opaque, false);

            var bleedArea = ( _tempTarget.Bounds.Size.ToVector2() - _graphicsDevice.Viewport.Bounds.Size.ToVector2());

            _graphicsDevice.SetRenderTarget(_finalTarget);
            RenderServices.DrawTextureQuad(_tempTarget,     // <=== Draws the temp buffer to the final buffer with a cheap shader
                _tempTarget.Bounds,
                new Rectangle(Vector2.Zero, _tempTarget.Bounds.Size.ToVector2() * scale),
                Matrix.Identity,
                Color.White, Game.Data.ShaderSimple, BlendState.NonPremultiplied, false);

#if DEBUG
            GameLogger.Verify(_debugTarget is not null);

            // Draw all the debug stuff in the main target again
            _graphicsDevice.SetRenderTarget(_debugTarget);
            _graphicsDevice.Clear(Color.Transparent);

            Game.Data.ShaderSimple.SetTechnique("Simple");
            Game.Data.ShaderSprite.SetTechnique("DiagonalLines");
            Game.Data.ShaderSprite.SetParameter("inputTime", Game.Now);
            DebugFxSpriteBatch.End();

            Game.Data.ShaderSimple.SetTechnique("Simple");
            Game.Data.ShaderSprite.SetTechnique("Alpha");
            DebugSpriteBatch.End();

            _graphicsDevice.SetRenderTarget(_finalTarget);
            RenderServices.DrawTextureQuad(_debugTarget,     // <=== Draws the debug buffer to the final buffer
                _debugTarget.Bounds,
                new Rectangle(cameraAdjust, _finalTarget.Bounds.Size.ToVector2() + scale * CAMERA_BLEED * 2),
                Matrix.Identity,
                Color.White, Game.Data.ShaderSimple, BlendState.AlphaBlend, false);
#endif

            // =======================================================>
            // Time to draw this game to the screen!!
            // =======================================================>
            _graphicsDevice.SetRenderTarget(null);

            if (RenderToScreen)
            {
                Game.Data.ShaderSimple.SetTechnique("Simple");
                RenderServices.DrawTextureQuad(_finalTarget,
                    _finalTarget.Bounds, _finalTarget.Bounds,
                    Matrix.Identity, Color.White, Game.Data.ShaderSimple, BlendState.Opaque, false);
            }

            Camera.Unlock();
        }

        private RenderTarget2D ApplyBloom(RenderTarget2D sceneRenderTarget, float threshold, float spread)
        {
            Game.Data.BloomShader.SetParameter("bloomThreshold", threshold);
            Game.Data.BloomShader.SetParameter("sWidth", (float)ScreenSize.X / Math.Max(1, spread));
            Game.Data.BloomShader.SetParameter("sHeight", (float)ScreenSize.Y / Math.Max(1, spread));

            Debug.Assert(_bloomBrightRenderTarget != null && _bloomBlurRenderTarget != null);

            // Extract bright areas
            _graphicsDevice.SetRenderTarget(_bloomBrightRenderTarget);
            _graphicsDevice.Clear(Color.Black);
            Game.Data.BloomShader.SetTechnique("Bloom_BrightPass");
            RenderServices.DrawTextureQuad(sceneRenderTarget,
                sceneRenderTarget.Bounds,
                _bloomBrightRenderTarget.Bounds,
                Matrix.Identity,
                Color.White, Game.Data.BloomShader, BlendState.Opaque, false);

            // Apply horizontal Gaussian blur
            _graphicsDevice.SetRenderTarget(_bloomBlurRenderTarget);
            Game.Data.BloomShader.SetTechnique("Bloom_GaussianBlurHorizontal");
            RenderServices.DrawTextureQuad(_bloomBrightRenderTarget,
                _bloomBrightRenderTarget.Bounds,
                _bloomBlurRenderTarget.Bounds,
                Matrix.Identity,
                Color.White, Game.Data.BloomShader, BlendState.Opaque, false);

            //// Apply vertical Gaussian blur
            _graphicsDevice.SetRenderTarget(_bloomBrightRenderTarget);
            Game.Data.BloomShader.SetTechnique("Bloom_GaussianBlurVertical");
            RenderServices.DrawTextureQuad(_bloomBlurRenderTarget,
                _bloomBlurRenderTarget.Bounds,
                _bloomBrightRenderTarget.Bounds,
                Matrix.Identity,
                Color.White, Game.Data.BloomShader, BlendState.Opaque, false);

            return _bloomBrightRenderTarget;
        }

        [MemberNotNull(
            nameof(_uiTarget),
            nameof(_mainTarget),
            nameof(_reflectedTarget),
            nameof(_reflectionTarget),
            nameof(_tempTarget),
            nameof(_debugTarget),
            nameof(_finalTarget))]
        public void UpdateBufferTarget(float scale)
        {
            string defaultPalettePath = FileHelper.GetPath("resources", Game.Profile.DefaultPalette) + ".png";

            if (FileHelper.Exists(defaultPalettePath))
            {
                if (Game.Data.CustomGameShader.Length != 0)
                {
                    Texture2D defaultPalette = Game.Data.FetchTexture(Game.Profile.DefaultPalette);
                    Game.Data.CustomGameShader[0]?.SetParameter("paletteTexture1", defaultPalette);
                }
            }
            else
            {
                GameLogger.Warning($"Default palette not set or not found({defaultPalettePath})! Choose one in GameProfile");
            }

            if (Game.Preferences.Downscale)
                ScreenSize = new Point(Camera.Width, Camera.Height);
            else
                ScreenSize = new Point(Camera.Width, Camera.Height) * scale;

            _uiTarget?.Dispose();
            _uiTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width,
                Camera.Height,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.DiscardContents
                );
            _graphicsDevice.SetRenderTarget(_uiTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _mainTarget?.Dispose();
            _mainTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_mainTarget);
            _graphicsDevice.Clear(BackColor);

            // Reflection
            _reflectionTarget?.Dispose();
            _reflectionTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_reflectionTarget);
            _graphicsDevice.Clear(Color.Transparent);

            // Reflected
            _reflectedTarget?.Dispose();
            _reflectedTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_reflectedTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _tempTarget?.Dispose();
            _tempTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_tempTarget);
            _graphicsDevice.Clear(Color.Transparent);

#if false
            if (Game.Preferences.Bloom)
            {
                _bloomTarget?.Dispose();
                _bloomTarget = new RenderTarget2D(
                    _graphicsDevice,
                    Camera.Width,
                    Camera.Height,
                    mipMap: false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24Stencil8,
                    0,
                    RenderTargetUsage.PreserveContents
                    );
                _graphicsDevice.SetRenderTarget(_bloomTarget);
                _graphicsDevice.Clear(Color.Transparent);

                _bloomBlurRenderTarget?.Dispose();
                _bloomBlurRenderTarget = new RenderTarget2D(
                    _graphicsDevice,
                    ScreenSize.X,
                    ScreenSize.Y,
                    mipMap: false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24Stencil8,
                    0,
                    RenderTargetUsage.DiscardContents
                    );

                _bloomBrightRenderTarget?.Dispose();
                _bloomBrightRenderTarget = new RenderTarget2D(
                    _graphicsDevice,
                    ScreenSize.X,
                    ScreenSize.Y,
                    mipMap: false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24Stencil8,
                    0,
                    RenderTargetUsage.DiscardContents
                    );
            }
#endif

            _debugTarget?.Dispose();
            _debugTarget = new RenderTarget2D(
                _graphicsDevice,
                Camera.Width + CAMERA_BLEED * 2,
                Camera.Height + CAMERA_BLEED * 2,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_debugTarget);
            _graphicsDevice.Clear(BackColor);

            _finalTarget?.Dispose();
            _finalTarget = new RenderTarget2D(
                _graphicsDevice,
                Calculator.RoundToInt(ScreenSize.X) + CAMERA_BLEED,
                Calculator.RoundToInt(ScreenSize.Y) + CAMERA_BLEED,
                mipMap: false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents
                );
            _graphicsDevice.SetRenderTarget(_finalTarget);
            _graphicsDevice.Clear(BackColor);

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

            Game.Data.ShaderSimple.SetTechnique("Simple");

            UnloadImpl();

            _graphicsDevice.SetRenderTarget(null);

            // if we do that it will cause an issue while resizing the window.
            // Game.GraphicsDevice.Reset();
        }

        protected virtual void UnloadImpl() { }

        public virtual void Dispose()
        {
            CachedTextTextures.Dispose();

            FloorSpriteBatch?.Dispose();
            GameplayBatch?.Dispose();
            LightBatch.Dispose();
            GameUiBatch?.Dispose();
            UiBatch?.Dispose();
            DebugFxSpriteBatch?.Dispose();
            DebugSpriteBatch?.Dispose();

            _floorBufferTarget?.Dispose();

            _uiTarget?.Dispose();
            _mainTarget?.Dispose();
            _bloomTarget?.Dispose();
            _debugTarget?.Dispose();
            _tempTarget?.Dispose();
            _finalTarget?.Dispose();
            _bloomBrightRenderTarget?.Dispose();
            _bloomBlurRenderTarget?.Dispose();
        }

        public void CreateDebugPreviewIfNecessary(BatchPreviewState currentState, RenderTarget2D target)
        {
#if DEBUG
            if (PreviewState == currentState)
            {
                _graphicsDevice.SetRenderTarget(_debugBatchPreview);
                _graphicsDevice.Clear(Color.Transparent);
                RenderServices.DrawTextureQuad(target,
                target.Bounds,
                    target.Bounds,
                    Matrix.Identity,
                    Color.White, Game.Data.PosterizerShader, BlendState.NonPremultiplied, false);
            }
#endif
        }
    }
}
