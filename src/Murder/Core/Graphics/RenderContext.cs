using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Murder.Core.Graphics;

public class RenderContext : IDisposable
{
    /// <summary>
    /// The active camera used for rendering scenes.
    /// </summary>
    public readonly Camera2D Camera;

    protected readonly RenderContextFlags Settings;

    /// <summary>
    /// Intended to be the main gameplay batch, influenced by the camera.
    /// </summary>
    public Batch2D GameplayBatch => GetBatch(Batches2D.GameplayBatchId);
    /// <summary>
    /// Renders behind the <see cref="GameplayBatch"/>, influenced by the camera.
    /// </summary>
    public Batch2D FloorBatch => GetBatch(Batches2D.FloorBatchId);
    /// <summary>
    /// Renders in front of the <see cref="GameplayBatch"/>, influenced by the camera.
    /// </summary>
    public Batch2D GameUiBatch => GetBatch(Batches2D.GameUiBatchId);
    /// <summary>
    /// Renders above everything, ignores any camera movement.
    /// </summary>
    public Batch2D UiBatch => GetBatch(Batches2D.UiBatchId);

    public Vector2 SubPixelOffset => _subPixelOffset;
    protected Vector2 _subPixelOffset;

    /// <summary>
    /// Only used if <see cref="RenderContextFlags.Debug"/> is set, has a nice effect to it. Influenced by the camera.
    /// </summary>
    public Batch2D DebugFxBatch => GetBatch(Batches2D.DebugFxBatchId);

    /// <summary>
    /// Only used if <see cref="RenderContextFlags.Debug"/> is set. Influenced by the camera.
    /// </summary>
    public Batch2D DebugBatch => GetBatch(Batches2D.DebugBatchId);

    protected RenderTarget2D? _floorBufferTarget;
    protected RenderTarget2D? _uiTarget;
    protected RenderTarget2D? _mainTarget;
    
    public RenderTarget2D? MainTarget => _mainTarget;

    public BatchPreviewState PreviewState;
    public bool PreviewStretch;

    public Viewport Viewport = new();

    /// <summary>
    /// Not used by the base RenderContext, but can be used by derived classes.
    /// </summary>
    public float ScreenFade = 0;

    public enum BatchPreviewState
    {
        None,
        Step1,
        Step2,
        Step3,
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

    public Batch2D[] _spriteBatches = new Batch2D[32];

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

    public Color BackColor => Game.Data.GameProfile.BackColor;

    public Texture2D? ColorGrade;

    protected readonly bool _useDebugBatches;

    private Rectangle? _takeScreenShot;
    private bool _initialized = false;
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
        Settings = settings;

        _useDebugBatches = settings.HasFlag(RenderContextFlags.Debug);
        _graphicsDevice = graphicsDevice;

        Initialize();
    }

    public virtual Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
    {
        return inspectingRenderTarget switch
        {
            RenderTargets.MainBufferTarget => _mainTarget,
            RenderTargets.FinalTarget => _finalTarget,
            RenderTargets.UiTarget => _uiTarget,
            _ => default
        } ?? throw new ArgumentException($"Unable to find a render target for {inspectingRenderTarget}.");
    }

