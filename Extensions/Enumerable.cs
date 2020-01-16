using System;
using System.Collections.Generic;

namespace RedisWindowsClient.Extensions
{
    internal static class Enumerable
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }
    }
}