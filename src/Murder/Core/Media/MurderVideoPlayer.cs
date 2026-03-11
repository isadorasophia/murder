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
using static Murder.Core.Graphics.RenderContext;

namespace Murder.Media;

public class MurderVideoPlayer : IDisposable
{
    [JsonIgnore]
    private VideoPlayer? _videoPlayer = null;

    private Point _finalSize = Point.Zero;

    private readonly RenderContext _render;

    private readonly string _path;
    private readonly Point _videoSize;

    public Color FadeColor = Color.Green;
    public float? FadeStart = null;
    public float FadeDuration = 1f;
    private float FadeDelta => FadeStart != null ? (Game.NowUnscaled - FadeStart.Value) / FadeDuration : 0;
    public bool IsDone => _videoPlayer is null || _videoPlayer.State == MediaState.Stopped || FadeDelta >= 1;

    public bool Started { get; private set; }

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

        _videoPlayer.Volume = Game.Data.Preferences.AllVolume;
        _videoPlayer.IsLooped = false;
        _videoPlayer.Play(video);

        _render.DoNotRender = true;
    }

    public void OnDraw(Point size)
    {
        if (_videoPlayer is null)
        {
            return;
        }

        Started = true;
        Texture2D videoTexture = _videoPlayer.GetTexture();

        Game.GraphicsDevice.SetRenderTarget(null);
        Game.GraphicsDevice.Clear(Color.Black);

        {
            _finalSize = size;

            // Scale to fill the screen while keeping aspect ratio.
            // float scale = MathF.Max((float)cameraSize.X / _videoSize.X, (float)cameraSize.Y / _videoSize.Y);

            Rectangle source = new(0, 0, _videoSize.X, _videoSize.Y);
            Rectangle destination = new(0, 0, _finalSize.X, _finalSize.Y);

            var (v, i) = RenderServices.GetSharedQuad(destination, source, new Vector2(source.Width, source.Height), Color.White, RenderServices.BLEND_NORMAL);
            RenderServices.DrawIndexedVertices(
                Game.GraphicsDevice, v, v.Length, i, i.Length / 3, Game.Data.ShaderSprite,
                BlendState.AlphaBlend,
                videoTexture,
                true);

        }

        if (FadeStart != null)
        {
            // same thing but with a black texture, used for fading out the video.
            Rectangle source = new(0, 0, _videoSize.X, _videoSize.Y);
            Rectangle destination = new(0, 0, _finalSize.X, _finalSize.Y);
            Color color = FadeColor * FadeDelta;

            var (v, i) = RenderServices.GetSharedQuad(destination, Rectangle.One, Vector2.One, color, RenderServices.BLEND_COLOR_ONLY);

            RenderServices.DrawIndexedVertices(
                Game.GraphicsDevice, v, v.Length, i!, i!.Length / 3, Game.Data.ShaderSprite,
                BlendState.AlphaBlend,
                videoTexture,
                true);
        }
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
