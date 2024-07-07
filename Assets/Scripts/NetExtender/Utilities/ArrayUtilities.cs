using System;

namespace NetExtender.Utilities
{
    public static class ArrayUtilities
    {
        public static T[] InnerChange<T>(this T[] array, Func<T, T> selector)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            
            for (Int32 i = 0; i < array.Length; i++)
            {
                T item = array[i];
                array[i] = selector(item);
            }
            
            return array;
        }
    }
}