using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Particles;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Requires(typeof(ParticleRendererSystem))]
    [Filter(typeof(ParticleSystemComponent))]
    [Watch(typeof(AlphaComponent))]
    public class ParticleAlphaTrackerSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            OnModified(world, entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUniqueParticleSystemWorldTracker()?.Tracker is not WorldParticleSystemTracker tracker)
            {
                return;
            }

            foreach (Entity e in entities)
            {
                AlphaComponent alpha = e.GetAlpha();
                tracker.SetAlpha(e.EntityId, alpha.Alpha);
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUniqueParticleSystemWorldTracker()?.Tracker is not WorldParticleSystemTracker tracker)
            {
                return;
            }

            foreach (Entity e in entities)
            {
                tracker.SetAlpha(e.EntityId, 1);
            }
        }
    }
}