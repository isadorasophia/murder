# CollisionCacheComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct CollisionCacheComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public CollisionCacheComponent()
```

```csharp
public CollisionCacheComponent(ImmutableHashSet<T> idList)
```

**Parameters** \
`idList` [ImmutableHashSet\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableHashSet-1?view=net-7.0) \

```csharp
public CollisionCacheComponent(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### CollidingWith
```csharp
public ImmutableHashSet<T> CollidingWith { get; }
```

**Returns** \
[ImmutableHashSet\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableHashSet-1?view=net-7.0) \
### ⭐ Methods
#### Contains(World)
```csharp
public bool Contains(World world)
```

**Parameters** \
`world` [World](../../Bang/World.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasId(int)
```csharp
public bool HasId(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Add(int)
```csharp
public CollisionCacheComponent Add(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[CollisionCacheComponent](../../Murder/Components/CollisionCacheComponent.html) \

#### Remove(int)
```csharp
public CollisionCacheComponent Remove(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[CollisionCacheComponent](../../Murder/Components/CollisionCacheComponent.html) \

#### GetCollidingEntities(World)
```csharp
public IEnumerable<T> GetCollidingEntities(World world)
```

**Parameters** \
`world` [World](../../Bang/World.html) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \



⚡