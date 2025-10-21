using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions;

[Sound]
[CustomName($"\uf2a2 {nameof(PlayEventWithSettingsInteraction)}")]
public readonly struct PlayEventWithSettingsInteraction : IInteraction
{
    public readonly SoundEventId? Event = null;

    public readonly ImmutableArray<IInteractiveComponent>? OnBeforePlay = null;

    public readonly SoundProperties Properties = SoundProperties.Persist;
    public readonly SoundLayer Layer = SoundLayer.Ambience;
    public readonly PlayEventSettings Settings = PlayEventSettings.IgnoreEntitySource;

    public PlayEventWithSettingsInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (Event is null)
        {
            return;
        }

        int entityId = Settings.HasFlag(PlayEventSettings.IgnoreEntitySource) ? -1 : interactor.EntityId;

        bool isPlaying = SoundServices.IsPlaying(Event.Value, entityId);
        if (isPlaying && 
            (Properties.HasFlag(SoundProperties.Persist) || 
             Properties.HasFlag(SoundProperties.SkipIfAlreadyPlaying)))
        {
            return;
        }

        if (OnBeforePlay is not null)
        {
            foreach (IInteractiveComponent c in OnBeforePlay.Value)
            {
                c.Interact(world, interactor, interacted);
            }
        }

        _ = SoundServices.Play(Event.Value, Layer, Properties, entityId: entityId);
    }
}
