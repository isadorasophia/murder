using Bang.Components;
using Bang.Interactions;
using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    [Flags]
    public enum InteractOnCollisionFlags
    { 
        None = 0,

        /// <summary>
        /// Whether only a player is able to activate this.
        /// </summary>
        PlayerOnly = 1,

        /// <summary>
        /// Whether this should not be activated again.
        /// </summary>
        Once = 1 << 1,

        OnceEveryLoad = 1 << 2,

        /// <summary>
        /// Skip notifying on exit if an actor is also inside.
        /// </summary>
        SkipExitIfInteractorInside = 1 << 3
    }

    public readonly struct InteractOnCollisionComponent : IComponent
    {
        [Tooltip("Interactions that will be triggered in addition to interactions in this entity.")]
        public readonly ImmutableArray<IInteractiveComponent> CustomEnterMessages = ImmutableArray<IInteractiveComponent>.Empty;

        [Tooltip("Interactions that will be triggered in addition to interactions in this entity.")]
        public readonly ImmutableArray<IInteractiveComponent> CustomExitMessages = ImmutableArray<IInteractiveComponent>.Empty;

        public readonly InteractOnCollisionFlags Flags { get; init; } = InteractOnCollisionFlags.None;

        public InteractOnCollisionComponent() { }

        public InteractOnCollisionComponent(InteractOnCollisionFlags flags) => Flags = flags;
    }
}