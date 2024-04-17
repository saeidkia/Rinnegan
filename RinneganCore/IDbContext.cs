using Microsoft.EntityFrameworkCore;

namespace RinneganCore
{
    public interface IDbContext<TDbContext> where TDbContext : DbContext, IDisposable
    {
        TDbContext db { get; set; }
    }
}
