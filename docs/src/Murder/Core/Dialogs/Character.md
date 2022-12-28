# Character

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public class Character
```

### ⭐ Constructors
```csharp
public Character(Guid guid, ImmutableArray<T> situations, int initial)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`situations` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`initial` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Situations
```csharp
public ImmutableDictionary<TKey, TValue> Situations;
```

All situations for the character.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
### ⭐ Methods
#### NextLine(World, Entity)
```csharp
public T? NextLine(World world, Entity target)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`target` [Entity](/Bang/Entities/Entity.html) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \



⚡