using Newtonsoft.Json;
using Bang.Components;
using Bang.Entities;
using System.Diagnostics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Utilities;

namespace Murder.Components
{
    /// <summary>
    /// Position component used to track entities positions within a grid.
    /// </summary>
    [Intrinsic]
    [DebuggerDisplay("[PositionComponent] X: {X}, Y: {Y}")]
    public readonly struct PositionComponent : IParentRelativeComponent, IEquatable<PositionComponent>
    {
        /// <summary>
        /// This is the X grid coordinate. See <see cref="Grid"/> for more details on our grid specs.
        /// </summary>
        [HideInEditor, JsonIgnore]
        public int Cx => (int)Math.Floor(X / Grid.CellSize);

        /// <summary>
        /// This is the Y grid coordinate. See <see cref="Grid"/> for more details on our grid specs.
        /// </summary>
        [HideInEditor, JsonIgnore]
        public int Cy => (int)Math.Floor(Y / Grid.CellSize);

        /// <summary>
        /// Relative X position of the component.
        /// </summary>
        public readonly float X;

        /// <summary>
        /// Relative Y position of the component.
        /// </summary>
        public readonly float Y;

        private readonly IComponent? _parent;
        
        /// <summary>
        /// Create a new <see cref="PositionComponent"/>.
        /// </summary>
        [JsonConstructor]
        public PositionComponent(float x, float y, IComponent? parent = default)
        {
            (X, Y) = (x, y);
            //(Cx, Cy) = ((int)Math.Floor(x / Grid.CellSize), (int)Math.Floor(y / Grid.CellSize));

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

        public static PositionComponent operator +(PositionComponent l, PositionComponent r) => new(l.X + r.X, l.Y + r.Y);

        public static PositionComponent operator -(PositionComponent l, PositionComponent r) => new(l.X - r.X, l.Y - r.Y);

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

        public override int GetHashCode() => (X, Y).GetHashCode();

        public int XSnap => Calculator.RoundToInt(X);
        public int YSnap => Calculator.RoundToInt(Y);

        public Point Point => new(XSnap, YSnap);
        public Vector2 Pos => new(X, Y);

        /// <summary>
        /// Return the global position of the component within the world.
        /// </summary>
        public PositionComponent GetGlobalPosition()
        {
            if (_parent is PositionComponent parentPosition)
            {
                return parentPosition + this;
            }

            return this;
        }

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
            PositionComponent parentGlobalPosition = ((PositionComponent)parentComponent).GetGlobalPosition();

            childEntity.ReplaceComponent(new PositionComponent(X, Y, parentGlobalPosition));
        }
    }
}
