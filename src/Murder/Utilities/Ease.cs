using Murder.Diagnostics;
using System;

namespace Murder.Utilities
{
    /// <summary>
    /// Static class with useful easer functions that can be used by Tweens.
    /// This was copied from:
    /// https://github.com/kylepulver/Otter/blob/master/Otter/Utility/Glide/Ease.cs
    /// </summary>
    public static partial class Ease
    {
        const float PI = 3.14159f;
        const float PI2 = PI / 2;
        const float B1 = 1 / 2.75f;
        const float B2 = 2 / 2.75f;
        const float B3 = 1.5f / 2.75f;
        const float B4 = 2.5f / 2.75f;
        const float B5 = 2.25f / 2.75f;
        const float B6 = 2.625f / 2.75f;

        /// <summary>
        /// Ease a value to its target and then back. Use this to wrap another easing function.
        /// </summary>
        public static Func<float, float> ToAndFrom(Func<float, float> easer)
        {
            return t => ToAndFrom(easer(t));
        }

        /// <summary>
        /// Do an ease according to <paramref name="kind"/>.
        /// </summary>
        public static float Evaluate(float t, EaseKind kind)
        {
            switch (kind)
            {
                case EaseKind.CubeInOut:
                    return CubeInOut(t);
                case EaseKind.ToAndFro:
                    return ToAndFrom(t);
                case EaseKind.Linear:
                    return Linear(t);
                case EaseKind.ElasticIn:
                    return ElasticIn(t);
                case EaseKind.ElasticOut:
                    return ElasticOut(t);
                case EaseKind.ElasticInOut:
                    return ElasticInOut(t);
                case EaseKind.QuadIn:
                    return QuadIn(t);
                case EaseKind.QuadOut:
                    return QuadOut(t);
                case EaseKind.QuadInOut:
                    return QuadInOut(t);
                case EaseKind.CubeIn:
                    return CubeIn(t);
                case EaseKind.CubeOut:
                    return CubeOut(t);
                case EaseKind.QuartIn:
                    return QuartIn(t);
                case EaseKind.QuartOut:
                    return QuartOut(t);
                case EaseKind.QuartInOut:
                    return QuartInOut(t);
                case EaseKind.QuintIn:
                    return QuintIn(t);
                case EaseKind.QuintOut:
                    return QuintOut(t);
                case EaseKind.QuintInOut:
                    return QuintInOut(t);
                case EaseKind.SineIn:
                    return SineIn(t);
                case EaseKind.SineOut:
                    return SineOut(t);
                case EaseKind.SineInOut:
                    return SineInOut(t);
                case EaseKind.BounceIn:
                    return BounceIn(t);
                case EaseKind.BounceOut:
                    return BounceOut(t);
                case EaseKind.BounceInOut:
                    return BounceInOut(t);
                case EaseKind.CircIn:
                    return CircIn(t);
                case EaseKind.CircOut:
                    return CircOut(t);
                case EaseKind.CircInOut:
                    return CircInOut(t);
                case EaseKind.ExpoIn:
                    return ExpoIn(t);
                case EaseKind.ExpoOut:
                    return ExpoOut(t);
                case EaseKind.ExpoInOut:
                    return ExpoInOut(t);
                case EaseKind.BackIn:
                    return BackIn(t);
                case EaseKind.BackOutSm:
                    return BackOutSm(t);
                case EaseKind.BackOut:
                    return BackOut(t);
                case EaseKind.BackInOut:
                    return BackInOut(t);
                default:
                    GameLogger.Warning("Implement this Ease kind!");
                    return CubeInOut(t);
            }
        }

        /// <summary>
        /// Ease a value to its target and then back.
        /// </summary>
        public static float ToAndFrom(float t)
        {
            return t < 0.5f ? t * 2 : 1 + ((t - 0.5f) / 0.5f) * -1;
        }

        /// <summary>
        /// Linear.
        /// </summary>
        /// <param name="t">Time.</param>
        /// <returns>Eased timescale.</returns>
        public static float Linear(float t)
        {
            return t;
        }

        /// <summary>
        /// Elastic in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float ElasticIn(float t)
        {
            return (float)(Math.Sin(13 * PI2 * t) * Math.Pow(2, 10 * (t - 1)));
        }

