using Bang.Components;
using Murder.Core;

namespace Murder.Components
{
    /// <summary>
    /// This is a struct that points to a singleton class.
    /// Reactive systems won't be able to subscribe to this component.
    /// </summary>
    [Requires(typeof(MapDimensionsComponent), typeof(TilesetComponent))]
    public readonly struct TileGridComponent : IModifiableComponent
    {
        public readonly TileGrid Grid;

        public int Width => Grid.Width;

        public int Height => Grid.Height;

        public TileGridComponent() => Grid = new(1, 1);

        public TileGridComponent(int width, int height)
        {
            Grid = new(width, height);
        }

        public void Subscribe(Action notification)
        { }

        public void Unsubscribe(Action notification)
        { }
    }
}
