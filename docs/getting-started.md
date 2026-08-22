# Getting Started with RinneganCore

This guide shows the minimum setup to use `RinneganCore` in an ASP.NET Core API.

## 1) Install Packages

```bash
dotnet add package RinneganCore
dotnet add package RinneganCore.Tools
```

## 2) Create a Project Wrapper

Create `Services/Rinnegan.cs`:

```csharp
using System.Net;

namespace Services;

public class Rinnegan<TRequest, TResponse> : RinneganCore.Rinnegan<TRequest, TResponse>
{
    protected override Result<TResponse> UnhandledException(Exception ex)
    {
        return new Result<TResponse>(HttpStatusCode.InternalServerError, "error", "Unexpected error");
    }
}
```

## 3) Create a Use-Case Service

Create `Services/Areas/User/Todo/New.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace Services.Areas.User.Todo;

public class New : Rinnegan<New.Request, New.Response>
{
    protected override Result<Response>? Execute()
    {
        return OK(new Response
        {
            id = DateTime.UtcNow.Ticks,
            title = request!.title!,
        });
    }

    public class Request
    {
        [Required]
        public string? title { get; set; }
    }

    public class Response
    {
        public long id { get; set; }
        public string title { get; set; } = string.Empty;
    }
}
```

For I/O-bound use-cases, override `ExecuteAsync()` instead of `Execute()`. Do not override both. Either choice still works with `RunAsActionResultAsync`.

## 4) Register Services in DI

In `Program.cs`:

```csharp
using System.Reflection;
using RinneganCore;

builder.Services.AddRinnegan(Assembly.GetAssembly(typeof(Services.Rinnegan<,>)));
```

## 5) Keep Controllers Thin

Create action in `Edge/Areas/User/Controllers/TodoController.cs`:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    [HttpPost]
    public Task<IActionResult> New(
        [FromServices] Services.Areas.User.Todo.New service,
        [FromBody] Services.Areas.User.Todo.New.Request request)
    {
        return service.RunAsActionResultAsync(request);
    }
}
```

## 6) Add Caching to Read Endpoints (Optional)

```csharp
protected override Cache? EnableCaching()
{
    return new Cache(_memoryCache, "todo_list", TimeSpan.FromMinutes(15));
}
```

## 7) Add Pagination with RinneganCore.Tools (Optional)

```csharp
public class Request : IPagedRequest
{
    public int page { get; set; } = 1;
    public int size { get; set; } = 20;
}
```

And inside `Execute()`:

```csharp
var result = query.ToPaged(request!);
return OK(result);
```

## Next

- Read [Concepts](concepts.md) for pipeline details.
- Read [Tools](tools.md) for utility APIs.
- Use `samples/HelloRinnegan` for a runnable example.
