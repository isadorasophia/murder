using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [CustomName(" Randomize Sprite")]
    public readonly struct RandomizeSpriteComponent : IComponent
    {
        public readonly bool RandomizeAnimation;
        public readonly bool RandomizeAnimationStart;
        [Tooltip("In 90 deg increments")]
        public readonly bool RandomRotate;
        public readonly bool RandomFlip;

    }
}