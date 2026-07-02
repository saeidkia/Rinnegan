# Migration: `Rinnegan` to `RinneganCore`

This document helps migrate from legacy `Rinnegan` (MVC5 / .NET Framework) to modern `RinneganCore` (ASP.NET Core / .NET 8).

## High-Level Difference

| Item | Legacy `Rinnegan` | `RinneganCore` |
|---|---|---|
| Target | .NET Framework 4.7.2 | .NET 8 |
| Web stack | ASP.NET MVC 5 | ASP.NET Core |
| Result mapping | `RinneganJsonResult` | `ObjectResult` |
| DI auto-scan | No | Yes (`AddRinnegan`) |
| Interface | None | `IRinnegan<TRequest, TResponse>` |
| Cache abstraction | Basic memory cache | Memory + distributed cache support |

## Migration Steps

1. Move project to ASP.NET Core first.
2. Replace NuGet package from `Rinnegan` to `RinneganCore`.
3. Create project wrapper inheriting `RinneganCore.Rinnegan<,>`.
4. Register services using `AddRinnegan(...)`.
5. Update controller actions to use `RunAsActionResult(Async)`.
6. Retest custom `UnhandledException` and localized messages in wrapper.

## Typical Code Migration

### Legacy

```csharp
public class Login : Rinnegan<Login.Request, Login.Response>
{
    protected override Result<Response> Execute() { ... }
}
```

### Core

```csharp
public class Login : Services.Rinnegan<Login.Request, Login.Response>
{
    protected override Result<Response>? Execute()
    {
        // same business logic, updated nullability and ASP.NET Core dependencies
    }
}
```

## Controller Migration

Use thin action style:

```csharp
[HttpPost]
public Task<IActionResult> Login(
    [FromServices] Services.Areas.Api.User.Login service,
    [FromBody] Services.Areas.Api.User.Login.Request request)
{
    return service.RunAsActionResultAsync(request);
}
```

## Notes

- Keep the old `Rinnegan` package only for legacy systems that cannot move off MVC5. Source lives in `legacy/Rinnegan`.
- New features should be built only on `RinneganCore`.
