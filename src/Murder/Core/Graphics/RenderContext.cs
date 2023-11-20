using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Murder.Core.Graphics;

public class RenderContext : IDisposable
{
    public readonly Camera2D Camera;

    public Batch2D GameplayBatch => GetBatch(Batches2D.GameplayBatchId);
    public Batch2D FloorBatch => GetBatch(Batches2D.FloorBatchId);
    public Batch2D LightBatch => GetBatch(Batches2D.LightBatchId);
    public Batch2D GameUiBatch => GetBatch(Batches2D.GameUiBatchId);
    public Batch2D UiBatch => GetBatch(Batches2D.UiBatchId);

    public Vector2 Scale => _scale;
    protected Vector2 _scale = Vector2.One;
    public Vector2 SubPixelOffset => _subPixelOffset;
    protected Vector2 _subPixelOffset;

    /// <summary>
    /// Only used if <see cref="RenderContextFlags.Debug"/> is set.
    /// </summary>
    public Batch2D DebugFxBatch => GetBatch(Batches2D.DebugFxBatchId);

    /// <summary>
    /// Only used if <see cref="RenderContextFlags.Debug"/> is set.
    /// </summary>
    public Batch2D DebugBatch => GetBatch(Batches2D.DebugBatchId);

    protected RenderTarget2D? _floorBufferTarget;

    protected RenderTarget2D? _uiTarget;
    protected RenderTarget2D? _mainTarget;
    public RenderTarget2D? MainTarget => _mainTarget;

    public BatchPreviewState PreviewState;
    public bool PreviewStretch;

    public enum BatchPreviewState
    {
        None,
        Step1,
        Step2,
        Step3,
        Step4,
        Step5,
        Gameplay,
        Ui,
        Lights,
        Reflected,
        Reflection,
        Debug
    }

    protected RenderTarget2D? _debugTargetPreview = null;

    protected RenderTarget2D? _debugTarget;
    /// <summary>
    /// Temporary buffer with the camera size. Used so we can apply effects
    /// such as limited palette and bloom on a smaller screen before applying
    /// it to the final target
    /// </summary>
    protected RenderTarget2D? _tempTarget;
    /// <summary>
    /// The final screen target, has the real screen size.
    /// </summary>
    protected RenderTarget2D? _finalTarget;

    protected GraphicsDevice _graphicsDevice;

    public Point GameBufferSize;

    public RenderTarget2D? LastRenderTarget => _finalTarget;

    public readonly CacheDictionary<string, Texture2D> CachedTextTextures = new(32);

    public readonly Batch2D[] _spriteBatches = new Batch2D[32];

    public Batch2D GetBatch(int index)
    {
        if (index < _spriteBatches.Length && _spriteBatches[index] != null)
        {
            return _spriteBatches[index];
        }

        GameLogger.Error($"Trying to access invalid Batch2D '{index}' on RenderContext.");
        return _spriteBatches[0];
    }

    public static readonly int CAMERA_BLEED = 4;

    public static readonly Vector2 CAMERA_BLEED_VECTOR = new(CAMERA_BLEED, CAMERA_BLEED);

    public bool RenderToScreen = true;

    public Point ScreenSize;
    public Color BackColor => Game.Data.GameProfile.BackColor;

    public Texture2D? ColorGrade;

    protected readonly bool _useDebugBatches;
    protected bool _useCustomShader;

    private Rectangle? _takeScreenShot;

    public void SwitchCustomShader(bool enable)
    {
        _useCustomShader = enable;
    }

    // Use the bloom shader before applying the final result to the screen
    public float Bloom = 0;

    public enum RenderTargets
    {
        MainBufferTarget,
        FinalTarget,
        UiTarget
    }

