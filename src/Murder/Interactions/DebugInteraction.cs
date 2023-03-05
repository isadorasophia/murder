using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Interactions
{
    public readonly struct DebugInteraction : Interaction
    {
        public readonly string Log;

        public DebugInteraction(string log)
        {
            Log = log;
        }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            GameLogger.Log(Log);
        }
    }
}
