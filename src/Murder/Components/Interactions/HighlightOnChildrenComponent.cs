using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// Component that specifies that any highlight should be applied on the children (instead of self).
    /// </summary>
    public readonly struct HighlightOnChildrenComponent : IComponent 
    {
        [ChildId]
        public readonly string? Child = null;

        public HighlightOnChildrenComponent() { }
    }
}