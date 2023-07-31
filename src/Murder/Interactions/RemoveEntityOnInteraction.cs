using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    public enum DestroyWho
    {
        None,
        Target,
        Interacted,
        Interactor,
        Parent
    }
    public readonly struct RemoveEntityOnInteraction : IInteraction
    {
        public readonly DestroyWho DestroyWho;
        [Tooltip("Useful to filter out reactive systems")]
        public readonly ImmutableArray<IComponent> AddComponentsBeforeRemoving = ImmutableArray<IComponent>.Empty;
        public RemoveEntityOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            GameLogger.Verify(interacted is not null);
            Entity? target = null;
            switch (DestroyWho)
            {
                case DestroyWho.Target:
                    if (interacted.TryGetIdTarget()?.Target is int targetId &&
                        world.TryGetEntity(targetId) is Entity targetEntity)
                    {
                        target = targetEntity;
                    }

                    // Also delete all entities defined in a collection.
                    if (interacted.TryGetIdTargetCollection()?.Targets is ImmutableDictionary<string, int> targets)
                    {
                        foreach (int entityId in targets.Values)
                        {
                            if (world.TryGetEntity(entityId) is Entity otherTarget)
                            {
                                target = otherTarget;
                            }
                        }
                    }
                    break;

                case DestroyWho.Interacted:
                    target = interacted;
                    break;
                    
                case DestroyWho.Interactor:
                    target = interactor;
                    break;
                case DestroyWho.Parent:
                    target = interacted?.TryFetchParent();
                    break;
                case DestroyWho.None:
                default:
                    break;
            }

            if (target != null)
            {
                foreach (var component in AddComponentsBeforeRemoving)
                {
                    target.AddComponent(component, component.GetType());
                }
                
                target.Destroy();
            }
        }
    }
}
