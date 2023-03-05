using Bang;
using Bang.Entities;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Save;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Interactions
{
    internal readonly struct BlackboardAction
    {
        /// <summary>
        /// List of requirements which will trigger the interaction.
        /// </summary>
        [ShowInEditor]
        private readonly ImmutableArray<CriterionNode> _requirements = ImmutableArray<CriterionNode>.Empty;

        [ShowInEditor]
        private readonly DialogAction _action = new();

        [ShowInEditor]
        [Tooltip("Whether this entity will only be triggered once.")]
        private readonly bool _triggeredOnlyOnce = false;
        
        public BlackboardAction()
        {
        }

        public void Invoke(World world, Entity interactor, Entity? interacted)
        {
            BlackboardTracker tracker = MurderSaveServices.CreateOrGetSave().BlackboardTracker;

            foreach (var rule in _requirements)
            {
                if (!tracker.Matches(rule.Criterion, interactor.TryGetSpeaker()?.Speaker, world, interacted?.EntityId, out _))
                {
                    // If any rule doesn't match, stop.
                    return;
                }
            }

            MurderSaveServices.DoAction(tracker, _action);

            if (_triggeredOnlyOnce)
            {
                interacted?.Destroy();
            }
        }
    }
}
