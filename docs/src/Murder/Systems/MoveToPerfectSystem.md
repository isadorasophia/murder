# MoveToPerfectSystem

**Namespace:** Murder.Systems \
**Assembly:** Murder.dll

```csharp
public class MoveToPerfectSystem : IFixedUpdateSystem, ISystem
```

Simple system for moving agents to another position. Looks for 'MoveTo' components and adds agent inpulses to it.

**Implements:** _[IFixedUpdateSystem](../../Bang/Systems/IFixedUpdateSystem.html), [ISystem](../../Bang/Systems/ISystem.html)_

### ⭐ Constructors
```csharp
public MoveToPerfectSystem()
```

### ⭐ Methods
#### FixedUpdate(Context)
```csharp
public virtual void FixedUpdate(Context context)
```

**Parameters** \
`context` [Context](../../Bang/Contexts/Context.html) \



⚡