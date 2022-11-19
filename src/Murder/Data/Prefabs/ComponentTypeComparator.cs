using Bang;

namespace Murder.Prefabs
{
    internal class ComponentTypeComparator : IEqualityComparer<Type>
    {
        private static readonly ComponentsLookup _lookup = World.FindLookupImplementation();

        public bool Equals(Type? x, Type? y)
        {
            if (x is null || y is null)
            {
                return x is null && y is null;
            }
            
            return _lookup.Id(x) == _lookup.Id(y);
        }

        public int GetHashCode(Type t) => _lookup.Id(t);
    }
}
