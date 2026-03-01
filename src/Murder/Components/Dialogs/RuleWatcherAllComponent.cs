using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Components;

[Unique]
[RuntimeOnly]
[DoNotPersistEntityOnSave]
public readonly struct RuleWatcherAllComponent : IModifiableComponent
{
    public void Subscribe(Action notification)
    {
        MurderSaveServices.CreateOrGetSave()?.BlackboardTracker.WatchAll(notification, BlackboardKind.All);
    }

    public void Unsubscribe(Action notification)
    {
        MurderSaveServices.TryGetSave()?.BlackboardTracker.ResetAllWatchers(notification, BlackboardKind.All);
    }
}
