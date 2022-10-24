namespace Murder.Utilities
{
    public static class Time
    {
        public static float Elapsed => Game.Instance.ElapsedTime;
        
        public static float Sin(float duration, float amplitude) => Sin(duration) * amplitude;

        public static float Sin(float duration)
        {
            if (duration > 0)
                return MathF.Sin(Elapsed * 2 * MathF.PI / duration);
            else
                return 0;
        }
    }
}
