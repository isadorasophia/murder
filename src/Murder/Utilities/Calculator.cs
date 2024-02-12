using Murder.Core.Geometry;
using System.Collections.Immutable;
using System.Numerics;

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


        public static (float value, float velocity) Spring(
          float value, float velocity, float targetValue,
          float damping, float frequency, float deltaTime
        )
        {
            float f = 1.0f + 2.0f * deltaTime * damping * frequency;
            float oo = frequency * frequency;
            float hoo = deltaTime * oo;
            float hhoo = deltaTime * hoo;
            float detInv = 1.0f / (f + hhoo);
            float detX = f * value + deltaTime * velocity + hhoo * targetValue;
            float detV = velocity + hoo * (targetValue - value);

            return (detX * detInv, detV * detInv);
        }

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

        /// <summary>
        /// Normalizes the given angle to be within the range of 0 to 2π radians.
        /// </summary>
        /// <param name="angle">The angle in radians.</param>
        /// <returns>The normalized angle.</returns>
        public static float NormalizeAngle(float angle)
        {
            // Normalize the angle to be within the range [0, 2π)
            return (angle % (2 * MathF.PI) + 2 * MathF.PI) % (2 * MathF.PI);
        }

        public static bool Blink(float speed, bool scaled)
        {
            if (speed == 0)
            {
                return false;
            }

            var duration = 1 / speed;
            return MathF.Round((scaled ? Game.Now : Game.NowUnscaled) * speed) % 2 == 0;
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
        public static bool AlmostEqual(float num1, float num2)
        {
            return Math.Abs(num1 - num2) <= float.Epsilon;
        }

        public static int WrapAround(int value, in int min, in int max)
        {
            if (max < min)
                throw new ArgumentException("Max must be greater than min.");

            int range = max - min + 1;

            // The modulo operation can yield a negative result for negative numbers,
            // so we adjust it to ensure the result is always between min and max.
            int wrappedValue = (value - min) % range;
            if (wrappedValue < 0)
                wrappedValue += range;

            return wrappedValue + min;
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
        /// Normalizes the given elapsed time to a range of 0 to 1 based on the specified maximum time.
        /// </summary>
        /// <param name="elapsed">The elapsed time to normalize. Typically Game.Now - startTime.</param>
        /// <param name="maxTime">The maximum time value that represents the upper bound of the normalization range. Must be greater than 0 to avoid division by zero errors.</param>
        /// <returns>A normalized time value between 0 and 1. If <paramref name="elapsed"/> is greater than <paramref name="maxTime"/>, the return value is clamped to 1. If <paramref name="elapsed"/> is less than 0, the return value is clamped to 0.</returns>
        /// <remarks>
        /// This method is useful for converting an absolute time value into a relative progress percentage. It's particularly handy for animations, transitions, or any scenario where you need to express elapsed time as a fraction of a total duration.
        /// </remarks>
        public static float ClampTime(float elapsed, float maxTime)
        {
            return Calculator.Clamp01(Math.Clamp(elapsed, 0, maxTime) / maxTime);
        }

        /// <summary>
        /// Normalizes elapsed time to a 0-1 range based on specified durations for an 'in', 'delay', and 'out' phase. The value goes from 0 to 1 then back to 0.
        /// </summary>
        /// <param name="elapsed">The total elapsed time since the beginning of the sequence. Typically Game.Now - startTime.</param>
        /// <param name="inDuration">The duration of the 'in' phase where the value ramps up to 1.</param>
        /// <param name="delayDuration">The duration of the delay phase where the value holds at 1.</param>
        /// <param name="outDuration">The duration of the 'out' phase where the value ramps down back to 0.</param>
        /// <returns>A float representing the normalized time within the 0-1 range. Returns 0 if the elapsed time is outside the total duration.</returns>
        /// <remarks>
        /// This method is useful for creating sequences with distinct phases, such as animations with an intro, a pause, and an outro.
        /// </remarks>
        public static float ClampTime(float elapsed, float inDuration, float delayDuration, float outDuration)
        {
            if (elapsed < 0 || elapsed > inDuration + delayDuration + outDuration)
            {
                return 0;
            }

            if (elapsed < inDuration)
            {
                return ClampTime(elapsed, inDuration); 
            }

            if (elapsed < inDuration + delayDuration)
            {
                return 1;
            }

            return 1 - ClampTime(elapsed - inDuration - delayDuration, outDuration);
        }

        /// <summary>
        /// Normalizes and eases elapsed time into a 0-1 range based on a maximum duration and an easing function.
        /// </summary>
        /// <param name="elapsed">The elapsed time to be normalized and eased. Typically Game.Now - startTime.</param>
        /// <param name="maxTime">The maximum time over which the normalization and easing are applied. The output will be 1 at this value.</param>
        /// <param name="ease">The type of easing function to apply to the normalized time.</param>
        /// <returns>A float representing the eased time value within the 0-1 range, based on the elapsed time and the specified easing function.</returns>
        /// <remarks>
        /// This method allows for the application of easing functions to the normalized time, making it suitable for animations or transitions where non-linear time progression is desired.
        /// </remarks>
        public static float ClampTime(float elapsed, float maxTime, EaseKind ease)
        {
            return Ease.Evaluate(Clamp01(Math.Clamp(elapsed, 0, maxTime) / maxTime), ease);
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

        public static float Remap(float input, float min, float max)
        {
            return min + (input) * (max - min) ;
        }
        
        public static float Lerp(float origin, float target, float factor)
        {
            return origin * (1 - factor) + target * factor;
        }
        public static int LerpInt(float origin, float target, float factor)
        {
            return RoundToInt(origin * (1 - factor) + target * factor);
        }

        public static double LerpSnap(float origin, float target, double factor, float threshold = 0.01f)
        {
            return Math.Abs(target - origin) < threshold ? target : origin * (1 - factor) + target * factor;
        }

        public static float LerpSnap(float origin, float target, float factor, float threshold = 0.01f)
        {
            return Math.Abs(target - origin) < threshold ? target : origin * (1 - factor) + target * factor;
        }

        public static int FloorToInt(float v) => (int)MathF.Floor(v);

        public static int CeilToInt(float v) => (int)MathF.Ceiling(v);
        public static int PolarSnapToInt(float v) => (int)(MathF.Sign(v) * Math.Ceiling(MathF.Abs(v)));

        public static int RoundToInt(double v) => RoundToInt((float)v);

        /// <summary>
        /// Rounds and converts a number to integer with <see cref="MathF.Round(float)"/>.
        /// </summary>
        public static int RoundToInt(float v) => (int)MathF.Round(v);

        public static int RoundToEven(float v) => (int)MathF.Round(v / 2, MidpointRounding.AwayFromZero) * 2;

        public static Point ToPoint(this Vector2 vector) => new(RoundToInt(vector.X), RoundToInt(vector.Y));
        public static Vector2 ToSysVector2(this Microsoft.Xna.Framework.Point point) => new((float)point.X, (float)point.Y);
        public static Vector2 ToSysVector2(this Microsoft.Xna.Framework.Vector2 vector) => new(vector.X, vector.Y);
        public static Microsoft.Xna.Framework.Vector2 ToXnaVector2(this Vector2 vector) => new(vector.X, vector.Y);
        public static Vector2 ToCore(this Vector2 vector) => new(vector.X, vector.Y);

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

        public static int Pow(int x, int y)
        {
            int ret = 1;

            while (y != 0)
            {
                if ((y & 1) == 1)
                    ret *= x;
                x *= x;
                y >>= 1;
            }

            return ret;
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

        /// <summary>
        /// Generates a normalized sine wave value oscillating between 0 and 1.
        /// </summary>
        /// <param name="speed">The speed of the oscillation. Higher values result in faster oscillation.</param>
        /// <param name="scaled">If set to true, the oscillation is based on scaled game time, accommodating game time scaling effects like slow motion. If false, uses unscaled game time, reflecting real-world time regardless of game time manipulation.</param>
        /// <returns>A float value between 0 and 1 representing the current position in the sine wave cycle, based on the game's time.</returns>
        /// <remarks>
        /// This method is useful for creating oscillating effects that need to be normalized within a 0 to 1 range, such as transparency, volume, or other properties that require smooth periodic variation over time.
        /// </remarks>
        public static float Wave(float speed, bool scaled = false)
        {
            return (1 + (float)Math.Sin((scaled ? Game.Now : Game.NowUnscaled) * speed)) / 2f;
        }
        /// <summary>
        /// Generates a sinusoidal wave value oscillating between a specified minimum and maximum.
        /// </summary>
        /// <param name="speed">The speed of the oscillation. Higher values result in faster oscillation.</param>
        /// <param name="min">The minimum value of the wave.</param>
        /// <param name="max">The maximum value of the wave.</param>
        /// <param name="scaled">If true, uses scaled game time for the oscillation; otherwise, uses unscaled game time.</param>
        /// <returns>A float value representing the current value of the wave, oscillating between <paramref name="min"/> and <paramref name="max"/>.</returns>
        /// /// <remarks>
        /// This method is useful for creating oscillating effects, such as transparency, volume, or other properties that require smooth periodic variation over time.
        /// </remarks>
        public static float Wave(float speed, float min, float max, bool scaled = false)
        {
            // Calculate the base sine wave, oscillating between -1 and 1.
            float sineWave = (float)Math.Sin((scaled ? Game.Now : Game.NowUnscaled) * speed);

            // Normalize the sine wave to a 0 to 1 range.
            float normalizedSineWave = (sineWave + 1) / 2f;

            // Scale the normalized wave to the desired range [min, max].
            return min + (normalizedSineWave * (max - min));
        }

        /// <summary>
        /// Converts a value to a spring oscillation.
        /// </summary>
        /// <param name="t">Value, where 1 is fully oscillating and 0 is stopped.</param>
        /// <param name="frequency">Frequency of the oscillation.</param>
        /// <returns>A spring oscillation value between -1 and 1.</returns>
        public static float ToSpringOscillation(float t, float frequency)
        {
            // For values below 0, there's no movement
            if (t <= 0) return 0;

            // Normalize the input so values > 1 will have the same amplitude as 1
            float normalizedT = (t > 1) ? 1 : t;

            // Adjust the frequency of oscillation based on the provided parameter.
            // The damping factor (Math.Exp(-7 * normalizedT)) ensures the oscillation reduces as 'normalizedT' approaches 0.
            return MathF.Sin(frequency * t * t) * MathF.Exp(-7 * normalizedT);
        }

        /// <summary>
        /// Snap the current angle into <paramref name="steps"/> degrees.
        /// </summary>
        /// <param name="finalAngle">The angle to snap.</param>
        /// <param name="steps">The number of degrees to snap to.</param>
        /// <returns>The snapped angle.</returns>
        public static float SnapAngle(float finalAngle, int steps)
        {
            float snappedAngle = MathF.Round(finalAngle / steps) * steps;
            return snappedAngle;
        }
        #endregion
    }
}