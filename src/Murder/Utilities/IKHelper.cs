using System.Numerics;

namespace Murder.Utilities;

public static class IKHelper
{
    public static Vector2? GetElbowPosition(Vector2 basePosition, Vector2 endPosition, float segmentLength1, float segmentLength2, bool flipElbow = false)
    {
        // Distance between base and end points
        float distance = Vector2.Distance(basePosition, endPosition);

        // Check if the target is reachable
        if (distance > segmentLength1 + segmentLength2)
        {
            return null; // Can't reach the target
        }

        // Check if the target is within too close range
        if (distance < Math.Abs(segmentLength1 - segmentLength2))
        {
            return null; // Target is too close to reach
        }

        // Find angle at base using Law of Cosines
        float angleBase = (float)Math.Acos((Math.Pow(segmentLength1, 2) + Math.Pow(distance, 2) - Math.Pow(segmentLength2, 2)) / (2 * segmentLength1 * distance));

        // Find the angle from base to end position
        Vector2 direction = Vector2.Normalize(endPosition - basePosition);
        float angleToTarget = (float)Math.Atan2(direction.Y, direction.X);

        // Calculate the elbow position
        float elbowAngle = angleToTarget + (flipElbow ? -angleBase : angleBase);
        Vector2 elbowPosition = basePosition + new Vector2((float)Math.Cos(elbowAngle), (float)Math.Sin(elbowAngle)) * segmentLength1;

        return elbowPosition;
    }

    public static Vector2 GetSafeElbowPosition(Vector2 basePosition, Vector2 endPosition, float segmentLength1, float segmentLength2, bool flipElbow = false)
    {
        // Distance between base and end points
        float distance = Vector2.Distance(basePosition, endPosition);

        // Normalize the direction from base to end
        Vector2 direction = Vector2.Normalize(endPosition - basePosition);

        // Adjust the end position if it's unreachable (too far)
        if (distance > segmentLength1 + segmentLength2)
        {
            // Bring the end closer to within reach, at the maximum arm extension
            endPosition = basePosition + direction * (segmentLength1 + segmentLength2);
            distance = segmentLength1 + segmentLength2;
        }
        else if (distance < Math.Abs(segmentLength1 - segmentLength2))
        {
            // Adjust end position to the minimum reach, at the closest valid point
            endPosition = basePosition + direction * Math.Abs(segmentLength1 - segmentLength2);
            distance = Math.Abs(segmentLength1 - segmentLength2);
        }

        // Calculate the angles as before
        float angleBase = (float)Math.Acos((Math.Pow(segmentLength1, 2) + Math.Pow(distance, 2) - Math.Pow(segmentLength2, 2)) / (2 * segmentLength1 * distance));
        float angleToTarget = (float)Math.Atan2(direction.Y, direction.X);

        // Calculate the "safe" elbow position with the option to flip the elbow direction
        float elbowAngle = angleToTarget + (flipElbow ? -angleBase : angleBase);
        Vector2 elbowPosition = basePosition + new Vector2((float)Math.Cos(elbowAngle), (float)Math.Sin(elbowAngle)) * segmentLength1;

        return elbowPosition;
    }

    public static Vector2 GetSafeElbowPosition(Vector2 basePosition, Vector2 endPosition, float segmentLength1, float segmentLength2, Vector2 elbowTarget, bool flipElbow = false)
    {
        // Calculate the distance between the base and end positions
        float distance = Vector2.Distance(basePosition, endPosition);

        // Clamp the distance to the maximum reach if it's out of bounds
        distance = MathF.Min(distance, segmentLength1 + segmentLength2);

        // Calculate the angle at basePosition using the Law of Cosines
        float angleA = MathF.Acos((segmentLength1 * segmentLength1 + distance * distance - segmentLength2 * segmentLength2) / (2 * segmentLength1 * distance));

        // Find the direction from base to end position
        Vector2 direction = (endPosition - basePosition).Normalized();

        // Rotate the direction by the angle to find the potential elbow positions
        Vector2 elbowOffset = direction * segmentLength1;
        Vector2 elbowPosition1 = basePosition + Vector2.Transform(elbowOffset, Matrix3x2.CreateRotation(angleA));
        Vector2 elbowPosition2 = basePosition + Vector2.Transform(elbowOffset, Matrix3x2.CreateRotation(-angleA));

        // Choose the elbow position based on flipElbow or proximity to elbowTarget
        Vector2 chosenElbow = flipElbow
            ? elbowPosition2
            : elbowPosition1;

        // If both options are valid, choose the one closer to the elbowTarget
        if (!flipElbow && Vector2.Distance(elbowPosition1, elbowTarget) > Vector2.Distance(elbowPosition2, elbowTarget))
        {
            chosenElbow = elbowPosition2;
        }

        return chosenElbow;
    }


}
