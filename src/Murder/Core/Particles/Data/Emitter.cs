using Murder.Attributes;
using Newtonsoft.Json;

namespace Murder.Core.Particles
{
    public readonly struct Emitter
    {
        [Tooltip("Maximum of particles that this emitter may have.")]
        public readonly int MaxParticlesPool = 100;
        
        [Tooltip("The shape which will be used to fire particles from.")]
        public readonly EmitterShape Shape = default;
        
        [Tooltip("Angle which the particles will be fired.")]
        [Angle]
        public readonly ParticleValueProperty Angle = ParticleValueProperty.Empty;

        [Tooltip("Amount of particles which will be fired every second.")]
        public readonly ParticleIntValueProperty ParticlesPerSecond = ParticleIntValueProperty.Empty;

        [Tooltip("Amount of particles which will be fired at once.")]
        public readonly ParticleIntValueProperty Burst = ParticleIntValueProperty.Empty;

        [Tooltip("Speed that will fire the particles.")]
        public readonly ParticleValueProperty Speed = ParticleValueProperty.Empty;

        [JsonConstructor]
        public Emitter() { }

        public Emitter(int maxParticles, EmitterShape shape, ParticleValueProperty angle, ParticleIntValueProperty particlesPerSecond,
            ParticleIntValueProperty burst, ParticleValueProperty speed)
        {
            MaxParticlesPool = maxParticles;
            Shape = shape;
            Angle = angle;
            ParticlesPerSecond = particlesPerSecond;
            Burst = burst;
            Speed = speed;
        }

        public Emitter WithShape(EmitterShape shape) =>
            new(MaxParticlesPool, shape, Angle, ParticlesPerSecond, Burst, Speed);
    }
}
