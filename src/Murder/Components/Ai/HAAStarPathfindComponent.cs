using Bang.Components;
using Murder.Attributes;
using Murder.Core.Ai;
using Newtonsoft.Json;

namespace Murder.Components
{
    /// <summary>
    /// This is a struct that points to a singleton class.
    /// Reactive systems won't be able to subscribe to this component.
    /// </summary>
    [Unique]
    [DoNotPersistEntityOnSave]
    public readonly struct HAAStarPathfindComponent : IModifiableComponent
    {
        [JsonProperty, Bang.Serialize]
        public readonly HAAStar Data;

        public HAAStarPathfindComponent(int width, int height)
        {
            Data = new(width, height);
        }

        public void Subscribe(Action notification)
        { }

        public void Unsubscribe(Action notification)
        { }
    }
}