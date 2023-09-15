# AddEntityOnInteraction

**Namespace:** Murder.Interactions \
**Assembly:** Murder.dll

```csharp
public sealed struct AddEntityOnInteraction : IInteraction
```

This will trigger an effect by placing [AddEntityOnInteraction._prefab](../../Murder/Interactions/AddEntityOnInteraction.html#_prefab) in the world.

**Implements:** _[IInteraction](../../Bang/Interactions/IInteraction.html)_

### ⭐ Constructors
```csharp
public AddEntityOnInteraction()
```

```csharp
public AddEntityOnInteraction(Guid prefab)
```

**Parameters** \
`prefab` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

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