    /// <summary>
    /// A context for how to render your game. Holds everything you need to draw on the screen.
    /// To make your own, extend this class and add it to <see cref="Game.CreateRenderContext(GraphicsDevice, Camera2D, RenderContextFlags)"/>
    /// Extending your <see cref="Batches2D"/> file is also recommended.
    /// </summary>
    public RenderContext(GraphicsDevice graphicsDevice, Camera2D camera, RenderContextFlags settings)
    {
        Camera = camera;

        _useDebugBatches = settings.HasFlag(RenderContextFlags.Debug);
        _useCustomShader = settings.HasFlag(RenderContextFlags.CustomShaders) && Game.Data.CustomGameShader.Length > 0;

        _graphicsDevice = graphicsDevice;
        Initialize();
    }

    public virtual Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
    {
        Texture2D? target = inspectingRenderTarget switch
        {
            RenderTargets.MainBufferTarget => _mainTarget,
            RenderTargets.FinalTarget => _finalTarget,
            RenderTargets.UiTarget => _uiTarget,
            _ => default
        };

        return target ?? throw new ArgumentException($"Unable to find a render target for {inspectingRenderTarget}.");
    }

    public virtual void Initialize()
    {
        RegisterSpriteBatch(Batches2D.GameplayBatchId,
            new("Gameplay",
            _graphicsDevice,
            true,
            Game.Data.ShaderSprite,
            BatchMode.DepthSortDescending,
            BlendState.AlphaBlend,
            SamplerState.PointClamp
            ));

        RegisterSpriteBatch(Batches2D.FloorBatchId,
            new("Floor",
            _graphicsDevice,
            true,
            Game.Data.ShaderSprite,
            BatchMode.DepthSortDescending,
            BlendState.AlphaBlend,
            SamplerState.PointClamp
            ));

        RegisterSpriteBatch(Batches2D.LightBatchId,
            new("Light",
            _graphicsDevice,
            true,
            Game.Data.ShaderSprite,
            BatchMode.DrawOrder,
            BlendState.Additive,
            SamplerState.AnisotropicClamp
            ));

        if (_useDebugBatches)
        {
            RegisterSpriteBatch(Batches2D.DebugFxBatchId,
                new("DebugFx",
                _graphicsDevice,
                true,
                Game.Data.ShaderSprite,
                BatchMode.DrawOrder,
                BlendState.AlphaBlend,
                SamplerState.PointClamp
                ));

            RegisterSpriteBatch(Batches2D.DebugBatchId,
                new("Debug",
                _graphicsDevice,
                true,
                Game.Data.ShaderSprite,
                BatchMode.DrawOrder,
                BlendState.AlphaBlend,
                SamplerState.PointClamp
                ));
        }

        RegisterSpriteBatch(Batches2D.GameUiBatchId,
            new("GameUi",
            _graphicsDevice,
            true,
            Game.Data.ShaderSprite,
            BatchMode.DepthSortDescending,
            BlendState.AlphaBlend,
            SamplerState.PointClamp
            ));

        RegisterSpriteBatch(Batches2D.UiBatchId,
            new("Ui",
            _graphicsDevice,
            false,
            Game.Data.ShaderSprite,
            BatchMode.DepthSortDescending,
            BlendState.AlphaBlend,
            SamplerState.PointClamp
            ));
    }

    public void RegisterSpriteBatch(int index, Batch2D batch)
    {
        if (index >= _spriteBatches.Length)
        {
            //[TODO] Resize _spriteBatches keeping it's values.
        }

        if (_spriteBatches[index] != null)
        {
            GameLogger.Warning($"Sprite batch {_spriteBatches[index].Name} is being overwritten by {batch.Name}. Was this on purpose?");
        }

        _spriteBatches[index] = batch;
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
        _graphicsDevice = graphicsDevice;
        if (Game.Profile.EnforceResolution && RenderToScreen)
        {
            Camera.UpdateSize(Game.Profile.GameWidth, Game.Profile.GameHeight);
        }
        else
        {
            Camera.UpdateSize(size.X, size.Y);
        }
        UpdateBufferTarget(scale);

        return true;
    }

