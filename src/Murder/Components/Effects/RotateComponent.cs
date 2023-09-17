using Bang.Components;

namespace Murder.Components
{
    public readonly struct RotateComponent : IComponent
    {
        /// <summary>
        /// In radians.
        /// </summary>
        public readonly float Rotation;

        public RotateComponent() { }
        public RotateComponent(float rotation)
        {
            Rotation = rotation;
        }
    }
}
