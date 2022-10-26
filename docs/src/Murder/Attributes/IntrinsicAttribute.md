# IntrinsicAttribute

**Namespace:** Murder.Attributes \
**Assembly:** Murder.dll

```csharp
public class IntrinsicAttribute : Attribute
```

This signalizes that a component is an intrinsic characteristic of the entity and 
            that it does not distinct as a separate entity.
            An entity with only intrinsic components will not be serialized.

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public IntrinsicAttribute()
```

### ⭐ Properties
#### TypeId
```csharp
public virtual Object TypeId { get; }
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \


⚡