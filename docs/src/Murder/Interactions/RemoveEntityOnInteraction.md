# RemoveEntityOnInteraction

**Namespace:** Murder.Interactions \
**Assembly:** Murder.dll

```csharp
public sealed struct RemoveEntityOnInteraction : IInteraction
```

**Implements:** _[IInteraction](../../Bang/Interactions/IInteraction.html)_

### ⭐ Constructors
```csharp
public RemoveEntityOnInteraction()
```

### ⭐ Properties
#### AddComponentsBeforeRemoving
```csharp
public readonly ImmutableArray<T> AddComponentsBeforeRemoving;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### DestroyWho
```csharp
public readonly DestroyWho DestroyWho;
```

**Returns** \
[DestroyWho](../../Murder/Interactions/DestroyWho.html) \
### ⭐ Methods
#### Interact(World, Entity, Entity)
```csharp
public virtual void Interact(World world, Entity interactor, Entity interacted)
```

**Parameters** \
`world` [World](../../Bang/World.html) \
`interactor` [Entity](../../Bang/Entities/Entity.html) \
`interacted` [Entity](../../Bang/Entities/Entity.html) \



⚡