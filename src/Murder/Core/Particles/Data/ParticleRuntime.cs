using Murder.Core.Geometry;

namespace Murder.Core.Particles
{
    public struct ParticleRuntime
    {
        public Vector2 Position;

        public float Alpha;
        public float Velocity;
        public float Rotation;

        public readonly float StartTime;
        public readonly float RotationSpeed;
        public readonly float Acceleration;
        public readonly float Friction;
        public readonly float Lifetime;

        public ParticleRuntime(
            float startTime,
            Vector2 position, 
            float alpha,
            float velocity,
            float rotation,
            float acceleration,
            float friction, 
            float rotationSpeed, 
            float lifetime)
        {
            Position = position;
            
            Alpha = alpha;
            Velocity = velocity;
            Rotation = rotation;

            StartTime = startTime;
            Acceleration = acceleration;
            Friction = friction;
            RotationSpeed = rotationSpeed;
            Lifetime = lifetime;
        }

        public void Step(float dt)
        {
            // Do acceleration.
            Velocity += Acceleration * dt;

            // Apply friction.
            Velocity = Velocity - Velocity * Friction * Friction * dt;

            Position += Vector2.FromAngle(Rotation) * Velocity * dt;
        }
    }
}
