using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore
{
    public interface IRinnegan<TRequest, TResponse>
    {
        Result<TResponse> RunExecute(TRequest? request);
        Task<Result<TResponse>> RunExecuteAsync(TRequest? request);
    }
}
