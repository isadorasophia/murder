using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Core
{
    public struct TileDimensions
    {
        public Vector2 Origin;
        public Point Size;

        public TileDimensions(Vector2 origin, Point size) => (Origin, Size) = (origin, size);

        public static implicit operator Rectangle(TileDimensions t) => new(t.Origin, t.Size);
    }
}
