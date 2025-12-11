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

        public BlackboardActionInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            MurderSaveServices.DoAction(_action);
        }
    }
}