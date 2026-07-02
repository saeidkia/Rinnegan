# RinneganCore

RinneganCore is a lightweight ASP.NET Core application-service framework focused on vertical-slice use-cases, consistent API responses, and low-ceremony controller integration.

## Why RinneganCore

- Keep controllers thin and move business logic into use-case services.
- Use one consistent response model with HTTP status + `data` + `errors`.
- Add cross-cutting behavior through project wrappers and service hooks.
- Register services automatically with `AddRinnegan`.

## Packages

| Package | Status | Purpose |
|---|---|---|
| `RinneganCore` | Active | Main framework: `Rinnegan<TRequest, TResponse>`, `Result<T>`, cache, DI scan |
| `RinneganCore.Tools` | Active | Companion utilities: pagination, Jalali date helpers, string and formatting utilities |
| `Rinnegan` | Deprecated | Legacy .NET Framework / MVC5 package in `legacy/Rinnegan` (compatibility only) |

## Install

```bash
dotnet add package RinneganCore
dotnet add package RinneganCore.Tools
```

## 30-Second Example

```csharp
// Program.cs
builder.Services.AddRinnegan(Assembly.GetAssembly(typeof(Services.Rinnegan<,>)));

// Controller action
[HttpPost]
public Task<IActionResult> New(
    [FromServices] Services.Areas.User.Todo.New service,
    [FromBody] Services.Areas.User.Todo.New.Request request)
{
    return service.RunAsActionResultAsync(request);
}
```

## Sample Project

A minimal runnable sample is provided at:

- `samples/HelloRinnegan`

It demonstrates:
- cached read endpoint
- create endpoint with validation
- paginated list endpoint with `RinneganCore.Tools`

## Documentation

- [Getting Started](docs/getting-started.md)
- [Concepts](docs/concepts.md)
- [RinneganCore.Tools](docs/tools.md)
- [Migration From Legacy Rinnegan](docs/migration-from-legacy.md)

## When To Use It

Use RinneganCore when you want:
- explicit use-case classes per feature
- controller-to-service flow without mediator boilerplate
- uniform result handling
- easy reuse across multiple internal backends

Avoid it when you need:
- a broad public ecosystem with many plugins
- full DDD/CQRS infrastructure out of the box
- a framework that enforces repository/domain boundaries

## License

MIT License. See [LICENSE](LICENSE).

Copyright (c) KiaIT.ir / Kia.Saeid

## Repository

- [https://github.com/saeidkia/Rinnegan](https://github.com/saeidkia/Rinnegan)
