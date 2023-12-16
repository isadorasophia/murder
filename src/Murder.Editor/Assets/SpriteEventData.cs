namespace Murder.Editor.Assets;

public class SpriteEventData
{
    public readonly Dictionary<string, Dictionary<int, string>> Events = new();

    public readonly Dictionary<string, HashSet<int>> DeletedEvents = new();

    /// <summary>
    /// Apply custom messages to an existing dictionary of events.
    /// </summary>
    public void FilterEventsForAnimation(string animation, ref Dictionary<int, string> events)
    {
        if (Events.TryGetValue(animation, out var customEvents))
        {
            foreach ((int frame, string message) in customEvents)
            {
                events[frame] = message;
            }
        }

        if (DeletedEvents.TryGetValue(animation, out var deletedEvents))
        {
            foreach (int frame in deletedEvents)
            {
                events.Remove(frame);
            }
        }
    }

    public Dictionary<int, string> GetEventsForAnimation(string animation)
    {
        if (!Events.TryGetValue(animation, out var dictionary))
        {
            dictionary = new();
            Events[animation] = dictionary;
        }

        return dictionary;
    }

    public void AddEvent(string animation, int frame, string message)
    {
        Dictionary<int, string> dict = GetEventsForAnimation(animation);
        dict[frame] = message;

        RemoveFromDeletedEvents(animation, frame);
    }

    public void RemoveEvent(string animation, int frame)
    {
        if (!Events.TryGetValue(animation, out var dictionary))
        {
            AddToDeletedEvent(animation, frame);
            return;
        }

        bool hadValueDefined = dictionary.Remove(frame);
        if (!hadValueDefined)
        {
            // Value was previously not tracked here. It is likely added by aseprite.
            // So let's override that!
            AddToDeletedEvent(animation, frame);
        }
    }

    private void AddToDeletedEvent(string animation, int frame)
    {
        if (!DeletedEvents.TryGetValue(animation, out var dictionary))
        {
            dictionary = new();
            DeletedEvents[animation] = dictionary;
        }

        dictionary.Add(frame);
    }

    private bool RemoveFromDeletedEvents(string animation, int frame)
    {
        if (!DeletedEvents.TryGetValue(animation, out var dictionary))
        {
            return false;
        }

        _ = dictionary.Remove(frame);
        if (dictionary.Count == 0)
        {
            DeletedEvents.Remove(animation);
        }

        return true;
    }

    public SpriteEventData() { }
}