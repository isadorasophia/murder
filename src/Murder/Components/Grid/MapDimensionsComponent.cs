using Newtonsoft.Json;
using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This is a struct that points to a singleton class.
    /// Reactive systems won't be able to subscribe to this component.
    /// </summary>
    [Unique]
    public readonly struct MapDimensionsComponent : IComponent
    {
        [Slider(minimum: 1)]
        public readonly int Width = 1;

        [Slider(minimum: 1)]
        public readonly int Height = 1;

        public MapDimensionsComponent() { }

        [JsonConstructor]
        public MapDimensionsComponent(int width, int height) 
        {
            (Width, Height) = (width, height);
        }
    }
}