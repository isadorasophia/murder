using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// Component that defines tethering behavior for an entity.
    /// </summary>
    public readonly struct TetheredComponent : IComponent
    {
        /// <summary>
        /// The entity ID of the target to which this entity is tethered.
        /// </summary>
        [Tooltip("The entity ID of the target to which this entity is tethered.")]
        public readonly int Target;

        /// <summary>
        /// The desired distance to maintain from the target entity.
        /// </summary>
        [Tooltip("The desired distance to maintain from the target entity.")]
        public readonly float Distance;

        /// <summary>
        /// The distance threshold beyond which the entity will snap to the target.
        /// </summary>
        [Tooltip("The distance threshold beyond which the entity will snap to the target.")]
        public readonly float SnapDistance;

        /// <summary>
        /// The maximum allowed angular deviation from the target's facing direction, in radians.
        /// </summary>
        [Tooltip("The maximum allowed angular deviation from the target's facing direction, in radians.")]
        public readonly float MaxAngleDeviation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TetheredComponent"/> struct.
        /// </summary>
        /// <param name="target">The entity ID of the target to which this entity is tethered.</param>
        /// <param name="distance">The desired distance to maintain from the target entity.</param>
        /// <param name="snapDistance">The distance threshold beyond which the entity will snap to the target.</param>
        /// <param name="maxAngleDeviation">The maximum allowed angular deviation from the target's facing direction, in radians.</param>
        public TetheredComponent(int target, float distance, float snapDistance, float maxAngleDeviation)
        {
            Target = target;
            Distance = distance;
            SnapDistance = snapDistance;
            MaxAngleDeviation = maxAngleDeviation;
        }
    }
}
