using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions;

public readonly struct TargetedInteractionCollectionItem()
{
    [Target]
    public readonly string Target = string.Empty;
    public readonly ImmutableArray<IInteractiveComponent> InteractionCollection = [];
}

public readonly struct TargetedInteractionCollection : IInteraction
{
    public readonly ImmutableArray<TargetedInteractionCollectionItem> Interactives = [];

    public TargetedInteractionCollection() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (interacted == null) 
        {
            GameLogger.Error("No interacted entity found");
            return;
        }

        // First we assume that the entity will have Guid to Targer ID collection
        if (interacted.TryGetIdTargetCollection() is not IdTargetCollectionComponent guidToIdTargetCollection)
        {
            GameLogger.Error("No Guid to ID collection found");
            return;
        }

        foreach (TargetedInteractionCollectionItem item in Interactives)
        {
            if (!guidToIdTargetCollection.Targets.TryGetValue(item.Target, out int id))
            {
                GameLogger.Error($"No GUID found for {item.Target}");
                continue;
            }

            if (world.TryGetEntity(id) is not Entity targetEntity)
            {
                GameLogger.Error($"No target entity found for {id}");
                continue;
            }

            foreach (var interactive in item.InteractionCollection)
            {
                interactive.Interact(world, interactor, targetEntity);
            }
        }
    }
}
