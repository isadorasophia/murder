using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [Requires(typeof(FacingComponent))]
    public readonly struct AgentSpriteComponent : IComponent
    {
        [GameAssetId(typeof(SpriteAsset))]
        public Guid AnimationGuid { get; init; } = Guid.Empty;

        [SpriteBatchReference]
        public readonly int TargetSpriteBatch = Batches2D.GameplayBatchId;

        public readonly int YSortOffset { get; init; } = 0;

        public readonly string IdlePrefix = "idle";
        public readonly string WalkPrefix = "walk";

        public AgentSpriteComponent()
        {
        }

        private AgentSpriteComponent(Guid animationGuid, int targetSpriteBatch, int ySortOffset, string idlePrefix, string walkPrefix)
        {
            AnimationGuid = animationGuid;
            TargetSpriteBatch = targetSpriteBatch;

            YSortOffset = ySortOffset;

            IdlePrefix = idlePrefix;
            WalkPrefix = walkPrefix;
        }

        public AgentSpriteComponent WithIdleAndWalkPrefix(string idle, string walk) =>
            new AgentSpriteComponent(AnimationGuid, TargetSpriteBatch, YSortOffset, idle, walk);
    }
}