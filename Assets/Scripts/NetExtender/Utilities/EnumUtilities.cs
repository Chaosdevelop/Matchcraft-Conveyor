using System;
using System.Runtime.CompilerServices;

namespace NetExtender.Utilities
{
    public static class EnumUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean In<T>(this T value) where T : unmanaged, Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }
}