﻿using System;
using System.Reflection;

namespace LaPakguette.PakLib
{
    internal static class ArrayExtensions
    {
        internal static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if( index > 0 )
                Array.Copy(source, 0, dest, 0, index);

            if( index < source.Length - 1 )
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        internal static T[] Add<T>(this T[] source, T item)
        {
            T[] dest = new T[source.Length + 1];
            Array.Copy(source, 0, dest, 0, source.Length);
            dest[source.Length] = item;

            return dest;
        }
    }
}
