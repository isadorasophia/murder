# CharacterRuntime

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public class CharacterRuntime
```

### ⭐ Constructors
```csharp
public CharacterRuntime(Character character, int situation)
```

**Parameters** \
`character` [Character](../..//Murder/Core/Dialogs/Character.html) \
`situation` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Methods
#### CheckRequirements(World, ImmutableArray<T>, out Int32&)
```csharp
public bool CheckRequirements(World world, ImmutableArray<T> requirements, Int32& score)
```

**Parameters** \
`world` [World](../..//Bang/World.html) \
`requirements` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`score` [int&](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasNewContentOnCurrentDialogue()
```csharp
public bool HasNewContentOnCurrentDialogue()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasNext(World, Entity, bool)
```csharp
public bool HasNext(World world, Entity target, bool track)
```

Returns whether the active dialog state for this dialogue is valid or not.

**Parameters** \
`world` [World](../..//Bang/World.html) \
`target` [Entity](../..//Bang/Entities/Entity.html) \
`track` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### NextLine(World, Entity)
```csharp
public T? NextLine(World world, Entity target)
```

**Parameters** \
`world` [World](../..//Bang/World.html) \
`target` [Entity](../..//Bang/Entities/Entity.html) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### DoChoice(int, World, Entity)
```csharp
public void DoChoice(int choice, World world, Entity target)
```

**Parameters** \
`choice` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`world` [World](../..//Bang/World.html) \
`target` [Entity](../..//Bang/Entities/Entity.html) \

#### StartAtSituation(int)
```csharp
public void StartAtSituation(int situation)
```

**Parameters** \
`situation` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