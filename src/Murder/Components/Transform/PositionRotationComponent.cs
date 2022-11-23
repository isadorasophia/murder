using Newtonsoft.Json;
using Bang.Components;
using Bang.Entities;
using System.Diagnostics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Utilities;

namespace Murder.Components
{
    /// <summary>
    /// Position component used to track entities positions within a grid.
    /// </summary>
    [Intrinsic]
    [DebuggerDisplay("X: {X}, Y: {Y}, A:{Angle}")]
    public readonly struct PositionRotationComponent : IMurderTransformComponent, IEquatable<PositionRotationComponent>
    {
        private readonly IMurderTransformComponent? _parent;

        [JsonProperty]
        private readonly float _x;

        [JsonProperty]
        private readonly float _y;

        [JsonProperty]
        private readonly float _angle;

        /// <summary>
        /// Relative X position of the component.
        /// </summary>
        public float X => _x;

        /// <summary>
        /// Relative Y position of the component.
        /// </summary>
        public float Y => _y;

        public float Angle => _angle * Calculator.TO_RAD;
        public Vector2 Scale => Vector2.One;

        /// <summary>
        /// Create a new <see cref="PositionRotationComponent"/>.
        /// </summary>
        /// <param name="x">Position x within the world.</param>
        /// <param name="y">Position y within the world.</param>
        /// <param name="angle">In degrees.</param>
        /// <param name="parent">Optional parent component.</param>
        [JsonConstructor]
        public PositionRotationComponent(float x, float y, float angle, IMurderTransformComponent? parent = default)
        {
            (_x, _y) = (x, y);
            _angle = angle;
            _parent = parent;
        }

        /// <summary>
        /// Create a new <see cref="PositionRotationComponent"/>.
        /// </summary>
        /// <param name="v">Vector coordinate.</param>
        /// <param name="angle">In degrees</param>
        public PositionRotationComponent(Vector2 v, float angle) : this(v.X, v.Y, angle)
        {
            _angle = angle;
        }

        /// <summary>
        /// Create a new <see cref="PositionRotationComponent"/>.
        /// </summary>
        /// <param name="p">Point coordinate.</param>
        /// <param name="angle"></param>
        public PositionRotationComponent(Point p, float angle) : this(p.X, p.Y, angle) { }

        public static bool operator ==(PositionRotationComponent l, PositionRotationComponent r) => l.Equals(r);

        public static bool operator !=(PositionRotationComponent l, PositionRotationComponent r) => !(l == r);

        public static PositionRotationComponent operator +(PositionRotationComponent l, PositionRotationComponent r) => new (l.X + r.X, l.Y + r.Y, l._angle + r._angle);

        public static PositionRotationComponent operator -(PositionRotationComponent l, PositionRotationComponent r) => new(l.X - r.X, l.Y - r.Y, l._angle - r._angle);

        public static PositionRotationComponent operator +(PositionRotationComponent l, IMurderTransformComponent r) => new(l.X + r.X, l.Y + r.Y, l._angle);

        public static PositionRotationComponent operator -(PositionRotationComponent l, IMurderTransformComponent r) => new(l.X - r.X, l.Y - r.Y, l._angle);

        public static PositionRotationComponent operator +(PositionRotationComponent l, Vector2 r) => new(l.X + r.X, l.Y + r.Y, l._angle);

        public static PositionRotationComponent operator -(PositionRotationComponent l, Vector2 r) => new(l.X - r.X, l.Y - r.Y, l._angle);
        
        public static explicit operator PositionComponent(PositionRotationComponent p) => new(p.X, p.Y, p._parent);

        public IMurderTransformComponent Add(Vector2 r) => this + r;

        public IMurderTransformComponent Subtract(Vector2 r) => this - r;

        public IMurderTransformComponent Add(IMurderTransformComponent r) => this + r;

        public IMurderTransformComponent Subtract(IMurderTransformComponent r) => this - r;

        public IMurderTransformComponent With(float x, float y) => new PositionRotationComponent(x, y, _angle, _parent);

        /// <summary>
        /// Return the global position of the component within the world.
        /// </summary>
        public IMurderTransformComponent GetGlobal()
        {
            if (_parent is PositionRotationComponent parentPosition)
            {
                var rotated = this.ToVector2().Rotate(parentPosition.Angle);
                return new PositionRotationComponent(
                    rotated.X + parentPosition._x,
                    rotated.Y + parentPosition.Y,
                    parentPosition._angle + _angle);
            }

            return this;
        }

        /// <summary>
        /// Creates a copy of component with the relative coordinates without its parent.
        /// </summary>
        /// <returns></returns>
        public IParentRelativeComponent WithoutParent() => new PositionRotationComponent(_x, _y, _angle);

        /// <summary>
        /// Whether this position is tracking a parent entity.
        /// </summary>
        public bool HasParent => _parent is not null;


        /// <summary>
        /// This tracks whenever a parent position has been modified.
        /// </summary>
        /// <param name="parentComponent">The parent position component.</param>
        /// <param name="childEntity">The entity of the child which owns this component.</param>
        public void OnParentModified(IComponent parentComponent, Entity childEntity)
        {
            IMurderTransformComponent parentGlobalPosition = ((IMurderTransformComponent)parentComponent).GetGlobal();

            childEntity.ReplaceComponent(new PositionRotationComponent(_x, _y, _angle, parentGlobalPosition));
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        /// <summary>
        /// Compares two position components. This will take their parents into account.
        /// </summary>
        public bool Equals(PositionRotationComponent other)
        {
            if (_parent is null || other._parent is null)
            {
                if (_parent is not null || other._parent is not null)
                {
                    return false;
                }
            }
            else if (!_parent.Equals(other._parent))
            {
                return false;
            }

            return other._x == _x && other._y == _y && other._angle == _angle;
        }

        public override bool Equals(object? obj) => obj is PositionRotationComponent c && this.Equals(c);
    }
}
