using Bang;
using Bang.Components;
using Murder.Core;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;

namespace Murder.Components
{
    /// <summary>
    /// This is a struct that points to a singleton class.
    /// Reactive systems won't be able to subscribe to this component.
    /// </summary>
    [Unique]
    [RuntimeOnly]
    public readonly struct MapComponent : IModifiableComponent
    {
        [JsonProperty]
        public readonly Map Map;

        [JsonProperty]
        public readonly int Width => Map.Width;

        [JsonProperty]
        public readonly int Height => Map.Height;

        [JsonConstructor]
        public MapComponent(int width, int height)
        {
            Map = new(width, height);
        }

        public void Subscribe(Action notification)
        { }

        public void Unsubscribe(Action notification)
        { }
    }
}