    public virtual void Begin()
    {
        // no one should interfere with camera settings at this point.
        Camera.Lock();

        for (int i = 0; i < _spriteBatches.Length; i++)
        {
            if (_spriteBatches[i] is Batch2D batch2D)
            {
                batch2D.Begin(Camera.WorldViewProjection);
            }
        }
    }

    public virtual void End()
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
        FloorBatch.End();     // <=== Floor batch
                              // =======================================================>

        _graphicsDevice.SetRenderTarget(_mainTarget);
        RenderServices.DrawTextureQuad(_tempTarget, _tempTarget.Bounds, _mainTarget.Bounds, Matrix.Identity, Color.White, BlendState.Opaque, Game.Data.ShaderSimple);
        CreateDebugPreviewIfNecessary(BatchPreviewState.Step1, _mainTarget);
        
        // Draw all the gameplay graphics to _mainTarget
        GameplayBatch.End();        // <=== Gameplay batch
        TakeScreenshotIfNecessary(_mainTarget);

        GameUiBatch.End();          // <=== Ui that follows the camera

        CreateDebugPreviewIfNecessary(BatchPreviewState.Gameplay, _mainTarget);

        _graphicsDevice.SetRenderTarget(_uiTarget);
        _graphicsDevice.Clear(new Color(0, 0, 0, 0));

        UiBatch.End();              // <=== Static Ui

        CreateDebugPreviewIfNecessary(BatchPreviewState.Ui, _uiTarget);

        _graphicsDevice.SetRenderTarget(_finalTarget);

        _scale = (_finalTarget.Bounds.Size.ToSysVector2() / _mainTarget.Bounds.Size.ToSysVector2());
        _scale = _scale.Ceiling();

