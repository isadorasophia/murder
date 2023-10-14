using Bang.Components;

namespace Murder.Components
{
    public readonly struct WaitForVacancyComponent : IComponent
    {
        public readonly bool AlertParent;

        public WaitForVacancyComponent(bool alertParent)
        {
            AlertParent = alertParent;
        }
    }
}