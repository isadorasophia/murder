# AgentMoveToSystem

**Namespace:** Murder.Systems.Agents \
**Assembly:** Murder.dll

```csharp
public class AgentMoveToSystem : IFixedUpdateSystem, ISystem
```

Simple system for moving agents to another position. Looks for 'MoveTo' components and adds agent inpulses to it.

**Implements:** _[IFixedUpdateSystem](../../../Bang/Systems/IFixedUpdateSystem.html), [ISystem](../../../Bang/Systems/ISystem.html)_

### ⭐ Constructors
```csharp
public AgentMoveToSystem()
```

### ⭐ Methods
#### FixedUpdate(Context)
```csharp
public virtual void FixedUpdate(Context context)
```

**Parameters** \
`context` [Context](../../../Bang/Contexts/Context.html) \



⚡