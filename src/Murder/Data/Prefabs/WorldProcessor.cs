using Bang;
using Bang.Entities;
using Murder.Assets;
using Murder.Components;
using Murder.Components.Serialization;
using Murder.Core;
using Murder.Core.Cutscenes;
using Murder.Diagnostics;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Data;

public class WorldProcessor
{
    public void PostProcessEntitiesFirstTime(World world, Dictionary<Guid, int> instancesToEntities)
    {
        ImmutableArray<Entity> entities = world.GetActivatedAndDeactivatedEntitiesWith(typeof(GuidToIdTargetComponent));
        foreach (Entity e in entities)
        {
            Guid guid = e.GetGuidToIdTarget().Target;
            e.RemoveGuidToIdTarget();

            if (!instancesToEntities.TryGetValue(guid, out int id))
            {
                GameLogger.Error($"Tried to reference an entity with guid '{guid}' that is not available in world. " +
                    "Are you trying to access a child entity, which is not supported yet?");

                continue;
            }

            e.SetIdTarget(id);
        }

        // Now, iterate over all the collection entities.
        entities = world.GetActivatedAndDeactivatedEntitiesWith(typeof(GuidToIdTargetCollectionComponent));
        foreach (Entity e in entities)
        {
            ImmutableArray<GuidId> guids = e.GetGuidToIdTargetCollection().Collection;
            e.RemoveGuidToIdTargetCollection();

            var builder = ImmutableDictionary.CreateBuilder<string, int>();
            foreach (GuidId guidId in guids)
            {
                if (!instancesToEntities.TryGetValue(guidId.Target, out int id))
                {
                    GameLogger.Error($"Tried to reference an entity with guid '{guidId.Target}' that is not available in world. " +
                        "Are you trying to access a child entity, which is not supported yet?");

                    continue;
                }

                builder[guidId.Id] = id;
            }

            e.SetIdTargetCollection(builder.ToImmutable());
        }

        // Preprocess cutscenes so we can use efficient dictionaries instead of arrays.
        ImmutableArray<Entity> cutscenes = world.GetActivatedAndDeactivatedEntitiesWith(typeof(CutsceneAnchorsEditorComponent));
        foreach (Entity e in cutscenes)
        {
            ImmutableArray<AnchorId> anchors = e.GetCutsceneAnchorsEditor().Anchors;
            e.RemoveCutsceneAnchorsEditor();

            var builder = ImmutableDictionary.CreateBuilder<string, Anchor>();
            foreach (AnchorId anchor in anchors)
            {
                builder[anchor.Id] = anchor.Anchor;
            }

            e.SetCutsceneAnchors(builder.ToImmutable());
        }

        // Load all events from the asset.
        ImmutableDictionary<Guid, GameAsset> assets = Game.Data.FilterAllAssets(typeof(WorldEventsAsset));
        foreach ((_, GameAsset asset) in assets)
        {
            if (asset is not WorldEventsAsset worldEvents)
            {
                GameLogger.Error("How this is not a world event asset?");

                // How this happened?
                continue;
            }

            foreach (TriggerEventOn trigger in worldEvents.Watchers)
            {
                if (trigger.World is Guid guid && guid != world.Guid())
                {
                    // Not meant to this world.
                    continue;
                }

                world.AddEntity(trigger.CreateComponents());
            }
        }

        PostProcessEntitiesFirstTimeImpl(world, instancesToEntities);

        // Keep track of the instances <-> entity map in order to do further processing.
        world.AddEntity(new InstanceToEntityLookupComponent(instancesToEntities));
    }

    protected virtual void PostProcessEntitiesFirstTimeImpl(World world, Dictionary<Guid, int> instancesToEntities)
    {
    }
}