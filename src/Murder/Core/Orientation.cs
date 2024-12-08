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