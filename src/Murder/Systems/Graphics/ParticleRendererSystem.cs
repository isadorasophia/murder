using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Particles;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;

namespace Murder.Systems
{
    [Filter(typeof(ParticleSystemWorldTrackerComponent))]
    public class ParticleRendererSystem : IStartupSystem, IFixedUpdateSystem, IMonoRenderSystem
    {
        public void Start(Context context)
        {
            if (context.HasAnyEntity)
            {
                // Somehow, someone already added a component for the tracker...?
                return;
            }

            context.World.AddEntity(new ParticleSystemWorldTrackerComponent());
        }
        
        public void FixedUpdate(Context context)
        {
            context.Entity.GetParticleSystemWorldTracker().Tracker.Step(
                context.World, Game.FixedDeltaTime);
        }
        
        public void Draw(RenderContext render, Context context)
        {
            WorldParticleSystemTracker worldTracker = context.Entity.GetParticleSystemWorldTracker().Tracker;
            
            foreach (ParticleSystemTracker tracker in worldTracker.FetchActiveParticleTrackers())
            {
                ParticleTexture texture = tracker.Particle.Texture;

                // If this particle is an asset, preload it!
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
                    float delta = particle.Delta;
                    
                    Color color = tracker.Particle.CalculateColor(delta) * particle.Alpha;
                    Vector2 scale = tracker.Particle.CalculateScale(delta);
                    float ySort = RenderServices.YSort(particle.Position.Y);
                    
                    switch (texture.Kind)
                    {
                        case ParticleTextureKind.Point:
                            RenderServices.DrawPoint(
                                render.GameplayBatch,
                                particle.Position.Point,
                                color, 
                                sorting: ySort);

                            break;

                        case ParticleTextureKind.Rectangle:
                            Rectangle rectangle = texture.Rectangle.AddPosition(particle.Position);

                            RenderServices.DrawRectangle(
                                render.GameplayBatch,
                                new Rectangle(rectangle.TopLeft - rectangle.Size * scale * 0.5f, rectangle.Size * scale),
                                color,
                                sorting: ySort);
                            break;

                        case ParticleTextureKind.Circle:
                            RenderServices.DrawCircle(
                                render.GameplayBatch,
                                particle.Position,
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
                                animationStartedTime: Game.Now - particle.Lifetime * delta,
                                animationDuration: -1,
                                offset: Vector2.Zero,
                                flipped: false,
                                rotation: particle.Rotation,
                                scale: scale,
                                color,
                                blend: RenderServices.BLEND_NORMAL,
                                sort: ySort);
                            
                            break;
                    }
                }
            }
        }
    }
}
