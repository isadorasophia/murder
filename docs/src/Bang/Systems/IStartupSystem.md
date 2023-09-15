# IStartupSystem

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public abstract IStartupSystem : ISystem
```

A system only called once when the world starts.

**Implements:** _[ISystem](../../Bang/Systems/ISystem.html)_

### ⭐ Methods
#### Start(Context)
```csharp
public abstract void Start(Context context)
```

This is called before any [IUpdateSystem.Update(Bang.Contexts.Context)](../../Bang/Systems/IUpdateSystem.html#update(context) call.

**Parameters** \
`context` [Context](../../Bang/Contexts/Context.html) \



⚡