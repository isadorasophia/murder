# StopMusicInteraction

**Namespace:** Murder.Interactions \
**Assembly:** Murder.dll

```csharp
public sealed struct StopMusicInteraction : IInteraction
```

**Implements:** _[IInteraction](../..//Bang/Interactions/IInteraction.html)_

### ⭐ Constructors
```csharp
public StopMusicInteraction()
```

### ⭐ Properties
#### FadeOut
```csharp
public readonly bool FadeOut;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Music
```csharp
public readonly T? Music;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
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