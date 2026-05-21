using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Assets;
using Murder.Attributes;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Interactions;

public enum AddEntityFlags
{
    None = 0,
    FromInteractor = 1,
    FromInteracted = 1 << 1,
    RemoveAfterTriggered = 1 << 2
}

/// <summary>
/// This will trigger an effect by placing <see cref="Prefab"/> in the world.
/// </summary>
public readonly struct AddEntityOnInteraction : IInteraction
{
    [GameAssetId(typeof(PrefabAsset))]
    [ShowInEditor]
    public readonly Guid Prefab = Guid.Empty;

    [ShowInEditor]
    public readonly ImmutableArray<IComponent>? CustomComponents = null;

    public AddEntityFlags Flags { get; init; } = AddEntityFlags.None;

    public AddEntityOnInteraction() { }

    public AddEntityOnInteraction(Guid prefab) => Prefab = prefab;

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        Entity result = AssetServices.Create(world, Prefab);

        if (CustomComponents is not null)
        {
            foreach (IComponent c in CustomComponents)
            {
                // We need to guarantee that any modifiable components added here are safe.
                IComponent component = c is IModifiableComponent ? SerializationHelper.DeepCopy(c) : c;
                result.AddOrReplaceComponent(component, component.GetType());
            }
        }

        // Adjust the position, if applicable.
        Entity? target = null;
        if (Flags.HasFlag(AddEntityFlags.FromInteractor))
        {
            target = interactor;
        }
        else if (Flags.HasFlag(AddEntityFlags.FromInteracted))
        {
            target = interacted;
        }

        if (target?.TryGetGlobalPosition() is Vector2 position)
        {
            result.SetPosition(position);
        }

        if (Flags.HasFlag(AddEntityFlags.RemoveAfterTriggered))
        {
            interacted?.RemoveInteractive();
        }
    }
}