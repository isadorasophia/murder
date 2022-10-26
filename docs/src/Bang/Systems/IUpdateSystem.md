# IUpdateSystem

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public abstract IUpdateSystem : ISystem
```

This is the update system and consists of a single update call.

**Implements:** _[ISystem](/Bang/Systems/ISystem.html)_

### ⭐ Methods
#### Update(Context)
```csharp
public abstract ValueTask Update(Context context)
```

Update method. Called once each frame.

**Parameters** \
`context` [Context](/Bang/Contexts/Context.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \



⚡