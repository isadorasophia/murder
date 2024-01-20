using Bang.Entities;
using Microsoft.Xna.Framework.Graphics;
using Murder.Components;
using Murder.Core;
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

public static class DirectionHelper
{
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

    public static Vector2 ToVector(this Direction direction)
    {
        float angle = direction.ToAngle();
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }

    public static Direction Reverse(this Direction direction)
    {
        return FromVector(direction.ToVector().Reverse());
    }

    public static SpriteEffects GetFlipped(this Direction direction)
    {
        var x = ToVector(direction).X;
        if (x < 0)
            return SpriteEffects.FlipHorizontally;
        else
            return SpriteEffects.None;
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
    public static (string suffix, bool flip) GetSuffixFromAngle(Entity entity, AgentSpriteComponent sprite, float angle)
    {
        if (entity.TryGetSpriteFacing() is SpriteFacingComponent spriteFacingComponent)
        {
            return spriteFacingComponent.GetSuffixFromAngle(angle);
        }
        
        float delta = angle / (MathF.PI * 2); // Gives us an angle from 0 to 1, with 0 being right and 0.5 being left
        var suffixes = sprite.Suffix.Split(',');
        var finalAngle = delta - (sprite.AngleSuffixOffset * Calculator.TO_RAD) / (MathF.PI * 2);
        string suffix = suffixes[Calculator.WrapAround(Calculator.RoundToInt(suffixes.Length * finalAngle), 0, suffixes.Length - 1)];
        bool flip = false;
        if (sprite.FlipWest && delta > 0.25f && delta < 0.75f)
        {
            flip = true;
        }

        return (suffix, flip);
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