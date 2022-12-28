using Bang.Components;
using Murder.Assets;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This will trigger an effect by placing <see cref="OnMatchPrefab"/> or <see cref="OnNotMatchPrefab"/> in the world.
    /// </summary>
    [Requires(typeof(InteractOnRuleMatchComponent))]
    public readonly struct PickEntityToAddOnStartComponent : IComponent
    {
        [GameAssetId(typeof(PrefabAsset))]
        [ShowInEditor]
        [Tooltip("Entity which will be instanciated if the rule matches.")]
        public readonly Guid OnMatchPrefab = Guid.Empty;

        [GameAssetId(typeof(PrefabAsset))]
        [ShowInEditor]
        [Tooltip("Entity which will be instanciated if the rule does not match.")]
        public readonly Guid OnNotMatchPrefab = Guid.Empty;

        public PickEntityToAddOnStartComponent() { }
    }
}
