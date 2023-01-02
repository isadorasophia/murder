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

namespace Murder.Systems
{
    [ParticleEditor]
    [Filter(typeof(ParticleTrackerComponent))]
    public class DebugParticlesSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                Vector2 position = e.GetGlobalTransform().Vector2;
                
                ParticleTracker tracker = e.GetParticleTracker().Tracker;
                EmitterShape texture = tracker.Emitter.Shape;
                
                switch (texture.Kind)
                {
                    case EmitterShapeKind.Point:
                        RenderServices.DrawPoint(
                            render.GameplayBatch,
                            position.Point,
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
                            texture.Line.PointA + position.Point,
                            texture.Line.PointB + position.Point,
                            Color.BrightGray);
                        break;
                        
                    case EmitterShapeKind.Circle:
                        RenderServices.DrawCircle(
                            render.GameplayBatch,
                            position.Point,
                            texture.Circle.Radius,
                            sides: 12,
                            Color.BrightGray);
                        break;
                }
            }
        }
    }
}
