# ContextAccessorFilter

**Namespace:** Bang.Contexts \
**Assembly:** Bang.dll

```csharp
public sealed enum ContextAccessorFilter : Enum, IComparable, IFormattable, IConvertible
```

Context accessor filter for a system.
            This will specify the kind of filter which will be performed on a certain list of component types.

**Implements:** _[Enum](https://learn.microsoft.com/en-us/dotnet/api/System.Enum?view=net-7.0), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable?view=net-7.0), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/System.IFormattable?view=net-7.0), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/System.IConvertible?view=net-7.0)_

### ⭐ Properties
#### AllOf
```csharp
public static const ContextAccessorFilter AllOf;
```

Only entities which has all of the listed components will be fed to the system.

**Returns** \
[ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html) \
#### AnyOf
```csharp
public static const ContextAccessorFilter AnyOf;
```

Filter entities which has any of the listed components will be fed to the system.

**Returns** \
[ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html) \
#### None
```csharp
public static const ContextAccessorFilter None;
```

No filter is required. This won't be applied when filtering entities to a system.
            This is used when a system will, for example, add a new component to an entity but does
            not require such component.

**Returns** \
[ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html) \
#### NoneOf
```csharp
public static const ContextAccessorFilter NoneOf;
```

Filter out entities that have the components listed.

**Returns** \
[ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html) \


⚡