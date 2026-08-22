# Rinnegan AI Context (ASP.NET Core)

Use this file in projects that consume `RinneganCore` so AI tools and developers follow the same architecture and conventions.

## What Rinnegan Is

`RinneganCore` is a lightweight application-service framework for ASP.NET Core.

It is:
- a use-case layer (`Rinnegan<TRequest, TResponse>`)
- a template-method execution pipeline
- a consistent `Result<T>` response model
- a DI auto-registration helper (`AddRinnegan`)

It is not:
- a full CQRS framework
- a MediatR replacement with handlers and pipeline behaviors
- a complete DDD framework

## Package Map

- `RinneganCore` (main package): service base class, result contract, cache wrapper, DI registration.
- `RinneganCore.Tools` (companion package): pagination, date/string/crypto helpers.
- `Rinnegan` (legacy .NET Framework package): deprecated and not recommended for new work.

## Expected Solution Structure

Common structure in consumer projects:

```text
Edge/                # HTTP layer (controllers only)
Services/            # use-cases and wrappers
Services/Rinnegan.cs # project-level wrapper over RinneganCore.Rinnegan<,>
Models/              # entities and DTOs
```

Typical flow:
1. Controller receives request.
2. Controller resolves a Rinnegan service via `[FromServices]`.
3. Controller calls `RunAsActionResult` or `RunAsActionResultAsync`.
4. Service pipeline runs and returns `Result<T>`.
5. Framework maps status code and body shape.

## Mandatory Coding Pattern

### 1) Service Inheritance

All business use-cases should inherit your project wrapper (preferred) or `RinneganCore.Rinnegan<TRequest, TResponse>`.

```csharp
public class New : Rinnegan<New.Request, New.Response>
{
    protected override Result<Response>? Execute()
    {
        // business logic
        return OK(new Response());
    }

    public class Request
    {
        [Required]
        public string? title { get; set; }
    }

    public class Response
    {
        public long id { get; set; }
    }
}
```

### 2) Service Registration

Register all services once:

```csharp
builder.Services.AddRinnegan(Assembly.GetAssembly(typeof(Services.Rinnegan<,>)));
```

`AddRinnegan(...)` scans classes inheriting `Rinnegan<,>` and registers them (default: transient).

Optional lifetime override:

```csharp
[Scope(ServiceLifetime.Scoped)]
public class HeavyService : Rinnegan<Request, Response>
{
}
```

### 3) Thin Controller Pattern

Controllers should orchestrate HTTP only and delegate logic to services.

```csharp
[HttpPost]
public Task<IActionResult> New(
    [FromServices] Services.Areas.User.Todo.New service,
    [FromBody] Services.Areas.User.Todo.New.Request request)
{
    return service.RunAsActionResultAsync(request);
}
```

Requestless action:

```csharp
[HttpGet]
public Task<IActionResult> Status(
    [FromServices] Services.Areas.Root.Status.Index service)
{
    return service.RunAsActionResultAsync(null);
}
```

## Execution Pipeline Reference

Prefer `RunAsActionResultAsync` / `RunExecuteAsync` in ASP.NET Core. The sync runner exists for compatibility.

Shared stages:

1. `Validation()`
2. `Executing` / `ExecutingAsync`
3. `FromCache` / `FromCacheAsync`
4. `Execute` / `ExecuteAsync`
5. `SaveCache` / `SaveCacheAsync` on successful result
6. `Executed` / `ExecutedAsync`
7. `OnSuccess` / `OnError` (or `*Async`)
8. `Finally` / `FinallyAsync`
9. `UnhandledException(ex)` if the first try-block throws

If every execute-stage hook returns `null`, the runner returns `204 NoContent`.

### Result-producing hooks

Return `null` to continue. Return `Result<TResponse>` to short-circuit.

