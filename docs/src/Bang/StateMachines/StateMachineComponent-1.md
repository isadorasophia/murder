# StateMachineComponent\<T\>

**Namespace:** Bang.StateMachines \
**Assembly:** Bang.dll

```csharp
public sealed struct StateMachineComponent<T> : IStateMachineComponent, IComponent, IModifiableComponent
```

Implements a state machine component.

**Implements:** _[IStateMachineComponent](../../Bang/StateMachines/IStateMachineComponent.html), [IComponent](../../Bang/Components/IComponent.html), [IModifiableComponent](../../Bang/Components/IModifiableComponent.html)_

### ⭐ Constructors
```csharp
public StateMachineComponent<T>()
```

Creates a new [StateMachineComponent<T>](../../Bang/StateMachines/StateMachineComponent-1.html).

```csharp
public StateMachineComponent<T>(T routine)
```

**Parameters** \
`routine` [T](../../) \

### ⭐ Properties
#### State
```csharp
public virtual string State { get; }
```

This will fire a notification whenever the state changes.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### Tick(float)
```csharp
public virtual bool Tick(float seconds)
```

Tick a yield operation in the state machine. The next tick will be called according to the returned [WaitKind](../../Bang/StateMachines/WaitKind.html).

**Parameters** \
`seconds` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Initialize(World, Entity)
```csharp
public virtual void Initialize(World world, Entity e)
```

Initialize the state machine with the world knowledge. Called before any tick.

**Parameters** \
`world` [World](../../Bang/World.html) \
`e` [Entity](../../Bang/Entities/Entity.html) \

#### OnDestroyed()
```csharp
public virtual void OnDestroyed()
```

Called right before the component gets destroyed.

#### Subscribe(Action)
```csharp
public virtual void Subscribe(Action notification)
```

Subscribe for notifications on this component.

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \

#### Unsubscribe(Action)
```csharp
public virtual void Unsubscribe(Action notification)
```

Stop listening to notifications on this component.

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \



⚡