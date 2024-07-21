using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Particles;
using Murder.Editor.Attributes;
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
            if (world.TryGetUniqueParticleSystemWorldTracker()?.Tracker is WorldParticleSystemTracker tracker)
            {
                foreach (Entity e in entities)
                {
                    if (!tracker.IsTracking(e.EntityId))
                    {
                        if (e.GetParticleSystem().Asset != Guid.Empty)
                        {
                            tracker.Track(e);
                        }
                    }

                    tracker.Activate(e.EntityId);
                }
            }
        }
    }
}