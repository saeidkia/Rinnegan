using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan
{
    public interface IDbContext<T> where T : class
    {
        T db { get; set; }
    }
}
