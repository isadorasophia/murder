using Newtonsoft.Json;
using Bang.Components;
using Murder.Attributes;
using Murder.Core.Physics;
using Murder.Core.Geometry;

namespace Murder.Components
{
    [Unique]
    [DoNotPersistEntityOnSave]
    internal readonly struct QuadtreeComponent : IModifiableComponent
    {
        [JsonIgnore]
        public readonly Quadtree Quadtree;

        public QuadtreeComponent(Rectangle size) =>
            Quadtree = new(size);

        public void Subscribe(Action notification)
        { }

        public void Unsubscribe(Action notification)
        { }
    }
}
