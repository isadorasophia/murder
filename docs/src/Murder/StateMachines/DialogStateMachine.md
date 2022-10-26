# DialogStateMachine

**Namespace:** Murder.StateMachines \
**Assembly:** Murder.dll

```csharp
public class DialogStateMachine : StateMachine
```

**Implements:** _[StateMachine](/Bang/StateMachines/StateMachine.html)_

### ⭐ Constructors
```csharp
public DialogStateMachine()
```

### ⭐ Properties
#### Entity
```csharp
protected Entity Entity;
```

**Returns** \
[Entity](/Bang/Entities/Entity.html) \
#### Name
```csharp
public string Name { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### World
```csharp
protected World World;
```

**Returns** \
[World](/Bang/World.html) \
### ⭐ Methods
#### OnStart()
```csharp
protected virtual void OnStart()
```

#### GoTo(Func<TResult>)
```csharp
protected virtual Wait GoTo(Func<TResult> routine)
```

**Parameters** \
`routine` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-1?view=net-7.0) \

**Returns** \
[Wait](/Bang/StateMachines/Wait.html) \

#### Reset()
```csharp
protected void Reset()
```

#### State(Func<TResult>)
```csharp
protected void State(Func<TResult> routine)
```

**Parameters** \
`routine` [Func\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-1?view=net-7.0) \

#### Talk()
```csharp
public IEnumerator<T> Talk()
```

**Returns** \
[IEnumerator\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerator-1?view=net-7.0) \



⚡