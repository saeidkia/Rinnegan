using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RinneganCore
{
    public class Cache
    {
        internal bool found = false;

        internal IMemoryCache? MemoryCache { get; set; }
        internal IDistributedCache? DistributedCache { get; set; }
        internal TimeSpan TimeSpan { get; set; }
        internal object Key { get; set; }
        internal bool Sliding { get; set; }
        private CacheType? Type { get; set; } = CacheType.None;

        public Cache(IMemoryCache memoryCache, object key, TimeSpan timeSpan, bool sliding = false)
        {
            this.MemoryCache = memoryCache;
            this.DistributedCache = null;
            this.Key = key;
            this.TimeSpan = timeSpan;
            Sliding = sliding;
            Type = CacheType.MemoryCache;
        }
        public Cache(IDistributedCache distributedCache, string key, TimeSpan timeSpan, bool sliding = false)
        {
            this.MemoryCache = null;
            this.DistributedCache = distributedCache;
            this.Key = key;
            this.TimeSpan = timeSpan;
            Sliding = sliding;
            Type = CacheType.DistributedCache;
        }


        public enum CacheType
        {
            None,
            MemoryCache,
            DistributedCache,
        }


        internal bool TryGetValue<T>(out T? result)
        {
            switch (this.Type)
            {
                case CacheType.MemoryCache:
                    return this.MemoryCache!.TryGetValue(this.Key, out result);


                case CacheType.DistributedCache:
                    var data = this.DistributedCache!.Get(this.Key.ToString()!);
                    if (data == null)
                        break;
                    result = JsonSerializer.Deserialize<T>(data);
                    return true;
            }
            result = default(T);
            return false;
        }

        internal async Task<T?> GetValueAsync<T>()
        {
            switch (this.Type)
            {
                case CacheType.MemoryCache:
                    return (T?)this.MemoryCache!.Get(this.Key);

                case CacheType.DistributedCache:
                    var data = await this.DistributedCache!.GetAsync(this.Key.ToString()!);
                    if (data == null)
                        break;
                    using (var stream = new MemoryStream(data))
                        return await JsonSerializer.DeserializeAsync<T>(stream);
            }
            return default(T);
        }


        internal void Set<T>(T value, TimeSpan expire)
        {
            if (value == null) return;

            switch (this.Type)
            {
                case CacheType.MemoryCache:
                    MemoryCacheEntryOptions memoryOptions = this.Sliding
                        ? new MemoryCacheEntryOptions { SlidingExpiration = expire }
                        : new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expire };
                    this.MemoryCache!.Set(this.Key, value, memoryOptions);
                    return;


                case CacheType.DistributedCache:
                    byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
                    DistributedCacheEntryOptions distributedOptions = this.Sliding
                        ? new DistributedCacheEntryOptions { SlidingExpiration = expire }
                        : new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expire };

                    this.DistributedCache!.Set(this.Key.ToString()!, data, distributedOptions);
                    return;
            }
        }

        internal async Task SetAsync<T>(T value, TimeSpan expire)
        {
            if (value == null) return;

            switch (this.Type)
            {
                case CacheType.MemoryCache:
                    MemoryCacheEntryOptions memoryOptions = this.Sliding
                        ? new MemoryCacheEntryOptions { SlidingExpiration = expire }
                        : new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expire };
                    this.MemoryCache!.Set(this.Key, value, memoryOptions);
                    return;


                case CacheType.DistributedCache:
                    byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
                    DistributedCacheEntryOptions distributedOptions = this.Sliding
                        ? new DistributedCacheEntryOptions { SlidingExpiration = expire }
                        : new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expire };

                    await this.DistributedCache!.SetAsync(this.Key.ToString()!, data, distributedOptions);
                    return;
            }
        }
    }
}
