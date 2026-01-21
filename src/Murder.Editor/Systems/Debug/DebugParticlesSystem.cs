using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Microsoft.Xna.Framework.Input;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Particles;
using Murder.Editor.Attributes;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems
{
    [EditorSystem]
    [ParticleEditor]
    [WorldEditor(startActive: true)]
    [Filter(typeof(ParticleSystemWorldTrackerComponent))]
    public class DebugParticlesSystem : IMurderRenderSystem
    {
        private static float _lastSpaceBarTime = 0;
        public void Draw(RenderContext render, Context context)
        {
            float hideAll;
            if (Game.Input.Pressed(Keys.Space))
            {
                _lastSpaceBarTime = Game.NowUnscaled;
            }
            if (Game.Input.Down(Keys.Space))
            {
                hideAll = 1 - Calculator.ClampTime(Game.NowUnscaled - _lastSpaceBarTime, .2f);
            }
            else if (Game.Input.Released(Keys.Space))
            {
                _lastSpaceBarTime = Game.NowUnscaled;
                hideAll = 0;
            }
            else
            {
                hideAll = Calculator.ClampTime(Game.NowUnscaled - _lastSpaceBarTime, 0.1f);
            }
            if (hideAll <= 0)
            {
                return;
            }

            WorldParticleSystemTracker worldTracker = context.Entity.GetParticleSystemWorldTracker().Tracker;

            foreach (ParticleSystemTracker tracker in worldTracker.FetchActiveParticleTrackers())
            {
                Vector2 position = tracker.LastEmitterPosition;
                EmitterShape texture = tracker.Emitter.Shape;

                switch (texture.Kind)
                {
                    case EmitterShapeKind.Point:
                        RenderServices.DrawPoint(
                            render.GameplayBatch,
                            position.Point(),
                            Color.BrightGray * hideAll);

                        break;

                    case EmitterShapeKind.Rectangle:
                        Rectangle rectangle = texture.Rectangle.AddPosition(position);

                        RenderServices.DrawRectangleOutline(
                            render.GameplayBatch,
                            rectangle,
                            Color.BrightGray * hideAll);
                        break;

                    case EmitterShapeKind.Line:
                        RenderServices.DrawLine(
                            render.GameplayBatch,
                            texture.Line.Start + position,
                            texture.Line.End + position,
                            Color.BrightGray * hideAll);
                        break;

                    case EmitterShapeKind.Circle:
                        RenderServices.DrawCircleOutline(
                            render.GameplayBatch,
                            position,
                            texture.Circle.Radius,
                            sides: 12,
                            Color.BrightGray * hideAll);
                        break;

                    case EmitterShapeKind.CircleOutline:
                        RenderServices.DrawCircleOutline(
                            render.GameplayBatch,
                            position,
                            texture.Circle.Radius,
                            sides: 12,
                            Color.BrightGray * hideAll);
                        break;
                }
            }
        }
    }
}