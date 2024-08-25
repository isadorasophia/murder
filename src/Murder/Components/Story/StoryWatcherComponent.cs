using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Components;

[Unique]
[RuntimeOnly]
[DoNotPersistEntityOnSave]
public readonly struct StoryWatcherComponent : IModifiableComponent
{
    public void Subscribe(Action notification)
    {
        MurderSaveServices.TryGetSave()?.BlackboardTracker.Watch(notification, BlackboardKind.Story);
    }

    public void Unsubscribe(Action notification)
    {
        MurderSaveServices.TryGetSave()?.BlackboardTracker.ResetWatcher(BlackboardKind.Story, notification);
    }
}
