using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static RinneganCore.Cache;

namespace RinneganCore
{
    public class Cache
    {
        internal bool found = false;

        internal IMemoryCache? MemoryCache { get; set; }
        internal IDistributedCache? DistributedCache { get; set; }
        internal TimeSpan Duration { get; set; }
        internal object Key { get; set; }
        internal bool Sliding { get; set; }
        internal CacheType? Type { get; set; } = CacheType.None;

        public Cache(IMemoryCache memoryCache, object key, TimeSpan duration, bool sliding = false)
        {
            this.MemoryCache = memoryCache;
            this.DistributedCache = null;
            this.Key = key;
            this.Duration = duration;
            Sliding = sliding;
            Type = CacheType.MemoryCache;
        }
        public Cache(IDistributedCache distributedCache, string key, TimeSpan duration, bool sliding = false)
        {
            this.MemoryCache = null;
            this.DistributedCache = distributedCache;
            this.Key = key;
            this.Duration = duration;
            Sliding = sliding;
            Type = CacheType.DistributedCache;
        }

        public Cache UpdateDuration(TimeSpan duration)
        {
            this.Duration = duration;
            return this;
        }
    }

    public enum CacheType
    {
        None,
        MemoryCache,
        DistributedCache,
    }

    internal static class CacheHandler
    {
        internal static bool TryGetValue<T>(this Cache cache, out T? result)
        {
            switch (cache.Type)
            {
                case CacheType.MemoryCache:
                    return cache.MemoryCache!.TryGetValue(cache.Key, out result);


                case CacheType.DistributedCache:
                    var data = cache.DistributedCache!.Get(cache.Key.ToString()!);
                    if (data == null)
                        break;
                    result = JsonSerializer.Deserialize<T>(data);
                    return true;
            }
            result = default(T);
            return false;
        }

        internal static async Task<T?> GetValueAsync<T>(this Cache cache)
        {
            switch (cache.Type)
            {
                case CacheType.MemoryCache:
                    return (T?)cache.MemoryCache!.Get(cache.Key);

                case CacheType.DistributedCache:
                    var data = await cache.DistributedCache!.GetAsync(cache.Key.ToString()!);
                    if (data == null)
                        break;
                    using (var stream = new MemoryStream(data))
                        return await JsonSerializer.DeserializeAsync<T>(stream);
            }
            return default(T);
        }


        internal static void Set<T>(this Cache cache, T value)
        {
            if (value == null) return;

            switch (cache.Type)
            {
                case CacheType.MemoryCache:
                    MemoryCacheEntryOptions memoryOptions = cache.Sliding
                        ? new MemoryCacheEntryOptions { SlidingExpiration = cache.Duration }
                        : new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = cache.Duration };
                    cache.MemoryCache!.Set(cache.Key, value, memoryOptions);
                    return;


                case CacheType.DistributedCache:
                    byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
                    DistributedCacheEntryOptions distributedOptions = cache.Sliding
                        ? new DistributedCacheEntryOptions { SlidingExpiration = cache.Duration }
                        : new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cache.Duration };

                    cache.DistributedCache!.Set(cache.Key.ToString()!, data, distributedOptions);
                    return;
            }
        }

        internal static async Task SetAsync<T>(this Cache cache, T value)
        {
            if (value == null) return;

            switch (cache.Type)
            {
                case CacheType.MemoryCache:
                    MemoryCacheEntryOptions memoryOptions = cache.Sliding
                        ? new MemoryCacheEntryOptions { SlidingExpiration = cache.Duration }
                        : new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = cache.Duration };
                    cache.MemoryCache!.Set(cache.Key, value, memoryOptions);
                    return;


                case CacheType.DistributedCache:
                    byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
                    DistributedCacheEntryOptions distributedOptions = cache.Sliding
                        ? new DistributedCacheEntryOptions { SlidingExpiration = cache.Duration }
                        : new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cache.Duration };

                    await cache.DistributedCache!.SetAsync(cache.Key.ToString()!, data, distributedOptions);
                    return;
            }
        }
    }
}
