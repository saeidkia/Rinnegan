# Changelog

All notable changes to this project are documented in this file.

## [1.3.13] - 2026-08-22

### Added
- Async lifecycle hooks: `ExecutingAsync`, `ExecutedAsync`, `OnSuccessAsync`, `OnErrorAsync`, `FinallyAsync`.

### Changed
- `Execute` / `ExecuteAsync` now return `Result<TResponse>?`. `null` still means continue / not implemented.
- `RunExecute` keeps two-way fallback onto async result-producing hooks via `GetAwaiter().GetResult()` so existing `ExecuteAsync`-only services still answer the sync runner.
- `RunExecuteAsync` prefers async hooks, then falls back to the sync flavor.
- Side-effect `*Async` defaults call the matching sync hook, so overriding only `OnSuccess` / `OnError` / `Finally` still runs on both runners.
- `ExecutedAsync` defaults to `Executed`.

### Docs
- Pipeline, hook fallback, and AI coding rules updated in `docs/concepts.md` and `RINNEGAN_AI_CONTEXT.md`.

## [1.3.12] - 2026-07-02

### Changed
- `RinneganCore`: removed unnecessary `Task.Run` in async execution pipeline (`RunExecuteAsync`) to avoid extra thread-pool dispatch overhead.

### Docs
- Added `README.md`.
- Added docs: `getting-started`, `concepts`, `tools`, `migration-from-legacy`.
- Added `LICENSE`.
- Added `samples/HelloRinnegan`.
- Expanded `RINNEGAN_AI_CONTEXT.md` for AI-assisted development.

## [1.3.11] - Previous

### Package
- `RinneganCore` version recorded in project metadata.

## [1.0.6] - Previous

### Package
- `RinneganCore.Tools` version recorded in project metadata.

## Legacy

- Legacy `Rinnegan` package remains deprecated and is kept for compatibility only.