        /// <summary>
        /// Elastic out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float ElasticOut(float t)
        {
            if (t == 1) return 1;
            return (float)(Math.Sin(-13 * PI2 * (t + 1)) * Math.Pow(2, -10 * t) + 1);
        }

        /// <summary>
        /// Elastic in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float ElasticInOut(float t)
        {
            if (t < 0.5)
            {
                return (float)(0.5 * Math.Sin(13 * PI2 * (2 * t)) * Math.Pow(2, 10 * ((2 * t) - 1)));
            }

            return (float)(0.5 * (Math.Sin(-13 * PI2 * ((2 * t - 1) + 1)) * Math.Pow(2, -10 * (2 * t - 1)) + 2));
        }

        /// <summary>
        /// Quadratic in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuadIn(float t)
        {
            return (float)(t * t);
        }

        /// <summary>
        /// Quadratic out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuadOut(float t)
        {
            return (float)(-t * (t - 2));
        }

        /// <summary>
        /// Quadratic in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuadInOut(float t)
        {
            return (float)(t <= .5 ? t * t * 2 : 1 - (--t) * t * 2);
        }

        /// <summary>
        /// Cubic in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float CubeIn(float t)
        {
            return (float)(t * t * t);
        }

        /// <summary>
        /// Cubic out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float CubeOut(float t)
        {
            return (float)(1 + (--t) * t * t);
        }

        /// <summary>
        /// Cubic in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float CubeInOut(float t)
        {
            return (float)(t <= .5 ? t * t * t * 4 : 1 + (--t) * t * t * 4);
        }

        /// <summary>
        /// Quart in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuartIn(float t)
        {
            return (float)(t * t * t * t);
        }

        /// <summary>
        /// Quart out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuartOut(float t)
        {
            return (float)(1 - (t -= 1) * t * t * t);
        }

        /// <summary>
        /// Quart in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuartInOut(float t)
        {
            return (float)(t <= .5 ? t * t * t * t * 8 : (1 - (t = t * 2 - 2) * t * t * t) / 2 + .5);
        }

        /// <summary>
        /// Quint in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuintIn(float t)
        {
            return (float)(t * t * t * t * t);
        }

        /// <summary>
        /// Quint out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuintOut(float t)
        {
            return (float)((t = t - 1) * t * t * t * t + 1);
        }

        /// <summary>
        /// Quint in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float QuintInOut(float t)
        {
            return (float)(((t *= 2) < 1) ? (t * t * t * t * t) / 2 : ((t -= 2) * t * t * t * t + 2) / 2);
        }

        /// <summary>
        /// Sine in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float SineIn(float t)
        {
            if (t == 1) return 1;
            return (float)(-Math.Cos(PI2 * t) + 1);
        }

        /// <summary>
        /// Sine out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float SineOut(float t)
        {
            return (float)(Math.Sin(PI2 * t));
        }

        /// <summary>
        /// Sine in and out
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float SineInOut(float t)
        {
            return (float)(-Math.Cos(PI * t) / 2 + .5);
        }

        /// <summary>
        /// Bounce in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float BounceIn(float t)
        {
            t = 1 - t;
            if (t < B1) return (float)(1 - 7.5625 * t * t);
            if (t < B2) return (float)(1 - (7.5625 * (t - B3) * (t - B3) + .75));
            if (t < B4) return (float)(1 - (7.5625 * (t - B5) * (t - B5) + .9375));
            return (float)(1 - (7.5625 * (t - B6) * (t - B6) + .984375));
        }

        /// <summary>
        /// Bounce out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float BounceOut(float t)
        {
            if (t < B1) return (float)(7.5625 * t * t);
            if (t < B2) return (float)(7.5625 * (t - B3) * (t - B3) + .75);
            if (t < B4) return (float)(7.5625 * (t - B5) * (t - B5) + .9375);
            return (float)(7.5625 * (t - B6) * (t - B6) + .984375);
        }

