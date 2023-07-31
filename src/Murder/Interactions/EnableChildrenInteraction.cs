using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Components;
using Murder.Prefabs;

namespace Murder.Interactions;

public readonly struct EnableChildrenInteraction : IInteraction
{
    
    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (interacted == null)
            return;

        foreach (var c in interacted.Children)
        {
            if (world.TryGetEntity(c) is Entity child)
            {
                if (child.IsActive)
                    continue;

                if (child.TryGetSprite() is SpriteComponent sprite)
                {
                    child.SetSprite(sprite.StartNow(Game.Now));
                }
                child.Activate();
            }
        }
    }
}
