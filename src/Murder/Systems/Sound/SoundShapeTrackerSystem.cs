using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Systems.Sound;

[Filter(typeof(SoundShapeComponent), typeof(AmbienceComponent))]
[Watch(typeof(AmbienceComponent))]
public class SoundShapeTrackerSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            SoundShapeComponent soundShape = e.GetSoundShape();

            if (e.TryGetAmbience() is AmbienceComponent ambience)
            {
                foreach (SoundEventIdInfo info in ambience.Events)
                {
                    SoundServices.Play(info.Id, e, info.Layer, properties: SoundProperties.Persist);
                }
            }
        }
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            SoundShapeComponent soundShape = e.GetSoundShape();

            if (e.TryGetAmbience() is AmbienceComponent ambience)
            {
                foreach (SoundEventIdInfo info in ambience.Events)
                {
                    SoundServices.Stop(info.Id, fadeOut: true);
                }
            }
        }
    }
}
