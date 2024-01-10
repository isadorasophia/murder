using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Diagnostics.CodeAnalysis;

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
        public virtual Task UnloadAsyncImpl() => Task.CompletedTask;

        public virtual int RefreshWindow(GraphicsDevice graphics, GameProfile settings)
        {
            GameLogger.Verify(RenderContext is not null);

            var scale = Math.Max(1, Calculator.RoundToInt((float)graphics.Viewport.Width / settings.GameWidth));
            scale = Math.Max(scale, Calculator.RoundToInt((float)graphics.Viewport.Height / settings.GameHeight));

            bool changed = RenderContext.RefreshWindow(graphics, new(
                Calculator.CeilToInt(graphics.Viewport.Width / (float)scale),
                Calculator.CeilToInt(graphics.Viewport.Height / (float)scale)
                ), scale);

            if (changed)
            {
                _onRefreshWindow?.Invoke();
            }
            GameLogger.Verify(RenderContext is not null);

            return scale;
        }

        public virtual void Start()
        {
            RefreshWindow(Game.GraphicsDevice, Game.Profile);

            World?.Start();
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

        public virtual void Draw()
        {
            if (World is null)
            {
                return;
            }

            if (!_calledStart)
            {
                return;
            }

            GameLogger.Verify(RenderContext is not null);

            OnBeforeDraw();

            RenderContext.Begin();
            World?.Draw(RenderContext);
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