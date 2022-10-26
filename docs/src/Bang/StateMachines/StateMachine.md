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
            Initialized in [StateMachine.Initialize(Bang.World,Bang.Entities.Entity)](/Bang/StateMachines/StateMachine.html).

**Returns** \
[Entity](/Bang/Entities/Entity.html) \
#### Name
```csharp
public string Name { get; private set; }
```

Name of the active state. Used for debug.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### World
```csharp
protected World World;
```

World of the state machine.
            Initialized in [StateMachine.Initialize(Bang.World,Bang.Entities.Entity)](/Bang/StateMachines/StateMachine.html).

**Returns** \
[World](/Bang/World.html) \
### ⭐ Methods
#### OnStart()
```csharp
protected virtual void OnStart()
```

Initialize the state machine. Called before the first [StateMachine.Tick(System.Single)](/Bang/StateMachines/StateMachine.html) call.

#### GoTo(Func<TResult>)
```csharp
protected virtual Wait GoTo(Func<TResult> routine)
```

Redirects the state machine to a new <paramref name="routine" />.

**Parameters** \
`routine` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-1?view=net-7.0) \
\

**Returns** \
[Wait](/Bang/StateMachines/Wait.html) \

#### Reset()
```csharp
protected void Reset()
```

This resets the current state of the state machine back to the beggining of that same state.

#### State(Func<TResult>)
```csharp
protected void State(Func<TResult> routine)
```

Set the current state of the state machine with <paramref name="routine" />.

**Parameters** \
`routine` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-1?view=net-7.0) \



⚡