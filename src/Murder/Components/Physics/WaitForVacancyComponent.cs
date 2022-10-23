using Bang.Components;

namespace Murder.Components
{
    internal readonly struct WaitForVacancyComponent : IComponent
    {
        internal readonly bool AlertParent;

        public WaitForVacancyComponent(bool alertParent)
        {
            AlertParent = alertParent;
        }
    }
}
