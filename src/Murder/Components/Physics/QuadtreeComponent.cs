using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;

namespace Murder.Components
{
    [Unique]
    [RuntimeOnly]
    [DoNotPersistEntityOnSave]
    public readonly struct QuadtreeComponent : IModifiableComponent
    {
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public readonly Quadtree Quadtree;

        public QuadtreeComponent(Rectangle size) =>
            Quadtree = new(size);

        public void Subscribe(Action notification)
        { }

        public void Unsubscribe(Action notification)
        { }
    }
}