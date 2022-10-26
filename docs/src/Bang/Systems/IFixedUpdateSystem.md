# IFixedUpdateSystem

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public abstract IFixedUpdateSystem : ISystem
```

System which will be called in fixed intervals.

**Implements:** _[ISystem](/Bang/Systems/ISystem.html)_

### ⭐ Methods
#### FixedUpdate(Context)
```csharp
public abstract ValueTask FixedUpdate(Context context)
```

Update calls which will be called in fixed intervals.

**Parameters** \
`context` [Context](/Bang/Contexts/Context.html) \
\

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \



⚡