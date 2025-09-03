using System.Collections.Generic;
using System.Linq;

namespace Scripts.Unity.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }
    }
}