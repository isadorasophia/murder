# UniqueAttribute

**Namespace:** Bang.Components \
**Assembly:** Bang.dll

```csharp
public class UniqueAttribute : Attribute
```

Marks a component as unique within our world.
            We should not expect two entities with the same component if it is declared as unique.

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public UniqueAttribute()
```

### ⭐ Properties
#### TypeId
```csharp
public virtual Object TypeId { get; }
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \


⚡