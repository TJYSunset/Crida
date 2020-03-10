using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Crida.Shared.Utilities
{
    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<(int offset, T value)>> OverflowSplit<T>(this IEnumerable<T> source,
            Func<T, int> sizeSelector, int bucketSize)
        {
            Debug.Assert(bucketSize > 0);
            var cursor = 0;
            var bucket = new List<(int, T)>();
            foreach (var item in source)
            {
                var size = sizeSelector(item);
                Debug.Assert(size >= 0);
                if (cursor + size > bucketSize && size > 0 && bucket.Count != 0)
                {
                    yield return bucket;
                    cursor = 0;
                    bucket = new List<(int, T)>();
                }

                bucket.Add((cursor, item));
                cursor += size;
            }

            if (bucket.Count > 0) yield return bucket;
        }
    }
}