    public virtual void Initialize()
    {
        if (_initialized)
        {
            GameLogger.Warning("RenderContext already initialized.");
            return;
        }
        _initialized = true;

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

    /// <summary>
    /// Sets up a new RenderTarget2D, disposing of the existing one if necessary.
    /// </summary>
    /// <param name="existingTarget">The existing RenderTarget2D to be disposed.</param>
    /// <param name="width">Width of the new RenderTarget2D.</param>
    /// <param name="height">Height of the new RenderTarget2D.</param>
    /// <param name="clearColor">Clear color for the new RenderTarget2D.</param>
    /// <param name="preserveContents">Whether to preserve the contents in the new RenderTarget2D.</param>
    /// <returns>A new instance of RenderTarget2D.</returns>
    protected RenderTarget2D SetupRenderTarget(RenderTarget2D? existingTarget, int width, int height, Color clearColor, bool preserveContents)
    {
        existingTarget?.Dispose();

        var target = new RenderTarget2D(
            _graphicsDevice,
            width,
            height,
            mipMap: false,
            SurfaceFormat.Color,
            DepthFormat.Depth16,
            0,
            preserveContents ? RenderTargetUsage.PreserveContents : RenderTargetUsage.DiscardContents
        );
        _graphicsDevice.SetRenderTarget(target);
        _graphicsDevice.Clear(clearColor);
        return target;
    }

    /// <summary>
    /// Registers a SpriteBatch at a specified index. If the index is already taken, it will be overwritten.
    /// </summary>
    /// <remarks>
    /// This will automatically trigger a <see cref="Batch2D.Begin(Matrix)"/> call on <see cref="Begin"/>, but will not trigger an <see cref="Batch2D.End()"/>.
    /// For now you need to extend <see cref="Batch2D.End()"/> and include it yourself.
    /// </remarks>
    /// <param name="index">The index at which to register the SpriteBatch.</param>
    /// <param name="batch">The SpriteBatch to register.</param>
    protected void RegisterSpriteBatch(int index, Batch2D batch)
    {
        // Resize _spriteBatches array if index is out of bounds
        if (index >= _spriteBatches.Length)
        {
            Batch2D[] newSpriteBatches = new Batch2D[index + 4];
            _spriteBatches.CopyTo(newSpriteBatches, 0);
            _spriteBatches = newSpriteBatches;
        }

        // Warn if overwriting an existing SpriteBatch
        if (_spriteBatches[index] != null)
        {
            GameLogger.Warning($"Sprite batch {_spriteBatches[index].Name} is being overwritten by {batch.Name}. Was this on purpose?");
        }

        // Assign the new SpriteBatch to the specified index
        _spriteBatches[index] = batch;
    }


    /// <summary>
    /// Refreshes the window with the new viewport size and camera scale.
    /// </summary>
    /// <returns>
    /// Whether the window actually required a refresh.
    /// </returns>
    public bool RefreshWindow(GraphicsDevice graphicsDevice, Point viewportSize, Point nativeResolution, ViewportResizeStyle viewportResizeMode)
    {
        // No changes, skip
        if (!Viewport.HasChanges(viewportSize, nativeResolution))
        {
            return false;
        }
        
        _graphicsDevice = graphicsDevice;
        Viewport = new Viewport(viewportSize, nativeResolution, viewportResizeMode);

        Camera.UpdateSize(Viewport.NativeResolution);
        UpdateViewport();

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

    /// <summary>
    /// Called right after <see cref="FloorBatch"/>, <see cref="GameplayBatch"/> and <see cref="GameUiBatch"/> end.
    /// </summary>
    /// <param name="mainTarget">Main target with the main game drawn in it. This target's size matches the game resolution.</param>
    protected virtual void AfterMainRender(RenderTarget2D mainTarget) { }

    /// <summary>
    /// Called right after the <see cref="UiBatch"/> ends.
    /// </summary>
    /// <param name="uiTarget">UI target with the game UI drawn in it. This target's size matches the game resolution.</param>
    protected virtual void AfterUiRender(RenderTarget2D uiTarget) { }

    /// <summary>
    /// Last chance to render anything before the contents are drawn on the screen!
    /// </summary>
    /// <param name="finalTarget">Final target containing everything that will be rendered on screen. This target's size is the size of the actual game window, since this is what will be rendered on screen.</param>
    protected virtual void BeforeScreenRender(RenderTarget2D finalTarget) { }

    public virtual void End()
    {
        GameLogger.Verify(
            _uiTarget is not null &&
            _mainTarget is not null &&
            _tempTarget is not null &&
            _finalTarget is not null,
            "Did not initialize buffer targets before calling RenderContext.End()?");


        // Setup the basic shader parameters

        Game.Data.ShaderPixel?.TrySetParameter("viewportSize", (_tempTarget.Bounds.Size() * Viewport.Scale).ToXnaVector2());
        Game.Data.ShaderPixel?.TrySetParameter("texelsScale", (Vector2.One / Viewport.Scale).ToXnaVector2());

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

        AfterMainRender(_mainTarget);

        _graphicsDevice.SetRenderTarget(_uiTarget);
        _graphicsDevice.Clear(Color.Transparent);

        UiBatch.End();              // <=== Static Ui

        CreateDebugPreviewIfNecessary(BatchPreviewState.Ui, _uiTarget);

        AfterUiRender(_uiTarget);

        _graphicsDevice.SetRenderTarget(_finalTarget);

        _subPixelOffset = new Vector2(
            Camera.Position.Point().X - Camera.Position.X - CAMERA_BLEED / 2,
            Camera.Position.Point().Y - Camera.Position.Y - CAMERA_BLEED / 2) * Viewport.Scale;

        _graphicsDevice.SetRenderTarget(_finalTarget);
        
        Game.Data.ShaderPixel?.TrySetParameter("viewportSize", (_mainTarget.Bounds.Size() * Viewport.Scale).ToXnaVector2());
        Game.Data.ShaderPixel?.TrySetParameter("textureSize", _mainTarget.Bounds.Size());
        RenderServices.DrawTextureQuad(_mainTarget,     // <=== Draws the game buffer to the final buffer using a optimized pixel shader
            _mainTarget.Bounds,
            new Rectangle(_subPixelOffset, _mainTarget.Bounds.Size() * Viewport.Scale),
            Matrix.Identity,
            Color.White, Game.Data.ShaderPixel, BlendState.Opaque, true);

        CreateDebugPreviewIfNecessary(BatchPreviewState.Step2, _finalTarget);
        
        _graphicsDevice.SetRenderTarget(_finalTarget);

        Game.Data.ShaderPixel?.TrySetParameter("viewportSize", (_uiTarget.Bounds.Size() * Viewport.Scale).ToXnaVector2());
        Game.Data.ShaderPixel?.TrySetParameter("textureSize", _uiTarget.Bounds.Size());
        RenderServices.DrawTextureQuad(_uiTarget,     // <=== Draws the ui buffer to the final buffer using a optimized pixel shader
            _uiTarget.Bounds,
            new Rectangle(Vector2.Zero, _uiTarget.Bounds.Size() * Viewport.Scale),
            Matrix.Identity,
            Color.White, Game.Data.ShaderPixel, BlendState.AlphaBlend, true);

        CreateDebugPreviewIfNecessary(BatchPreviewState.Step3, _finalTarget);

        if (_useDebugBatches)
        {
            GameLogger.Verify(_debugTarget is not null);

            // Draw all the debug stuff in the main target again
            _graphicsDevice.SetRenderTarget(_debugTarget);
            _graphicsDevice.Clear(Color.Transparent);

            Game.Data.ShaderSimple?.SetTechnique("DefaultTechnique");
            Game.Data.ShaderSprite?.SetTechnique("DiagonalLines");
            Game.Data.ShaderSprite?.SetParameter("inputTime", Game.Now);
            DebugFxBatch.End();

            Game.Data.ShaderSimple?.SetTechnique("DefaultTechnique");
            Game.Data.ShaderSprite?.SetTechnique("Alpha");
            GetBatch(Batches2D.DebugBatchId).End();

            CreateDebugPreviewIfNecessary(BatchPreviewState.Debug, _debugTarget);
            _graphicsDevice.SetRenderTarget(_finalTarget);

            RenderServices.DrawTextureQuad(_debugTarget,     // <=== Draws the debug buffer to the final buffer
                _debugTarget.Bounds,
                new Rectangle(_subPixelOffset, _finalTarget.Bounds.Size() + Viewport.Scale * CAMERA_BLEED * 2),
                Matrix.Identity,
                Color.White, Game.Data.ShaderSimple, BlendState.AlphaBlend, false);
        }
        BeforeScreenRender(_finalTarget);

        // =======================================================>
        // Time to draw this game to the screen!!
        // =======================================================>
        _graphicsDevice.SetRenderTarget(null);

        if (RenderToScreen)
        {
            _graphicsDevice.Clear(Game.Profile.BackColor);

            // Draw the game normally
            if (_debugTargetPreview == null || PreviewState == BatchPreviewState.None)
            {
                // Draw the final buffer to the viewport output rectangle
                RenderServices.DrawTextureQuad(_finalTarget,
                    _finalTarget.Bounds, 
                    Viewport.OutputRectangle,
                    Matrix.Identity, Color.White, Game.Data.ShaderSimple, BlendState.Opaque, false);
            }
            // We are in preview mode, draw the preview buffer instead
            else
            {
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
    public virtual void UpdateViewport()
    {
        _uiTarget = SetupRenderTarget(_uiTarget, Camera.Width, Camera.Height, Color.Transparent, false);
        _mainTarget = SetupRenderTarget(_mainTarget, Camera.Width + CAMERA_BLEED * 2, Camera.Height + CAMERA_BLEED * 2, BackColor, true);
        _tempTarget = SetupRenderTarget(_tempTarget, Camera.Width + CAMERA_BLEED * 2, Camera.Height + CAMERA_BLEED * 2, Color.Transparent, true);
        _debugTarget = SetupRenderTarget(_debugTarget, Camera.Width + CAMERA_BLEED * 2, Camera.Height + CAMERA_BLEED * 2, BackColor, true);
        _finalTarget = SetupRenderTarget(_finalTarget, Viewport.OutputRectangle.Size.X + CAMERA_BLEED, Viewport.OutputRectangle.Size.Y + CAMERA_BLEED, BackColor, true);

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
        UnloadImpl();

        _graphicsDevice.SetRenderTarget(null);

        // if we do that it will cause an issue while resizing the window.
        // Game.GraphicsDevice.Reset();
    }

    /// <summary>
    /// Override for custom unload implementations in derived classes.
    /// </summary>
    protected virtual void UnloadImpl() { }
    
    /// <summary>
    /// Disposes of all associated resources.
    /// </summary>
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
    
    /// <summary>
    /// Saves a screenshot of the specified camera area.
    /// </summary>
    /// <param name="cameraRect">Area of the camera to capture.</param>
    public void SaveScreenShot(Rectangle cameraRect)
    {
        _takeScreenShot = cameraRect;
    }
}