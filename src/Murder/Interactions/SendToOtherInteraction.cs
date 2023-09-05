using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Messages;
using Murder.Services;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    public readonly struct SendToOtherInteraction : IInteraction
    {
        /// <summary>
        /// Guid of the target entity.
        /// </summary>
        [InstanceId]
        public readonly ImmutableArray<string> _targets = ImmutableArray<string>.Empty;

        public readonly IMessage? Message = null;

        public SendToOtherInteraction()
        {
        }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (interacted == null || Message is null)
            {
                return;
            }
            
            foreach (var item in _targets)
            {
                var target = interacted.TryFindTarget(world) ?? interacted.TryFindTarget(world, item);
                target?.SendMessage(Message);
            }
        }
    }
}
