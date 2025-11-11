using Bang.Entities;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Helpers;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Services;

public static class SoundServices
{
    public static ValueTask PlayWithParameter<T>(
        SoundEventId id,
        Entity? target,
        SoundLayer layer,
        SoundProperties properties,
        ParameterId parameter,
        T value)
    {
        ParameterValue? parameterValue;

        try
        {
            parameterValue = new(parameter, Convert.ToSingle(value));
        }
        catch (Exception e) when (e is FormatException or OverflowException)
        {
            GameLogger.Warning($"{value} is not a valid float.");
            parameterValue = null;
        }

        return Play(id, target, layer, properties, parameterValue);
    }

    public static ValueTask Play(
        SoundEventId id, 
        Entity? target, 
        SoundLayer layer = SoundLayer.Any, 
        SoundProperties properties = SoundProperties.None)
    {
        return Play(id, target, layer, properties, parameter: null);
    }

    private static ValueTask Play(
        SoundEventId id,
        Entity? target,
        SoundLayer layer,
        SoundProperties properties,
        ParameterValue? parameter)
    {
        if (id.IsGuidEmpty || Game.Instance.IsSkippingDeltaTimeOnUpdate)
        {
            return default;
        }

        SoundSpatialAttributes? attributes = GetSpatialAttributes(target);
        return Play(id, layer, properties, attributes, target?.EntityId ?? -1, parameter);
    }

    public static ValueTask TryPlay(SoundEventId? id)
    {
        if (id is null)
        {
            return ValueTask.CompletedTask;
        }

        return Play(id.Value);
    }

    public static async ValueTask Play(
        SoundEventId id, 
        SoundLayer layer = SoundLayer.Any, 
        SoundProperties properties = SoundProperties.None, 
        SoundSpatialAttributes? attributes = null,
        int entityId = -1,
        ParameterValue? parameter = null)
    {
        if (id.IsGuidEmpty || Game.Instance.IsSkippingDeltaTimeOnUpdate)
        {
            // Do not play sounds if we are currently skipping... I think?
            return;
        }

        await Game.Sound.PlayEvent(id, new PlayEventInfo { Layer = layer, Properties = properties, Attributes = attributes, EntityId = entityId, Parameter = parameter });
    }

    public static async ValueTask PlayMusic(SoundEventId id)
    {
        await Game.Sound.PlayEvent(id, new PlayEventInfo { Layer = SoundLayer.Music, Properties = SoundProperties.Persist });
    }

    public static void SetParameter<T>(ParameterId id, SoundEventId sound, T value)
    {
        try
        {
            Game.Sound.SetParameter(sound, id, Convert.ToSingle(value));
        }
        catch (Exception e) when (e is FormatException || e is OverflowException)
        {
            GameLogger.Warning($"{value} is not a valid float.");
        }
    }

    public static float GetGlobalParameter(ParameterId id)
    {
        return Game.Sound.GetGlobalParameter(id);
    }

    public static void SetGlobalParameter<T>(ParameterId id, T value)
    {
        try
        {
            Game.Sound.SetGlobalParameter(id, Convert.ToSingle(value));
        }
        catch (Exception e) when (e is FormatException || e is OverflowException)
        {
            GameLogger.Warning($"{value} is not a valid float.");
        }
    }

    public static void Stop(SoundEventId? id, bool fadeOut, int entityId = -1)
    {
        Game.Sound.Stop(id, entityId, fadeOut);
    }

    /// <summary>
    /// Stop all the ongoing events.
    /// </summary>
    /// <param name="fadeOut">Whether it should fade out in fmod.</param>
    public static void StopAll(bool fadeOut)
    {
        Stop(SoundLayer.Any, fadeOut);
    }

    public static void Stop(SoundLayer layer, bool fadeOut)
    {
        Game.Sound.Stop(layer, fadeOut);
    }

    public static void Resume(SoundLayer layer)
    {
        Game.Sound.Resume(layer);
    }

    public static void Pause(SoundLayer layer)
    {
        Game.Sound.Pause(layer);
    }

