using System.Net;
using RinneganCore;

namespace HelloRinnegan.Services;

public class Rinnegan<TRequest, TResponse> : RinneganCore.Rinnegan<TRequest, TResponse>
{
    protected override Result<TResponse> UnhandledException(Exception ex)
    {
        return new Result<TResponse>(HttpStatusCode.InternalServerError, "error", ex.Message);
    }

    protected override Result<TResponse> NotFound(string value, string key = "")
    {
        return new Result<TResponse>(HttpStatusCode.NotFound, key, $"{value} not found");
    }
}
