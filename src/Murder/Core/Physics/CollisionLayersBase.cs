namespace Murder.Core.Physics
{
    public class CollisionLayersBase
    {
        public const int NONE = 0;
        public const int SOLID = 1 << 0;
        public const int TRIGGER = 1 << 1;
        public const int HITBOX = 1 << 2;
        public const int ACTOR = 1 << 3;
        public const int HOLE = 1 << 4;
        public const int CARVE = 1 << 5;
        public const int BLOCK_VISION = 1 << 6;
        public const int RAYIGNORE = 1 << 7;
        public const int PATHFIND = 1 << 8;

        /// <summary>
        /// This class should never be instanced
        /// </summary>
        protected CollisionLayersBase() { }
    }
}