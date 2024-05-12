using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Particles;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(ParticleSystemComponent))]
    public class ParticleDestroyerSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            WorldParticleSystemTracker worldTracker = context.World.GetUniqueParticleSystemWorldTracker().Tracker;

            foreach (var e in context.Entities)
            {
                var particles = e.GetParticleSystem();
                if (particles.DestroyWhenEmpty && !worldTracker.HasParticles(e))
                {
                    e.Destroy();
                }
            }
        }
    }
}