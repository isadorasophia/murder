
using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Interactions;
using Bang.Systems;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Interactions;
using Murder.Messages;
using Murder.Save;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems.Graphics;

/// <summary>
/// Requires a InteractOnRuleMatch system
/// </summary>
[Filter(typeof(RuleWatcherComponent))]
[Watch(typeof(RuleWatcherComponent))]
internal class PlayAnimationOnRuleMatchSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        CheckAndTriggerRules(world);
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        CheckAndTriggerRules(world);
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        CheckAndTriggerRules(world);
    }

    private int CheckEntity(World world, BlackboardTracker tracker, PlayAnimationOnRuleComponent ruleComponent)
    {
        for (int i = 0; i < ruleComponent.Rules.Length; i++)
        {
            var node = ruleComponent.Rules[i];

            if (BlackboardHelpers.Match(world, tracker, node.Requirements))
            {
                return i;
            }
        }

        return -1;
    }
    
    private void CheckAndTriggerRules(World world)
    {
        SaveData save = MurderSaveServices.CreateOrGetSave();
        BlackboardTracker tracker = save.BlackboardTracker;

        // Fetch all entities which might be affected by a rule change.
        ImmutableArray<Entity> interactives = world.GetEntitiesWith(typeof(PlayAnimationOnRuleComponent));
        foreach (Entity e in interactives)
        {
            if (e.IsDestroyed)
            {
                continue;
            }

            // Match each of its requirements.
            PlayAnimationOnRuleComponent ruleComponent = e.GetPlayAnimationOnRule();

            int matched = CheckEntity(world, tracker, ruleComponent);

            // Check if entity rule already matched before
            if (e.TryGetAnimationRuleMatched() is AnimationRuleMatchedComponent previousMatch && previousMatch.RuleIndex == matched)
                continue;

            if (matched >= 0) 
            {
                e.PlaySpriteAnimation(ruleComponent.Rules[matched].Animation);
                
                // Set matched component so it won't keep set it again
                e.SetAnimationRuleMatched(matched);
            }
        }
    }
}
