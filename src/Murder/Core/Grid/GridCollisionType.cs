namespace Murder.Core
{
    [Flags]
    public enum GridCollisionType : uint
    {
        None = 0x0,

        /// <summary>
        /// These are static collision grids.
        /// This will be any grid occupied by a tile (e.g. wall), for example.
        /// </summary>
        Static = 0x1,
        
        /// <summary>
        /// These are the "carve" types.
        /// This will be any grid occupied by an entity which has a collider of a "carve" type.
        /// </summary>
        Carve = 0x2,

        /// <summary>
        /// These should block any line of sight from going trough them
        /// </summary>
        BlockVision = 0x4,

        /// <summary>
        /// Whether this is an obstacle for pathfinding.
        /// </summary>
        IsObstacle = 0x8,

        /// <summary>
        /// Whether this is an obstacle for pathfinding and walking, but not for flying things.
        /// </summary>
        Hole = 0x16
    }
}
