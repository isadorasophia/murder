using Bang.Components;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    public readonly struct CustomCollisionMask : IComponent
    {
        [CollisionLayer]
        public readonly int CollisionMask = CollisionLayersBase.SOLID | CollisionLayersBase.HOLE;

        public CustomCollisionMask()
        {
        }
        public CustomCollisionMask(int collisionMask)
        {
            CollisionMask = collisionMask;
        }
    }
}