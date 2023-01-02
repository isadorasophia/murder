using Bang.Components;
using Murder.Core.Particles;

namespace Murder.Components
{
    public readonly struct ParticleTrackerComponent : IComponent
    {
        public readonly ParticleTracker Tracker;

        public ParticleTrackerComponent(ParticleTracker tracker)
        {
            Tracker = tracker;
        }

        public bool Step(float dt) => Tracker.Step(dt);
    }
}
