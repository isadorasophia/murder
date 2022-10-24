using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Components
{
    internal readonly struct AgentSpriteComponent : IComponent
    {
        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid AnimationGuid = Guid.Empty;

        public readonly int YSortOffset = 0;

        public readonly string IdlePrefix = "idle";
        public readonly string WalkPrefix = "walk";
        public readonly string UpSuffix = "up";

        public AgentSpriteComponent() 
        {
        }

    }
}
