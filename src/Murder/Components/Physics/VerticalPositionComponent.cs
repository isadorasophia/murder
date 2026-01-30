using Bang.Components;

namespace Murder.Components
{
    public readonly struct VerticalPositionComponent : IComponent
    {
        public const float GRAVITY = 0.1f;

        public readonly float Z = 0;
        public readonly float ZVelocity = 0;
        public readonly bool HasGravity { get; init; } = true;

        public VerticalPositionComponent() { }

        public VerticalPositionComponent(float z, float zVelocity, bool hasGravity)
        {
            Z = z;
            ZVelocity = zVelocity;
            HasGravity = hasGravity;
        }

        public VerticalPositionComponent UpdatePosition(float deltaTime, float bounciness, float multiplier)
        {
            if (!HasGravity)
            {
                return this;
            }

            var newZVelocity = ZVelocity + deltaTime * 600 * multiplier;
            var newZ = Z - newZVelocity * deltaTime;
            if (newZ < 0)
            {
                newZ = 0;
                newZVelocity = -newZVelocity * bounciness;

                if (MathF.Abs(newZVelocity) <= deltaTime * 600)
                {
                    newZVelocity = 0;
                }
            }

            return new VerticalPositionComponent(newZ, newZVelocity, HasGravity);
        }
    }
}