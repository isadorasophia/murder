using Bang.Components;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    public enum DestroyOnAnimationCompleteFlags
    {
        Destroy = 0,
        Deactivate = 1,
        RemoveSolid = 2,
        None = 3,
        RemoveDeactivateHighlight = 4
    }

    public readonly struct DestroyOnAnimationCompleteComponent : IComponent
    {
        public readonly DestroyOnAnimationCompleteFlags Settings = DestroyOnAnimationCompleteFlags.Destroy;

        [SpriteBatchReference]
        public readonly int? ChangeSpriteBatchOnComplete { get; init; } = null; // Batches2D.GameplayBatchId;

        public DestroyOnAnimationCompleteComponent() { }

        public DestroyOnAnimationCompleteComponent(DestroyOnAnimationCompleteFlags settings) =>
            Settings = settings;

        public DestroyOnAnimationCompleteComponent(DestroyOnAnimationCompleteFlags settings, int? changeSpriteBatch) : this(settings) =>
            ChangeSpriteBatchOnComplete = changeSpriteBatch;
    }
}