        _subPixelOffset = new Vector2(
            Camera.Position.Point().X - Camera.Position.X - CAMERA_BLEED / 2,
            Camera.Position.Point().Y - Camera.Position.Y - CAMERA_BLEED / 2)
            .Multiply(_scale).Point();

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
            new Rectangle(Vector2.Zero, _mainTarget.Bounds.Size.ToSysVector2()),
            Matrix.Identity,
            Color.White, gameShader, BlendState.Opaque, false);
        CreateDebugPreviewIfNecessary(BatchPreviewState.Step2, _tempTarget);

        _graphicsDevice.SetRenderTarget(_finalTarget);
        RenderServices.DrawTextureQuad(_tempTarget,     // <=== Draws the game buffer to the final buffer using a cheap shader
            _tempTarget.Bounds,
            new Rectangle(_subPixelOffset, _tempTarget.Bounds.Size.ToSysVector2().Multiply(_scale)),
            Matrix.Identity,
            Color.White, Game.Data.ShaderSimple, BlendState.Opaque, false);

        _graphicsDevice.SetRenderTarget(_tempTarget);
        _graphicsDevice.Clear(Color.Black);
        LightBatch.End();
        CreateDebugPreviewIfNecessary(BatchPreviewState.Lights, _tempTarget);

        _graphicsDevice.SetRenderTarget(_finalTarget);
        Game.Data.PosterizerShader.SetParameter("levels", 16f);
        Game.Data.PosterizerShader.SetParameter("aberrationStrength", 0.04f);

        RenderServices.DrawTextureQuad(_tempTarget,     // <=== Draws the light buffer to the final buffer using an additive blend
            _tempTarget.Bounds,
            new Rectangle(_subPixelOffset, _tempTarget.Bounds.Size.ToSysVector2().Multiply(_scale)),
            Matrix.Identity,
            Color.White * 0.75f, Game.Data.PosterizerShader, BlendState.Additive, false);

        CreateDebugPreviewIfNecessary(BatchPreviewState.Step3, _finalTarget);
        
        _graphicsDevice.SetRenderTarget(_tempTarget);
        _graphicsDevice.Clear(Color.Transparent);
        RenderServices.DrawTextureQuad(_uiTarget,     // <=== Draws the ui buffer to a temp buffer with the fancy shader
            _uiTarget.Bounds,
            new Rectangle(Vector2.Zero, _uiTarget.Bounds.Size.ToSysVector2()),
            Matrix.Identity,
            Color.White, gameShader, BlendState.Opaque, false);

        var bleedArea = (_tempTarget.Bounds.Size.ToSysVector2() - _graphicsDevice.Viewport.Bounds.Size.ToSysVector2());


        _graphicsDevice.SetRenderTarget(_finalTarget);
        RenderServices.DrawTextureQuad(_tempTarget,     // <=== Draws the temp buffer to the final buffer with a cheap shader
            _tempTarget.Bounds,
            new Rectangle(Vector2.Zero, _tempTarget.Bounds.Size.ToSysVector2().Multiply(_scale)),
            Matrix.Identity,
            Color.White, Game.Data.ShaderSimple, BlendState.NonPremultiplied, false);
        CreateDebugPreviewIfNecessary(BatchPreviewState.Step4, _finalTarget);

        if (_useDebugBatches)
        {
            GameLogger.Verify(_debugTarget is not null);

            // Draw all the debug stuff in the main target again
            _graphicsDevice.SetRenderTarget(_debugTarget);
            _graphicsDevice.Clear(Color.Transparent);

            Game.Data.ShaderSimple.SetTechnique("Simple");
            Game.Data.ShaderSprite.SetTechnique("DiagonalLines");
            Game.Data.ShaderSprite.SetParameter("inputTime", Game.Now);
            DebugFxBatch.End();

            Game.Data.ShaderSimple.SetTechnique("Simple");
            Game.Data.ShaderSprite.SetTechnique("Alpha");
            GetBatch(Batches2D.DebugBatchId).End();

            CreateDebugPreviewIfNecessary(BatchPreviewState.Debug, _debugTarget);
            _graphicsDevice.SetRenderTarget(_finalTarget);

            RenderServices.DrawTextureQuad(_debugTarget,     // <=== Draws the debug buffer to the final buffer
                _debugTarget.Bounds,
                new Rectangle(_subPixelOffset, _finalTarget.Bounds.Size.ToSysVector2() + _scale * CAMERA_BLEED * 2),
                Matrix.Identity,
                Color.White, Game.Data.ShaderSimple, BlendState.AlphaBlend, false);
        }

        // =======================================================>
        // Time to draw this game to the screen!!
        // =======================================================>
        _graphicsDevice.SetRenderTarget(null);
        if (RenderToScreen)
        {
            _graphicsDevice.Clear(Game.Profile.BackColor);

            if (_debugTargetPreview == null || PreviewState == BatchPreviewState.None)
            {
                Game.Data.ShaderSimple.SetTechnique("Simple");
                if (Game.Profile.EnforceResolution)
                {
                    float windowAspect = (float)_graphicsDevice.Viewport.Height / _graphicsDevice.Viewport.Width;
                    var trim = new Rectangle(0, 0, _finalTarget.Bounds.Width - CAMERA_BLEED * 2, _finalTarget.Bounds.Height - CAMERA_BLEED * 2);

                    if (windowAspect < Game.Profile.Aspect)
                    {
                        RenderServices.DrawTextureQuad(_finalTarget,
                            trim,
                            new Rectangle(
                                -(_graphicsDevice.Viewport.Height / Game.Profile.Aspect - _graphicsDevice.Viewport.Width) / 2f,
                                0,
                                _graphicsDevice.Viewport.Height / Game.Profile.Aspect,
                                _graphicsDevice.Viewport.Height),
                            Matrix.Identity, Color.White, Game.Data.ShaderSimple, BlendState.Opaque, Game.Profile.ScalingFilter);
                    }
                    else
                    {
                        RenderServices.DrawTextureQuad(_finalTarget,
                            trim,
                            new Rectangle(
                                0,
                                -(_graphicsDevice.Viewport.Width * Game.Profile.Aspect - _graphicsDevice.Viewport.Height) / 2f,
                                _graphicsDevice.Viewport.Width,
                                _graphicsDevice.Viewport.Width * Game.Profile.Aspect),
                            Matrix.Identity, Color.White, Game.Data.ShaderSimple, BlendState.Opaque, Game.Profile.ScalingFilter);
                    }
                }
                else
                {
                    RenderServices.DrawTextureQuad(_finalTarget,
                        _finalTarget.Bounds, _finalTarget.Bounds,
                        Matrix.Identity, Color.White, Game.Data.ShaderSimple, BlendState.Opaque, false);
                }
            }
            else
            {
                Game.Data.ShaderSimple.SetTechnique("Simple");
                RenderServices.DrawTextureQuad(_debugTargetPreview,
                    _debugTargetPreview.Bounds, PreviewStretch ? _finalTarget.Bounds : _debugTargetPreview.Bounds,
                    Matrix.Identity, Color.White, Game.Data.ShaderSimple, BlendState.Opaque, false);
            }
        }

        Camera.Unlock();
    }

    protected void TakeScreenshotIfNecessary(RenderTarget2D target)
    {
        if (_takeScreenShot is Rectangle screenshotArea)
        {
            Vector2 position = (Camera.WorldToScreenPosition(screenshotArea.TopLeft));
            Point size = new(screenshotArea.Width, screenshotArea.Height);

            using var screenshot = new RenderTarget2D(_graphicsDevice, size.X, size.Y);
            _graphicsDevice.SetRenderTarget(screenshot);

            RenderServices.DrawTextureQuad(target, new Rectangle(position, size * Camera.Zoom), new Rectangle(Vector2.Zero, size), Matrix.Identity, Color.White, BlendState.Opaque);

            string fileName = $"screenshot-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), fileName); // or any other directory you want to save in

            using var stream = File.OpenWrite(filePath);
            screenshot.SaveAsPng(stream, size.X, size.Y);

            // Open the directory in the file explorer
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{filePath}\"",
                    UseShellExecute = true
                });
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", $"-R \"{filePath}\"");
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", Path.GetDirectoryName(filePath)!);
            }
            else
            {
                Console.WriteLine($"File saved at {filePath}. Open manually as the OS is not recognized.");
            }
            _takeScreenShot = null;

        }
    }

    [MemberNotNull(
        nameof(_uiTarget),
        nameof(_mainTarget),
        nameof(_tempTarget),
        nameof(_debugTarget),
        nameof(_finalTarget))]
    public virtual void UpdateBufferTarget(float scale)
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

        foreach (var batch in _spriteBatches)
        {
            batch?.Dispose();
        }

        _floorBufferTarget?.Dispose();
        _uiTarget?.Dispose();
        _mainTarget?.Dispose();
        _debugTarget?.Dispose();
        _tempTarget?.Dispose();
        _finalTarget?.Dispose();
    }

    [Conditional("DEBUG")]
    public void CreateDebugPreviewIfNecessary(BatchPreviewState currentState, RenderTarget2D target)
    {
        if (PreviewState == currentState)
        {
            if (_debugTargetPreview == null || _debugTargetPreview.Bounds != target.Bounds)
            {
                _debugTargetPreview?.Dispose();
                _debugTargetPreview = new RenderTarget2D(_graphicsDevice, target.Width, target.Height);
            }

            _graphicsDevice.SetRenderTarget(_debugTargetPreview);
            _graphicsDevice.Clear(Color.Transparent);
            RenderServices.DrawTextureQuad(target,
            target.Bounds,
                target.Bounds,
                Matrix.Identity,
                Color.White, Game.Data.ShaderSimple, BlendState.NonPremultiplied, false);
        }
    }

    public void SaveScreenShot(Rectangle cameraRect)
    {
        _takeScreenShot = cameraRect;
    }
}