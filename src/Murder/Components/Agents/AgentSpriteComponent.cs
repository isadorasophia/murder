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

        public readonly int YSortOffset = 0;

        public readonly string IdlePrefix = "idle";
        public readonly string WalkPrefix = "walk";

        /// <summary>
        /// LEGACY for keeping serialization, do no use
        /// use <see cref="SpriteFacingComponent"/> to configure this instead.
        /// </summary>
        public string Suffix { set { } }

        /// <summary>
        /// LEGACY for keeping serialization, do no use
        /// use <see cref="SpriteFacingComponent"/> to configure this instead.
        /// </summary>
        public readonly float AngleSuffixOffset { set { } }

        /// <summary>
        /// LEGACY for keeping serialization, do no use
        /// use <see cref="SpriteFacingComponent"/> to configure this instead.
        /// </summary>
        public readonly bool FlipWest { set { } }

        public AgentSpriteComponent()
        {
        }

        private AgentSpriteComponent(Guid guid, int batch, int ySort, string idlePrefix, string walkPrefix, bool flipWest)
        {
            AnimationGuid = guid;
            TargetSpriteBatch = batch;

            YSortOffset = ySort;

            IdlePrefix = idlePrefix;
            WalkPrefix = walkPrefix;
        }
    }
}