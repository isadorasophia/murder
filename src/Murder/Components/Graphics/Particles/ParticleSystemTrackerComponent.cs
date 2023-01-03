using Bang.Components;
using Murder.Attributes;
using Murder.Core.Particles;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;

namespace Murder.Components
{
    /// <summary>
    /// This tracks all the particle systems that are currently active in the world.
    /// </summary>
    [Unique]
    [RuntimeOnly]
    [DoNotPersistEntityOnSave]
    public readonly struct ParticleSystemWorldTrackerComponent : IComponent
    {
        [JsonIgnore]
        public readonly WorldParticleSystemTracker Tracker = new();

        public ParticleSystemWorldTrackerComponent() { }
    }
}
