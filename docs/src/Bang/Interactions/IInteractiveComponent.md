# IInteractiveComponent

**Namespace:** Bang.Interactions \
**Assembly:** Bang.dll

```csharp
public abstract IInteractiveComponent : IComponent
```

Component that will interact with another entity.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Methods
#### Interact(World, Entity, Entity)
```csharp
public abstract void Interact(World world, Entity interactor, Entity interacted)
```

This is the logic which will be immediately called once the <paramref name="interactor" /> interacts with the
            <paramref name="interacted" />.

**Parameters** \
`world` [World](../../Bang/World.html) \
`interactor` [Entity](../../Bang/Entities/Entity.html) \
`interacted` [Entity](../../Bang/Entities/Entity.html) \



⚡