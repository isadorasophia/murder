# TalkToInteraction

**Namespace:** Murder.Interactions \
**Assembly:** Murder.dll

```csharp
public sealed struct TalkToInteraction : Interaction
```

**Implements:** _[Interaction](/Bang/Interactions/Interaction.html)_

### ⭐ Constructors
```csharp
public TalkToInteraction()
```

### ⭐ Properties
#### Character
```csharp
public readonly Guid Character;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### Situation
```csharp
public readonly int Situation;
```

This is the starter situation for the interaction.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Interact(World, Entity, Entity)
```csharp
public virtual void Interact(World world, Entity interactor, Entity? interacted)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`interactor` [Entity](/Bang/Entities/Entity.html) \
`interacted` [Entity](/Bang/Entities/Entity.html) \



⚡