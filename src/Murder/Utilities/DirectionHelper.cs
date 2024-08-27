using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Helpers;

/// <summary>
/// 8 directions, enumerated clockwise, starting from right = 0:
/// </summary>
public enum Direction
{
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft,
    Up,
    UpRight
}

[Flags]
public enum DirectionFlags
{
    Right = 0x1,
    Down = 0x10,
    Left = 0x100,
    Up = 0x1000
}

public static class DirectionHelper
{
    public static DirectionFlags ToDirectionFlag(this Direction direction)
    {
        switch (direction)
        {
            case Direction.UpRight:
            case Direction.UpLeft:
            case Direction.Up: 
                return DirectionFlags.Up;

            case Direction.Down: 
            case Direction.DownLeft:
            case Direction.DownRight: 
                return DirectionFlags.Down;

            case Direction.Left: 
                return DirectionFlags.Left;

            case Direction.Right:
            default:
                return DirectionFlags.Right;
        }
    }

    public static bool IsDiagonal(this Direction direction)
    {
        return direction switch
        {
            Direction.UpRight => true,
            Direction.UpLeft => true,
            Direction.DownRight => true,
            Direction.DownLeft => true,
            _ => false,
        };
    }

    public static Direction RoundTo4Directions(this Direction direction, Orientation bias)
    {
        if (bias == Orientation.Horizontal)
        {
            switch (direction)
            {
                case Direction.DownRight:
                case Direction.UpRight:
                    return Direction.Right;
                case Direction.DownLeft:
                case Direction.UpLeft:
                    return Direction.Left;
                default:
                    return direction; // This will return either Right, Down, Left, or Up unchanged
            }
        }
        else
        {
            switch (direction)
            {
                case Direction.UpRight:
                case Direction.UpLeft:
                    return Direction.Up;
                case Direction.DownRight:
                case Direction.DownLeft:
                    return Direction.Down;
                default:
                    return direction; // This will return either Right, Down, Left, or Up unchanged
            }
        }
    }

    public static ImmutableArray<string> Cardinal8 = ["e", "se", "s", "sw", "w", "nw", "n", "ne"];
    public static ImmutableArray<string> Cardinal4 = ["e", "s", "w", "n"];
    public static ImmutableArray<(string, bool)> Cardinal8Flipped = [
            ("e", false),
            ("se", false),
            ("s", false),
            ("se", true),   // sw
            ("e", true),    // w
            ("ne", true),   // nw
            ("n", false),
            ("ne", false)
        ];
    public static ImmutableArray<(string, bool)> Cardinal4Flipped = [
            ("e", false),
            ("s", false),
            ("e", true),    // w
            ("n", false)
        ];

    public static string ToCardinal(this Direction direction, string n, string e, string s, string w)
    {
        switch (direction)
        {
            case Direction.Up: return n;
            case Direction.UpLeft: return n + w;
            case Direction.UpRight: return n + e;
            case Direction.Down: return s;
            case Direction.DownLeft: return s + w;
            case Direction.DownRight: return s + e;
            case Direction.Left: return w;
            case Direction.Right: return e;
            default:
                throw new Exception("Direction is not suported yet!");
        }
    }
    public static string ToCardinal4(this Direction direction, string n, string e, string s, bool verticalPriority)
    {
        switch (direction)
        {
            case Direction.Up: return n;
            case Direction.UpLeft: return verticalPriority ? n : e;
            case Direction.UpRight: return verticalPriority ? n : e;
            case Direction.Down: return s;
            case Direction.DownLeft: return verticalPriority ? s : e;
            case Direction.DownRight: return verticalPriority ? s : e;
            case Direction.Left: return e;
            case Direction.Right: return e;
            default:
                throw new Exception("Direction is not suported yet!");
        }
    }
    public static string ToCardinal(this Direction direction)
    {
        return Cardinal8[(int)direction];
    }

    public static (string, bool) ToCardinalFlipped(this Direction direction)
    {
        return Cardinal8Flipped[(int)direction];
    }

    public static (string, bool) ToCardinalFlipped(this Direction direction, string n, string e, string s)
    {
        switch (direction)
        {
            case Direction.Up: return (n, false);
            case Direction.UpLeft: return (n + e, true);
            case Direction.UpRight: return (n + e, false);
            case Direction.Down: return (s, false);
            case Direction.DownLeft: return (s + e, true);
            case Direction.DownRight: return (s + e, false);
            case Direction.Left: return (e, true);
            case Direction.Right: return (e, false);
            default:
                throw new Exception("Direction is not suported yet!");
        }
    }

    public static Direction Random()
    {
        Random random = new Random();
        switch (random.Next(8))
        {
            case 0: return Direction.Up;
            case 1: return Direction.Down;
            case 2: return Direction.Left;
            case 3: return Direction.Right;
            case 4: return Direction.UpLeft;
            case 5: return Direction.UpRight;
            case 6: return Direction.DownLeft;
            case 7: return Direction.DownRight;
            default:
                throw new Exception("This can't happen!");
        }
    }
    public static Direction RandomCardinal()
    {
        Random random = new Random();
        switch (random.Next(4))
        {
            case 0: return Direction.Up;
            case 1: return Direction.Down;
            case 2: return Direction.Left;
            case 3: return Direction.Right;
            default:
                throw new Exception("This can't happen!");
        }
    }

