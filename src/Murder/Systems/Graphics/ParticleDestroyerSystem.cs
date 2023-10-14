using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(ParticleSystemComponent))]
    public class ParticleDestroyerSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            WorldParticleSystemTracker worldTracker = context.World.GetUnique<ParticleSystemWorldTrackerComponent>().Tracker;


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