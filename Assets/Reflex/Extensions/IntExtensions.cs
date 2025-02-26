using System;

namespace Reflex.Extensions
{
    internal static class IntExtensions
    {
        public static int CeilPowerOf2(this int x)
        {
            if (x < 2)
            {
                return 1;
            }

            return (int)Math.Pow(2, (int)Math.Log(x - 1, 2) + 1);
        }
    }
}