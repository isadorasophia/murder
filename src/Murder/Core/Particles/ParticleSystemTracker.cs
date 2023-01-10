using Murder.Core.Geometry;
using Murder.Utilities;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Murder.Core.Particles
{
    public struct ParticleSystemTracker
    {
        [JsonProperty]
        public readonly Particle Particle;

        [JsonProperty]
        public readonly Emitter Emitter;
        
        private readonly ParticleRuntime[] _particles;
        private int _currentLength = 0;

        private readonly int _seed = 0;

        private bool _hasStarted = false;
        private Random? _random = default;
        
        private float _time = 0;
        private float _lastTimeSpawned = 0;

        /// <summary>
        /// Interval of time before a new particle is spawned.
        /// </summary>
        private float _intervalPerParticleSpawn = 0;

        private Vector2 _lastEmitterPosition = Vector2.Zero;
        
        /// <summary>
        /// The last position of the emitter.
        /// </summary>
        public Vector2 LastEmitterPosition => _lastEmitterPosition;

        public float CurrentTime => _time;


        public ParticleSystemTracker(Emitter emitter, Particle particle, int seed = 0)
        {
            Particle = particle;
            Emitter = emitter;
            
            _seed = seed;
            _particles = new ParticleRuntime[Emitter.MaxParticlesPool];
        }

        public ReadOnlySpan<ParticleRuntime> Particles => new(_particles, 0, _currentLength);

        /// <summary>
        /// Makes a "step" throughout the particle system.
        /// </summary>
        /// <param name="dt">Delta time.</param>
        /// <param name="allowSpawn">Whether spawning new entities is allowed, e.g. the entity is not deactivated.</param>
        /// <param name="emitterPosition">Emitter position in game where the particles are fired from.</param>
        /// <returns>Returns whether the emitter is still running.</returns>
        public bool Step(float dt, bool allowSpawn, Vector2 emitterPosition)
        {
            _lastEmitterPosition = emitterPosition;
            
            if (!_hasStarted)
            {
                Start(emitterPosition);
            }

            for (int i = 0; i < _currentLength; ++i)
            {
                _particles[i].Step(Particle, _time, dt);

                if (Particle.FollowEntityPosition)
                {
                    _particles[i].UpdateFromPosition(emitterPosition);
                }
                
                if (_particles[i].Delta == 1)
                {
                    // Pool the particles back.
                    _particles[i] = _particles[_currentLength - 1];
                    _particles[_currentLength - 1] = default;
                    
                    _currentLength--;
                }
            }

            _time += dt;
            if (allowSpawn)
            {
                SpawnNewParticlesPerSec(emitterPosition);
            }

            return false;
        }

        public void Start(Vector2 emitterPosition)
        {
            _hasStarted = true;
            _random = new Random(_seed);

            _time = _lastTimeSpawned = 0;
            _currentLength = Calculator.RoundToInt(Emitter.Burst.GetValue(_random));
            
            for (int i = 0; i < _currentLength; ++i)
            {
                _particles[i] = CreateParticle(emitterPosition);
            }
            
            _intervalPerParticleSpawn = 1f / Emitter.ParticlesPerSecond.GetRandomValue(_random);
        }
        
        private void SpawnNewParticlesPerSec(Vector2 emitterPosition)
        {
            Debug.Assert(_random is not null);
            
            if (_currentLength >= _particles.Length)
            {
                // Maximum particles already hit.
                return;
            }

            if (_time - _lastTimeSpawned > _intervalPerParticleSpawn)
            {
                _particles[_currentLength++] = CreateParticle(emitterPosition);
                _lastTimeSpawned = _time;
            }
        }

        private ParticleRuntime CreateParticle(Vector2 emitterPosition)
        {
            Debug.Assert(_random is not null);

            return new ParticleRuntime(
                _time,
                Particle.LifeTime.GetRandomValue(_random),
                Emitter.Shape.GetRandomPosition(_random), // Implement something based on the shape and angle.
                fromPosition: emitterPosition,
                gravity: Particle.Gravity.GetRandomValue(_random),
                startAlpha: Particle.Alpha.GetValueAt(0),
                Emitter.Speed.GetRandomValue(_random) + Particle.StartVelocity.GetValueAt(0),
                Emitter.Angle.GetRandomValue(_random),
                Particle.Acceleration.GetValueAt(0),
                Particle.Friction.GetValueAt(0),
                Particle.RotationSpeed.GetValueAt(0)
            );
        }
    }
}
