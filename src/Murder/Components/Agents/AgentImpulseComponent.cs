using Bang.Components;
using Murder.Helpers;
using System.Numerics;

namespace Murder.Components
{
    public readonly struct AgentImpulseComponent : IComponent
    {
        public readonly Vector2 Impulse;

        public readonly Direction? Direction;

        /// <summary>
        /// Whether this should be cleared on the end of every frame.
        /// </summary>
        public readonly bool Clear = true;

        public AgentImpulseComponent(Vector2 impulse) : this(impulse, DirectionHelper.FromVector(impulse)) { }

        public AgentImpulseComponent(Vector2 impulse, Direction? direction)
        {
            Impulse = impulse;
            Direction = direction;
        }

        public AgentImpulseComponent(Direction direction, bool clear) : this(direction.ToVector(), direction)
        {
            Clear = clear;
        }
    }
}