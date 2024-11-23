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
    [Filter(typeof(ParticleSystemComponent))]
    [Watch(typeof(ParticleSystemComponent))]
    public class ParticleTrackerSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUniqueParticleSystemWorldTracker()?.Tracker is WorldParticleSystemTracker tracker)
            {
                foreach (Entity e in entities)
                {
                    if (e.IsDeactivated)
                    {
                        continue;
                    }

                    if (e.GetParticleSystem().Asset != Guid.Empty)
                    {
                        tracker.Track(e);
                    }
                }
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUniqueParticleSystemWorldTracker()?.Tracker is WorldParticleSystemTracker tracker)
            {
                foreach (Entity e in entities)
                {
                    tracker.Synchronize(e);
                }
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUniqueParticleSystemWorldTracker()?.Tracker is WorldParticleSystemTracker tracker)
            {
                foreach (Entity e in entities)
                {
                    tracker.Untrack(e);
                }
            }
        }

        public void OnActivated(World world, ImmutableArray<Entity> entities) => OnAdded(world, entities);

        public void OnDeactivated(World world, ImmutableArray<Entity> entities) => OnRemoved(world, entities);
    }
}