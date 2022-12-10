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

        [Tooltip("Starting on East, going clockwise")]
        public readonly string Suffix = "e,se,s,se,e,ne,n,ne";
        public readonly bool FlipWest = true;
        public AgentSpriteComponent() 
        {
        }

    }
}
