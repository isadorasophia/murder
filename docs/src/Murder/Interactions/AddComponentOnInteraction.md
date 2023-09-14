# AddComponentOnInteraction

**Namespace:** Murder.Interactions \
**Assembly:** Murder.dll

```csharp
public sealed struct AddComponentOnInteraction : IInteraction
```

This will trigger an effect by placing [AddComponentOnInteraction.Component](../../Murder/Interactions/AddComponentOnInteraction.html#Component) in the world.

**Implements:** _[IInteraction](../..//Bang/Interactions/IInteraction.html)_

### ⭐ Properties
#### Component
```csharp
public readonly IComponent Component;
```

**Returns** \
[IComponent](../..//Bang/Components/IComponent.html) \
#### IsTargetSelf
```csharp
public readonly bool IsTargetSelf;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### Interact(World, Entity, Entity)
```csharp
public virtual void Interact(World world, Entity interactor, Entity interacted)
```

**Parameters** \
`world` [World](../..//Bang/World.html) \
`interactor` [Entity](../..//Bang/Entities/Entity.html) \
`interacted` [Entity](../..//Bang/Entities/Entity.html) \



⚡