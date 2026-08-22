# RinneganCore Concepts

## Core Abstraction

`Rinnegan<TRequest, TResponse>` is the base class for use-case services.

A service:
- receives `TRequest`
- returns `Result<TResponse>`
- executes in a predictable pipeline

## Pipeline

There are two runners:

- `RunExecute` / `RunAsActionResult` — sync
- `RunExecuteAsync` / `RunAsActionResultAsync` — async (preferred in ASP.NET Core)

Both follow the same stages:

1. `Validation()`
2. `Executing` / `ExecutingAsync` — preconditions
3. Cache read (`FromCache` / `FromCacheAsync`)
4. `Execute` / `ExecuteAsync` — business logic
5. Cache write on success (`SaveCache` / `SaveCacheAsync`)
6. `Executed` / `ExecutedAsync` — final projection
7. `OnSuccess` / `OnError` (or the async variants)
8. `Finally` (or `FinallyAsync`)

If the first try-block throws, `UnhandledException(ex)` becomes the result.

If every execute-stage hook returns `null`, the runner substitutes `204 NoContent`.

### Result-producing hooks

These return `Result<TResponse>?`. `null` means “not implemented / continue”. A non-null result short-circuits later execute-stage hooks.

| Stage | Sync runner (`RunExecute`) | Async runner (`RunExecuteAsync`) |
|---|---|---|
| Preconditions | `Executing()` then `ExecutingAsync()` | `ExecutingAsync()` then `Executing()` |
| Cache read | `FromCache()` | `FromCacheAsync()` |
| Business logic | `Execute()` then `ExecuteAsync()` | `ExecuteAsync()` then `Execute()` |
| Projection | `Executed()` then `ExecutedAsync()` | `ExecutedAsync()` then `Executed()` |

This two-way fallback is intentional: a service that only overrides `Execute()` still works when the controller calls `RunAsActionResultAsync`, and a service that only overrides `ExecuteAsync()` still works when the caller uses `RunExecute` (the sync runner blocks on the async hook).

Do not override both flavors of the same stage unless you need different behavior per runner. If both return a result, the runner’s preferred flavor wins (left column vs right column above).

`ExecutedAsync`’s default calls `Executed`. Overriding only `Executed` therefore runs on both runners.

### Side-effect hooks

These do not change the returned `Result`. Each runner calls only its own flavor:

- `RunExecute` → `OnSuccess` / `OnError`, then `Finally`
- `RunExecuteAsync` → `OnSuccessAsync` / `OnErrorAsync`, then `FinallyAsync`

Defaults:

- `OnSuccessAsync` → `OnSuccess`
- `OnErrorAsync` → `OnError`
- `FinallyAsync` → `Finally`

Overriding only the sync hook covers both runners. Overriding an async hook without calling `base` replaces that hook on the async runner only.

Use the async variants for I/O (email, HTTP, extra database work). Keep CPU-only work in the sync hooks.

### What to override

- Structural request rules: DataAnnotations on the request, handled by `Validation()`.
- Business prechecks: `Executing()` or `ExecutingAsync()`.
- Core logic: `Execute()` (sync) or `ExecuteAsync()` (I/O). Prefer one, not both.
- Payload reshape: `Executed()` or `ExecutedAsync()`.
- Telemetry / side effects: `OnSuccess` / `OnError` / `Finally`, or the `*Async` pair.

### Pipeline Example

```csharp
protected override Result<Response>? Executing()
{
    if (request?.userId == null)
        return BadRequest("User is required", nameof(request.userId));

    if (!_permissions.CanCreate(request.userId.Value))
        return Forbidden("Access denied", nameof(request.userId));

    return null; // continue
}
```

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
- return `RunAsActionResultAsync` (use `RunAsActionResult` only from already-sync callers)

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
var result = await _otherService.RunExecuteAsync(input);
if (!result.IsSuccess)
    return Error(result);
```

`RunExecute` remains valid for already-sync callers. This avoids duplicated business workflows.

## Caching

Override `EnableCaching()` in read-heavy deterministic use-cases.

Supports:
- `IMemoryCache`
- `IDistributedCache`

Cache reads use `FromCache()` on the sync runner and `FromCacheAsync()` on the async runner. Cache writes happen only for successful results, before `Executed`.
