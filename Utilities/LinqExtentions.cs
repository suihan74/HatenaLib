using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib.Utilities
{
    static class LinqExtentions
    {
        public static IEnumerable<T> DistinctBy<T,TKey>(this IEnumerable<T> source, Func<T,TKey> selector)
        {
            var cache = new HashSet<TKey>();
            foreach (var value in source)
            {
                var key = selector(value);
                if (!cache.Contains(key))
                {
                    cache.Add(key);
                    yield return value;
                }
            }
        }
    }
}
