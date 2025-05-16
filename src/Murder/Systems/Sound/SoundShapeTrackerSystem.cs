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
using System.Diagnostics;
using System.Numerics;

namespace Murder.Systems.Sound;

[Filter(typeof(SoundShapeComponent), typeof(AmbienceComponent))]
[Watch(typeof(AmbienceComponent))]
public abstract class SoundShapeTrackerSystem : IFixedUpdateSystem, IReactiveSystem
{
    public void OnActivated(World world, ImmutableArray<Entity> entities)
    {
        OnAdded(world, entities);
    }

    public void OnDeactivated(World world, ImmutableArray<Entity> entities)
    {
        OnRemoved(world, entities);
    }

    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        if (GetListenerPosition(world, ListenerKind.Camera) is not Vector2 cameraPosition ||
            GetListenerPosition(world, ListenerKind.Player) is not Vector2 playerPosition)
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

                    if (ambience.Listener == ListenerKind.Player)
                    {
                        // If the listener is actually the player, we'll have to be creative so fmod can use the relative position
                        // of the player instead of the camera.
                        UpdateEmitterPosition(e, playerPosition, cameraPosition);
                    }
                    else
                    {
                        UpdateEmitterPosition(e, cameraPosition);
                    }
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
                    SoundServices.Stop(info.Id, fadeOut: true, -1); // can this backfire? o_O
                }
            }
        }
    }

    public void FixedUpdate(Context context)
    {
        if (GetListenerPosition(context.World, ListenerKind.Camera) is not Vector2 cameraPosition ||
            GetListenerPosition(context.World, ListenerKind.Player) is not Vector2 playerPosition)
        {
            return;
        }

        foreach (Entity e in context.Entities)
        {
            AmbienceComponent ambience = e.GetAmbience();
            SoundShapeComponent soundShape = e.GetSoundShape();

            // If the shape doesn't change and the listener is already the default one (camera), move on.
            if (soundShape.ShapeStyle == ShapeStyle.Points && soundShape.Points.Length <= 1 &&
                ambience.Listener == ListenerKind.Camera)
            {
                continue;
            }

            if (ambience.Listener == ListenerKind.Player)
            {
                // If the listener is actually the player, we'll have to be creative so fmod can use the relative position
                // of the player instead of the camera.
                UpdateEmitterPosition(e, playerPosition, cameraPosition);
            }
            else
            {
                UpdateEmitterPosition(e, cameraPosition);
            }
        }
    }

    /// <summary>
    /// Update emitter position at <paramref name="e"/>.
    /// </summary>
    /// <param name="e">The entity that is emitting the sound.</param>
    /// <param name="listenerPosition">The listener position.</param>
    /// <param name="relativeToGlobalListenerAt">
    /// If <paramref name="listenerPosition"/> is different than the global listener,
    /// this will have the location of the global listener, so we can apply the distance when passing that through the emitter.
    /// </param>
    /// <remarks>
    /// It doesn't seem like we can update two listeners position simultaneously.
    /// </remarks>
    public static void UpdateEmitterPosition(Entity e, Vector2 listenerPosition, Vector2? relativeToGlobalListenerAt = null)
    {
        if (e.TryGetAmbience() is not AmbienceComponent ambience)
        {
            // This might be called from another filter, so double-check.
            return;
        }

        SoundShapeComponent soundShape = e.GetSoundShape();
        Vector2 position = e.GetGlobalTransform().Vector2;

        Vector2 soundOrigin = position;

        // Only calculate the closest point when the shape is not a single point.
        if (soundShape.ShapeStyle != ShapeStyle.Points || soundShape.Points.Length > 1)
        {
            ShapePosition soundPosition = soundShape.GetSoundPosition(listenerPosition - position);
            soundOrigin = soundPosition.ClosestPoint + position;
        }

        if (relativeToGlobalListenerAt is not null)
        {
            // Let's fake the actual closest point so it looks closer than it is.
            soundOrigin = soundOrigin - listenerPosition + relativeToGlobalListenerAt.Value;
        }

        foreach (SoundEventIdInfo info in ambience.Events)
        {
            SoundServices.TrackEventSourcePosition(info.Id, e.EntityId, soundOrigin);
        }
    }

    public abstract Vector2? GetListenerPosition(World world, ListenerKind listener);
}
