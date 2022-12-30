namespace Murder.Core.Particles
{
    public ref struct ParticleTracker
    {
        public readonly Emitter Emitter;

        public readonly Particle Particle;

        public readonly ParticleRuntime[] Particles;

        public ParticleTracker(Emitter emitter, Particle particle)
        {
            /* Calculate total of particles based on Emitter */
            Particles = new ParticleRuntime[emitter.MaxParticles];
        }

        /// <summary>
        /// Makes a "step" throughout the particle system.
        /// </summary>
        /// <param name="dt">Delta time.</param>
        /// <returns>Returns whether the emitter is still running.</returns>
        public bool Step(float dt)
        {
            return false;
        }
    }
}
