using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan
{
    public class Cache
    {
        internal bool found = false;

        internal TimeSpan TimeSpan { get; set; }
        internal string Key { get; set; }

        public Cache(string key, TimeSpan timeSpan)
        {
            this.Key = key;
            this.TimeSpan = timeSpan;
        }
    }
}
