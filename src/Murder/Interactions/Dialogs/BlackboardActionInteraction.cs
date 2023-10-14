using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Save;
using Murder.Services;

namespace Murder.Interactions
{
    public readonly struct BlackboardActionInteraction : IInteraction
    {
        [ShowInEditor]
        private readonly DialogAction _action = new();

        [ShowInEditor]
        [Tooltip("Whether this entity will only be triggered once.")]
        private readonly bool _triggeredOnlyOnce = false;

        public BlackboardActionInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            BlackboardTracker tracker = MurderSaveServices.CreateOrGetSave().BlackboardTracker;
            MurderSaveServices.DoAction(tracker, _action);

            if (_triggeredOnlyOnce)
            {
                interacted?.Destroy();
            }
        }
    }
}