    /// <summary>
    /// Converts an angle (in radians) to a Direction enum.
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>The corresponding Direction.</returns>
    public static Direction FromAngle(float angle)
    {
        // Normalize the angle to be within the range [0, 2π)
        angle = (angle % (2 * MathF.PI) + 2 * MathF.PI) % (2 * MathF.PI);

        // Determine the direction based on the angle
        int directionIndex = (int)MathF.Round(8 * angle / (2 * MathF.PI)) % 8;
        return (Direction)directionIndex;
    }
    
    /// <summary>
    /// The angle of the direction, in radians.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static float ToAngle(this Direction direction)
    {
        return ((int)direction) * 2 * MathF.PI / 8f;
    }

    public static Direction FromVectorWith4Directions(Vector2 vector)
    {
        float angle = MathF.Atan2(vector.Y, vector.X);
        int quadra = Calculator.RoundToInt(4 * angle / (2 * MathF.PI) + 4) % 4;
        switch (quadra)
        {
            case 0: return Direction.Right;
            case 1: return Direction.Down;
            case 2: return Direction.Left;
            default: return Direction.Up;
        }
    }

    public static Direction FromVector(Vector2 vector)
    {
        float angle = MathF.Atan2(vector.Y, vector.X);
        int quadra = Calculator.RoundToInt(8 * angle / (2 * MathF.PI) + 8) % 8;
        return (Direction)quadra;
    }

    public static Direction FromVectorWithHorizontal(Vector2 vector)
    {
        // Check if the vector is pointing more to the left or right
        if (vector.X < 0)
        {
            return Direction.Left;
        }
        else
        {
            return Direction.Right;
        }
    }

    public static Direction FromVectorWithVertical(Vector2 vector)
    { 
        // Check if the vector is pointing more upward or downward
        if (vector.Y < 0)
        {
            return Direction.Up;
        }
        else
        {
            return Direction.Down;
        }
    }

    public static Vector2 ToVector(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                return new Vector2(1, 0);
            case Direction.DownRight:
                return new Vector2(1, 1);
            case Direction.Down:
                return new Vector2(0, 1);
            case Direction.DownLeft:
                return new Vector2(-1, 1);
            case Direction.Left:
                return new Vector2(-1, 0);
            case Direction.UpLeft:
                return new Vector2(-1, -1);
            case Direction.Up:
                return new Vector2(0, -1);
            case Direction.UpRight:
                return new Vector2(1, -1);
            default:
                float angle = direction.ToAngle();
                return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }

    public static Direction Reverse(this Direction direction)
    {
        return FromVector(direction.ToVector().Reverse());
    }

    public static ImageFlip GetFlippedHorizontal(this Direction direction)
    {
        var vector = ToVector(direction);
        // Added a small threshold to avoid flipping when the vector is very close to 0
        var horizontalFlags = (vector.X < 0 && MathF.Abs(vector.X) > 0.01f) ? ImageFlip.Horizontal : ImageFlip.None;
        return horizontalFlags;
    }

    public static ImageFlip GetFlipped(this Direction direction)
    {
        var vector = ToVector(direction);
        // Added a small threshold to avoid flipping when the vector is very close to 0
        var horizontalFlags = (vector.X < 0 && MathF.Abs(vector.X) > 0.01f) ? ImageFlip.Horizontal : ImageFlip.None;
        var verticalFlags = (vector.Y < 0 && MathF.Abs(vector.Y) > 0.01f) ? ImageFlip.Vertical : ImageFlip.None;
        return horizontalFlags | verticalFlags;
    }

    public static bool Flipped(this Direction direction)
    {
        var x = ToVector(direction).X;
        if (x < 0)
            return true;
        else
            return false;
    }
    public static Direction Invert(this Direction direction)
    {
        return DirectionHelper.FromAngle(direction.ToAngle() + MathF.PI);
    }

    public static Direction LookAtEntity(Entity e, Entity target)
    {
        Vector2 direction = target.GetGlobalTransform().Vector2 - e.GetGlobalTransform().Vector2;
        return FromVector(direction);
    }

    public static Direction LookAtPosition(Entity e, Vector2 target)
    {
        Vector2 direction = target - e.GetGlobalTransform().Vector2;
        return FromVector(direction);
    }

    /// <summary>
    /// Get the suffix from a suffix list based on an angle
    /// </summary>
    public static (string suffix, bool flip) GetSuffixFromAngle(Entity entity, AgentSpriteComponent _, float angle)
    {
        if (entity.TryGetSpriteFacing() is SpriteFacingComponent spriteFacingComponent)
        {
            return spriteFacingComponent.GetSuffixFromAngle(angle);
        }

        //GameLogger.Log($"Entity {entity} does not have a SpriteFacingComponent, using default suffixes");

        return (string.Empty, false);
    }

    public static (string name, bool flipped) GetName(int i, int totalDirections, bool flipWest)
    {
        switch (totalDirections)
        {
            case 2:
                return (i == 0 ? "s" : "n", false);
            case 4:
                return flipWest ? Cardinal4Flipped[i] : (Cardinal4[i], false);
            case 8:
                return flipWest ? Cardinal8Flipped[i] : (Cardinal8[i], false);
            default:
                throw new ArgumentException("Unsupported number of directions. Only 2, 4 or 8 are supported.");
        }
    }
}