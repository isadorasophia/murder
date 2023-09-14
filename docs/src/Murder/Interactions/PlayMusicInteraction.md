# PlayMusicInteraction

**Namespace:** Murder.Interactions \
**Assembly:** Murder.dll

```csharp
public sealed struct PlayMusicInteraction : IInteraction
```

**Implements:** _[IInteraction](../../Bang/Interactions/IInteraction.html)_

### ⭐ Constructors
```csharp
public PlayMusicInteraction()
```

### ⭐ Properties
#### Music
```csharp
public readonly SoundEventId Music;
```

**Returns** \
[SoundEventId](../../Murder/Core/Sounds/SoundEventId.html) \
#### PreviousMusic
```csharp
public readonly T? PreviousMusic;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### StopPrevious
```csharp
public readonly bool StopPrevious;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
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