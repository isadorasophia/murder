using Bang.Components;

namespace Murder.Components
{
    public class RotateComponent : IComponent
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
