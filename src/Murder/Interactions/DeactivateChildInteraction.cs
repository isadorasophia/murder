using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Interactions
{
    public readonly struct DeactivateChildInteraction() : IInteraction
    {
        [ChildId, Serialize, ShowInEditor]
        private readonly ImmutableArray<string> _child = [];

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (interacted is Entity target)
            {
                foreach (var child in _child)
                {
                    if (target.TryFetchChild(child) is Entity c)
                    {
                        c.Deactivate();
                    }
                }
            }
        }

        private static void LogFailedTargetWarning()
        {
            GameLogger.Warning("DisableChildInteraction has no valid target");
        }
    }
}
