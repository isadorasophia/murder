using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Components
{
    [Requires(typeof(FacingComponent))]
    public readonly struct AgentSpriteComponent : IComponent
    {
        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid AnimationGuid = Guid.Empty;

        public readonly TargetSpriteBatches TargetSpriteBatch = TargetSpriteBatches.Gameplay;

        public readonly int YSortOffset = 0;

        public readonly string IdlePrefix = "idle";
        public readonly string WalkPrefix = "walk";

        [Tooltip("Starting on 0, going clockwise")]
        public readonly string Suffix = "e,se,s,se,e,ne,n,ne";
        [Tooltip("The amount in degrees to add to the first position, starting on east"), Slider(0,360)]
        public readonly float AngleSuffixOffset = 0;
        public readonly bool FlipWest = true;

        public AgentSpriteComponent() 
        {
        }

        private AgentSpriteComponent(Guid guid, TargetSpriteBatches batch, int ySort, string idlePrefix, string walkPrefix,
            string suffix, float angleSuffixOffset, bool flipWest)
        {
            AnimationGuid = guid;
            TargetSpriteBatch = batch;

            YSortOffset = ySort;

            IdlePrefix = idlePrefix;
            WalkPrefix = walkPrefix;

            Suffix = suffix;
            AngleSuffixOffset = angleSuffixOffset;
            FlipWest = flipWest;
        }

        public AgentSpriteComponent WithAnimation(Guid animationGuid, bool flip) =>
            new(animationGuid, TargetSpriteBatch, YSortOffset, IdlePrefix, WalkPrefix, Suffix, AngleSuffixOffset, flip);
    }
}
