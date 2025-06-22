using System.Collections.Immutable;

namespace Murder.Utilities.Attributes;

public enum EventMessageAttributeFlags
{
    None = 0,
    PropagateToParent = 1 << 0,

    /// <summary>
    /// This will check for a method in the component of name: "VerifyEventMessages" before
    /// displaying them in the editor.
    /// </summary>
    CheckDisplayOnlyIf = 1 << 1
}

/// <summary>
/// Notifies the editor that a set of events is available on this entity.
/// </summary>
public class EventMessagesAttribute : Attribute
{
    public readonly string[] Events;

    public readonly EventMessageAttributeFlags Flags;

    /// <summary>
    /// Also check for any sprite events in the specified fields of the component.
    /// </summary>
    public readonly string[]? CheckForSpriteFields = null;

    public EventMessagesAttribute(params string[] events) => Events = events;

    public EventMessagesAttribute(EventMessageAttributeFlags flags, params string[] events) : this(events)
    {
        Flags = flags;
    }

    public EventMessagesAttribute(EventMessageAttributeFlags flags, string[] events, string[] spriteFields) : this(flags, events)
    {
        CheckForSpriteFields = spriteFields;
    }
}
