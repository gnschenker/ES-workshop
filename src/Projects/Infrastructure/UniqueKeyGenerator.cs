using System;
using System.Collections.Generic;
using Projects.Domain;

namespace Projects.Infrastructure
{
    public class UniqueKeyGenerator : IUniqueKeyGenerator
    {
        private readonly static object sync = new object();
        private readonly Dictionary<Type, int> cache = new Dictionary<Type, int>();

        public int GetId<T>()
        {
            lock (sync)
            {
                var key = typeof(T);
                if (cache.ContainsKey(key) == false)
                    cache.Add(key, 0);
                cache[key]++;
                return cache[key];
            }
        }
    }
}