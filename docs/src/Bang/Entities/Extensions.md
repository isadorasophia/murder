# Extensions

**Namespace:** Bang.Entities \
**Assembly:** Bang.dll

```csharp
public static class Extensions
```

Quality of life extensions for the default components declared in Bang.

### ⭐ Methods
#### HasInteractive(Entity)
```csharp
public bool HasInteractive(Entity e)
```

Checks whether this entity possesses a component of type [IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html) or not.

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasStateMachine(Entity)
```csharp
public bool HasStateMachine(Entity e)
```

Checks whether this entity possesses a component of type [IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html) or not.

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasTransform(Entity)
```csharp
public bool HasTransform(Entity e)
```

Checks whether this entity possesses a component of type [ITransformComponent](../../Bang/Components/ITransformComponent.html) or not.

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveInteractive(Entity)
```csharp
public bool RemoveInteractive(Entity e)
```

Removes the component of type [IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveStateMachine(Entity)
```csharp
public bool RemoveStateMachine(Entity e)
```

Removes the component of type [IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveTransform(Entity)
```csharp
public bool RemoveTransform(Entity e)
```

Removes the component of type [ITransformComponent](../../Bang/Components/ITransformComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### WithInteractive(Entity, IInteractiveComponent)
```csharp
public Entity WithInteractive(Entity e, IInteractiveComponent component)
```

Adds or replaces the component of type [IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`component` [IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### WithStateMachine(Entity, IStateMachineComponent)
```csharp
public Entity WithStateMachine(Entity e, IStateMachineComponent component)
```

Adds or replaces the component of type [IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`component` [IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### WithTransform(Entity, ITransformComponent)
```csharp
public Entity WithTransform(Entity e, ITransformComponent component)
```

Adds or replaces the component of type [ITransformComponent](../../Bang/Components/ITransformComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`component` [ITransformComponent](../../Bang/Components/ITransformComponent.html) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### GetInteractive(Entity)
```csharp
public IInteractiveComponent GetInteractive(Entity e)
```

Gets a component of type [IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html) \

#### TryGetInteractive(Entity)
```csharp
public IInteractiveComponent TryGetInteractive(Entity e)
```

Gets a [IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html) if the entity has one, otherwise returns null.

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html) \

#### GetStateMachine(Entity)
```csharp
public IStateMachineComponent GetStateMachine(Entity e)
```

Gets a component of type [IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html) \

#### TryGetStateMachine(Entity)
```csharp
public IStateMachineComponent TryGetStateMachine(Entity e)
```

Gets a [IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html) if the entity has one, otherwise returns null.

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html) \

#### GetTransform(Entity)
```csharp
public ITransformComponent GetTransform(Entity e)
```

Gets a component of type [ITransformComponent](../../Bang/Components/ITransformComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[ITransformComponent](../../Bang/Components/ITransformComponent.html) \

#### TryGetTransform(Entity)
```csharp
public ITransformComponent TryGetTransform(Entity e)
```

Gets a [ITransformComponent](../../Bang/Components/ITransformComponent.html) if the entity has one, otherwise returns null.

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[ITransformComponent](../../Bang/Components/ITransformComponent.html) \

#### SetInteractive(Entity, IInteractiveComponent)
```csharp
public void SetInteractive(Entity e, IInteractiveComponent component)
```

Adds or replaces the component of type [IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`component` [IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html) \

#### SetStateMachine(Entity, IStateMachineComponent)
```csharp
public void SetStateMachine(Entity e, IStateMachineComponent component)
```

Adds or replaces the component of type [IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`component` [IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html) \

#### SetTransform(Entity, ITransformComponent)
```csharp
public void SetTransform(Entity e, ITransformComponent component)
```

Adds or replaces the component of type [ITransformComponent](../../Bang/Components/ITransformComponent.html).

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`component` [ITransformComponent](../../Bang/Components/ITransformComponent.html) \



⚡