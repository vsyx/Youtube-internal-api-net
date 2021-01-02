using System;
using System.Collections.Concurrent;

namespace YoutubeApi.Util
{
    public static partial class MemoizeUtils 
    {
        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> f) where T : notnull
        {
            var cache = new ConcurrentDictionary<T, TResult>();
            return a => cache.GetOrAdd(a, f);
        }
    }
}
