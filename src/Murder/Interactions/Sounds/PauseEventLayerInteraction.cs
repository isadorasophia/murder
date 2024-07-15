using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions;

[CustomName("\uf2a2 Pause events on interaction")]
public readonly struct PauseEventLayerInteraction : IInteraction
{
    public readonly ImmutableArray<SoundLayer> Layers = [];

    public readonly bool Pause = true;

    public PauseEventLayerInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        foreach (SoundLayer layer in Layers)
        {
            if (Pause)
            {
                SoundServices.Pause(layer);
            }
            else
            {
                SoundServices.Resume(layer);
            }
        }
    }
}
