using Bang.Components;

namespace Murder.Components
{
    public readonly struct InteractOnButtonPressComponent : IComponent
    {
        public readonly int Priority;
        
        public readonly bool HightlightOnRange = true;
        
        public readonly string InteractionName = string.Empty;

        public InteractOnButtonPressComponent() { }

        public InteractOnButtonPressComponent(bool hightlightOnRange, string interactionName)
        {
            HightlightOnRange = hightlightOnRange;
            InteractionName = interactionName;
        }
    }
}
