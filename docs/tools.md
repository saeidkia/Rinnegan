# RinneganCore.Tools

`RinneganCore.Tools` is a companion package with practical utilities commonly used in consumer projects.

## Main Features

## Pagination

Types:
- `IPagedRequest`
- `PagedResult`
- `PagedResult<T>`
- `PagingExtension.ToPaged()`

Typical usage:

```csharp
public class Request : IPagedRequest
{
    public int page { get; set; } = 1;
    public int size { get; set; } = 20;
}

protected override Result<PagedResult<Response>>? Execute()
{
    var query = _db.Items.Where(x => !x.Deleted);
    return OK(query.Select(x => new Response { id = x.Id }).ToPaged(request!));
}
```

## Formatter Extensions

Common helpers:
- `ToJson()`
- `FromJson<T>()`
- `ToEnum<T>()`
- `TryToEnum<T>()`

Use for safe conversion and serialization in app workflows.

## String Extensions

Includes:
- Persian and English digit conversion
- input cleanup and normalization
- content type detection by extension

Useful for Persian-language products and mixed locale input processing.

## Date/Time Extensions

Provides Jalali (Persian calendar) conversion and parsing helpers.

Useful in:
- UI date mapping
- report endpoints
- localized date validation

## Crypto Extensions

Contains:
- SHA256 hash helper
- AES encrypt/decrypt helper
- random string generation helper

Recommendation:
- Use these for utility-level app tasks.
- For high-security and regulated flows, pair with strong key management and platform security controls.

## Describer Attribute

`DescriberAttribute` adds name/category metadata for endpoints or services and is often used for admin logging/reporting surfaces.
