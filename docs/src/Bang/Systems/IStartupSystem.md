# IStartupSystem

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public abstract IStartupSystem : ISystem
```

A startup system is only called once the world starts.

**Implements:** _[ISystem](/Bang/Systems/ISystem.html)_

### ⭐ Methods
#### Start(Context)
```csharp
public abstract ValueTask Start(Context context)
```

This is called before any [IUpdateSystem.Update(Bang.Contexts.Context)](/bang/systems/iupdatesystem.html#update(context) call.

**Parameters** \
`context` [Context](/Bang/Contexts/Context.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \



⚡