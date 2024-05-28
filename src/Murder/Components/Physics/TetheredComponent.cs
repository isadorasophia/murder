using Bang.Components;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    /// <summary>
    /// Represents a single tether point for an entity.
    /// </summary>
    public readonly struct TetherPoint
    {
        /// <summary>
        /// The entity ID of the tether target.
        /// </summary>
        public readonly int Target;

        /// <summary>
        /// The desired distance from the tether target.
        /// </summary>
        public readonly float Length;

        /// <summary>
        /// The maximum allowable angle deviation from the target's facing direction.
        /// </summary>
        public readonly float MaxAngleDeviation;

        /// <summary>
        /// The distance at which the entity snaps to the tether target.
        /// </summary>
        public readonly float SnapDistance;


        public TetherPoint(int target, float distance, float maxAngleDeviation, float snapDistance)
        {
            Target = target;
            Length = distance;
            MaxAngleDeviation = maxAngleDeviation;
            SnapDistance = snapDistance;
        }
    }

    [RuntimeOnly]
    public readonly struct TetheredComponent : IComponent
    {
        public readonly ImmutableArray<TetherPoint> TetherPoints;
        public readonly ImmutableDictionary<int, TetherPoint> TetherPointsDict;

        public TetheredComponent(ImmutableArray<TetherPoint> tetherPoints)
        {
            TetherPoints = tetherPoints;

            var builder = ImmutableDictionary.CreateBuilder<int, TetherPoint>();
            foreach (var point in tetherPoints)
            {
                builder.Add(point.Target, point);
            }
            TetherPointsDict = builder.ToImmutable();
        }
    }
}
