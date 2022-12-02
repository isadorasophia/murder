using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Components;
using Murder.Utilities;

namespace Murder.Interactions
{
    /// <summary>
    /// This will trigger an effect by placing <see cref="Component"/> in the world.
    /// </summary>
    public readonly struct AddComponentOnInteraction : Interaction
    {
        public readonly IComponent Component;

        [Tooltip("Whether the component will be added on this entity itself.")]
        public readonly bool IsTargetSelf;

        public void Interact(World world, Entity interactor, Entity interacted)
        {
            // We need to guarantee that any modifiable components added here are safe.
            IComponent c = Component is IModifiableComponent ? SerializationHelper.DeepCopy(Component) : Component;

            if (IsTargetSelf)
            {
                interacted.AddOrReplaceComponent(Component, Component.GetType());
            }
            else
            {
                Entity e = world.AddEntity(c);

                // Also propagate the target interaction, if any.
                if (interacted.TryGetTargetInteraction() is TargetInteractionComponent target)
                {
                    e.SetTargetInteraction(target);
                }
            }
        }
    }
}
