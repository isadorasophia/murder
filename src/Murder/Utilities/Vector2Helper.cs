using System.Numerics;

namespace Murder.Utilities
{
    public static class Vector2Helper
    {
        private static readonly Vector2 _center = new(0.5f, 0.5f);

        public static Vector2 Center => _center;
    }
}