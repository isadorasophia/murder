namespace Murder.Utilities.Attributes;

/// <summary>
/// Notifies the editor that a set of events is available on this entity.
/// </summary>
public class EventMessagesAttribute : Attribute
{
    public readonly string[] Events;

    public readonly bool PropagateToParent;

    public EventMessagesAttribute(params string[] events) => Events = events;

    public EventMessagesAttribute(bool propagateToParent, params string[] events) : this(events)
    {
        PropagateToParent = propagateToParent;
    }
}
