# AfterInteractRule

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed enum AfterInteractRule : Enum, IComparable, IFormattable, IConvertible
```

**Implements:** _[Enum](https://learn.microsoft.com/en-us/dotnet/api/System.Enum?view=net-7.0), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable?view=net-7.0), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/System.IFormattable?view=net-7.0), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/System.IConvertible?view=net-7.0)_

### ⭐ Properties
#### Always
```csharp
public static const AfterInteractRule Always;
```

Always interact whenever the rule gets triggered (added or modified).

**Returns** \
[AfterInteractRule](../..//Murder/Components/AfterInteractRule.html) \
#### InteractOnlyOnce
```csharp
public static const AfterInteractRule InteractOnlyOnce;
```

**Returns** \
[AfterInteractRule](../..//Murder/Components/AfterInteractRule.html) \
#### InteractOnReload
```csharp
public static const AfterInteractRule InteractOnReload;
```

Instead of removing this component once triggered, this will only disable it.

**Returns** \
[AfterInteractRule](../..//Murder/Components/AfterInteractRule.html) \
#### RemoveEntity
```csharp
public static const AfterInteractRule RemoveEntity;
```

Instead of removing this component once triggered, this will remove the entity.

**Returns** \
[AfterInteractRule](../..//Murder/Components/AfterInteractRule.html) \


⚡