using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Editor.Components
{
    /// <summary>
    /// Editor effect for an entity that has just been placed in the world.
    /// </summary>
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct PlacedInWorldComponent : IComponent 
    {
        public readonly float PlacedTime = 0;

        public PlacedInWorldComponent() { }

        public PlacedInWorldComponent(float time) => PlacedTime = time;
    }
}
