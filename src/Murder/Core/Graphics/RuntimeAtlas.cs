using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Numerics;

namespace Murder.Core.Graphics;
public class RuntimeAtlas : IDisposable
{
    public readonly Vector2 Size;

    public static readonly List<RuntimeAtlas> AllLoadedAtlas = new();

    private readonly RenderTarget2D _atlasRenderTarget;
    private readonly RenderTarget2D _chunkBrush;
    private readonly Batch2D _batch;

    private readonly Point _chunkSize;
    private readonly Point _chunkCount;

    private int _nextAvailableId = 0;
    private int WrapId(int id) => Calculator.WrapAround(id, 0, _totalChunks - 1);

    private int _disposed = -1;
    private int _currentlyBatching = -1;

    private readonly int _totalChunks;

    public bool _debug = false;
    public RuntimeAtlas(string name, Point atlasSize, Point chunkSize)
    {
        // Cut down any remaining pixels from the atlas to save space
        _chunkCount = new Point(Calculator.FloorToInt(atlasSize.X / chunkSize.X), Calculator.FloorToInt(atlasSize.Y / chunkSize.Y));
        Point finalAtlasSize = chunkSize * _chunkCount;
        
        Size = new Vector2(finalAtlasSize.X, finalAtlasSize.Y);
        _chunkSize = chunkSize;

        _chunkBrush = new RenderTarget2D(Game.GraphicsDevice, chunkSize.X, chunkSize.Y);
        _chunkBrush.Name = $"{name}_brush";

        _atlasRenderTarget = new RenderTarget2D(Game.GraphicsDevice, finalAtlasSize.X, finalAtlasSize.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        _atlasRenderTarget.Name = name;
        Game.GraphicsDevice.SetRenderTarget(_atlasRenderTarget);
        Game.GraphicsDevice.Clear(Color.Transparent);

        _batch = new Batch2D("Mask",
            Game.GraphicsDevice,
            Game.Data.ShaderSprite,
            BatchMode.DepthSortDescending,
            SamplerState.PointClamp,
            DepthStencilState.None
            );

        // Initialize all possible free chunk indices
        _totalChunks = (finalAtlasSize.X / chunkSize.X) * (finalAtlasSize.Y / chunkSize.Y);
        
        AllLoadedAtlas.Add(this);
    }

    public int PlaceChunk()
    {
        if (_nextAvailableId + 1 > _totalChunks) // We have looped around
        {
            _disposed++;
        }
        FreeChunk(_nextAvailableId + 1);

        return _nextAvailableId++;
    }

    public void FreeChunk(int chunkId)
    {
        Game.GraphicsDevice.SetRenderTarget(_chunkBrush);
        Game.GraphicsDevice.Clear(Color.Transparent);
        Game.GraphicsDevice.SetRenderTarget(_atlasRenderTarget);
        
        Rectangle rect = GetRect(chunkId);

        var erase = new BlendState();
        erase.AlphaBlendFunction = BlendFunction.Min;

        RenderServices.DrawTextureQuad(
            _chunkBrush,
            _chunkBrush.Bounds,
            rect,
            Matrix.Identity,
            Color.Transparent,
            BlendState.Opaque);
    }

    public Batch2D Begin(int chunkId)
    {
        if (_currentlyBatching >= 0)
        {
            throw new Exception("Already batching a chunk!");
        }

        _currentlyBatching = chunkId;
        _batch.Begin(Matrix.Identity);

        return _batch;
    }
    public void End()
    {
        if (_currentlyBatching < 0)
        {
            throw new Exception("Not batching a chunk!");
        }

        // First we draw the chunk to the brush, this way we don't "bleed" into other chunks
        Game.GraphicsDevice.SetRenderTarget(_chunkBrush);
        Game.GraphicsDevice.Clear(Color.Transparent);
        
        if (_debug)
        {
            RenderServices.DrawText(_batch, MurderFonts.PixelFont, _currentlyBatching.ToString(), new Vector2(0, 0), new DrawInfo(0));
        }

        _batch.End();

        // Noew draw the brush to the atlas
        Game.GraphicsDevice.SetRenderTarget(_atlasRenderTarget);

        Rectangle rect = GetRect(_currentlyBatching);
        RenderServices.DrawTextureQuad(_chunkBrush, _chunkBrush.Bounds, rect, Matrix.Identity, Color.White, BlendState.AlphaBlend);

        if (_debug)
        {
            RenderServices.DrawQuadOutline(rect, Color.Gray);
        }

        _currentlyBatching = -1;
    }

    private Rectangle GetRect(int id)
    {
        int wrappedId = WrapId(id);
        return new Rectangle(
            (wrappedId % _chunkCount.X) * _chunkSize.X,
            Calculator.FloorToInt(wrappedId / (float)_chunkCount.X) * _chunkSize.Y,
            _chunkSize.X,
            _chunkSize.Y);
    }

    /// <summary>
    /// Draws a chunk to the screen and returns if it was successful
    /// </summary>
    public bool Draw(int id, Batch2D batch, Vector2 position, DrawInfo drawInfo)
    {
        if (id <= _disposed)
        {
            // Do not draw disposed tiles
            return false;
        }

        Rectangle chunkLocation = GetRect(id);
        batch.Draw(_atlasRenderTarget, 
            position.ToXnaVector2(), 
            _chunkSize, 
            chunkLocation,
            drawInfo.Sort,
            drawInfo.Rotation,
            drawInfo.Scale.ToXnaVector2(),
            drawInfo.ImageFlip,
            drawInfo.Color,
            drawInfo.Origin.ToXnaVector2(),
            drawInfo.GetBlendMode(),
            drawInfo.BlendState
        );

        return true;
    }

    public void Cleanup()
    {
        _currentlyBatching = -1;
        _nextAvailableId = 0;
        _disposed = -1;
    }

    public void Dispose()
    {
        _atlasRenderTarget.Dispose();
        _chunkBrush.Dispose();
        AllLoadedAtlas.Remove(this);
    }

    public RenderTarget2D GetFullAtlas() => _atlasRenderTarget;
    public RenderTarget2D GetBrush() => _chunkBrush;

    internal bool HasCache(int id)
    {
        return id > 0 && id > _disposed;
    }
}
