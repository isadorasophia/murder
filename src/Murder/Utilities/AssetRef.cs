using Murder.Assets;

namespace Murder.Utilities
{
    public readonly struct AssetRef<T> where T : GameAsset
    {
        public static AssetRef<T> Empty => new AssetRef<T>();
        
        public readonly Guid Guid = Guid.Empty;

        public AssetRef(Guid guid)
        {
            Guid = guid;
        }

        public T Asset => Game.Data.GetAsset<T>(Guid);
        public T? TryAsset => Game.Data.TryGetAsset<T>(Guid);
    }
}