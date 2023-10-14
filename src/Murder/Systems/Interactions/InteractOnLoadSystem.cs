using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets;
using Murder.Components;
using Murder.Save;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Filter(typeof(RemoveEntityOnRuleMatchAtLoadComponent))]
    [Watch(typeof(RemoveEntityOnRuleMatchAtLoadComponent))]
    internal class InteractOnLoadSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            SaveData save = MurderSaveServices.CreateOrGetSave();
            BlackboardTracker tracker = save.BlackboardTracker;

            foreach (Entity e in entities)
            {
                var component = e.GetRemoveEntityOnRuleMatchAtLoad();
                if (BlackboardHelpers.Match(world, tracker, component.Requirements))
                {
                    if (e.TryGetIdTarget()?.Target is int targetId && world.TryGetEntity(targetId) is Entity target)
                    {
                        target.Destroy();
                    }

                    e.Destroy();
                }

                e.RemoveRemoveEntityOnRuleMatchAtLoad();
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}