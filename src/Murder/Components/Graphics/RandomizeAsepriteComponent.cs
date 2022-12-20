using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct RandomizeAsepriteComponent : IComponent
    {
        public readonly bool RandomizeAnimation;
        public readonly bool RandomizeAnimationStart;
        [Tooltip("In 90 deg increments")]
        public readonly bool RandomRotate;
    }
}
