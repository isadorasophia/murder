namespace Murder.Core.Physics
{
    public class CollisionLayersBase
    {
        public const int NONE = 0;
        public const int SOLID = 100;
        public const int TRIGGER = 101;
        public const int HITBOX = 102;
        public const int ACTOR = 103;
        
        /// <summary>
        /// This class should never be instanced
        /// </summary>
        protected CollisionLayersBase() { }
    }
}
