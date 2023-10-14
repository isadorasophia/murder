using Bang.Components;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;

namespace Murder.Components
{
    /// <summary>
    /// This is a struct that points to a singleton class.
    /// Reactive systems won't be able to subscribe to this component.
    /// </summary>
    [Requires(typeof(RoomComponent))]
    public readonly struct TileGridComponent : IModifiableComponent
    {
        public readonly TileGrid Grid;

        [Slider(minimum: 1)]
        public readonly int Width = 1;

        [Slider(minimum: 1)]
        public readonly int Height = 1;

        public readonly Point Origin = Point.Zero;

        public readonly IntRectangle Rectangle => new(Origin, new(Width, Height));

        public TileGridComponent() : this(1, 1) { }

        public TileGridComponent(TileGrid grid)
        {
            Grid = grid;

            Origin = grid.Origin;
            (Width, Height) = (grid.Width, grid.Height);
        }

        public TileGridComponent(Point origin, int width, int height) : this(new(origin, width, height)) { }

        public TileGridComponent(int width, int height) : this(Point.Zero, width, height) { }

        public void Subscribe(Action notification) => Grid.OnModified += notification;

        public void Unsubscribe(Action notification) => Grid.OnModified -= notification;
    }
}