using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
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
        public void Draw(RenderContext render, Context context)
        {
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
                            Color.BrightGray);

                        break;

                    case EmitterShapeKind.Rectangle:
                        Rectangle rectangle = texture.Rectangle.AddPosition(position);

                        RenderServices.DrawRectangleOutline(
                            render.GameplayBatch,
                            rectangle,
                            Color.BrightGray);
                        break;

                    case EmitterShapeKind.Line:
                        RenderServices.DrawLine(
                            render.GameplayBatch,
                            texture.Line.Start + position,
                            texture.Line.End + position,
                            Color.BrightGray);
                        break;

                    case EmitterShapeKind.Circle:
                        RenderServices.DrawCircleOutline(
                            render.GameplayBatch,
                            position,
                            texture.Circle.Radius,
                            sides: 12,
                            Color.BrightGray);
                        break;

                    case EmitterShapeKind.CircleOutline:
                        RenderServices.DrawCircleOutline(
                            render.GameplayBatch,
                            position,
                            texture.Circle.Radius,
                            sides: 12,
                            Color.BrightGray);
                        break;
                }
            }
        }
    }
}