using Murder.Core.Geometry;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;

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


        /// <summary>
        /// Returns from 0 if vectors point in opposite directions to 1 if they point to the same direction.
        /// </summary>
        internal static float Vector2Similarity(Vector2 a, Vector2 b)
        {
            if (a == Vector2.Zero || b == Vector2.Zero)
                return 0;
            Vector2 aNormalized = a.NormalizedWithSanity();
            Vector2 bNormalized = b.NormalizedWithSanity();

            return Remap(Vector2.Dot(aNormalized, bNormalized), -1, 1, 0, 1);
        }

        public static Vector2 ClosestPointOnSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            // Vector from point A to point B
            Vector2 ab = b - a;

            // Vector from point A to the circle center (X, Y)
            Vector2 ac = point - a;

            // Project ac onto ab, but limit the projection to be within the segment
            float abLengthSquared = ab.LengthSquared();
            float projection = Vector2.Dot(ac, ab) / abLengthSquared;

            // Clamp the projection value between 0 and 1 to ensure the closest point is within the segment
            projection = Math.Clamp(projection, 0, 1);

            // The closest point is A + projection * AB
            return a + projection * ab;
        }

        public static (Vector2 point, float delta) DetailedClosestPointOnSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            // Vector from point A to point B
            Vector2 ab = b - a;

            // Vector from point A to the circle center (X, Y)
            Vector2 ac = point - a;

            // Project ac onto ab, but limit the projection to be within the segment
            float abLengthSquared = ab.LengthSquared();
            float projection = Vector2.Dot(ac, ab) / abLengthSquared;

            // Clamp the projection value between 0 and 1 to ensure the closest point is within the segment
            projection = Math.Clamp(projection, 0, 1);

            // The closest point is A + projection * AB
            return (a + projection * ab, projection);
        }

        public static float DistancePointToLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            float lineLengthSquared = (lineEnd - lineStart).LengthSquared();
            if (lineLengthSquared == 0.0) return Vector2.Distance(point, lineStart);

            // Project point onto the line, clamping to the line segment
            float t = Math.Clamp(Vector2.Dot(point - lineStart, lineEnd - lineStart) / lineLengthSquared, 0, 1);
            Vector2 projection = lineStart + t * (lineEnd - lineStart);

            // Return the distance from the point to the projection
            return Vector2.Distance(point, projection);
        }

        public static bool IsInteger(float value)
        {
            return value % 1 == 0;
        }
        public static bool IsInteger(Vector2 value)
        {
            return value.X % 1 == 0 && value.Y % 1 == 0;
        }
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
            {
                return min;
            }

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

        public static float ClampTime(float start, float now, float duration)
        {
            return ClampTime(now - start, duration);
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
            return Calculator.Clamp01(Math.Clamp(elapsed, 0, Math.Max(0, maxTime)) / maxTime);
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
            return Ease.Evaluate(ClampTime(elapsed, maxTime), ease);
        }

        /// <summary>
        /// Maps a value within the range [0, 1] to a parabolic shape, where the corners are mapped to 0 and the center is mapped to 1.
        /// The shape of the parabola can be controlled by the shape parameter.
        /// </summary>
        /// <param name="x">The input value within the range [0, 1].</param>
        /// <param name="shape">
        /// The power to which the parabolic function is raised, controlling the sharpness of the curve.
        /// Typical range is [0.5, 3]:
        /// - Values less than 1 make the curve wider and flatter.
        /// - Values greater than 1 make the curve sharper and more peaked.
        /// </param>
        /// <returns>A float value representing the parabolic mapping of the input value.</returns>
        public static float Parabola(float x, float shape)
        {
            return MathF.Pow(4.0f * x * (1.0f - x), shape);
        }

        /// <summary>
        /// Maps a value within the range [0, 1] to a power curve shape, where the corners are mapped to 0.
        /// The shape of the curve can be controlled independently on either side of the curve using the parameters a and b.
        /// </summary>
        /// <param name="x">The input value within the range [0, 1].</param>
        /// <param name="a">
        /// The power controlling the shape of the curve near the left corner (x = 0).
        /// Typical range is [0.5, 3]:
        /// - Lower values (&lt; 1) create a steeper rise near x = 0.
        /// - Higher values (&gt; 1) create a more gradual rise near x = 0.
        /// </param>
        /// <param name="b">
        /// The power controlling the shape of the curve near the right corner (x = 1).
        /// Typical range is [0.5, 3]:
        /// - Lower values (&lt; 1) create a steeper drop near x = 1.
        /// - Higher values (&gt; 1) create a more gradual drop near x = 1.
        /// </param>
        /// <returns>A float value representing the power curve mapping of the input value.</returns>
        public static float PowerCurve(float x, float a, float b)
        {
            float k = MathF.Pow(a + b, a + b) / (MathF.Pow(a, a) * MathF.Pow(b, b));
            return k * MathF.Pow(x, a) * MathF.Pow(1.0f - x, b);
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

        public static Vector2 Lerp(Vector2 origin, Vector2 target, float factor)
        {
            return new Vector2(Lerp(origin.X, target.X, factor), Lerp(origin.Y, target.Y, factor));
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

        /// <summary>
        /// Smoothly interpolates between two vectors over time using a linear interpolation method.
        /// </summary>
        /// <param name="a">The starting vector.</param>
        /// <param name="b">The target vector.</param>
        /// <param name="deltaTime">The elapsed time since the last interpolation step.</param>
        /// <param name="halfLife">The half-life period, representing the time it takes to reach half of the remaining distance to the target vector.</param>
        /// <returns>A new vector that is the result of the smooth interpolation between the starting and target vectors.</returns>
        /// <remarks>
        /// This method interpolates each component of the vector individually using the <see cref="LerpSmooth(float, float, float, float)"/> method.
        /// It is similar to a regular linear interpolation (lerp) but is designed to work effectively even when not using a fixed timestep.
        /// This makes it particularly useful for smooth transitions in animations or physics simulations where the update interval can vary.
        /// </remarks>
        public static Vector2 LerpSmooth(Vector2 a, Vector2 b, float deltaTime, float halfLife)
        {
            return new Vector2(LerpSmooth(a.X, b.X, deltaTime, halfLife), LerpSmooth(a.Y, b.Y, deltaTime, halfLife));
        }

        /// <summary>
        /// Smoothly interpolates between two angles over time using a linear interpolation method.
        /// </summary>
        /// <param name="a">The starting angle in radians.</param>
        /// <param name="b">The target angle in radians.</param>
        /// <param name="deltaTime">The elapsed time since the last interpolation step.</param>
        /// <param name="halfLife">The half-life period, representing the time it takes to reach half of the remaining distance to the target angle.</param>
        /// <returns>A new angle in radians that is the result of the smooth interpolation between the starting and target angles.</returns>
        /// <remarks>
        /// This method ensures that the interpolation takes the shortest path around the circle by normalizing the angles and adjusting them if necessary.
        /// It is particularly useful for smoothly interpolating rotational values where direct linear interpolation could result in a longer path.
        /// </remarks>
        public static float LerpSmoothAngle(float a, float b, float deltaTime, float halfLife)
        {
            a = NormalizeAngle(a);
            b = NormalizeAngle(b);
            float delta = MathF.Abs(a - b);
            if (delta > MathF.PI)
            {
                if (a > b)
                    a -= MathF.PI * 2;
                else
                    b -= MathF.PI * 2;
            }
            return LerpSmooth(a, b, deltaTime, halfLife);
        }

        /// <summary>
        /// Smoothly interpolates between two float values over time using an exponential decay formula.
        /// </summary>
        /// <param name="a">The starting value.</param>
        /// <param name="b">The target value.</param>
        /// <param name="deltaTime">The elapsed time since the last interpolation step.</param>
        /// <param name="halfLife">The half-life period, representing the time it takes to reach half of the remaining distance to the target value.</param>
        /// <returns>A new float value that is the result of the smooth interpolation between the starting and target values.</returns>
        /// <remarks>
        /// This method uses an exponential decay formula to interpolate the values, making it more suitable for smooth transitions even when not using a fixed timestep.
        /// If the difference between the two values is less than a small threshold (0.001), it directly returns the target value to avoid unnecessary calculations.
        /// </remarks>
        public static float LerpSmooth(float a, float b, float deltaTime, float halfLife)
        {
            return Math.Abs(a- b) < 0.001f? b : b + (a - b) * float.Exp2(-deltaTime / halfLife);
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

        // Generates a deterministic Vector2 within a circle based on a seed.
        public static Vector2 DeterministicVector2InACircle(int seed, float radius)
        {
            float angle = DeterministicFloat(seed, 0, 2 * MathF.PI); // Angle in radians
            float length = MathF.Sqrt(DeterministicFloat(seed * 0x1a874b9d, 0, 1)) * radius; // Square root for uniform distribution

            float x = length * MathF.Cos(angle);
            float y = length * MathF.Sin(angle);

            return new Vector2(x, y);
        }

        // Generates a deterministic Vector2 based on a seed.
        public static Vector2 DeterministicVector2(int seed, float radius)
        {
            float offsetX = (((seed * 0x8da6b343) & 0xFFFF) / (float)0xFFFF) - 0.5f;
            float offsetY = (((seed * 0xd8163841) & 0xFFFF) / (float)0xFFFF) - 0.5f;
            return new Vector2(offsetX, offsetY) * radius;
        }

        // Generates a deterministic float in the range [min, max) based on a seed.
        public static float DeterministicFloat(int seed, float min = 0.0f, float max = 1.0f)
        {
            float normalized = (((seed * 0x8da6b343) & 0xFFFF) / (float)0xFFFF);
            return min + (max - min) * normalized;
        }

        // Generates a deterministic int in the range [min, max) based on a seed.
        public static int DeterministicInt(int seed, int min, int max)
        {
            // Use a large prime number to mix the seed a bit
            int hash = (int)(seed * 0x8da6b343);
            int range = max - min;
            if (range < 0) throw new ArgumentOutOfRangeException("max must be greater than min.");

            // Simple modulo might introduce some bias, but for game purposes, it might be sufficiently random.
            return min + (Math.Abs(hash) % range);
        }

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

        #region Colors

        /// <summary>
        /// Returns the result of multiplying two unsigned 8-bit values.
        /// </summary>
        /// <param name="a">The multiplicand</param>
        /// <param name="b">The multiplier</param>
        /// <returns>The result of multiplying two unsigned 8-bit values.</returns>
        public static byte MultiplyUnsigned8Bit(byte a, int b)
        {
            int v = a * b + 0x80;
            return (byte)((v >> 8) + v >> 8);
        }
        #endregion
    }
}