| Stage | `RunExecute` | `RunExecuteAsync` |
|---|---|---|
| Preconditions | `Executing()` then `ExecutingAsync()` | `ExecutingAsync()` then `Executing()` |
| Cache | `FromCache()` | `FromCacheAsync()` |
| Business logic | `Execute()` then `ExecuteAsync()` | `ExecuteAsync()` then `Execute()` |
| Projection | `Executed()` then `ExecutedAsync()` | `ExecutedAsync()` then `Executed()` |

Implement **one** of `Execute` / `ExecuteAsync`, not both, unless the two runners must differ. Either choice still answers both runners (the sync runner blocks on an async-only `ExecuteAsync`).

`ExecutedAsync` defaults to `Executed`. Override only `Executed` when the projection is sync.

### Side-effect hooks

Each runner calls only its own flavor. Async defaults call the sync hook:

- `OnSuccessAsync` → `OnSuccess`
- `OnErrorAsync` → `OnError`
- `FinallyAsync` → `Finally`

Override the sync hook for work that should run on both paths. Override `*Async` (without `base`) only when the async runner should replace that behavior. Put I/O side effects in the async hooks.

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

## Result Contract

`Result<T>` includes:
- HTTP status (stored internally, applied to response status code)
- `data` for successful responses
- `errors` dictionary for validation/business errors

Common helper methods:
- `OK(data)`
- `BadRequest(value, key)`
- `Forbidden(value, key)`
- `NotFound(value, key)`
- `UnprocessableContent(value, key)`
- `BadGateway(value, key)`
- `Error(result)` for forwarding upstream error

### Typical JSON Shapes

Success:

```json
{
  "data": {
    "id": 123,
    "title": "Sample"
  },
  "errors": {}
}
```

Validation error:

```json
{
  "data": null,
  "errors": {
    "title": "The title field is required."
  }
}
```

Not found:

```json
{
  "data": null,
  "errors": {
    "id": "Todo not found"
  }
}
```

## Project Wrapper Pattern

Most projects define a project wrapper:

```csharp
public class Rinnegan<TRequest, TResponse> : RinneganCore.Rinnegan<TRequest, TResponse>
{
    protected override Result<TResponse> UnhandledException(Exception ex)
    {
        // custom logging and localizable response
        return new Result<TResponse>(HttpStatusCode.InternalServerError, "error", "Unexpected error");
    }
}
```

Benefits:
- centralized error mapping
- localized messages
- request/response logging
- reusable helper methods

## Area Wrapper Pattern

Use area wrappers for shared area-specific concerns:
- API key validation
- IP whitelist checks
- audit logging
- admin-only preconditions

Example layout:

```text
Services/
  Rinnegan.cs
  Areas/
    Api/Rinnegan.cs
    Admin/Rinnegan.cs
    User/Rinnegan.cs
```

Rule:
- use the nearest area wrapper as base class so shared checks run automatically.

## Service Composition Pattern

A Rinnegan service can call another service.

```csharp
var payment = await _newPayment.RunExecuteAsync(new NewPayment.Request
{
    amount = request!.amount
});

if (!payment.IsSuccess)
    return Error(payment);
```

Guidelines:
- Prefer `RunExecuteAsync` when the caller is already async; `RunExecute` remains valid for sync callers.
- propagate domain/business errors using `Error(result)`.
- avoid duplicating workflows in multiple services.
- keep nested calls understandable (do not create deep hidden chains).

## Caching Pattern

If a service should be cached, override `EnableCaching()`:

```csharp
protected override Cache? EnableCaching()
{
    return new Cache(_memoryCache, "todo_list", TimeSpan.FromMinutes(15));
}
```

Options:
- `IMemoryCache` for local in-process cache
- `IDistributedCache` for shared cache (Redis, SQL, etc.)
- sliding or absolute expiration

Cache behavior:
- cache read happens before execute
- cache write happens only when result is success

## Background Service Pattern

Some projects include a generic background runner around Rinnegan services.

Use when:
- long-running side tasks
- delayed retries
- async non-blocking tasks after request completion

Rule:
- background tasks should call use-cases, not duplicate business logic.

