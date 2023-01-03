using Murder.Core.Geometry;

namespace Murder.Core.Particles
{
    public struct ParticleRuntime
    {
        public Vector2 Position => _fromPosition + _localPosition;

        public float Alpha;
        public float Velocity;
        public float Rotation;

        public readonly float StartTime;
        public readonly float RotationSpeed;
        public readonly float Acceleration;
        public readonly float Friction;
        public readonly float Lifetime;

        public Vector2 _localPosition;
        
        /// <summary>
        /// Used to track the position where this was fired.
        /// This is updated if <see cref="Particle.FollowEntityPosition"/> is set.
        /// </summary>
        private Vector2 _fromPosition = Vector2.Zero;

        public ParticleRuntime(
            float startTime,
            Vector2 position,
            Vector2 fromPosition,
            float alpha,
            float velocity,
            float rotation,
            float acceleration,
            float friction, 
            float rotationSpeed, 
            float lifetime)
        {
            _localPosition = position;
            _fromPosition = fromPosition;

            Alpha = alpha;
            Velocity = velocity;
            Rotation = rotation;

            StartTime = startTime;
            Acceleration = acceleration;
            Friction = friction;
            RotationSpeed = rotationSpeed;
            Lifetime = lifetime;
        }

        public void UpdateFromPosition(Vector2 from)
        {
            _fromPosition = from;
        }

        public void Step(float dt)
        {
            // Do acceleration.
            Velocity += Acceleration * dt;

            // Apply friction.
            Velocity = Velocity - Velocity * Friction * Friction * dt;

            _localPosition += Vector2.FromAngle(Rotation) * Velocity * dt;
        }
    }
}
