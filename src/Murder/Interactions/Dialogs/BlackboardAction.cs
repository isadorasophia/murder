using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Save;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        [ShowInEditor, Tooltip("Rule requirements that need to be matched for the actions/interactions to happen")]
        private readonly ImmutableArray<CriterionNode> _requirements = ImmutableArray<CriterionNode>.Empty;

        [ShowInEditor, Tooltip("Blackboard actions that will happen when triggered.")]
        private readonly ImmutableArray<DialogAction> _actions = ImmutableArray<DialogAction>.Empty;

        [ShowInEditor, Tooltip("Interactions that will play when triggered")]
        public readonly ImmutableArray<IInteractiveComponent> _interactions = ImmutableArray<IInteractiveComponent>.Empty;

        [ShowInEditor, Tooltip("Whether this entity will only be triggered once.")]
        private readonly bool _triggeredOnlyOnce = false;
        
        public BlackboardAction()
        {
        }

        public void Invoke(World world, Entity interactor, Entity? interacted)
        {
            BlackboardTracker tracker = MurderSaveServices.CreateOrGetSave().BlackboardTracker;

            if (interactor is null) Debugger.Break();

            if (!BlackboardHelpers.Match(world, tracker, _requirements))
            {
                return;
            }

            foreach (var action in _actions)
            {
                MurderSaveServices.DoAction(tracker, action);
            }

            foreach (IInteractiveComponent interactive in _interactions)
            {
                interactive.Interact(world, interactor, interacted);
            }

            if (_triggeredOnlyOnce)
            {
                interacted?.Destroy();
            }
        }
    }
}
