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
    public readonly struct PositionComponent : IMurderTransformComponent, IEquatable<PositionComponent>
    {
        private readonly IMurderTransformComponent? _parent;

        [JsonProperty]
        private readonly float _x;

        [JsonProperty]
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
        public IMurderTransformComponent GetGlobal()
        {
            if (_parent is PositionComponent parentPosition)
            {
                return parentPosition + this;
            }
            
            return this;
        }
        
        /// <summary>
        /// Create a new <see cref="PositionComponent"/>.
        /// </summary>
        [JsonConstructor]
        public PositionComponent(float x, float y, IMurderTransformComponent? parent = default)
        {
            (_x, _y) = (x, y);
            _parent = parent;
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

        public static explicit operator PositionRotationComponent(PositionComponent p) => new(p.X, p.Y, 0, p._parent);

        public IMurderTransformComponent Add(Vector2 r) => this + r;

        public IMurderTransformComponent Subtract(Vector2 r) => this - r;

        public IMurderTransformComponent Add(IMurderTransformComponent r) => this + r;

        public IMurderTransformComponent Subtract(IMurderTransformComponent r) => this - r;

        /// <summary>
        /// Creates a copy of component with the relative coordinates without its parent.
        /// </summary>
        /// <returns></returns>
        public IParentRelativeComponent WithoutParent() => new PositionComponent(X, Y);

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

            childEntity.ReplaceComponent(new PositionComponent(X, Y, parentGlobalPosition));
        }
        
        public override int GetHashCode() => (X, Y).GetHashCode();

        /// <summary>
        /// Compares two position components. This will take their parents into account.
        /// </summary>
        public bool Equals(PositionComponent other)
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

        public override bool Equals(object? obj) => obj is PositionComponent c && this.Equals(c);
    }
}
