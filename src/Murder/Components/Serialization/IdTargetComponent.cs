using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This is a component used to track other entities when triggering an interaction or other
    /// action.
    /// </summary>
    [RuntimeOnly]
    [PersistOnSave]
    public readonly struct IdTargetComponent : IComponent
    {
        /// <summary>
        /// Id of the target entity.
        /// </summary>
        public readonly int Target;

        public IdTargetComponent(int target) => Target = target;
    }
}
