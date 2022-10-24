using Bang.Components;

namespace Murder.Components
{
    public readonly struct FacingComponent : IComponent
    {
        public readonly bool Flipped;
        public readonly bool LookingUp;

        public FacingComponent(bool flipped, bool lookingUp)
        {
            Flipped = flipped;
            LookingUp = lookingUp;
        }
    }
}
