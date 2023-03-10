using Bang.Components;

namespace Murder.Components
{
    public readonly struct InteractOnButtonPressComponent : IComponent
    {
        public readonly int Priority;
        
        public readonly bool HightlightOnRange = true;

        public InteractOnButtonPressComponent() { }

        public InteractOnButtonPressComponent(bool hightlightOnRange) => HightlightOnRange = hightlightOnRange;
    }
}
