using Murder.Core.Sounds;
using Murder.Services;

namespace Murder.Editor.Utilities;

public class EditorSoundServices
{
    public static void Play(SoundEventId sound)
    {
        if (Game.Preferences.SoundVolume == 0 || Game.Preferences.MusicVolume == 0)
        {
            // Make sure all our sounds are on here, so there is no confusion.
            Game.Preferences.SetSoundVolume(1);
            Game.Preferences.SetMusicVolume(1);
        }

        SoundServices.StopAll(fadeOut: false);

        /* Hardcode the position to the same as the listener (we are debugging, excuse me) */
        Game.Sound.UpdateListener(new SoundSpatialAttributes());
        _ = SoundServices.Play(sound, SoundLayer.Any, SoundProperties.Persist, attributes: new SoundSpatialAttributes());
    }
}
