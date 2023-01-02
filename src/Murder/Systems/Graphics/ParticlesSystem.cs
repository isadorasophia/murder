using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Particles;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;

namespace Murder.Systems
{
    [Filter(typeof(ParticleTrackerComponent))]
    public class ParticlesSystem : IFixedUpdateSystem, IMonoRenderSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                e.GetParticleTracker().Tracker.Step(Game.FixedDeltaTime);
            }
        }
        
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                ParticleTracker tracker = e.GetParticleTracker().Tracker;
                ParticleTexture texture = tracker.Particle.Texture;

                AsepriteAsset? asset = default;
                string? animationId = default;
                
                if (texture.Kind == ParticleTextureKind.Asset)
                {
                    asset = Game.Data.TryGetAsset<AsepriteAsset>(texture.Asset);
                    if (asset is null)
                    {
                        // Unable to find asset for particle?
                        return;
                    }

                    animationId = asset.Animations.First().Key;
                }

                foreach (ParticleRuntime particle in tracker.Particles)
                {
                    float delta = Calculator.Clamp01((tracker.Lifetime - particle.StartTime) / particle.Lifetime);
                    Color color = tracker.Particle.CalculateColor(delta);

                    switch (texture.Kind)
                    {
                        case ParticleTextureKind.Point:
                            RenderServices.DrawPoint(
                                render.GameplayBatch,
                                particle.Position.Point,
                                color);

                            break;

                        case ParticleTextureKind.Rectangle:
                            Rectangle rectangle = texture.Rectangle.AddPosition(particle.Position);

                            RenderServices.DrawRectangle(
                                render.GameplayBatch,
                                rectangle,
                                color);
                            break;

                        case ParticleTextureKind.Circle:
                            RenderServices.DrawCircle(
                                render.GameplayBatch,
                                particle.Position.ToPoint(),
                                texture.Circle.Radius,
                                sides: 12,
                                color);
                            break;

                        case ParticleTextureKind.Asset:
                            Debug.Assert(asset is not null && animationId is not null);

                            RenderServices.RenderSprite(
                                render.GetSpriteBatch(TargetSpriteBatches.Gameplay),
                                render.Camera,
                                particle.Position,
                                animationId,
                                asset,
                                particle.StartTime,
                                animationDuration: -1,
                                offset: Vector2.Zero,
                                flipped: false,
                                rotation: particle.Rotation,
                                scale: Vector2.One,
                                color,
                                blend: RenderServices.BLEND_NORMAL,
                                sort: 1);
                                
                            break;
                    }
                }
            }
        }
    }
}
