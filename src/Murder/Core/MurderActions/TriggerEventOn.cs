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

    [Flags]
    public enum TriggerEventsProperty
    {
        None = 0,
        OnlyInteractOnce = 0x1,
        OnlyOnStart = 0x10
    }

    public readonly struct TriggerEventOn
    {
        public readonly string Name = string.Empty;

        [Tooltip("If none, this will always automatically start")]
        public readonly ImmutableArray<RequirementsCollection> Requirements = ImmutableArray<RequirementsCollection>.Empty;

        public readonly ImmutableArray<IInteractiveComponent> Triggers = ImmutableArray<IInteractiveComponent>.Empty;

        [Tooltip("Which world does this apply")]
        [GameAssetId<WorldAsset>]
        [Default("Only on world...")]
        public readonly Guid? World = null;

        public readonly TriggerEventsProperty Properties = TriggerEventsProperty.None;

        public TriggerEventOn() { }

        public TriggerEventOn(string name) => Name = name;

        public IComponent[] CreateComponents()
        {
            List<IComponent> result = [];
            if (Requirements.IsEmpty)
            {
                result.Add(new InteractOnStartComponent());
            }
            else
            {
                var builder = ImmutableArray.CreateBuilder<InteractOnRuleMatchComponent>();
                foreach (RequirementsCollection r in Requirements)
                {
                    builder.Add(new InteractOnRuleMatchComponent(
                        InteractOn.AddedOrModified,
                        Properties.HasFlag(TriggerEventsProperty.OnlyInteractOnce) ? 
                            AfterInteractRule.RemoveComponent : AfterInteractRule.Always,
                        r.Requirements));
                }

                result.Add(new InteractOnRuleMatchCollectionComponent(builder.ToImmutable()));
            }

            result.Add(new InteractiveComponent<InteractionCollection>(new InteractionCollection(Triggers)));

            return [.. result];
        }
    }
}