using Bang.Components;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Components;

public enum ShapeStyle
{
    Points,
    OpenLines,
    ClosedLines,
    ClosedShape,
}

public readonly struct ShapePosition
{
    /// <summary>
    /// Closest point or line point if sound shape is a Line.
    /// If the listener is inside a ClosedShape the point is always the listener position.
    /// </summary>
    public readonly Vector2 ClosestPoint;

    /// <summary>
    /// Current distance in pixels of the listener to the closest point.
    /// </summary>
    public readonly float Distance;

    /// <summary>
    /// Volume ratio from 0 to 1, using the appropriate easing.
    /// </summary>
    public readonly float EasedDistance;

    /// <summary>
    /// The index of the closest point in the array.
    /// </summary>
    public readonly int ClosestIndex;

    public ShapePosition(Vector2 closestPoint, float distance, float easedDistance, int closestIndex)
    {
        ClosestPoint = closestPoint;
        Distance = distance;
        EasedDistance = easedDistance;
        ClosestIndex = closestIndex;
    }
}

[Requires(typeof(PositionComponent))]
[Sound]
public readonly struct SoundShapeComponent : IComponent
{
    public readonly ImmutableArray<Vector2> Points { get; init; } = [Vector2.Zero];
    public readonly ShapeStyle ShapeStyle = ShapeStyle.Points;
    public readonly float MinRange = 10;
    public readonly float MaxRange = 200;
    public readonly EaseKind EaseKind = EaseKind.Linear;

    public SoundShapeComponent()
    {
    }

    public SoundShapeComponent(ImmutableArray<Vector2> points)
    {
        Points = points;
    }

    public SoundShapeComponent(ImmutableArray<Vector2> points, ShapeStyle shapeStyle, float minRange, float maxRange, EaseKind easeKind)
    {
        Points = points;
        ShapeStyle = shapeStyle;
        MinRange = minRange;
        MaxRange = maxRange;
        EaseKind = easeKind;
    }

    public ShapePosition GetSoundPosition(Vector2 listenerPosition)
    {
        float closestDistance = float.MaxValue;
        Vector2 closestPoint = listenerPosition;
        int closestIndex = -1;

        // Check if the listener is inside a closed shape
        if (ShapeStyle == ShapeStyle.ClosedShape && new Polygon(Points).Contains(listenerPosition))
        {
            return new ShapePosition(closestPoint, 0, 1, -1);
        }

        // Loop through all points to find the closest one or the closest point on a line segment
        for (int i = 0; i < Points.Length; i++)
        {
            Vector2 currentPoint = Points[i];

            if (ShapeStyle != ShapeStyle.Points)
            {
                // Determine the next point index
                int nextIndex = (i + 1) % Points.Length;

                // For OpenLines, do not wrap around at the end
                if (ShapeStyle == ShapeStyle.OpenLines && i == Points.Length - 1)
                    break;

                Vector2 nextPoint = new Vector2(Points[nextIndex].X, Points[nextIndex].Y);

                // Find the closest point on the line segment
                var line = new Line2(currentPoint, nextPoint);
                Vector2 closestOnSegment = line.GetClosestPoint(listenerPosition);
                float distance = Vector2.Distance(listenerPosition, closestOnSegment);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = closestOnSegment;

                    // Check which point of the line segment is the closest
                    if (Vector2.DistanceSquared(listenerPosition, currentPoint) < Vector2.DistanceSquared(listenerPosition, nextPoint))
                    {
                        closestIndex = i;
                    }
                    else
                    {
                        closestIndex = nextIndex;
                    }
                }
            }
            else
            {
                // For Points shape style, just consider the point itself
                float distance = Vector2.Distance(listenerPosition, currentPoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = currentPoint;
                    closestIndex = i;
                }
            }
        }

        // Calculate the eased distance (volume ratio)
        float ratio = 1;
        if (closestDistance > MinRange)
        {
            float normalizedDistance = (closestDistance - MinRange) / (MaxRange - MinRange);
            ratio = Ease.Evaluate(1 - MathF.Min(1, normalizedDistance), EaseKind);
        }

        return new ShapePosition(closestPoint, closestDistance, ratio, closestIndex);
    }
}