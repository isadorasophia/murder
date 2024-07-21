using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Particles;
using Murder.Editor.Attributes;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Numerics;

namespace Murder.Systems
{
    [EditorSystem]
    [Filter(typeof(ParticleSystemWorldTrackerComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(InvisibleComponent))]
    public class ParticleRendererSystem : IStartupSystem, IMurderRenderSystem, IUpdateSystem
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

        public void Update(Context context)
        {
            context.Entity.GetParticleSystemWorldTracker().Tracker.Step(context.World);
        }

        public void Draw(RenderContext render, Context context)
        {
            WorldParticleSystemTracker worldTracker = context.Entity.GetParticleSystemWorldTracker().Tracker;

            foreach (ParticleSystemTracker tracker in worldTracker.FetchActiveParticleTrackers())
            {
                ParticleTexture texture = tracker.Particle.Texture;
                Batch2D batch = render.GetBatch((int)tracker.Particle.SpriteBatch);

                // If this particle is an asset, preload it!
                SpriteAsset? asset = default;
                Texture2D? simpleTexture = null;
                string? animationId = default;

                if (texture.Kind == ParticleTextureKind.Texture && !string.IsNullOrEmpty(texture.Texture))
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
                                particle.Position.Point(),
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
                            {
                                var size = new Vector2(texture.Circle.Radius * scale.X, texture.Circle.Radius * scale.Y);
                                var halfSize = size / 2f;
                                RenderServices.DrawFilledCircle(
                                    batch,
                                    new Rectangle(particle.Position.X - halfSize.X, particle.Position.Y - halfSize.Y,
                                    size.X, size.Y),
                                    Circle.EstipulateSidesFromRadius(Math.Max(size.X, size.Y)),
                                    new DrawInfo(ySort) { Color = color });
                            }
                            break;

                        case ParticleTextureKind.CircleOutline:
                            {
                                var size = new Vector2(texture.Circle.Radius * scale.X, texture.Circle.Radius * scale.Y);
                                var halfSize = size / 2f;
                                RenderServices.DrawCircleOutline(
                                    batch,
                                    new Rectangle(particle.Position, size),
                                    Circle.EstipulateSidesFromRadius(Math.Max(size.X, size.Y)),
                                    color);
                            }
                            break;

                        case ParticleTextureKind.Asset:
                            Debug.Assert(asset is not null && animationId is not null);

                            RenderServices.DrawSprite(
                                batch,
                                particle.Position,
                                Rectangle.Empty,
                                animationId,
                                asset,
                                animationStartedTime: Game.Now - particle.Lifetime * delta,
                                animationDuration: -1,
                                animationLoop: true,
                                origin: Vector2.Zero,
                                imageFlip: ImageFlip.None,
                                rotation: particle.Rotation,
                                scale: scale,
                                color,
                                blend: RenderServices.BLEND_NORMAL,
                                sort: ySort,
                                currentTime: Game.Now);

                            break;

                        case ParticleTextureKind.Texture:

                            if (simpleTexture != null)
                            {
                                batch.Draw(simpleTexture,
                                    particle.Position.ToXnaVector2(),
                                    new Microsoft.Xna.Framework.Vector2(simpleTexture.Bounds.Width, simpleTexture.Bounds.Height),
                                    simpleTexture.Bounds,
                                    ySort,
                                    particle.Rotation,
                                    scale.ToXnaVector2(),
                                    ImageFlip.None,
                                    color,
                                    Vector2Helper.Center.ToXnaVector2() * simpleTexture.Bounds.XnaSize(),
                                    RenderServices.BLEND_NORMAL
                                    );
                            }
                            else
                            {
                                Vector2 defaultSize = new Vector2(32, 32);
                                RenderServices.DrawRectangle(
                                batch,
                                new Rectangle(particle.Position - defaultSize * scale * 0.5f, defaultSize * scale),
                                color,
                                sorting: ySort);
                            }
                            break;
                    }
                }
            }
        }
    }
}