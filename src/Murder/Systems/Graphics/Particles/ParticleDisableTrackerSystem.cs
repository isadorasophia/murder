using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Particles;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [EditorSystem]
    [Requires(typeof(ParticleRendererSystem))]
    [Filter(typeof(DisableParticleSystemComponent))]
    [Watch(typeof(DisableParticleSystemComponent))]
    public class ParticleDisableTrackerSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUniqueParticleSystemWorldTracker()?.Tracker is WorldParticleSystemTracker tracker)
            {
                foreach (Entity e in entities)
                {
                    tracker.Deactivate(e.EntityId);
                }
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities) { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUniqueParticleSystemWorldTracker()?.Tracker is not WorldParticleSystemTracker tracker)
            {
                return;
            }

            foreach (Entity e in entities)
            {
                if (e.HasDisableParticleSystem())
                {
                    // added in the same frame?
                    continue;
                }

                if (!tracker.IsTracking(e.EntityId))
                {
                    if (e.TryGetParticleSystem() is ParticleSystemComponent particle &&
                        particle.Asset != Guid.Empty)
                    {
                        tracker.Track(e);
                    }
                }

                tracker.Activate(e.EntityId);
            }
        }
    }
}