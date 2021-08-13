using System;
using System.Collections.Generic;

namespace LaPakguette.PakLib.Models
{
    public enum CompressionMethod
    {
        None = -1,
        Zlib,
        Oodle
    }

    internal static class CompressionMethodHelper
    {
        internal static string GetName(this CompressionMethod compressionMethod)
        {
            return Enum.GetName(typeof(CompressionMethod), compressionMethod).ToLower();
        }

        internal static string[] GetNames(this List<CompressionMethod> compressionMethods)
        {
            var result = new string[compressionMethods.Count];
            for (var i = 0; i < compressionMethods.Count; i++) result[i] = compressionMethods[i].GetName();
            return result;
        }
    }
}