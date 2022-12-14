using Bang.Components;

namespace Murder.Components
{
    public readonly struct IsCollidingComponent : IComponent
    { 
        /// <summary>
        /// Id of the entity that caused this collision.
        /// </summary>
        public readonly int InteractorId;

        public IsCollidingComponent(int interactorId) => InteractorId = interactorId;
    }
}
