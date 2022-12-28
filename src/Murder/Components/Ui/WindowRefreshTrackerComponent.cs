using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This will watch for rule changes based on the blackboard system.
    /// </summary>
    [Unique]
    [RuntimeOnly]
    [DoNotPersistEntityOnSave]
    public struct WindowRefreshTrackerComponent : IModifiableComponent
    {
        public void Subscribe(Action notification)
        {
            Game.Instance.ActiveScene?.AddOnWindowRefresh(notification);
        }

        public void Unsubscribe(Action notification)
        {
            Game.Instance.ActiveScene?.ResetWindowRefreshEvents();
        }
    }
}
