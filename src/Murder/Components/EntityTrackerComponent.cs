using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This is a component used to track other entities within the world.
    /// action.
    /// </summary>
    [RuntimeOnly]
    [PersistOnSave]
    public readonly struct EntityTrackerComponent : IComponent
    {
        /// <summary>
        /// Id of the target entity.
        /// </summary>
        public readonly int Target;

        public EntityTrackerComponent(int target) => Target = target;
    }
}