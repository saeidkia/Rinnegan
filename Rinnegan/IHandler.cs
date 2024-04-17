using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan
{
    public interface IHandler<THandler> where THandler : class
    {
        THandler handler { get; set; }
    }
}
