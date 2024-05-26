using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;
using System.Numerics;
using Murder.Diagnostics;
using Murder.Helpers;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.AllOf, typeof(TetheredComponent))]
internal class TetherSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        foreach (var e in context.Entities)
        {
            Vector2 position = e.GetGlobalTransform().Vector2;
            TetheredComponent tetheredComponent = e.GetTethered();
            if (context.World.TryGetEntity(tetheredComponent.Target) is not Entity target)
            {
                GameLogger.Warning($"Couldn't find target {tetheredComponent.Target} for {e.EntityId}, removing TetheredComponent");
                e.RemoveTethered();
                continue;
            }

            float desiredDistance = tetheredComponent.Distance;
            Vector2 targetPosition = target.GetGlobalTransform().Vector2;
            Vector2 direction = (targetPosition - position).NormalizedWithSanity();

            // Get target's facing direction
            Vector2 targetFacing = target.GetFacing().Direction.ToVector();
            float maxAngleDeviation = tetheredComponent.MaxAngleDeviation;

            // Calculate the cosine of the maximum allowed angle deviation
            float maxDotProduct = MathF.Cos(maxAngleDeviation);

            // Calculate the dot product between the target's facing direction and the direction to the entity
            float dotProduct = Vector2.Dot(direction, targetFacing);

            // If the dot product is less than the cosine of the maximum allowed deviation, adjust the direction
            if (dotProduct < maxDotProduct)
            {
                // Calculate the perpendicular vector to targetFacing
                Vector2 perpendicular = new Vector2(-targetFacing.Y, targetFacing.X);

                // Determine the sign of the adjustment
                float sign = Vector2.Dot(perpendicular, direction) > 0 ? 1 : -1;

                // Adjust the direction to be within the allowed deviation
                float angle = MathF.Acos(maxDotProduct);
                float adjustedAngle = sign * angle;
                direction = Vector2.Transform(targetFacing, Matrix3x2.CreateRotation(adjustedAngle)).NormalizedWithSanity();
            }

            Vector2 desiredPosition = targetPosition - direction * desiredDistance;
            Vector2 newPosition = Calculator.Lerp(position, desiredPosition, 0.5f);

            // Calculate the distance between the new position and the target position
            float distance = Vector2.Distance(newPosition, targetPosition);
            float snapDistance = tetheredComponent.SnapDistance;

            e.SetFacing(DirectionHelper.FromVector(direction));

            // If the distance is smaller than the desired distance, do nothing
            if (distance < desiredDistance)
            {
                continue;
            }

            // If the distance is greater than the snap distance, snap the entity to the target
            if (distance > snapDistance)
            {
                newPosition = targetPosition + direction * snapDistance;
            }

            // Apply the new position to the entity
            e.SetGlobalPosition(newPosition);
        }
    }
}
