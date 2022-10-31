using Bang.Components;
using Microsoft.Xna.Framework;
using Murder.Helpers;

namespace Murder.Components
{
    public readonly struct FacingComponent : IComponent
    {
        public readonly bool Flipped;
        public readonly bool LookingUp;

        public readonly Direction Direction;
        public readonly Vector2 DirectionVector;
        
        /// <summary>
        /// Creates a FacingComponent using a Direction as a base.
        /// </summary>
        /// <param name="direction"></param>
        public FacingComponent(Direction direction)
        {
            Direction = direction;
            DirectionVector = direction.ToVector();
            Flipped = DirectionVector.X<0;
            LookingUp = DirectionVector.Y<0;
        }


        /// <summary>
        /// Creates a FacingComponent using a Vector2 as a base.
        /// </summary>
        /// <param name="direction"></param>
        public FacingComponent(Vector2 direction)
        {
            Direction = DirectionHelper.FromVector(direction);
            DirectionVector = direction;
            Flipped = DirectionVector.X < 0;
            LookingUp = DirectionVector.Y < 0;
        }
    }
}
