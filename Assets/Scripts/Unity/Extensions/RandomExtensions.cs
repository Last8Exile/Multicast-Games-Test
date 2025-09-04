using System;
using System.Collections.Generic;

namespace Scripts.Unity.Extensions
{
    public static class RandomExtensions
    {
        public static readonly Random Shared = new();

        public static void Shuffle<T>(this IList<T> list, Random random = null)
        {
            random ??= Shared;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
