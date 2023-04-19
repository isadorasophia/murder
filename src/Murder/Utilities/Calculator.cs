using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using System;
using System.Collections;
using System.Collections.Immutable;

namespace Murder.Utilities
{
    /// <summary>
    /// Calculator helper class.
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// Default layers count.
        /// TODO: Make this customizable.
        /// </summary>
        public static int LayersCount = 65536; // 16 bits layers [-32768, 32768]

        public const float TO_DEG = 180 / MathF.PI;
        public const float TO_RAD = MathF.PI / 180;

        #region Lists and Arrays

        public static T? TryGet<T>(this ImmutableArray<T> values, int index) where T : struct
        {
            if (index < values.Length)
            {
                return values[index];
            }
            else
                return null;
        }

        public static T? TryGet<T>(this IList<T> values, int index) where T : struct
        {
            if (index < values.Count)
            {
                return values[index];
            }
            else
                return null;
        }

        /// <summary>
        /// Add <paramref name="item"/> to <paramref name="list"/>. Skip if already present.
        /// Cost O(n).
        /// </summary>
        public static T AddOnce<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }

            return item;
        }

        public static T[] RepeatingArray<T>(T value, int size)
        {
            var array = new T[size];
            array.Populate(value);
            return array;
        }

        public static int OneD(this Point p, int width)
        {
            return p.X + p.Y * width;
        }

        public static int OneD(int x, int y, int width)
        {
            return x + y * width;
        }

        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        #endregion

        #region Math

        public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
        {
            float a = -0.5f * p0 + 1.5f * p1 - 1.5f * p2 + 0.5f * p3;
            float b = p0 - 2.5f * p1 + 2f * p2 - 0.5f * p3;
            float c = -0.5f * p0 + 0.5f * p2;
            float d = p1;

            return a * t * t * t + b * t * t + c * t + d;
        }

        public static float InterpolateSmoothCurve(IList<float> values, float t)
        {
            int count = values.Count;
            if (count == 0)
                return 0;

            if (count == 1)
                return values[0];

            float scaledT = t * (count - 1);
            int index = Math.Clamp(FloorToInt(scaledT), 0, count - 1);

            float p0 = values[Math.Max(index - 1, 0)];
            float p1 = values[index];
            float p2 = values[Math.Min(index + 1, count - 1)];
            float p3 = values[Math.Min(index + 2, count - 1)];

            float localT = scaledT - index;

            return CatmullRom(p0, p1, p2, p3, localT);
        }


        public static float SmoothStep(float value, float min, float max)
        {
            bool invert = false;
            if (max < min)
            {
                var temp = max;
                max = min;
                min = temp;
                invert = true;
            }

            // Clamp the value between min and max
            float clampedValue = Math.Clamp(value, min, max);

            // Normalize the value to a range of 0 to 1
            float t = (clampedValue - min) / (max - min);

            // Apply the smoothstep function
            float smoothStepValue = t * t * (3.0f - 2.0f * t);

            if (invert)
                return smoothStepValue;
            else
                return 1 - smoothStepValue;
        }

        public static Vector2 GetPositionInSemicircle(float ratio, Vector2 center, float radius, float startAngle, float endAngle)
        {
            // Convert the start and end angles from degrees to radians
            float startAngleRadians = startAngle * TO_RAD;
            float endAngleRadians = endAngle * TO_RAD;

            // Convert the ratio to an angle in radians within the specified range
            float angleRadians = Calculator.Lerp(startAngleRadians, endAngleRadians, ratio);


            // Calculate the x and y coordinates of the sprite on the semicircle
            float x = center.X + radius * MathF.Cos(angleRadians);
            float y = center.Y + radius * MathF.Sin(angleRadians);

            return new Vector2(x, y);
        }

        public static bool Blink(float speed, bool scaled)
        {
            if (speed == 0)
            {
                return false;
            }

            var duration = 1 / speed;
            return MathF.Round((scaled ? Game.Now : Game.NowUnescaled) * speed) % 2 == 0;
        }

        public static bool SameSign(float num1, float num2)
        {
            return num1 == 0 || num2 == 0 || num1 > 0 && num2 > 0 || num1 < 0 && num2 < 0;
        }

        public static bool SameSignOrSimilar(float num1, float num2)
        {
            return
                num1 == 0 || num2 == 0 ||
                num1 > 0 && num2 > 0 || num1 < 0 && num2 < 0 ||
                MathF.Abs(num1 - num2) < float.Epsilon;
        }

        public static int WrapAround(int value, in int min, in int max)
        {
            if (max < 0)
                return 0;

            while (value < min)
                value += max + 1;
            while (value > max)
                value -= max + 1;

            return value;
        }

        public static float ConvertLayerToLayerDepth(int layer)
        {
            return (LayersCount / 2 - layer) / (float)LayersCount;
        }

        public static int ConvertLayerDepthToLayer(float layerDepth)
        {
            return (LayersCount / 2) - (int)(layerDepth * LayersCount);
        }

        /// <summary>
        /// Takes an elapsed time and coverts it to a 0-1 range
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="maxTime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static float ClampTime(float elapsed, float maxTime)
        {
            return Calculator.Clamp01(Math.Clamp(elapsed, 0, maxTime) / maxTime);
        }

        public static float Approach(float from, float target, float amount)
        {
            if (from > target)
                return Math.Max(from - amount, target);
            else
                return Math.Min(from + amount, target);
        }

        public static Vector2 Approach(in Vector2 from, in Vector2 target, float amount)
        {
            if (from == target)
                return target;

            var diff = target - from;
            if (diff.LengthSquared() <= amount * amount)
                return target;
            else
                return from + diff.Normalized() * amount;
        }

        public static Vector2 Normalized(this in Vector2 vector2)
        {
            var result = vector2;
            result.Normalized();
            return result;
        }

        public static float Clamp01(float v) => Math.Clamp(v, 0.0f, 1.0f);
        public static float Clamp01(int v) => Math.Clamp(v, 0, 1);

        public static float Remap(float input, float inputMin, float inputMax, float min, float max)
        {
            return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
        }
        public static float Lerp(float origin, float target, float factor)
        {
            return origin * (1 - factor) + target * factor;
        }
        public static int LerpInt(float origin, float target, float factor)
        {
            return RoundToInt(origin * (1 - factor) + target * factor);
        }

        public static float LerpSnap(float origin, float target, float factor, float threshold = 0.01f)
        {
            return Math.Abs(target - origin) < threshold ? target : origin * (1 - factor) + target * factor;
        }

        public static int FloorToInt(float v) => (int)MathF.Floor(v);

        public static int CeilToInt(float v) => (int)MathF.Ceiling(v);
        public static int PolarSnapToInt(float v) => (int)(MathF.Sign(v) * Math.Ceiling(MathF.Abs(v)));

        /// <summary>
        /// Rounds and converts a number to integer with <see cref="MathF.Round(float)"/>.
        /// </summary>
        public static int RoundToInt(float v) => (int)MathF.Round(v);

        public static int RoundToEven(float v) => (int)MathF.Round(v / 2, MidpointRounding.AwayFromZero) * 2;

        public static Point ToPoint(this System.Numerics.Vector2 vector) => new Point(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));
        public static System.Numerics.Vector2 ToSysVector2(this Microsoft.Xna.Framework.Point point) => new System.Numerics.Vector2((float)point.X, (float)point.Y);
        public static System.Numerics.Vector2 ToSysVector2(this Microsoft.Xna.Framework.Vector2 vector) => new System.Numerics.Vector2(vector.X, vector.Y);
        public static Vector2 ToXnaVector2(this System.Numerics.Vector2 vector) => new Vector2(vector.X, vector.Y);
        public static Vector2 ToCore(this System.Numerics.Vector2 vector) => new Vector2(vector.X, vector.Y);

        public static int ManhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }

        public static float Min(params float[] values)
        {
            var min = float.MaxValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < min)
                    min = values[i];
            }

            return min;
        }

        public static float Max(params float[] values)
        {
            var max = float.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > max)
                    max = values[i];
            }

            return max;
        }

        #endregion

        #region random

        public static Vector2 RandomPointInCircleEdge()
        {
            var angle = Random.Shared.NextDouble() * Math.PI * 2;
            var x = Math.Cos(angle);
            var y = Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }
        public static Vector2 RandomPointInsideCircle()
        {
            var angle = Random.Shared.NextDouble() * Math.PI * 2;
            var radius = Math.Sqrt(Random.Shared.NextDouble());
            var x = radius * Math.Cos(angle);
            var y = radius * Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// Returns if a value if zero withing a margin of error. (epsilon)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static bool IsAlmostZero(float value)
        {
            if (value < float.Epsilon && value > -float.Epsilon)
            {
                return true;
            }
            return false;
        }

        public static float ClampNearZero(float value, float minimum)
        {
            if (Math.Abs(value) < minimum)
                return minimum * Calculator.CleverSign(value);

            return value;
        }

        /// <summary>
        /// Returns the sign of a value. 1 if positive or Zero, -1 if negative.
        /// </summary>
        private static float CleverSign(float value)
        {
            if (value > 0)
                return 1;
            else if (value < 0)
                return -1;
            else
                return 1;
        }
        #endregion
    }
}