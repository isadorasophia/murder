using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// Makes that the animation plays at a different rate.
    /// </summary>
    public readonly struct AnimationSpeedOverload : IComponent
    {
        public readonly float Rate;
        public readonly bool Persist;

        public AnimationSpeedOverload(float rate, bool persist)
        {
            Rate = rate;
            Persist = persist;
        }
    }
}
