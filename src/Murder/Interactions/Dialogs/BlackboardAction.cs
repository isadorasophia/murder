using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Save;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    public readonly struct BlackboardAction
    {
        /// <summary>
        /// List of requirements which will trigger the interaction.
        /// </summary>
        [ShowInEditor, Tooltip("Rule requirements that need to be matched for the actions/interactions to happen")]
        private readonly ImmutableArray<CriterionNode> _requirements = [];

        [ShowInEditor, Tooltip("Blackboard actions that will happen when triggered.")]
        private readonly ImmutableArray<DialogAction> _actions = [];

        [ShowInEditor, Tooltip("Interactions that will play when triggered")]
        public readonly ImmutableArray<IInteractiveComponent>? _interactions = null;

        public BlackboardAction()
        {
        }

        public void Invoke(World world, Entity interactor, Entity? interacted)
        {
            BlackboardTracker tracker = MurderSaveServices.CreateOrGetSave().BlackboardTracker;

            if (!BlackboardHelpers.Match(world, tracker, _requirements))
            {
                return;
            }

            foreach (DialogAction action in _actions)
            {
                MurderSaveServices.DoAction(tracker, action);
            }

            if (_interactions is not null)
            {
                foreach (IInteractiveComponent interactive in _interactions.Value)
                {
                    interactive.Interact(world, interactor, interacted);
                }
            }
        }
    }
}