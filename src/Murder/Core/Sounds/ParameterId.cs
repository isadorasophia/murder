using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Murder.Core.Sounds
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ParameterId : IEqualityComparer<ParameterId>
    {
        public uint Data1 { get; init; }

        public uint Data2 { get; init; }
        
        public readonly string? Name { get; init; }
        
        public bool Equals(ParameterId x, ParameterId y)
        {
            return x.Data1 == y.Data1 && x.Data2 == y.Data2;
        }

        public int GetHashCode([DisallowNull] ParameterId obj)
        {
            return Data1.GetHashCode() + Data2.GetHashCode();
        }

        public bool IsGuidEmpty => Data1 == 0 && Data2 == 0;

        public ParameterId WithPath(string path) =>
            new ParameterId { Data1 = Data1, Data2 = Data2, Name = path };
    }
}
