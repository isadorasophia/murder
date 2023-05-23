using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Utilities;
using Newtonsoft.Json;

namespace Murder.Interactions
{
    /// <summary>
    /// This will set up a landing plot by adding a child to it.
    /// </summary>
    public readonly struct AddChildOnInteraction : Interaction
    {
        [JsonProperty]
        private readonly AssetRef<PrefabAsset> _child = new();

        [JsonProperty]
        private readonly string? _name = null;

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
        }
    }
}
