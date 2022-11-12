using Bang.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Utilities;

namespace Murder.Components
{
    /// <summary>
    /// This is an interface for transform components within the game.
    /// </summary>
    public interface IMurderTransformComponent : ITransformComponent
    {
        /// <summary>
        /// Relative X position of the component.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// Relative Y position of the component.
        /// </summary>
        public float Y { get; }

        public float Angle { get; }
        public Vector2 Scale { get; }

        public IMurderTransformComponent GetGlobal();

        public virtual Vector2 Vector2 => new(X, Y);
        
        public virtual Point Point => new(Calculator.RoundToInt(X), Calculator.RoundToInt(Y));

        /// <summary>
        /// This is the X grid coordinate. See <see cref="Grid"/> for more details on our grid specs.
        /// </summary>
        public virtual int Cx => (int)Math.Floor(X / Grid.CellSize);

        /// <summary>
        /// This is the Y grid coordinate. See <see cref="Grid"/> for more details on our grid specs.
        /// </summary>
        public virtual int Cy => (int)Math.Floor(Y / Grid.CellSize);
    }
}
