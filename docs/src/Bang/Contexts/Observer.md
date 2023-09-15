# Observer

**Namespace:** Bang.Contexts \
**Assembly:** Bang.dll

```csharp
public abstract class Observer
```

Base class for context. This shares implementation for any other class that decides to tweak
            the observer behavior (which hasn't happened yet).

### ⭐ Properties
#### Entities
```csharp
public abstract virtual ImmutableArray<T> Entities { get; }
```

Entities that are currently watched in the world.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### World
```csharp
public readonly World World;
```

World that it observes.

**Returns** \
[World](../../Bang/World.html) \


⚡