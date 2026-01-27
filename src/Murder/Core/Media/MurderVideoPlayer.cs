using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Murder.Assets;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Services;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Murder.Media;

public class MurderVideoPlayer : IDisposable
{
    [JsonIgnore]
    private VideoPlayer? _videoPlayer = null;

    private VertexInfo[]? _verts = null;
    private short[]? _indices = null;
    private Point _finalSize = Point.Zero; 

    private readonly RenderContext _render;

    private readonly string _path;
    private readonly Point _videoSize;

    public bool IsDone => _videoPlayer is null || _videoPlayer.State == MediaState.Stopped;

    public bool Started => _verts is not null;

    public Point FinalSize => _finalSize;

    /// <param name="render">Render context</param>
    /// <param name="path">Path relative to <see cref="GameProfile.VideoPath"/></param>
    /// <param name="size">Size of the video</param>
    public MurderVideoPlayer(RenderContext render, string path, Point size)
    {
        _render = render;

        _path = path;
        _videoSize = size;

        _finalSize = _videoSize;
    }

    public void Play()
    {
        string fullPath = FileHelper.GetPath(Game.Data.BinResourcesDirectoryPath, Game.Profile.VideoPath, _path);
        if (!File.Exists(fullPath))
        {
            GameLogger.Error($"Unable to play video from {fullPath}, invalid path!");
            return;
        }

        Video video = Video.FromUriEXT(uri: new(fullPath, UriKind.Absolute), Game.GraphicsDevice);

        _videoPlayer = new();
        _videoPlayer.IsLooped = false;
        _videoPlayer.Play(video);

        _render.DoNotRender = true;
    }

    public void OnDraw(RenderContext render)
    {
        if (_videoPlayer is null)
        {
            return;
        }

        Texture2D videoTexture = _videoPlayer.GetTexture();

        Game.GraphicsDevice.SetRenderTarget(null);
        Game.GraphicsDevice.Clear(Color.Black);

        if (_verts is null)
        {
            _finalSize = render.Viewport.Size;

            // Scale to fill the screen while keeping aspect ratio.
            // float scale = MathF.Max((float)cameraSize.X / _videoSize.X, (float)cameraSize.Y / _videoSize.Y);

            Rectangle source = new(0, 0, _videoSize.X, _videoSize.Y);
            Rectangle destination = new(0, 0, _finalSize.X, _finalSize.Y);

            (_verts, _indices) = RenderServices.MakeTexturedQuad(destination, source, new Vector2(source.Width, source.Height), Color.White, RenderServices.BLEND_NORMAL);
        }

        RenderServices.DrawIndexedVertices(
            Game.GraphicsDevice, _verts, _verts.Length, _indices!, _indices!.Length / 3, Game.Data.ShaderSprite,
            BlendState.Opaque,
            videoTexture,
            true);
    }

    public void StopAndDispose()
    {
        Dispose();
    }

    public void Dispose()
    {
        _render.DoNotRender = false;

        _videoPlayer?.Dispose();
        _videoPlayer = null;
    }
}
