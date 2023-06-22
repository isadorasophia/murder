using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Murder.Core.Sounds
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SoundEventId : IEqualityComparer<SoundEventId>, IEquatable<SoundEventId>
    {
        public int Data1 { get; init; }

        public int Data2 { get; init; }

        public int Data3 { get; init; }

        public int Data4 { get; init; }

        public readonly string? Path { get; init; }

        public readonly string EditorName 
        {
            get
            {
                if (Path is null)
                {
                    return "(\uf188 Event path not found)";
                }

                int index = Path.IndexOf('/');
                if (index == -1)
                {
                    return Path;
                }

                return Path.AsSpan().Slice(index + 1).ToString();
            }
        }

        public bool Equals(SoundEventId x, SoundEventId y)
        {
            return x.Data1 == y.Data1 &&
                x.Data2 == y.Data2 &&
                x.Data3 == y.Data3 &&
                x.Data4 == y.Data4;
        }

        public int GetHashCode([DisallowNull] SoundEventId obj)
        {
            return Data1.GetHashCode() + Data2.GetHashCode() +
                Data3.GetHashCode() + Data4.GetHashCode();
        }

        public bool IsGuidEmpty => Data1 == 0 && Data2 == 0 && Data3 == 0 && Data4 == 0;

        public SoundEventId WithPath(string path) =>
            new SoundEventId { Data1 = Data1, Data2 = Data2, Data3 = Data3, Data4 = Data4, Path = path };

        public bool Equals(SoundEventId other) => Equals(this, other);
    }
}
