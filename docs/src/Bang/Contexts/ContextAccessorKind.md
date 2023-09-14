# ContextAccessorKind

**Namespace:** Bang.Contexts \
**Assembly:** Bang.dll

```csharp
public sealed enum ContextAccessorKind : Enum, IComparable, IFormattable, IConvertible
```

Context accessor kind for a system.
            This will specify the kind of operation that each system will perform, so the world
            can parallelize efficiently each system execution.

**Implements:** _[Enum](https://learn.microsoft.com/en-us/dotnet/api/System.Enum?view=net-7.0), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable?view=net-7.0), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/System.IFormattable?view=net-7.0), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/System.IConvertible?view=net-7.0)_

### ⭐ Properties
#### Read
```csharp
public static const ContextAccessorKind Read;
```

This will specify that the system implementation will only perform read operations.

**Returns** \
[ContextAccessorKind](../..//Bang/Contexts/ContextAccessorKind.html) \
#### Write
```csharp
public static const ContextAccessorKind Write;
```

This will specify that the system implementation will only perform write operations.

**Returns** \
[ContextAccessorKind](../..//Bang/Contexts/ContextAccessorKind.html) \


⚡