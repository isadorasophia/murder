using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct RotationComponent : IComponent
    {
        /// <summary>
        /// In radians.
        /// </summary>
        [Slider(0f, MathF.PI * 2)]
        public readonly float Rotation;

        public RotationComponent() { }
        public RotationComponent(float rotation)
        {
            Rotation = rotation;
        }
    }
}