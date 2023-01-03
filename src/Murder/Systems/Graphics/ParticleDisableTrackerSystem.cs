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
    [Filter(typeof(DisableParticleSystemComponent))]
    [Watch(typeof(DisableParticleSystemComponent))]
    public class ParticleDisableTrackerSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            if (world.TryGetUnique<ParticleSystemWorldTrackerComponent>()?.Tracker is WorldParticleSystemTracker tracker)
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
            if (world.TryGetUnique<ParticleSystemWorldTrackerComponent>()?.Tracker is WorldParticleSystemTracker tracker)
            {
                foreach (Entity e in entities)
                {
                    tracker.Activate(e.EntityId);
                }
            }
        }
    }
}
