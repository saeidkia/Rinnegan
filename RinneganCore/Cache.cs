using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore
{
    public class Cache
    {
        internal bool found = false;

        internal IMemoryCache MemoryCache { get; set; }
        internal TimeSpan TimeSpan { get; set; }
        internal object Key { get; set; }

        public Cache(IMemoryCache memoryCache, object key, TimeSpan timeSpan)
        {
            this.MemoryCache = memoryCache;
            this.Key = key;
            this.TimeSpan = timeSpan;
        }
    }
}
