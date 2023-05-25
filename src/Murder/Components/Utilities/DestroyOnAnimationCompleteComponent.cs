using Bang.Components;

namespace Murder.Components
{
    public readonly struct DestroyOnAnimationCompleteComponent : IComponent
    {
        /// <summary>
        /// Whether it should deactivate instead of destroying.
        /// </summary>
        public readonly bool DeactivateOnComplete = false;

        public DestroyOnAnimationCompleteComponent() { }

        public DestroyOnAnimationCompleteComponent(bool deactivateOnComplete) => 
            DeactivateOnComplete = deactivateOnComplete;
    }
}
