namespace Murder.Core
{
    public struct MapTile
    {
        public GridCollisionType CollisionType = GridCollisionType.None;
        public GridVisionStatus VisionStatus = GridVisionStatus.None;

        public float ExploredAt = float.MaxValue;
        public int Weight = 1;

        public MapTile() { }
    }
}
