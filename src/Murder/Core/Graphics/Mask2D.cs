using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Core;

public class Mask2D : IDisposable
{
    public readonly Vector2 Size;

    private readonly RenderTarget2D _renderTarget;
    private RenderTarget2D? _previousRenderTarget;

    public RenderTarget2D RenderTarget => _renderTarget;
    private readonly Batch2D _batch;
    private readonly Color _color;
    public Mask2D(int width, int height, Color? color = null)
    {
        _renderTarget = new(Game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        _batch = new Batch2D("Mask",
            Game.GraphicsDevice,
            Game.Data.ShaderSprite,
            BatchMode.DepthSortDescending,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None
            );
        _color = color ?? Color.Transparent;

        Size = new(width, height);
    }

    public Mask2D(Vector2 size, Color? color = null) : this((int)size.X, (int)size.Y, color)
    {
    }

    public Batch2D Begin(bool debug = false)
    {
        if (Game.GraphicsDevice.GetRenderTargets().Length > 0)
        {
            _previousRenderTarget = (RenderTarget2D)Game.GraphicsDevice.GetRenderTargets()[0].RenderTarget;
        }

        // TODO: Should this be in the End() instead?
        SetRenderTarget();

        _batch.Begin(Matrix.Identity);

        if (debug)
        {
            _batch.DrawRectangleOutline(_renderTarget.Bounds, Color.Red);
        }

        return _batch;
    }

    private void SetRenderTarget()
    {
        Game.GraphicsDevice.SetRenderTarget(_renderTarget);
        Game.GraphicsDevice.Clear(_color);
    }

    public void End(Batch2D targetBatch, Vector2 position, Vector2 camera, DrawInfo drawInfo)
    {
        _batch.SetTransform(camera.ToXnaVector2());
        End(targetBatch, position, drawInfo);
    }

    public void End(Batch2D targetBatch, Vector2 position, DrawInfo drawInfo)
    {
        _batch.End();

        targetBatch.Draw(
            _renderTarget, 
            position.ToXnaVector2(), 
            _renderTarget.Bounds.XnaSize(), 
            _renderTarget.Bounds, 
            drawInfo.Sort,
            drawInfo.Rotation, 
            drawInfo.Scale.ToXnaVector2(), 
            drawInfo.ImageFlip, 
            drawInfo.Color,
            drawInfo.Origin.ToXnaVector2(), 
            drawInfo.GetBlendMode());

        if (_previousRenderTarget is not null)
        {
            Game.GraphicsDevice.SetRenderTarget(_previousRenderTarget);
        }
    }

    /// <summary>
    /// Ends the batch (if it is still running) and draws the render target to the target batch. If already ended, it will just draw the render target.
    /// </summary>
    public void Draw(Batch2D targetBatch, Vector2 position, DrawInfo drawInfo)
    {
        if (_batch.IsBatching)
        {
            SetRenderTarget();
            _batch.End();
            RenderServices.DrawQuadOutline(_renderTarget.Bounds, Color.Red);
        }

        targetBatch.Draw(
            _renderTarget, 
            position.ToXnaVector2(), 
            _renderTarget.Bounds.XnaSize(), 
            _renderTarget.Bounds, 
            drawInfo.Sort,
            drawInfo.Rotation, 
            drawInfo.Scale.ToXnaVector2(), 
            drawInfo.ImageFlip, 
            drawInfo.Color,
            drawInfo.Origin.ToXnaVector2(), 
            drawInfo.GetBlendMode());
    }

    public bool IsDisposed => _batch.IsDisposed;

    public void Dispose()
    {
        _renderTarget.Dispose();
        _batch.Dispose();
    }
}