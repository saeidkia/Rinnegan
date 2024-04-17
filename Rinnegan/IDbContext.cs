using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan
{
    public interface IDbContext<TDbContext> where TDbContext : DbContext, IDisposable
    {
        TDbContext db { get; set; }
    }
}
