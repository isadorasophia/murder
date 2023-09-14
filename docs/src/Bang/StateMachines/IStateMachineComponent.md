# IStateMachineComponent

**Namespace:** Bang.StateMachines \
**Assembly:** Bang.dll

```csharp
public abstract IStateMachineComponent : IComponent
```

See [StateMachine](../../Bang/StateMachines/StateMachine.html) for more details. This is the component implementation.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Properties
#### State
```csharp
public abstract virtual string State { get; }
```

Name of the state machine. This is mostly used to debug.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### Tick(float)
```csharp
public abstract bool Tick(float dt)
```

Tick a yield operation in the state machine. The next tick will be called according to the returned [WaitKind](../../Bang/StateMachines/WaitKind.html).

**Parameters** \
`dt` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Initialize(World, Entity)
```csharp
public abstract void Initialize(World world, Entity e)
```

Initialize the state machine with the world knowledge. Called before any tick.

**Parameters** \
`world` [World](../../Bang/World.html) \
`e` [Entity](../../Bang/Entities/Entity.html) \

#### OnDestroyed()
```csharp
public abstract void OnDestroyed()
```

Called right before the component gets destroyed.



⚡