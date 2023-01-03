using Murder.Core.Geometry;
using Murder.Utilities;

namespace Murder.Core.Particles
{
    public struct ParticleRuntime
    {
        public Vector2 Position => _fromPosition + _localPosition;

        public readonly float Lifetime;

        public float Alpha;
        public float Velocity;
        public float Rotation;

        public float RotationSpeed;
        public float Acceleration;
        public float Friction;

        /// <summary>
        /// This is the lifetime of the particle over 0 to 1.
        /// </summary>
        public float Delta { get; private set; }

        private Vector2 _localPosition;
        
        /// <summary>
        /// Used to track the position where this was fired.
        /// This is updated if <see cref="Particle.FollowEntityPosition"/> is set.
        /// </summary>
        private Vector2 _fromPosition = Vector2.Zero;
        
        private readonly float _startTime = 0;

        public ParticleRuntime(
            float startTime,
            float lifetime,
            Vector2 position,
            Vector2 fromPosition,
            float startAlpha,
            float startVelocity,
            float startRotation,
            float startAcceleration,
            float startFriction, 
            float startRotationSpeed)
        {
            _startTime = startTime;
            Lifetime = lifetime;
            
            _localPosition = position;
            _fromPosition = fromPosition;

            Alpha = startAlpha;
            Velocity = startVelocity;
            Rotation = startRotation;

            Acceleration = startAcceleration;
            Friction = startFriction;
            RotationSpeed = startRotationSpeed;
        }

        public void UpdateFromPosition(Vector2 from)
        {
            _fromPosition = from;
        }

        /// <summary>
        /// Makes a step for a runtime particle.
        /// </summary>
        /// <param name="particle">Value on the particle based on the asset.</param>
        /// <param name="currentTime">Current time that the particle system exists.</param>
        /// <param name="dt">Delta time since the last Step call.</param>
        public void Step(in Particle particle, float currentTime, float dt)
        {
            Delta = Calculator.Clamp01((currentTime - _startTime) / Lifetime);

            Alpha = particle.Alpha.GetValueAt(Delta);
            Acceleration = particle.Acceleration.GetValueAt(Delta);
            Friction = particle.Friction.GetValueAt(Delta);
            Rotation = particle.Rotation.GetValueAt(Delta);

            // Do acceleration.
            Velocity += Acceleration * dt;

            // Apply friction.
            Velocity = Velocity - Velocity * Friction * Friction * dt;

            _localPosition += Vector2.FromAngle(Rotation) * Velocity * dt;
        }
    }
}
