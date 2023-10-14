using Bang.Components;

namespace Murder.Components
{
    public readonly struct PrefabRefComponent : IComponent
    {
        public readonly Guid AssetGuid;

        public PrefabRefComponent(Guid assetGui) =>
            AssetGuid = assetGui;
    }
}