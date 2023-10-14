using Bang.Components;
using Murder.Helpers;
using System.Numerics;

namespace Murder.Components
{
    public readonly struct AgentImpulseComponent : IComponent
    {
        public readonly Vector2 Impulse;

        public readonly Direction Direction;

        public AgentImpulseComponent(Vector2 impulse) : this(impulse, DirectionHelper.FromVector(impulse)) { }

        public AgentImpulseComponent(Vector2 impulse, Direction direction)
        {
            Impulse = impulse;
            Direction = direction;
        }
    }
}