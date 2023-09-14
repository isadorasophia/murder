# Context

**Namespace:** Bang.Contexts \
**Assembly:** Bang.dll

```csharp
public class Context : Observer, IDisposable
```

Context is the pool of entities accessed by each system that defined it.

**Implements:** _[Observer](../..//Bang/Contexts/Observer.html), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Properties
#### Entities
```csharp
public virtual ImmutableArray<T> Entities { get; }
```

Entities that are currently active in the context.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Entity
```csharp
public Entity Entity { get; }
```

Get the single entity present in the context.
            This assumes that the context targets a unique component.
            TODO: Add flag that checks for unique components within this context.

**Returns** \
[Entity](../..//Bang/Entities/Entity.html) \
#### HasAnyEntity
```csharp
public bool HasAnyEntity { get; }
```

Whether the context has any entity active.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### World
```csharp
public readonly World World;
```

**Returns** \
[World](../..//Bang/World.html) \
### ⭐ Methods
#### Dispose()
```csharp
public virtual void Dispose()
```



⚡