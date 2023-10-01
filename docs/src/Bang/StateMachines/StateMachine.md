# StateMachine

**Namespace:** Bang.StateMachines \
**Assembly:** Bang.dll

```csharp
public abstract class StateMachine
```

This is a basic state machine for an entity.
            It is sort-of anti-pattern of ECS at this point. This is a trade-off
            between adding content and using ECS at the core of the game.

### ⭐ Constructors
```csharp
protected StateMachine()
```

### ⭐ Properties
#### Entity
```csharp
protected Entity Entity;
```

Entity of the state machine.
            Initialized in [StateMachine.Initialize(Bang.World,Bang.Entities.Entity)](../../Bang/StateMachines/StateMachine.html).

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \
#### Name
```csharp
public string Name { get; private set; }
```

Name of the active state. Used for debug.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### PersistStateOnSave
```csharp
protected virtual bool PersistStateOnSave { get; }
```

Whether the state machine active state should be persisted on serialization.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### World
```csharp
protected World World;
```

World of the state machine.
            Initialized in [StateMachine.Initialize(Bang.World,Bang.Entities.Entity)](../../Bang/StateMachines/StateMachine.html).

**Returns** \
[World](../../Bang/World.html) \
### ⭐ Methods
#### OnMessage(IMessage)
```csharp
protected virtual void OnMessage(IMessage message)
```

Implemented by state machine implementations that want to listen to message
            notifications from outer systems.

**Parameters** \
`message` [IMessage](../../Bang/Components/IMessage.html) \

#### OnStart()
```csharp
protected virtual void OnStart()
```

Initialize the state machine. Called before the first [StateMachine.Tick(System.Single)](../../Bang/StateMachines/StateMachine.html) call.

#### Transition(Func<TResult>)
```csharp
protected virtual void Transition(Func<TResult> routine)
```

Redirects the state machine to a new <paramref name="routine" /> without doing
            a tick.

**Parameters** \
`routine` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-1?view=net-7.0) \
\

#### GoTo(Func<TResult>)
```csharp
protected virtual Wait GoTo(Func<TResult> routine)
```

Redirects the state machine to a new <paramref name="routine" />.

**Parameters** \
`routine` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-1?view=net-7.0) \
\

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \

#### Reset()
```csharp
protected void Reset()
```

This resets the current state of the state machine back to the beginning of that same state.

#### State(Func<TResult>)
```csharp
protected void State(Func<TResult> routine)
```

Set the current state of the state machine with <paramref name="routine" />.

**Parameters** \
`routine` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-1?view=net-7.0) \

#### SwitchState(Func<TResult>)
```csharp
protected void SwitchState(Func<TResult> routine)
```

Redirects the state machine to a new <paramref name="routine" /> without doing
            a tick.

**Parameters** \
`routine` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-1?view=net-7.0) \
\

#### OnDestroyed()
```csharp
public virtual void OnDestroyed()
```

Clean up right before the state machine gets cleaned up.
            Callers must call the base implementation.



⚡