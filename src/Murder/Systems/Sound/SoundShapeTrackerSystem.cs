using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Systems.Sound;

[Filter(typeof(SoundShapeComponent), typeof(AmbienceComponent))]
[Watch(typeof(AmbienceComponent))]
public abstract class SoundShapeTrackerSystem : IFixedUpdateSystem, IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        if (GetListenerPosition(world) is not Vector2 listenerPosition)
        {
            return;
        }

        foreach (Entity e in entities)
        {
            SoundShapeComponent soundShape = e.GetSoundShape();

            if (e.TryGetAmbience() is AmbienceComponent ambience)
            {
                foreach (SoundEventIdInfo info in ambience.Events)
                {
                    SoundServices.Play(info.Id, e, info.Layer, properties: SoundProperties.Persist);
                    UpdateEmitterPosition(e, listenerPosition);
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
                    SoundServices.Stop(info.Id, fadeOut: true, e.EntityId);
                }
            }
        }
    }

    public void FixedUpdate(Context context)
    {
        if (GetListenerPosition(context.World) is not Vector2 listenerPosition)
        {
            return;
        }

        foreach (Entity e in context.Entities)
        {
            SoundShapeComponent soundShape = e.GetSoundShape();
            if (soundShape.ShapeStyle == ShapeStyle.Points && soundShape.Points.Length <= 1)
            {
                continue;
            }

            UpdateEmitterPosition(e, listenerPosition);
        }
    }

    public static void UpdateEmitterPosition(Entity e, Vector2 listenerPosition)
    {
        if (e.TryGetAmbience() is not AmbienceComponent ambience)
        {
            // This might be called from another filter, so double-check.
            return;
        }

        SoundShapeComponent soundShape = e.GetSoundShape();
        Point position = e.GetGlobalTransform().Point;

        SoundPosition soundPosition = soundShape.GetSoundPosition(listenerPosition - position);
        Vector2 closestPoint = soundPosition.ClosestPoint + position;

        foreach (SoundEventIdInfo info in ambience.Events)
        {
            SoundServices.TrackEventSourcePosition(info.Id, e.EntityId, closestPoint);
        }
    }

    public abstract Vector2? GetListenerPosition(World world);
}
