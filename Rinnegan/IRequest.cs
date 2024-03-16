using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan
{
    public interface IRequest<T>
    {
        T request { get; set; }
    }
}
