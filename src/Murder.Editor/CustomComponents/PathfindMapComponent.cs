using Bang.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Utilities.Attributes;

namespace Murder.Editor.Components;

/// <summary>
/// This is a struct that points to a singleton class.
/// Reactive systems won't be able to subscribe to this component.
/// </summary>
[Unique]
[RuntimeOnly]
public readonly struct PathfindMapComponent : IModifiableComponent
{
    public readonly Map Map;

    public readonly int Width => Map.Width;

    public readonly int Height => Map.Height;

    public PathfindMapComponent(int width, int height)
    {
        Map = new(Point.Zero, width, height);
    }

    public void Subscribe(Action notification)
    { }

    public void Unsubscribe(Action notification)
    { }
}