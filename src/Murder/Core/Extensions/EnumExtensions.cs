using System.Runtime.CompilerServices;

namespace Murder.Core.Extensions;

internal static class EnumExtensions
{
    /// <summary>
    /// Implementation of .HasFlag which does not box the Enum and thus does not allocate. Useful in parts
    /// where a `HasFlag` check needs to happen every frame.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NonAllocatingHasFlag<TEnum>(this TEnum lhs, TEnum rhs) where TEnum : unmanaged, Enum
    {
        unsafe
        {
            return sizeof(TEnum) switch
            {
                1 => (*(byte*)(&lhs) & *(byte*)(&rhs)) > 0,
                2 => (*(ushort*)(&lhs) & *(ushort*)(&rhs)) > 0,
                4 => (*(uint*)(&lhs) & *(uint*)(&rhs)) > 0,
                8 => (*(ulong*)(&lhs) & *(ulong*)(&rhs)) > 0,
                _ => throw new Exception("Size does not match a known Enum backing type.")
            };
        }
    }
}