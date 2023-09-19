using Bang;
using Bang.Entities;
using Bang.Interactions;
using Microsoft.Xna.Framework;
using Murder.Utilities;

namespace Murder.Interactions
{
    public class SetPositionInteraction : IInteraction
    {
        public Point Position;
        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            interacted?.SetGlobalPosition(Position.ToSysVector2());
        }
    }
}