    public static void TrackEventSourcePosition(SoundEventId eventId, Entity e)
    {
        SoundSpatialAttributes? attributes = GetSpatialAttributes(e);
        if (attributes is null)
        {
            GameLogger.Error("How is the entity attribute null?");
            return;
        }

        Game.Sound.UpdateEvent(eventId, e.EntityId, attributes.Value);
    }

    public static void TrackEventSourcePosition(SoundEventId eventId, int entityId, Vector2 position)
    {
        SoundSpatialAttributes attributes = GetSpatialAttributes(position);
        Game.Sound.UpdateEvent(eventId, entityId, attributes);
    }

    public static bool IsPlaying(SoundEventId eventId, int entityId)
    {
        return Game.Sound.IsEventPlaying(eventId, entityId);
    }

    /// <summary>
    /// Return the spatial attributes for playing a sound from <paramref name="target"/>.
    /// </summary>
    public static SoundSpatialAttributes? GetSpatialAttributes(Entity? target)
    {
        SoundSpatialAttributes attributes = new();
        if (target is null)
        {
            return null;
        }

        Entity root = EntityServices.FindRootEntity(target);
        if (root.TryGetTransform() is IMurderTransformComponent transform)
        {
            attributes.Position = new Vector3(transform.Vector2.X, transform.Vector2.Y, z: 0);
        }

        if (target.TryGetFacing()?.Direction is Direction direction)
        {
            attributes.Direction = direction;
        }

        return attributes;
    }

    /// <summary>
    /// Return the spatial attributes for playing a sound from <paramref name="position"/>.
    /// </summary>
    /// <param name="position">Position for listener.</param>
    public static SoundSpatialAttributes GetSpatialAttributes(Vector3 position)
    {
        SoundSpatialAttributes attributes = new()
        {
            Position = position,
            Direction = Direction.Up
        };

        return attributes;
    }
    /// <summary>
    /// Return the spatial attributes for playing a sound from <paramref name="position"/>.
    /// </summary>
    /// <param name="position">Position for listener.</param>
    public static SoundSpatialAttributes GetSpatialAttributes(Vector2 position)
    {
        SoundSpatialAttributes attributes = new()
        {
            Position = new(position.X, position.Y, 0),
            Direction = Direction.Up
        };

        return attributes;
    }

    /// <summary>
    /// Try to get a sound id associated with an <paramref name="animationEventId"/>
    /// on an entity with an <see cref="EventListenerComponent"/>.
    /// </summary>
    /// <param name="e">Target entity.</param>
    /// <param name="animationEventId">Animation string identifier.</param>
    /// <returns>If found, the sound event id for the animation event.</returns>
    public static SoundEventId? TryGetSoundForEvent(Entity e, string animationEventId)
    {
        EventListenerComponent? eventListener = e.TryGetEventListener() ?? e.TryFetchChild("interaction")?.TryGetEventListener();
        if (eventListener is null)
        {
            return null;
        }

        if (eventListener.Value.Events.TryGetValue(animationEventId, out SpriteEventInfo info))
        {
            return info.Sound;
        }

        return null;
    }

    public static ImmutableDictionary<string, SpriteEventInfo> ReplaceIdentifiers(
        ImmutableDictionary<string, SpriteEventInfo> source,
        Func<string, string> converter)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, SpriteEventInfo>();

        foreach ((string id, SpriteEventInfo info) in source)
        {
            string newIdentifier = converter(id);
            builder[newIdentifier] = new(newIdentifier, info.Sound, info.Persisted, info.Interactions);
        }

        return builder.ToImmutable();
    }

    public static Vector3 ToListenerVector3(Vector2 position)
    {
        return new(position.X, position.Y, 1);
    }

    public static void UpdateListenerPosition(Vector2 position)
    {
        Vector3 position3d = ToListenerVector3(position);
        SoundSpatialAttributes attributes = GetSpatialAttributes(position3d);

        Game.Sound.UpdateListener(attributes);
    }
}