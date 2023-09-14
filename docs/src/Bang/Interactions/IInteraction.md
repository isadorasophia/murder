# IInteraction

**Namespace:** Bang.Interactions \
**Assembly:** Bang.dll

```csharp
public abstract IInteraction
```

An interaction is any logic which will be immediately sent to another entity.

### ⭐ Methods
#### Interact(World, Entity, Entity)
```csharp
public abstract void Interact(World world, Entity interactor, Entity interacted)
```

Contract immediately performed once <paramref name="interactor" /> interacts with <paramref name="interacted" />.

**Parameters** \
`world` [World](../../Bang/World.html) \
`interactor` [Entity](../../Bang/Entities/Entity.html) \
`interacted` [Entity](../../Bang/Entities/Entity.html) \



⚡