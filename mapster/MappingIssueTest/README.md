# Mapster RecordUID Mapping Bug

A minimal reproduction of a Mapster bug where `RecordUID` (and similarly named
properties) are silently **not mapped** between source and destination records
in Mapster **v7.4.0 and earlier**.

## The Bug

Given two records with an identically named `RecordUID` property:

```csharp
public sealed record Company(string Name, string? ShortName, string? RecordUID);
public sealed record CompanyDto(string Name, string? ShortName, string? RecordUID);
```

Calling `company.Adapt<CompanyDto>()` leaves `RecordUID` as `null` on the
destination, even though the name matches exactly.

## Root Cause

Mapster's default mapping strategy in v7 and earlier uses **case-sensitive**
matching. However, the internal name normalization for certain patterns
(like `RecordUID`) could cause a mismatch — the source property was not
found during mapping, so the destination was silently skipped.

This was a bug in Mapster's property resolution logic, not a configuration
issue on the user's side.

## Workaround (v7 and earlier)

```shell
dotnet add package Mapster --version 7.4.0
```

Force case-insensitive mapping globally or per-type:

```csharp
TypeAdapterConfig.GlobalSettings.Default
    .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);
```

This works because case-insensitive matching widens the search and finds
the property where the strict match failed.

## The Fix

Upgrade to **Mapster 10.0.0 or later**, where the property resolution
logic was corrected. No workaround is needed.

```shell
dotnet add package Mapster --version 10.0.0
```

## Why This Matters

- The workaround (`IgnoreCase(true)`) is **not semantically correct** for
  most applications — it can cause unintended matches between properties
  that differ only by case.
- The bug was silent: no exception, no warning. The destination property
  was simply left at its default value (`null`).
- If you have this workaround in production code, you can safely remove
  it after upgrading to Mapster 10+.

## Running the Reproduction

```shell
dotnet run
```

Expected output with Mapster v7.4.0 or earlier:

```
UID is null
```

Expected output with Mapster v10.0.0 or later:

```
UID is 123
```

## Tested Versions

| Version           | Behavior                     |
|-------------------|------------------------------|
| 7.4.0 and earlier | ❌ Bug: RecordUID not mapped |
| 10.0.0 and later  | ✅ Fixed                     |

