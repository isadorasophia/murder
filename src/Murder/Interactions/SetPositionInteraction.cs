using Bang;
using Bang.Entities;
using Bang.Interactions;
using Microsoft.Xna.Framework;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Interactions
{
    public class SetPositionInteraction : IInteraction
    {
        public Point Position;
        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (interacted != null)
            {
                interacted.SetGlobalPosition(Position.ToVector2());
            }
        }
    }
}
