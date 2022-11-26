using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components
{
    public readonly struct VerticalPositionComponent : IComponent
    {
        public const float GRAVITY = 0.1f;

        public readonly float Z = 0;
        public readonly float ZVelocity = 0;
        public readonly bool HasGravity = true;

        public VerticalPositionComponent() {}

        public VerticalPositionComponent(float z, float zVelocity, bool hasGravity)
        {
            Z = z;
            ZVelocity = zVelocity;
            HasGravity = hasGravity;
        }

        public VerticalPositionComponent UpdatePosition(float deltaTime)
        {
            if (!HasGravity)
            {
                return this;
            }

            var newZVelocity = ZVelocity + deltaTime * 600;
            var newZ = Z - newZVelocity * deltaTime;
            if (newZ < 0)
            {
                newZ = 0;
                newZVelocity = -newZVelocity * 0.6f;
                if (MathF.Abs(newZVelocity) <= 1f)
                {
                    newZVelocity = 0;
                }
            }

            return new VerticalPositionComponent(newZ, newZVelocity, HasGravity);
        }
    }
}
