// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetExtender.Utilities.Types
{
    public static class GenericUtilities
    {
        private static MethodInfo MemberwiseCloneMethod { get; } = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private static Converter<Object, Object> MemberwiseCloneDelegate { get; } = (Converter<Object, Object>) MemberwiseCloneMethod.CreateDelegate(typeof(Converter<Object, Object>));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNullIfNotNull("value")]
        public static T? MemberwiseClone<T>(this T? value)
        {
            return value is not null ? (T) MemberwiseCloneDelegate.Invoke(value) : default;
        }
    }
}