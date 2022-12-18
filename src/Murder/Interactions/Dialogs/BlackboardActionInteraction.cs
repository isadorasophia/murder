using Bang.Entities;
using Bang;
using Murder.Core.Dialogs;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Save;
using Murder.Utilities;

namespace Murder.Interactions
{
    public readonly struct BlackboardActionInteraction : Interaction
    {
        [ShowInEditor]
        private readonly DialogAction _action;

        [ShowInEditor]
        [Tooltip("Whether this entity will only be triggered once.")]
        private readonly bool _triggeredOnlyOnce;
        
        public void Interact(World world, Entity interactor, Entity interacted)
        {
            BlackboardTracker tracker = MurderSaveServices.CreateOrGetSave().BlackboardTracker;
            MurderSaveServices.DoAction(tracker, _action);

            if (_triggeredOnlyOnce)
            {
                interacted.Destroy();
            }
        }
    }
}
