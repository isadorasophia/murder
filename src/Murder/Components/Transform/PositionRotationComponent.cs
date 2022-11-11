using Newtonsoft.Json;
using Bang.Components;
using Bang.Entities;
using System.Diagnostics;
using Murder.Attributes;
using Murder.Core.Geometry;

namespace Murder.Components
{
    /// <summary>
    /// Position component used to track entities positions within a grid.
    /// </summary>
    [Intrinsic]
    [DebuggerDisplay("X: {X}, Y: {Y}")]
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

        public float Angle => _angle;

        // TODO: Implement matrix!!!
        [JsonProperty]
        public Matrix Matrix => throw new NotImplementedException();

        /// <summary>
        /// Create a new <see cref="PositionRotationComponent"/>.
        /// </summary>
        [JsonConstructor]
        public PositionRotationComponent(float x, float y, IMurderTransformComponent? parent = default)
        {
            (_x, _y) = (x, y);
            _parent = parent;
        }

        /// <summary>
        /// Create a new <see cref="PositionRotationComponent"/>.
        /// </summary>
        /// <param name="v">Vector coordinate.</param>
        public PositionRotationComponent(Vector2 v) : this(v.X, v.Y) { }

        /// <summary>
        /// Create a new <see cref="PositionRotationComponent"/>.
        /// </summary>
        /// <param name="p">Point coordinate.</param>
        public PositionRotationComponent(Point p) : this(p.X, p.Y) { }

        public static bool operator ==(PositionRotationComponent l, PositionRotationComponent r) => l.Equals(r);

        public static bool operator !=(PositionRotationComponent l, PositionRotationComponent r) => !(l == r);

        public static PositionRotationComponent operator +(PositionRotationComponent l, PositionRotationComponent r) => l + (IMurderTransformComponent)r;

        public static PositionRotationComponent operator -(PositionRotationComponent l, PositionRotationComponent r) => l - (IMurderTransformComponent)r;

        public static PositionRotationComponent operator +(PositionRotationComponent l, IMurderTransformComponent r) => new(l.X + r.X, l.Y + r.Y);

        public static PositionRotationComponent operator -(PositionRotationComponent l, IMurderTransformComponent r) => new(l.X - r.X, l.Y - r.Y);

        public static PositionRotationComponent operator +(PositionRotationComponent l, Vector2 r) => new(l.X + r.X, l.Y + r.Y);

        public static PositionRotationComponent operator -(PositionRotationComponent l, Vector2 r) => new(l.X - r.X, l.Y - r.Y);

        /// <summary>
        /// Return the global position of the component within the world.
        /// </summary>
        public IMurderTransformComponent GetGlobal()
        {
            if (_parent is PositionRotationComponent parentPosition)
            {
                return parentPosition + this;
            }

            return this;
        }

        /// <summary>
        /// Creates a copy of component with the relative coordinates without its parent.
        /// </summary>
        /// <returns></returns>
        public IParentRelativeComponent WithoutParent() => new PositionRotationComponent(X, Y);

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

            childEntity.ReplaceComponent(new PositionRotationComponent(X, Y, parentGlobalPosition));
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

            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object? obj) => obj is PositionRotationComponent c && this.Equals(c);
    }
}