        /// <summary>
        /// Bounce in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float BounceInOut(float t)
        {
            if (t < .5)
            {
                t = 1 - t * 2;
                if (t < B1) return (float)((1 - 7.5625 * t * t) / 2);
                if (t < B2) return (float)((1 - (7.5625 * (t - B3) * (t - B3) + .75)) / 2);
                if (t < B4) return (float)((1 - (7.5625 * (t - B5) * (t - B5) + .9375)) / 2);
                return (float)((1 - (7.5625 * (t - B6) * (t - B6) + .984375)) / 2);
            }
            t = t * 2 - 1;
            if (t < B1) return (float)((7.5625 * t * t) / 2 + .5);
            if (t < B2) return (float)((7.5625 * (t - B3) * (t - B3) + .75) / 2 + .5);
            if (t < B4) return (float)((7.5625 * (t - B5) * (t - B5) + .9375) / 2 + .5);
            return (float)((7.5625 * (t - B6) * (t - B6) + .984375) / 2 + .5);
        }

        /// <summary>
        /// Circle in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float CircIn(float t)
        {
            return (float)(-(Math.Sqrt(1 - t * t) - 1));
        }

        /// <summary>
        /// Circle out
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float CircOut(float t)
        {
            return (float)(Math.Sqrt(1 - (t - 1) * (t - 1)));
        }

        /// <summary>
        /// Circle in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float CircInOut(float t)
        {
            return (float)(t <= .5 ? (Math.Sqrt(1 - t * t * 4) - 1) / -2 : (Math.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2);
        }

        /// <summary>
        /// Exponential in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float ExpoIn(float t)
        {
            return (float)(Math.Pow(2, 10 * (t - 1)));
        }

        /// <summary>
        /// Exponential out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float ExpoOut(float t)
        {
            if (t == 1) return 1;
            return (float)(-Math.Pow(2, -10 * t) + 1);
        }

        /// <summary>
        /// Exponential in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float ExpoInOut(float t)
        {
            if (t == 1) return 1;
            return (float)(t < .5 ? Math.Pow(2, 10 * (t * 2 - 1)) / 2 : (-Math.Pow(2, -10 * (t * 2 - 1)) + 2) / 2);
        }

        /// <summary>
        /// Back in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float BackIn(float t)
        {
            return (float)(t * t * (2.70158 * t - 1.70158));
        }

        /// <summary>
        /// Back out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float BackOutSm(float t)
        {
            return (float)(1 - (--t) * (t) * (-1.5f * t - 0.5f));
        }
        /// <summary>
        /// Back out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float BackOut(float t)
        {
            return (float)(1 - (--t) * (t) * (-2.70158 * t - 1.70158));
        }

        /// <summary>
        /// Back in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static float BackInOut(float t)
        {
            t *= 2;
            if (t < 1) return (float)(t * t * (2.70158 * t - 1.70158) / 2);
            t--;
            return (float)((1 - (--t) * (t) * (-2.70158 * t - 1.70158)) / 2 + .5);
        }

        public static float ZeroToOne(Func<float, float> easeMethod, float duration, float tweenStart)
        {
            var delta = Math.Clamp(Game.Now - tweenStart, 0, duration) / duration;
            return easeMethod(delta);
        }


        public static float JumpArc(float t)
        {
            return MathF.Sin(Calculator.Clamp01(t) * MathF.PI);
        }
        // Predefined curve representing the intensity of the flicker effect over time
        private static readonly float[] flickerCurve = {
        0.0f, 0.2f, 0.5f, 0.8f, 1.0f, 0.7f,  1.0f, 1.0f,0.4f, 0.3f, 0.6f, 0.9f, 0.6f, 0.3f, 0.1f, 0.0f
    };

        // Method to get the flicker intensity based on a 0-1 range input, with smooth interpolation
        public static float FlickerRandom(float t)
        {
            // Clamp t to be within the 0-1 range
            t = Math.Clamp(t, 0.0f, 1.0f);

            // Map t to a fractional index in the flickerCurve array
            float scaledIndex = t * (flickerCurve.Length - 1);
            int lowerIndex = (int)Math.Floor(scaledIndex);
            int upperIndex = Math.Min(lowerIndex + 1, flickerCurve.Length - 1);

            // Get the two closest values from the curve
            float lowerValue = flickerCurve[lowerIndex];
            float upperValue = flickerCurve[upperIndex];

            // Calculate the fractional part for interpolation
            float fractionalPart = scaledIndex - lowerIndex;

            // Interpolate between the two values
            return Calculator.Lerp(lowerValue, upperValue, fractionalPart);
        }
    }
}