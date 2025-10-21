using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions;

[Flags]
public enum PlayEventSettings
{
    None = 0,
    IgnoreEntitySource = 1
}

[Sound]
[CustomName($"\uf2a2 {nameof(PlayEventInteraction)}")]
public readonly struct PlayEventInteraction : IInteraction
{
    public readonly SoundEventId? Event = null;

    public readonly SoundProperties Properties = SoundProperties.Persist;
    public readonly SoundLayer Layer = SoundLayer.Ambience;
    public readonly PlayEventSettings Settings = PlayEventSettings.None;

    public PlayEventInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (Event is null)
        {
            return;
        }

        int entityId = Settings.HasFlag(PlayEventSettings.IgnoreEntitySource) ? -1 : interactor.EntityId;

        if (Properties.HasFlag(SoundProperties.StopOtherEventsInLayer))
        {
            SoundServices.Stop(id: null, fadeOut: true);
        }

        SoundProperties properties = Properties;
        if (Properties.HasFlag(SoundProperties.Persist))
        {
            properties |= SoundProperties.SkipIfAlreadyPlaying;
        }

        if (interactor.HasOnExitMessage())
        {
            SoundServices.Stop(Event.Value, fadeOut: true, entityId);
        }
        else
        {
            if (entityId == -1)
            {
                _ = SoundServices.Play(Event.Value, Layer, properties);
            }
            else
            {
                _ = SoundServices.Play(Event.Value, interactor, Layer, properties);
            }
        }
    }
}
