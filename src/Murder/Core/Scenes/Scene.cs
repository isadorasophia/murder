using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Core
{
    public abstract class Scene : IDisposable
    {
        /// <summary>
        /// Context renderer unique to this scene.
        /// </summary>
        public RenderContext? RenderContext { get; private set; }

        public abstract MonoWorld? World { get; }

        private bool _calledStart = false;

        [MemberNotNull(nameof(RenderContext))]
        public virtual ValueTask LoadContentAsync(GraphicsDevice graphics, GameProfile settings)
        {
            RenderContext = new RenderContext(
                graphics, 
                camera: new(settings.GameWidth, settings.GameHeight, settings.GameScale), 
                useCustomShader: true);

            return default;
        }

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

        public virtual int RefreshWindow(GraphicsDevice graphics, GameProfile settings, float downsample) 
        {
            GameLogger.Verify(RenderContext is not null);

            var scale = Calculator.RoundToInt((float)graphics.Viewport.Width / settings.GameWidth);
            scale = Math.Max(scale, Calculator.RoundToInt((float)graphics.Viewport.Height / settings.GameHeight));

            RenderContext.RefreshWindow(graphics, new(
                Calculator.RoundToInt(graphics.Viewport.Width / scale),
                Calculator.RoundToInt(graphics.Viewport.Height / scale)
                ), scale);

            return scale;
        }

        public virtual void Start()
        {
            _ = World?.Start();
            _calledStart = true;

            RefreshWindow(Game.GraphicsDevice, Game.Profile, Game.Instance.Downsample);
        }

        public virtual void Update()
        {
            if (!_calledStart)
            {
                Start();
            }

            _ = World?.Update();
        }

        public virtual void FixedUpdate()
        {
            if (!_calledStart)
            {
                Start();
            }

            _ = World?.FixedUpdate();
        }

        public virtual void Draw()
        {
            GameLogger.Verify(RenderContext is not null);

            _ = World?.PreDraw();

            RenderContext.Begin();
            _ = World?.Draw(RenderContext);
            RenderContext.End();
        }

        /// <summary>
        /// Scenes that would like to implement a Gui should use this method.
        /// </summary>
        public virtual void DrawGui()
        {
            GameLogger.Verify(RenderContext is not null);

            _ = World?.DrawGui(RenderContext);
        }

        public void Unload()
        {
            RenderContext?.Unload();
        }

        public void Dispose()
        {
            RenderContext?.Dispose();
        }
    }
}
