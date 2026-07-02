using Microsoft.Extensions.Caching.Memory;
using RinneganCore;

namespace HelloRinnegan.Services.Areas.Greeting;

public class Index : Rinnegan<object?, string>
{
    private readonly IMemoryCache _memoryCache;

    public Index(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    protected override Cache? EnableCaching()
    {
        return new Cache(_memoryCache, "sample_greeting", TimeSpan.FromMinutes(5));
    }

    protected override Result<string>? Execute()
    {
        return OK("Hello from Rinnegan sample");
    }
}
