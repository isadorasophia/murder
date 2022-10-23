using Newtonsoft.Json;
using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// This is a struct that points to a singleton class.
    /// Reactive systems won't be able to subscribe to this component.
    /// </summary>
    [Unique]
    public readonly struct MapDimensionsComponent : IComponent
    {
        [JsonProperty]
        public readonly int Width;

        [JsonProperty]
        public readonly int Height;

        [JsonConstructor]
        public MapDimensionsComponent(int width, int height) 
        {
            (Width, Height) = (width, height);
        }
    }
}