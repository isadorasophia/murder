using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Interactions
{
    public readonly struct AdvancedBlackboardInteraction : Interaction
    {
        [JsonProperty, ShowInEditor]
        private readonly ImmutableArray<BlackboardAction> _actions = ImmutableArray<BlackboardAction>.Empty;

        public AdvancedBlackboardInteraction()
        {
        }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            foreach (var action in _actions)
            {
                action.Invoke(world, interactor, interacted);
            }
        }
    }
}
