using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Microsoft.Xna.Framework.Graphics;
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
                Batch2D batch = render.GetSpriteBatch(tracker.Particle.SpriteBatch);

                // If this particle is an asset, preload it!
                SpriteAsset? asset = default;
                Texture2D? simpleTexture = null;
                string? animationId = default;

                if (texture.Kind == ParticleTextureKind.Texture)
                {
                    simpleTexture = Game.Data.FetchTexture(texture.Texture);
                }
                else if (texture.Kind == ParticleTextureKind.Asset)
                {
                    asset = Game.Data.TryGetAsset<SpriteAsset>(texture.Asset);
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
                    float ySort = RenderServices.YSort(particle.Position.Y + tracker.Particle.SortOffset);
                    
                    switch (texture.Kind)
                    {
                        case ParticleTextureKind.Point:
                            RenderServices.DrawPoint(
                                batch,
                                particle.Position.Point,
                                color, 
                                sorting: ySort);

                            break;

                        case ParticleTextureKind.Rectangle:
                            Rectangle rectangle = texture.Rectangle.AddPosition(particle.Position);

                            RenderServices.DrawRectangle(
                                batch,
                                new Rectangle(rectangle.TopLeft - rectangle.Size * scale * 0.5f, rectangle.Size * scale),
                                color,
                                sorting: ySort);
                            break;

                        case ParticleTextureKind.Circle:
                            var size = new Vector2(texture.Circle.Radius * scale.X, texture.Circle.Radius * scale.Y);
                            var halfSize = size / 2f;
                            RenderServices.DrawFilledCircle(
                                batch,
                                new Rectangle(particle.Position.X - halfSize.X, particle.Position.Y - halfSize.Y,
                                size.X, size.Y),
                                8,
                                new DrawInfo(ySort) { Color = color });
                            break;

                        case ParticleTextureKind.Asset:
                            Debug.Assert(asset is not null && animationId is not null);

                            RenderServices.DrawSprite(
                                batch,
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

                        case ParticleTextureKind.Texture:
                            Debug.Assert(simpleTexture != null, "Particle with Texture kind requires, well, a texture.");
                            batch.Draw(simpleTexture,
                                particle.Position,
                                new Microsoft.Xna.Framework.Vector2(simpleTexture.Bounds.Size.X, simpleTexture.Bounds.Size.Y),
                                simpleTexture.Bounds,
                                ySort,
                                particle.Rotation,
                                scale,
                                ImageFlip.None,
                                color,
                                Vector2.Center,
                                RenderServices.BLEND_NORMAL
                                );
                            break;
                    }
                }
            }
        }
    }
}
