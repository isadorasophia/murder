using Murder.Diagnostics;

namespace Murder.Utilities
{
    /// <summary>
    /// Static class with useful easer functions that can be used by Tweens.
    /// This was copied from:
    /// https://github.com/kylepulver/Otter/blob/master/Otter/Utility/Glide/Ease.cs
    /// </summary>
    public static partial class Ease
    {
        /// <summary>
        /// Ease a value to its target and then back. Use this to wrap another easing function.
        /// </summary>
        public static Func<double, double> ToAndFrom(Func<double, double> easer)
        {
            return t => ToAndFrom(easer(t));
        }

        /// <summary>
        /// Do an ease according to <paramref name="kind"/>.
        /// </summary>
        public static double Evaluate(double t, EaseKind kind)
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
        public static double ToAndFrom(double t)
        {
            return t < 0.5f ? t * 2 : 1 + ((t - 0.5f) / 0.5f) * -1;
        }

        /// <summary>
        /// Linear.
        /// </summary>
        /// <param name="t">Time.</param>
        /// <returns>Eased timescale.</returns>
        public static double Linear(double t)
        {
            return t;
        }

        /// <summary>
        /// Elastic in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double ElasticIn(double t)
        {
            return (double)(Math.Sin(13 * PI2 * t) * Math.Pow(2, 10 * (t - 1)));
        }

        /// <summary>
        /// Elastic out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double ElasticOut(double t)
        {
            if (t == 1) return 1;
            return (double)(Math.Sin(-13 * PI2 * (t + 1)) * Math.Pow(2, -10 * t) + 1);
        }

        /// <summary>
        /// Elastic in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double ElasticInOut(double t)
        {
            if (t < 0.5)
            {
                return (double)(0.5 * Math.Sin(13 * PI2 * (2 * t)) * Math.Pow(2, 10 * ((2 * t) - 1)));
            }

            return (double)(0.5 * (Math.Sin(-13 * PI2 * ((2 * t - 1) + 1)) * Math.Pow(2, -10 * (2 * t - 1)) + 2));
        }

        /// <summary>
        /// Quadratic in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuadIn(double t)
        {
            return (double)(t * t);
        }

        /// <summary>
        /// Quadratic out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuadOut(double t)
        {
            return (double)(-t * (t - 2));
        }

        /// <summary>
        /// Quadratic in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuadInOut(double t)
        {
            return (double)(t <= .5 ? t * t * 2 : 1 - (--t) * t * 2);
        }

        /// <summary>
        /// Cubic in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double CubeIn(double t)
        {
            return (double)(t * t * t);
        }

        /// <summary>
        /// Cubic out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double CubeOut(double t)
        {
            return (double)(1 + (--t) * t * t);
        }

        /// <summary>
        /// Cubic in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double CubeInOut(double t)
        {
            return (double)(t <= .5 ? t * t * t * 4 : 1 + (--t) * t * t * 4);
        }

        /// <summary>
        /// Quart in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuartIn(double t)
        {
            return (double)(t * t * t * t);
        }

        /// <summary>
        /// Quart out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuartOut(double t)
        {
            return (double)(1 - (t -= 1) * t * t * t);
        }

        /// <summary>
        /// Quart in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuartInOut(double t)
        {
            return (double)(t <= .5 ? t * t * t * t * 8 : (1 - (t = t * 2 - 2) * t * t * t) / 2 + .5);
        }

        /// <summary>
        /// Quint in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuintIn(double t)
        {
            return (double)(t * t * t * t * t);
        }

        /// <summary>
        /// Quint out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuintOut(double t)
        {
            return (double)((t = t - 1) * t * t * t * t + 1);
        }

        /// <summary>
        /// Quint in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double QuintInOut(double t)
        {
            return (double)(((t *= 2) < 1) ? (t * t * t * t * t) / 2 : ((t -= 2) * t * t * t * t + 2) / 2);
        }

        /// <summary>
        /// Sine in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double SineIn(double t)
        {
            if (t == 1) return 1;
            return (double)(-Math.Cos(PI2 * t) + 1);
        }

        /// <summary>
        /// Sine out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double SineOut(double t)
        {
            return (double)(Math.Sin(PI2 * t));
        }

        /// <summary>
        /// Sine in and out
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double SineInOut(double t)
        {
            return (double)(-Math.Cos(PI * t) / 2 + .5);
        }

