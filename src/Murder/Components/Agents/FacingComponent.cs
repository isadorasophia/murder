using Bang.Components;
using Murder.Helpers;

namespace Murder.Components
{
    public readonly struct FacingComponent : IComponent
    {
        public readonly Direction Direction;

        /// <summary>
        /// Creates a FacingComponent using a Direction as a base.
        /// </summary>
        /// <param name="direction"></param>
        public FacingComponent(Direction direction)
        {
            Direction = direction;
        }
    }
}