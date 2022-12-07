using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct AgentSpriteComponent : IComponent
    {
        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid AnimationGuid = Guid.Empty;

        public readonly int YSortOffset = 0;

        public readonly string IdlePrefix = "idle";
        public readonly string WalkPrefix = "walk";

        public readonly string UpSuffix = "n";
        public readonly string DownSuffix = "s";
        public readonly string LeftSuffix = "W";
        public readonly string RightSuffix = "e";
        public AgentSpriteComponent() 
        {
        }

    }
}
