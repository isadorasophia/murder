using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This will watch for rule changes based on the blackboard system.
    /// </summary>
    [Unique]
    [DoNotPersistEntityOnSave]
    public struct RuleWatcherComponent : IModifiableComponent
    {
        public void Subscribe(Action notification)
        {
            Game.Data.TryGetActiveSaveData()?.BlackboardTracker.Watch(notification);
        }

        public void Unsubscribe(Action notification)
        {
            Game.Data.TryGetActiveSaveData()?.BlackboardTracker.ResetWatchers();
        }
    }
}
