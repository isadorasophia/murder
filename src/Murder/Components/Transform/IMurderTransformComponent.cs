using Bang.Components;
using Bang.Entities;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Utilities;
using System.Numerics;

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

        public IMurderTransformComponent Add(Vector2 r);

        public IMurderTransformComponent Subtract(Vector2 r);

        public virtual IMurderTransformComponent Add(Point r) => Add(r.ToVector2());
        
        public virtual IMurderTransformComponent Subtract(Point r) => Subtract(r.ToVector2());

        public IMurderTransformComponent Add(IMurderTransformComponent r);
        
        public IMurderTransformComponent Subtract(IMurderTransformComponent r);
        
        public IMurderTransformComponent With(float x, float y);

        public IMurderTransformComponent With(Vector2 p) => With(p.X, p.Y);

        public virtual IMurderTransformComponent With(Point p) => With(p.X, p.Y);

        public virtual Vector2 Vector2 => new(X, Y);

        public virtual Point Point => new(Calculator.RoundToInt(X), Calculator.RoundToInt(Y));

        /// <summary>
        /// This is the X grid coordinate. See <see cref="GridConfiguration"/> for more details on our grid specs.
        /// </summary>
        public virtual int Cx => (int)Math.Floor(X / Grid.CellSize);

        /// <summary>
        /// This is the Y grid coordinate. See <see cref="GridConfiguration"/> for more details on our grid specs.
        /// </summary>
        public virtual int Cy => (int)Math.Floor(Y / Grid.CellSize);
    }
    
    public static class MurderTransformExtensions
    {
        public static IMurderTransformComponent GetMurderTransform(this Entity e)
            => e.GetComponent<IMurderTransformComponent>(BangComponentTypes.Transform);

        public static bool HasMurderTransform(this Entity e)
            => e.HasComponent(BangComponentTypes.Transform);

        public static IMurderTransformComponent? TryGetMurderTransform(this Entity e)
            => e.HasMurderTransform() ? e.GetMurderTransform() : null;

        public static void SetMurderTransform(this Entity e, IMurderTransformComponent component)
        {
            e.AddOrReplaceComponent(component, BangComponentTypes.Transform);
        }

        public static bool RemoveMurderTransform(this Entity e)
            => e.RemoveComponent(BangComponentTypes.Transform);
    }
}
