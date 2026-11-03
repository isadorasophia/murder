using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    public enum DestroyWho
    {
        None,
        Target,
        Interacted,
        Interactor,
        Parent,
        Children
    }

    public readonly struct RemoveEntityOnInteraction : IInteraction
    {
        public readonly DestroyWho DestroyWho;

        [Target]
        public readonly string? Target = null;

        public readonly RemoveStyle? RemoveWith = null;

        [Tooltip("Useful to filter out reactive systems")]
        public readonly ImmutableArray<IComponent>? AddComponentsBeforeRemoving = null;
        
        public RemoveEntityOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            GameLogger.Verify(interacted is not null);

            Entity? target = null;
            switch (DestroyWho)
            {
                case DestroyWho.Target:
                    int? targetId = Target is not null ? 
                        interacted.FindTarget(Target) : 
                        interacted.TryGetIdTarget()?.Target;

                    if (targetId is not null && world.TryGetEntity(targetId.Value) is Entity targetEntity)
                    {
                        target = targetEntity;
                    }

                    // Also delete all entities defined in a collection.
                    if (Target is null && 
                        interacted.TryGetIdTargetCollection()?.Targets is ImmutableDictionary<string, int> targets)
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
                    target = interacted.TryFetchParent();
                    break;

                case DestroyWho.Children:
                    // For now, this only destroys the first child.
                    if (interacted.Children.Length > 0)
                    {
                        target = world.TryGetEntity(interacted.Children[0]);
                    }
                    break;
                case DestroyWho.None:
                default:
                    break;
            }

            if (target != null)
            {
                if (AddComponentsBeforeRemoving is not null)
                {
                    foreach (IComponent component in AddComponentsBeforeRemoving)
                    {
                        target.AddComponent(component, component.GetType());
                    }
                }
                
                RemoveStyle style = RemoveWith ?? RemoveStyle.Destroy;
                switch (style)
                {
                    case RemoveStyle.Deactivate:
                        target.Deactivate();
                        break;

                    default:
                        target.Destroy();
                        break;
                }
            }
        }
    }
}