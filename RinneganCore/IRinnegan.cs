using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore
{
    public interface IRinnegan<TDbContext, THandler> : IDbContext<TDbContext>, IHandler<THandler> where TDbContext : DbContext where THandler : class
    {
    }
}
