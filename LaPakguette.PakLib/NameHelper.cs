using System.Linq;

namespace LaPakguette.PakLib
{
    internal class NameHelper
    {
        internal static bool CheckUnicodeString(string value)
        {
            const int MaxAnsiCode = 255;

            return value.Any(c => c > MaxAnsiCode);
        }
    }
}