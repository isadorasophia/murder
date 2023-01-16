using Murder.Core.Physics;

namespace Murder.Core
{
    public struct MapTile
    {
        public int CollisionType = CollisionLayersBase.NONE;
        public int Weight = 1;

        public MapTile() { }
    }
}
