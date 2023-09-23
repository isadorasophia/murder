using Bang;
using Bang.Entities;
using Bang.Interactions;
using Microsoft.Xna.Framework;
using Murder.Utilities;

namespace Murder.Interactions
{
    public readonly struct SetPositionInteraction : IInteraction
    {
        public readonly Point Position = new();

        public SetPositionInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            interacted?.SetGlobalPosition(Position.ToSysVector2());
        }
    }
}
