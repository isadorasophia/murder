using Murder.Utilities;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Murder.Core.Particles
{
    public class ParticleTracker
    {
        [JsonProperty]
        public readonly Particle Particle;

        [JsonProperty]
        public readonly Emitter Emitter;

        [JsonProperty]
        private readonly ParticleRuntime[] _particles;

        private readonly int _seed = 0;

        private bool _hasStarted = false;
        private Random? _random = default;

        private int _currentLength = 0;
        
        private float _time = 0;
        private float _lastTickTime = 0;

        public float Lifetime => _time;

        public ParticleTracker(Emitter emitter, Particle particle, int seed = 0)
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
        /// <returns>Returns whether the emitter is still running.</returns>
        public bool Step(float dt)
        {
            if (!_hasStarted)
            {
                Start();
            }

            for (int i = 0; i < _currentLength; ++i)
            {
                _particles[i].Step(dt);
                
                if (_time - _particles[i].StartTime > _particles[i].Lifetime)
                {
                    // Pool the particles back.
                    _particles[i] = _particles[_currentLength - 1];
                    _particles[_currentLength - 1] = default;
                    
                    _currentLength--;
                }
            }

            _time += dt;
            if (_time - _lastTickTime >= 1)
            {
                DoPerSec();
            }

            return false;
        }

        public void Start()
        {
            _hasStarted = true;
            _random = new Random(_seed);

            _time = _lastTickTime = 0;
            _currentLength = Calculator.RoundToInt(Emitter.Burst.GetValue(_random));
            
            for (int i = 0; i < _currentLength; ++i)
            {
                _particles[i] = CreateParticle();
            }
        }
        
        private void DoPerSec()
        {
            Debug.Assert(_random is not null);
            
            int length = Calculator.RoundToInt(Emitter.ParticlesPerSecond.GetValue(_random));

            if (_currentLength + length > _particles.Length)
            {
                // Maximum particles already hit.
                return;
            }
            
            for (int i = _currentLength; i < _currentLength + length; ++i)
            {
                _particles[i] = CreateParticle();
            }
            
            _currentLength += length;
        }

        private ParticleRuntime CreateParticle()
        {
            Debug.Assert(_random is not null);

            return new ParticleRuntime(
                _time,
                Emitter.Shape.GetRandomPosition(_random), // Implement something based on the shape and angle.
                Particle.Alpha.GetValue(_random),
                Particle.StartVelocity.GetValue(_random),
                Emitter.Angle.GetValue(_random) + Particle.Rotation.GetValue(_random),
                Particle.Acceleration.GetValue(_random),
                Particle.Friction.GetValue(_random),
                Particle.RotationSpeed.GetValue(_random),
                Particle.LifeTime.GetValue(_random)
            );
        }
    }
}
