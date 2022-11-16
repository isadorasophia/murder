using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets.Graphics;
using Murder.Core.Graphics;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Data;
using Murder.Components;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Murder.Helpers
{
    /// <summary>
    /// 8 directions, enumerated counterclockwise, starting from right = 0:
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
        public static ImmutableArray<string> Cardinal = ImmutableArray.Create("e", "se", "s", "sw", "w", "nw", "n", "ne");
        public static ImmutableArray<(string, bool)> CardinalFlipped = ImmutableArray.Create(
            ("e", false), 
            ("se", false),
            ("s", false),
            ("se", true),   // sw
            ("e", true),    // w
            ("ne", true),   // nw
            ("n", false),
            ("ne", false) );

        public static ImGuiDir ToImGui(this Direction direction)
        {
            switch (direction)
            {
                    case Direction.Up: return ImGuiDir.Up;
                    case Direction.Down: return ImGuiDir.Down;
                    case Direction.Left: return ImGuiDir.Left;
                    case Direction.Right: return ImGuiDir.Right;
                default:
                    throw new Exception("Direction is not suported yet!");
            }
        }
        public static char ToIcon(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return '';
                case Direction.Down: return '';
                case Direction.Left: return '';
                case Direction.Right: return '';
                default:
                    throw new Exception("Direction is not suported yet!");
            }
        }

        public static string ToCardinal(this Direction direction)
        {
            return Cardinal[(int)direction];
        }
        public static (string, bool) ToCardinalFlipped(this Direction direction)
        {
            return CardinalFlipped[(int)direction];
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

        public static float Angle (this Direction direction)
        {
            return ((int)direction) * 2 * MathF.PI / 8f;
        }

        public static Direction FromVector(Vector2 vector)
        {
            float angle = MathF.Atan2(vector.Y, vector.X);
            int quadra = Calculator.RoundToInt(8 * angle / (2 * MathF.PI) + 8) % 8;
            return (Direction)quadra;
        }

        public static Vector2 ToVector(this Direction direction)
        {
            float angle = direction.Angle();
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static Direction Reverse(this Direction direction)
        {
            return DirectionHelper.FromVector(direction.ToVector().Reverse());
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
    }
}