## Pagination Pattern (`RinneganCore.Tools`)

For list endpoints, use:
- `IPagedRequest`
- `PagedResult<T>`
- `.ToPaged(request)`

Example:

```csharp
public class Request : IPagedRequest
{
    public int page { get; set; } = 1;
    public int size { get; set; } = 20;
}
```

## Naming Conventions

Follow current convention in your repositories:
- use-case class names: `New`, `Edit`, `List`, `Preview`, `Delete`, `Verify`, `Login`, `Index`
- place use-cases by area and feature
- define `Request` / `Response` nested models when helpful

## Validation Conventions

- Use DataAnnotations on request models.
- Keep structural validation in DTO attributes.
- Keep business validation in `Executing()` or `ExecutingAsync()`.
- Use consistent error keys (`nameof(request.property)` where possible).

## Controller Rules

Controllers should:
- validate HTTP concerns (route, binding, auth attributes)
- delegate all business logic to service
- return service result directly using `RunAsActionResult(Async)`

Controllers should not:
- query `DbContext` directly
- perform business calculations
- duplicate service-level permission logic

## Anti-Patterns to Avoid

1. Business logic inside controllers.
2. Bypassing project wrappers and inheriting `RinneganCore.Rinnegan<,>` directly without reason.
3. Duplicating API auth checks in every service instead of area wrapper.
4. Returning custom ad-hoc anonymous objects outside `Result<T>`.
5. Mixing many unrelated workflows into one use-case.
6. Ignoring helper methods and manually building status/error objects repeatedly.
7. Overusing static shared mutable state in services.
8. Overriding both `Execute` and `ExecuteAsync` (or both `Executing` flavors) without intending different runner behavior.
9. Calling `.Result` / `.Wait()` on Rinnegan services from ASP.NET Core actions; use `RunAsActionResultAsync` instead.

## Per-Project Customization Template

Fill this section in each consumer repository.

```md
### Project Overrides
- Wrapper path: `Services/Rinnegan.cs`
- API area wrapper path: `Services/Areas/Api/Rinnegan.cs`
- Admin area wrapper path: `Services/Areas/Admin/Rinnegan.cs`
- Handler/context helper: `Services/Handler.cs`
- Error storage location: `Storage/Errors/`
- Extra conventions:
  - ...
```

## AI Coding Checklist (Mandatory)

When AI writes code in this repo, it must:

1. Add new business features as Rinnegan services, not controller logic.
2. Use the project wrapper inheritance chain first.
3. Keep controller actions thin and use `[FromServices]`.
4. Use `RunAsActionResultAsync` in actions (sync `RunAsActionResult` only when the caller is already sync).
5. Use `Result<T>` helper methods (`OK`, `BadRequest`, etc.) consistently.
6. Put business prechecks in `Executing()` or `ExecutingAsync()`.
7. Put core logic in **either** `Execute()` **or** `ExecuteAsync()`, not both. Use `ExecuteAsync()` when the use-case awaits I/O.
8. Put post-result side effects in `OnSuccess` / `OnError` / `Finally`, or the `*Async` variants for I/O.
9. Use `EnableCaching()` only for deterministic read scenarios.
10. Reuse existing services via composition when workflow already exists.
11. Follow naming and folder conventions used in existing features.

## Suggested Placement

You can keep this file in one of these locations:
- `RINNEGAN_AI_CONTEXT.md`
- `docs/RINNEGAN_AI_CONTEXT.md`
- `AGENTS.md` (as section)
- `.cursor/rules/rinnegan.mdc`

## Quick Start Snippet

```csharp
// Program.cs
builder.Services.AddRinnegan(Assembly.GetAssembly(typeof(Services.Rinnegan<,>)));

// Controller
[HttpPost]
public Task<IActionResult> New(
    [FromServices] Services.Areas.User.Item.New service,
    [FromBody] Services.Areas.User.Item.New.Request request)
{
    return service.RunAsActionResultAsync(request);
}
```

