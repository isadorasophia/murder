using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Numerics;

namespace Murder.Core;

public class Mask2D : IDisposable
{
    public Point Size { get; private set; }

    private RenderTarget2D _renderTarget;
    private RenderTarget2D? _previousRenderTarget;
    private bool _debug;

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
            SamplerState.PointClamp,
            DepthStencilState.None
            );
        _color = color ?? Color.Transparent;

        Size = new(width, height);
    }
    public Mask2D(Vector2 size, Color? color = null) : this((int)size.X, (int)size.Y, color)
    {
    }
    public Mask2D(Point size, Color? color = null) : this((int)size.X, (int)size.Y, color)
    {
    }

    public void Resize(Point size)
    {
        if (size == Size)
        {
            return; // No need to resize
        }

        if (size.X <= 0 || size.Y <= 0)
        {
            throw new ArgumentException("Size must be greater than zero.");
        }
        if (_renderTarget.Width == size.X && _renderTarget.Height == size.Y)
        {
            return; // No need to resize
        }

        if (_renderTarget.Width < size.X || _renderTarget.Height < size.Y)
        {
            _renderTarget.Dispose();
            _renderTarget = new RenderTarget2D(Game.GraphicsDevice, (int)size.X, (int)size.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        Size = size;
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
        _debug = debug;

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

    /// <summary>
    /// Stops the current batch and resets the render target to the previous one, if any. Does NOT draw the render target to any batch.
    /// </summary>
    public void Stop()
    {
        if (_batch.IsBatching)
        {
            if (_debug)
            {
                _batch.DrawRectangleOutline(new Rectangle(0, 0, Size.X, Size.Y), Color.Red);
            }
            _batch.End();
        }
        if (_previousRenderTarget is not null)
        {
            Game.GraphicsDevice.SetRenderTarget(_previousRenderTarget);
            _previousRenderTarget = null;
        }
    }

    /// <summary>
    /// Ends the current batch and draws the render target to the target batch at the specified position with the given draw info.
    /// </summary>
    public void End(Batch2D targetBatch, Vector2 position, DrawInfo drawInfo)
    {
        if (_batch.IsBatching)
        {
            if (_debug)
            {
                _batch.DrawRectangleOutline(new Rectangle(0, 0, Size.X, Size.Y), Color.Red);
            }
            _batch.End();
        }

        targetBatch.Draw(
            _renderTarget,
            position.ToXnaVector2(),
            new Microsoft.Xna.Framework.Vector2(Size.X, Size.Y),
            new Microsoft.Xna.Framework.Rectangle(0, 0, Size.X, Size.Y),
            drawInfo.Sort,
            drawInfo.Rotation,
            drawInfo.Scale.ToXnaVector2(),
            drawInfo.ImageFlip,
            drawInfo.Color,
            drawInfo.Origin.ToXnaVector2(),
            drawInfo.GetBlendMode(),
            drawInfo.BlendState);

        if (_previousRenderTarget is not null)
        {
            Game.GraphicsDevice.SetRenderTarget(_previousRenderTarget);
            _previousRenderTarget = null;
        }
    }

    public void Dispose()
    {
        _renderTarget.Dispose();
    }
}