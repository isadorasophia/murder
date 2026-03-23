using Murder.Assets;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Assets;

public class EditorCosmeticsAsset : GameAsset
{
    public readonly EventListenerEditorComponent Sounds = new();

    private EventListenerComponent? _listener = null;

    public EditorCosmeticsAsset() { }

    public void PlayEvent(string @event)
    {
        _listener ??= Sounds.ToEventListener();

        if (_listener.Value.Events.TryGetValue(@event, out SpriteEventInfo info) &&
            info.Sound is SoundEventId sound)
        {
            _ = SoundServices.Play(sound);
        }
    }

    protected override void OnModified()
    {
        _listener = null;
    }
}
