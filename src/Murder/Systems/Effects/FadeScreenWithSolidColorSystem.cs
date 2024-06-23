using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Services;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Systems
{
    /// <summary>
    /// System responsible for fading in and out entities.
    /// This is not responsible for the screen fade transition.
    /// </summary>
    [Filter(typeof(FadeScreenWithSolidColorComponent))]
    [Watch(typeof(FadeScreenWithSolidColorComponent))]
    [DoNotPause]
    public class FadeScreenWithSolidColorSystem : IUpdateSystem, IReactiveSystem, IMurderRenderSystem
    {
        private float _fadeInTime = -1;
        private float _fadeOutTime = -1;

        private Color _color;
        private float _duration;

        private float _currentAlpha = 0;
        private float _currentSort = .05f;

        private int _targetBatch = Batches2D.UiBatchId;

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            OnModified(world, entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities) 
        {
            _fadeInTime = -1;
            _fadeOutTime = -1;

            foreach (Entity e in entities)
            {
                FadeScreenWithSolidColorComponent f = e.GetFadeScreenWithSolidColor();
                switch (f.FadeType)
                {
                    case FadeType.In:
                        GameLogger.Log("Received fade in.");

                        _fadeInTime = Game.NowUnscaled;
                        break;

                    case FadeType.Out:
                        GameLogger.Log("Received fade out.");

                        _fadeOutTime = Game.NowUnscaled;
                        break;
                }

                _color = f.Color;
                _duration = f.Duration;
                _currentSort = f.Sort;
                _targetBatch = f.BatchId;
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

        public void Update(Context context)
        {
            if (_fadeInTime == -1 && _fadeOutTime == -1)
            {
                return;
            }

            if (_fadeInTime != -1)
            {
                _currentAlpha = Math.Min(Game.NowUnscaled - _fadeInTime, _duration) / _duration;

                if (_currentAlpha == 1)
                {
                    _fadeInTime = -1;
                }
            }

            if (_fadeOutTime != -1)
            {
                _currentAlpha = 1 - Math.Min(Game.NowUnscaled - _fadeOutTime, _duration) / _duration;

                if (_currentAlpha == 0)
                {
                    _fadeOutTime = -1;
                }
            }
        }

        public void Draw(RenderContext render, Context context)
        {
            if (_currentAlpha == 0)
            {
                return;
            }

            RenderServices.DrawRectangle(
                render.GetBatch(_targetBatch),
                new Rectangle(Vector2.Zero, render.Camera.SafeBounds.Size),
                _color * _currentAlpha,
                _currentSort);
        }
    }
}