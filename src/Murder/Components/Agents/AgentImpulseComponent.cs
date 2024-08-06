using Bang.Components;
using Murder.Helpers;
using System.Numerics;

namespace Murder.Components
{
    public enum AgentImpulseFlags
    {
        None = 0,

        /// <summary>
        /// Whether this should be cleared on the end of every frame.
        /// </summary>
        DoNotClear = 1,

        /// <summary>
        /// Whether this should affect the agent facing.
        /// </summary>
        IgnoreFacing = 0x10
    }

    public readonly struct AgentImpulseComponent : IComponent
    {
        public readonly Vector2 Impulse;

        public readonly Direction? Direction;

        /// <summary>
        /// Whether this should be cleared on the end of every frame.
        /// </summary>
        public readonly AgentImpulseFlags Flags = AgentImpulseFlags.None;

        public AgentImpulseComponent(Vector2 impulse, Direction? direction, AgentImpulseFlags flags)
        {
            Impulse = impulse;
            Direction = direction;
            Flags = flags;
        }

        public AgentImpulseComponent(Vector2 impulse) : this(impulse, DirectionHelper.FromVector(impulse)) { }

        public AgentImpulseComponent(Vector2 impulse, AgentImpulseFlags flags) : this(impulse, DirectionHelper.FromVector(impulse), flags) { }

        public AgentImpulseComponent(Vector2 impulse, Direction? direction) : this(impulse, direction, AgentImpulseFlags.None) { }

        public AgentImpulseComponent(Direction direction, AgentImpulseFlags flags) : this(direction.ToVector(), direction, flags) { }
    }
}