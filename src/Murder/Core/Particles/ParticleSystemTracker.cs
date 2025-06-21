using Murder.Core.Geometry;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Numerics;

namespace Murder.Core.Particles
{
    public struct ParticleSystemTracker
    {
        [Bang.Serialize]
        public readonly Particle Particle;

        [Bang.Serialize]
        public readonly Emitter Emitter;

        /// <summary>
        /// Alpha for the particles of the whole system. This is used when the entity
        /// has an alpha source, for example.
        /// </summary>
        public float Alpha { private get; set; } = 1;

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
        /// <param name="allowSpawn">Whether spawning new entities is allowed, e.g. the entity is not deactivated.</param>
        /// <param name="emitterPosition">Emitter position in game where the particles are fired from.</param>
        /// <param name="cameraArea">Emmiters outside of this area won't step</param>
        /// <param name="id">Entity id, used when generating the correct seed for the particle.</param>
        public void Step(bool allowSpawn, Vector2 emitterPosition, Rectangle cameraArea, int id)
        {
            _lastEmitterPosition = emitterPosition;

            if (!_hasStarted)
            {
                Start(emitterPosition, id);
            }

            float dt = Emitter.ScaledTime ? Game.DeltaTime : Game.UnscaledDeltaTime;
            
            for (int i = 0; i < _currentLength; ++i)
            {
                _particles[i].Step(Particle, _time, dt);

                if (Particle.FollowEntityPosition)
                {
                    _particles[i].UpdateFromPosition(emitterPosition);
                }

                _particles[i].UpdateAlpha(Alpha);

                if (_particles[i].Delta == 1)
                {
                    // Pool the particles back.
                    _particles[i] = _particles[Math.Min(_particles.Length - 1, _currentLength - 1)];
                    _particles[_currentLength - 1] = default;

                    _currentLength--;
                }
            }

            _time += dt;

            Rectangle boundingBox = Emitter.BoundingBoxSize().AddPosition(emitterPosition);
            if (!boundingBox.Touches(cameraArea))
            {
                return;
            }

            if (allowSpawn)
            {
                SpawnNewParticlesPerSec(emitterPosition);
            }
        }

        public void Start(Vector2 emitterPosition, int id)
        {
            _hasStarted = true;
            _random = new Random((int)(_seed + emitterPosition.X + emitterPosition.Y / 320f) + id * 111);

            _time = _lastTimeSpawned = 0;
            _currentLength = Calculator.RoundToInt(Emitter.Burst.GetValue(_random));

            if (_particles.Length < _currentLength)
            {
                return;
            }

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
                Particle.RotationSpeed.GetValueAt(0),
                fromAlpha: Alpha
            );
        }
    }
}