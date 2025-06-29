using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Threading.Channels;

namespace Murder.Core
{
    public abstract class Scene : IDisposable
    {
        /// <summary>
        /// Context renderer unique to this scene.
        /// </summary>
        public RenderContext? RenderContext { get; private set; }

        public bool Loaded { get; private set; }

        public abstract MonoWorld? World { get; }

        protected bool _calledStart = false;

        /// <summary>
        /// Used to track events when the window (UI) refreshes.
        /// </summary>
        private Action? _onRefreshWindow = default;

        [MemberNotNull(nameof(RenderContext))]
        public virtual void Initialize(GraphicsDevice graphics, GameProfile settings, RenderContextFlags flags)
        {
            RenderContext = Game.Instance.CreateRenderContext(
                graphics,
                camera: new(settings.GameWidth, settings.GameHeight),
                flags);
        }

        public void LoadContent()
        {
            LoadContentImpl();

            _calledStart = false;
            Loaded = true;
        }

        public virtual void LoadContentImpl() { }

        /// <summary>
        /// Reload the active scene.
        /// </summary>
        public void Reload()
        {
            _calledStart = false;

            ReloadImpl();
        }

        public void Resume() { }

        /// <summary>
        /// Rests the current scene temporarily.
        /// </summary>
        public void Suspend() { }

        public virtual void ReloadImpl() { }
        public virtual void ResumeImpl() { }
        public virtual void SuspendImpl() { }
        protected virtual Task UnloadAsyncImpl() => Task.CompletedTask;

        /// <summary>
        /// Refresh the window size, updating the camera and render context.
        /// </summary>
        /// <param name="viewportSize"></param>
        /// <param name="graphics"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual void RefreshWindow(Point viewportSize, GraphicsDevice graphics, GameProfile settings)
        {
            GameLogger.Verify(RenderContext is not null, "RenderContext should not be null at this point.");

            Point nativeResolution = new Point(settings.GameWidth, settings.GameHeight);
            ViewportResizeStyle viewportResizeMode = settings.ResizeStyle;

            bool changed = RenderContext.RefreshWindow(graphics, viewportSize, nativeResolution, viewportResizeMode);

            if (changed)
            {
                _onRefreshWindow?.Invoke();
            }
        }

        public virtual void Start()
        {
            World?.Start();

            // Since the viewport might be some other texture, we need to reset it to the main render target so we can measure it.
            Game.GraphicsDevice.SetRenderTarget(null);
            RefreshWindow(new Point(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Game.GraphicsDevice, Game.Profile);
            _calledStart = true;
        }

        public virtual void Update()
        {
            if (!_calledStart)
            {
                Start();
            }

            World?.Update();
        }

        public virtual void FixedUpdate()
        {
            if (!_calledStart)
            {
                Start();
            }

            World?.FixedUpdate();
        }

        public virtual void OnBeforeDraw()
        {
            if (World is null)
            {
                return;
            }

            if (!_calledStart)
            {
                return;
            }

            World?.PreDraw();
        }

        public virtual bool DrawStart()
        {
            if (World is null)
            {
                return false;
            }

            if (!_calledStart)
            {
                return false;
            }

            GameLogger.Verify(RenderContext is not null);

            OnBeforeDraw();

            RenderContext.Begin();
            World?.Draw(RenderContext);
            return true;
        }

        public void DrawEnd()
        {
            GameLogger.Verify(RenderContext is not null);
            RenderContext.End();
        }

        /// <summary>
        /// Scenes that would like to implement a Gui should use this method.
        /// </summary>
        public virtual void DrawGui()
        {
            GameLogger.Verify(RenderContext is not null);

            if (!_calledStart)
            {
                return;
            }

            World?.DrawGui(RenderContext);
        }

        /// <summary>
        /// This will trigger UI refresh operations.
        /// </summary>
        public void AddOnWindowRefresh(Action notification)
        {
            _onRefreshWindow += notification;
        }

        /// <summary>
        /// This will reset all watchers of trackers.
        /// </summary>
        public void ResetWindowRefreshEvents()
        {
            _onRefreshWindow = null;
        }

        public void Unload()
        {
            RenderContext?.Unload();
            ResetWindowRefreshEvents();

            _ = UnloadAsyncImpl();
        }

        public void Dispose()
        {
            RenderContext?.Dispose();
        }
    }
}