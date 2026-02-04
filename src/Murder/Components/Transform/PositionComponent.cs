using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Utilities.Attributes;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Murder.Components
{
    /// <summary>
    /// Position component used to track entities positions within a grid.
    /// </summary>
    [Intrinsic]
    [DebuggerDisplay("X: {X}, Y: {Y}")]
    [CustomName($" Position")]
    public readonly struct PositionComponent : IMurderTransformComponent, IEquatable<PositionComponent>
    {
        private readonly Vector2? _globalPosition;

        [Serialize]
        private readonly float _x;

        [Serialize]
        private readonly float _y;

        /// <summary>
        /// Relative X position of the component.
        /// </summary>
        public float X => _x;

        /// <summary>
        /// Relative Y position of the component.
        /// </summary>
        public float Y => _y;

        public float Angle => 0;

        public Vector2 Scale => Vector2.One;

        /// <summary>
        /// Return the global position of the component within the world.
        /// </summary>
        public Vector2 GetGlobal()
        {
            return _globalPosition ?? new(X, Y);
        }

        /// <summary>
        /// Return the global position of the component within the world.
        /// </summary>
        public PositionComponent SetGlobal(Vector2 position)
        {
            if (_globalPosition is not null)
            {
                Vector2 parentPosition = _globalPosition.Value - new Vector2(X, Y);
                Vector2 localAfter = position - parentPosition;

                return new(localAfter.X, localAfter.Y, globalPosition: position);
            }
            else
            {
                return new(position);
            }
        }

        /// <summary>
        /// Create a new <see cref="PositionComponent"/>.
        /// </summary>
        [JsonConstructor]
        public PositionComponent(float x, float y) : this(x, y, null) { }

        public PositionComponent(float x, float y, Vector2? globalPosition)
        {
            (_x, _y) = (x, y);
            _globalPosition = globalPosition;
        }

        /// <summary>
        /// Create a new <see cref="PositionComponent"/>.
        /// </summary>
        /// <param name="v">Vector coordinate.</param>
        public PositionComponent(Vector2 v) : this(v.X, v.Y) { }

        /// <summary>
        /// Create a new <see cref="PositionComponent"/>.
        /// </summary>
        /// <param name="p">Point coordinate.</param>
        public PositionComponent(Point p) : this(p.X, p.Y) { }

        public static bool operator ==(PositionComponent l, PositionComponent r) => l.Equals(r);

        public static bool operator !=(PositionComponent l, PositionComponent r) => !(l == r);

        public static PositionComponent operator +(PositionComponent l, PositionComponent r) => l + (IMurderTransformComponent)r;

        public static PositionComponent operator -(PositionComponent l, PositionComponent r) => l - (IMurderTransformComponent)r;

        public static PositionComponent operator +(PositionComponent l, IMurderTransformComponent r) => new(l.X + r.X, l.Y + r.Y);

        public static PositionComponent operator -(PositionComponent l, IMurderTransformComponent r) => new(l.X - r.X, l.Y - r.Y);

        public static PositionComponent operator +(PositionComponent l, Vector2 r) => new(l.X + r.X, l.Y + r.Y);

        public static PositionComponent operator -(PositionComponent l, Vector2 r) => new(l.X - r.X, l.Y - r.Y);

        public IMurderTransformComponent Add(Vector2 r) => this + r;

        public IMurderTransformComponent Subtract(Vector2 r) => this - r;

        public IMurderTransformComponent Add(IMurderTransformComponent r) => this + r;

        public IMurderTransformComponent Subtract(IMurderTransformComponent r) => this - r;

        public IMurderTransformComponent WithLocal(float x, float y) => WithLocalPosition(x, y);

        public PositionComponent WithLocalPosition(float x, float y)
        {
            if (_globalPosition is not Vector2 globalPosition)
            {
                return new PositionComponent(x, y);
            }

            // otherwise, subtract whatever used to be our global position and apply the new one.
            return new PositionComponent(x, y, new Vector2(globalPosition.X - X + x, globalPosition.Y - Y + y));
        }

        /// <summary>
        /// Creates a copy of component with the relative coordinates without its parent.
        /// </summary>
        /// <returns></returns>
        public IParentRelativeComponent WithoutParent() => new PositionComponent(X, Y);

        /// <summary>
        /// Whether this position is tracking a parent entity.
        /// </summary>
        public bool HasParent => _globalPosition is not null;

        /// <summary>
        /// This tracks whenever a parent position has been modified.
        /// </summary>
        /// <param name="parentComponent">The parent position component.</param>
        /// <param name="childEntity">The entity of the child which owns this component.</param>
        public void OnParentModified(IComponent parentComponent, Entity childEntity)
        {
            Vector2 parentGlobalPosition = ((IMurderTransformComponent)parentComponent).GetGlobal();

            Vector2 globalPosition = parentGlobalPosition + new Vector2(X, Y);
            childEntity.ReplaceComponent(new PositionComponent(X, Y, globalPosition));
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        /// <summary>
        /// Compares two position components. This will take their parents into account.
        /// </summary>
        public bool Equals(PositionComponent other)
        {
            if (_globalPosition is null || other._globalPosition is null)
            {
                if (_globalPosition is not null || other._globalPosition is not null)
                {
                    return false;
                }
            }
            else if (!_globalPosition.Equals(other._globalPosition))
            {
                return false;
            }

            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object? obj) => obj is PositionComponent c && this.Equals(c);
    }
}