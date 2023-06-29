using Bang.Components;
using Bang.Interactions;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Interactions;
using System.Collections.Immutable;

namespace Murder.Core
{
    public readonly struct RequirementsCollection
    {
        /// <summary>
        /// List of requirements which will trigger the interactive component within the same entity.
        /// </summary>
        public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;

        public RequirementsCollection() { }
    }

    public readonly struct TriggerEventOn
    {
        public readonly string Name = string.Empty;

        public readonly ImmutableArray<RequirementsCollection> Requirements = ImmutableArray<RequirementsCollection>.Empty;

        public readonly ImmutableArray<IInteractiveComponent> Triggers = ImmutableArray<IInteractiveComponent>.Empty;

        [Tooltip("Which world does this apply")]
        [GameAssetId<WorldAsset>]
        [Default("Only on world...")]
        public readonly Guid? World = null;

        [Tooltip("Whether this should be applied only once for each world")]
        public readonly bool OnlyOnce = false;

        public TriggerEventOn() { }

        public TriggerEventOn(string name) => Name = name;

        public IComponent[] CreateComponents()
        {
            var builder = ImmutableArray.CreateBuilder<InteractOnRuleMatchComponent>();
            foreach (RequirementsCollection r in Requirements)
            {
                builder.Add(new InteractOnRuleMatchComponent(
                    InteractOn.AddedOrModified, 
                    OnlyOnce ? AfterInteractRule.RemoveEntity : AfterInteractRule.Always, 
                    r.Requirements));
            }

            return new IComponent[]
            {
                new InteractOnRuleMatchCollectionComponent(builder.ToImmutable()),
                new InteractiveComponent<InteractionCollection>(new InteractionCollection(Triggers))
            };
        }
    }
}
