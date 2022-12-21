using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    /// <summary>
    /// This will trigger an effect by placing <see cref="_prefab"/> in the world.
    /// </summary>
    public readonly struct AddEntityOnInteraction : Interaction
    {
        [GameAssetId(typeof(PrefabAsset))]
        [ShowInEditor]
        private readonly Guid _prefab = Guid.Empty;

        [ShowInEditor]
        private readonly ImmutableArray<IComponent> _customComponents = ImmutableArray<IComponent>.Empty;
        
        public AddEntityOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity interacted)
        {
            Entity result = AssetServices.Create(world, _prefab);
            
            foreach (IComponent c in _customComponents)
            {
                // We need to guarantee that any modifiable components added here are safe.
                IComponent component = c is IModifiableComponent ? SerializationHelper.DeepCopy(c) : c;
                result.AddOrReplaceComponent(component, component.GetType());
            }
            
            // Adjust the position, if applicable.
            if (interacted.TryGetTransform() is IMurderTransformComponent transform)
            {
                result.SetTransform(transform);
            }

            // Remove after triggered.
            interacted.RemoveInteractive();
        }
    }
}
