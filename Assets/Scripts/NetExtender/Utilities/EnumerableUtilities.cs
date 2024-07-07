using System;
using System.Collections.Generic;
using System.Linq;

namespace NetExtender.Utilities
{
    public static class EnumerableUtilities
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            return source.Where(item => item is not null)!;
        }
        
        public static IEnumerable<T?> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            return source.Where(item => item.HasValue);
        }
    }
}