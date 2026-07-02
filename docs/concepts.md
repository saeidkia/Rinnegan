# RinneganCore Concepts

## Core Abstraction

`Rinnegan<TRequest, TResponse>` is the base class for use-case services.

A service:
- receives `TRequest`
- returns `Result<TResponse>`
- executes in a predictable pipeline

## Pipeline

Execution order:
1. `Validation()`
2. `Executing()`
3. `FromCache()` / `FromCacheAsync()`
4. `Execute()` / `ExecuteAsync()`
5. `SaveCache()` / `SaveCacheAsync()`
6. `Executed(result)`
7. `OnSuccess(result)` or `OnError(result)`
8. `Finally(result)`

If an unhandled exception escapes, `UnhandledException(ex)` is called.

## Hook Intent

- `Validation`: request structural validation (DataAnnotations).
- `Executing`: preconditions and short-circuit checks.
- `Execute`: business logic.
- `Executed`: final projection or normalization.
- `OnSuccess` / `OnError`: side effects and telemetry.
- `Finally`: always-run cleanup.

Rule of thumb:
- return `null` to continue
- return `Result<TResponse>` to stop pipeline and return that result

## Result Model

`Result<T>` has:
- `data`: success payload
- `errors`: error dictionary
- internal status code used for HTTP response status

Use built-in helpers:
- `OK`
- `BadRequest`
- `Forbidden`
- `NotFound`
- `UnprocessableContent`
- `BadGateway`
- `Error`

## Controllers

Controllers are transport adapters:
- bind request DTOs
- call service
- return `RunAsActionResult(Async)`

Do not place business logic in controllers.

## DI and Registration

Use:

```csharp
builder.Services.AddRinnegan(Assembly.GetAssembly(typeof(Services.Rinnegan<,>)));
```

Registration defaults to transient. Override with:

```csharp
[Scope(ServiceLifetime.Scoped)]
```

## Wrappers and Areas

Most projects define:
- project wrapper (`Services.Rinnegan<,>`)
- optional area wrappers (`Services.Areas.Api.Rinnegan<,>`, etc.)

This keeps cross-cutting logic centralized:
- API key checks
- IP checks
- error localization
- logging and audit behavior

## Composition

Services can call services:

```csharp
var result = _otherService.RunExecute(input);
if (!result.IsSuccess)
    return Error(result);
```

This avoids duplicated business workflows.

## Caching

Override `EnableCaching()` in read-heavy deterministic use-cases.

Supports:
- `IMemoryCache`
- `IDistributedCache`

Cache writes happen only for successful results.
