#region

using System.Collections.Generic;
using System.Linq;
using static System.Math;

#endregion

namespace BIS.Core
{
    public static class Methods
    {
        public static void Swap<T>(ref T v1, ref T v2)
        {
            T tmp = v1;
            v1 = v2;
            v2 = tmp;
        }

        public static bool EqualsFloat(float f1, float f2, float tolerance = 0.0001f)
        {
            float dif = Abs(f1 - f2);
            return dif <= tolerance;
        }

        public static IEnumerable<T> Yield<T>(this T src)
        {
            yield return src;
        }

        public static IEnumerable<T> Yield<T>(params T[] elems)
        {
            return elems;
        }

        public static string CharsToString(this IEnumerable<char> chars)
        {
            return new string(chars.ToArray());
        }
    }
}