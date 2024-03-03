using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Assets;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Utilities;
using Newtonsoft.Json;

namespace Murder.Interactions
{
    [Flags]
    public enum AddChildProperties
    {
        None = 0,
        SendEventComponentToParent = 1
    }

    /// <summary>
    /// This will set up a landing plot by adding a child to it.
    /// </summary>
    public readonly struct AddChildOnInteraction : IInteraction
    {
        [JsonProperty]
        private readonly AssetRef<PrefabAsset> _child = new();

        [JsonProperty]
        private readonly string? _name = null;

        [JsonProperty]
        private readonly AddChildProperties _properties = AddChildProperties.None;

        public AddChildOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (interacted is null)
            {
                GameLogger.Error("Invalid null entity on AddChildInteraction.");
                return;
            }

            Entity child = _child.Asset.CreateAndFetch(world);
            interacted.AddChild(child.EntityId, _name);

            if (_properties.HasFlag(AddChildProperties.SendEventComponentToParent) && 
                child.TryGetEventListener() is EventListenerComponent c)
            {
                EventListenerComponent? previousEvent = interacted.TryGetEventListener();
                if (previousEvent is null)
                {
                    interacted.SetEventListener(c);
                }
                else
                {
                    interacted.SetEventListener(previousEvent.Value.Merge(c.Events));
                }

                child.RemoveEventListener();
            }
        }
    }
}