        /// <summary>
        /// Bounce in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double BounceIn(double t)
        {
            t = 1 - t;
            if (t < B1) return (double)(1 - 7.5625 * t * t);
            if (t < B2) return (double)(1 - (7.5625 * (t - B3) * (t - B3) + .75));
            if (t < B4) return (double)(1 - (7.5625 * (t - B5) * (t - B5) + .9375));
            return (double)(1 - (7.5625 * (t - B6) * (t - B6) + .984375));
        }

        /// <summary>
        /// Bounce out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double BounceOut(double t)
        {
            if (t < B1) return (double)(7.5625 * t * t);
            if (t < B2) return (double)(7.5625 * (t - B3) * (t - B3) + .75);
            if (t < B4) return (double)(7.5625 * (t - B5) * (t - B5) + .9375);
            return (double)(7.5625 * (t - B6) * (t - B6) + .984375);
        }

        /// <summary>
        /// Bounce in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double BounceInOut(double t)
        {
            if (t < .5)
            {
                t = 1 - t * 2;
                if (t < B1) return (double)((1 - 7.5625 * t * t) / 2);
                if (t < B2) return (double)((1 - (7.5625 * (t - B3) * (t - B3) + .75)) / 2);
                if (t < B4) return (double)((1 - (7.5625 * (t - B5) * (t - B5) + .9375)) / 2);
                return (double)((1 - (7.5625 * (t - B6) * (t - B6) + .984375)) / 2);
            }
            t = t * 2 - 1;
            if (t < B1) return (double)((7.5625 * t * t) / 2 + .5);
            if (t < B2) return (double)((7.5625 * (t - B3) * (t - B3) + .75) / 2 + .5);
            if (t < B4) return (double)((7.5625 * (t - B5) * (t - B5) + .9375) / 2 + .5);
            return (double)((7.5625 * (t - B6) * (t - B6) + .984375) / 2 + .5);
        }

        /// <summary>
        /// Circle in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double CircIn(double t)
        {
            return (double)(-(Math.Sqrt(1 - t * t) - 1));
        }

        /// <summary>
        /// Circle out
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double CircOut(double t)
        {
            return (double)(Math.Sqrt(1 - (t - 1) * (t - 1)));
        }

        /// <summary>
        /// Circle in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double CircInOut(double t)
        {
            return (double)(t <= .5 ? (Math.Sqrt(1 - t * t * 4) - 1) / -2 : (Math.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2);
        }

        /// <summary>
        /// Exponential in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double ExpoIn(double t)
        {
            return (double)(Math.Pow(2, 10 * (t - 1)));
        }

        /// <summary>
        /// Exponential out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double ExpoOut(double t)
        {
            if (t == 1) return 1;
            return (double)(-Math.Pow(2, -10 * t) + 1);
        }

        /// <summary>
        /// Exponential in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double ExpoInOut(double t)
        {
            if (t == 1) return 1;
            return (double)(t < .5 ? Math.Pow(2, 10 * (t * 2 - 1)) / 2 : (-Math.Pow(2, -10 * (t * 2 - 1)) + 2) / 2);
        }

        /// <summary>
        /// Back in.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double BackIn(double t)
        {
            return (double)(t * t * (2.70158 * t - 1.70158));
        }

        /// <summary>
        /// Back out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double BackOutSm(double t)
        {
            return (double)(1 - (--t) * (t) * (-1.5f * t - 0.5f));
        }
        /// <summary>
        /// Back out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double BackOut(double t)
        {
            return (double)(1 - (--t) * (t) * (-2.70158 * t - 1.70158));
        }

        /// <summary>
        /// Back in and out.
        /// </summary>
        /// <param name="t">Time elapsed.</param>
        /// <returns>Eased timescale.</returns>
        public static double BackInOut(double t)
        {
            t *= 2;
            if (t < 1) return (double)(t * t * (2.70158 * t - 1.70158) / 2);
            t--;
            return (double)((1 - (--t) * (t) * (-2.70158 * t - 1.70158)) / 2 + .5);
        }

        public static double ZeroToOne(Func<double, double> easeMethod, double duration, double tweenStart)
        {
            var delta = Math.Clamp(Game.Now - tweenStart, 0, duration) / duration;
            return easeMethod(delta);
        }
    }
}