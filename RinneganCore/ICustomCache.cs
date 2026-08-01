using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore;

public interface ICustomCache
{
    public T Get<T>(object key);
    public Task<T> GetAsync<T>(object key);


    public void Set<T>(object key, T value, TimeSpan duration, bool sliding);
    public Task SetAsync<T>(object key, T value, TimeSpan duration, bool sliding);
}
