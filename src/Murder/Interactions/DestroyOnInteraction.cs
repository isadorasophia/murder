using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Utilities;

namespace Murder.Interactions;

public readonly struct DestroyOnInteraction : IInteraction
{
    public readonly TargetEntity Target = TargetEntity.Self;

    public DestroyOnInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        switch (Target)
        {
            case TargetEntity.Self:
                interacted?.Destroy();
                break;
        }
    }
}
