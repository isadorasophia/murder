using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// This is recorded as part of <see cref="PersistInteractionComponent"/>
    /// when a <see cref="Messages.InteractMessage"/> is sent.
    /// This should be manually cleaned up by the entity.
    /// </summary>
    public readonly struct InteractionMessageReceivedComponent : IComponent { }
}
