using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore
{
    public interface IRinnegan<TRequest, TResponse>
    {
        Rinnegan<TRequest,TResponse> RunExecute(TRequest? request);
        Task<Rinnegan<TRequest,TResponse>> RunExecuteAsync(TRequest? request);
    }
}
