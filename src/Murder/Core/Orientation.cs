using System.Numerics;

namespace Murder.Core;

public enum Orientation
{
    Horizontal,
    Vertical,
    Both
}

public static class OrientationHelper
{
    public static Orientation GetDominantOrientation(Vector2 vector)
    {
        float absX = Math.Abs(vector.X);
        float absY = Math.Abs(vector.Y);

        return (absX > absY) ? Orientation.Horizontal : Orientation.Vertical;
    }

    /// <summary>
    /// Calculate how much a vector is in the given orientation
    /// </summary>
    /// <param name="vector">the vector</param>
    /// <param name="orientation">horizontal, vertical or both</param>
    /// <returns>a value between 0 and 1 where 0 represents not at all / orthogonal, and 1 represents completely aligned</returns>
    public static float GetOrientationAmount(Vector2 vector, Orientation orientation)
    {
        if (orientation == Orientation.Both)
            return 1f;

        float absX = Math.Abs(vector.X);
        float absY = Math.Abs(vector.Y);
        float total = absX + absY;

        if (total == 0)
            return 0; // It's a zero vector

        if (orientation == Orientation.Horizontal)
        {
            return absX / total;
        }
        else
        {
            return absY / total;
        }
    }
}