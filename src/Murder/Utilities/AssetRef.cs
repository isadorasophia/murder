using Murder.Assets;

namespace Murder.Utilities
{
    public readonly struct AssetRef<T> where T : GameAsset
    {
        public static AssetRef<T> Empty => new AssetRef<T>();

        public bool HasValue => Guid != Guid.Empty;
        public readonly Guid Guid = Guid.Empty;

        public AssetRef(Guid guid)
        {
            Guid = guid;
        }

        public T Asset => Game.Data.GetAsset<T>(Guid);
        public T? TryAsset => Guid == Guid.Empty ? null : Game.Data.TryGetAsset<T>(Guid);
    }
}