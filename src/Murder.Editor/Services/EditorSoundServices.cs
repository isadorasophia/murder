using Bang;
using Bang.Contexts;
using Bang.Entities;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

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

    private static Type[]? _soundComponents = null;

    public static ImmutableArray<Entity> GetSoundEntities(World world)
    {
        _soundComponents ??= ReflectionHelper.FetchComponentsWithAttribute<SoundAttribute>(cache: true);

        return world.GetEntitiesWith(ContextAccessorFilter.AnyOf, _soundComponents);
    }
}
