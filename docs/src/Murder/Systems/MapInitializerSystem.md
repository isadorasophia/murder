# MapInitializerSystem

**Namespace:** Murder.Systems \
**Assembly:** Murder.dll

```csharp
public class MapInitializerSystem : IStartupSystem, ISystem
```

**Implements:** _[IStartupSystem](../../Bang/Systems/IStartupSystem.html), [ISystem](../../Bang/Systems/ISystem.html)_

### ⭐ Constructors
```csharp
public MapInitializerSystem()
```

### ⭐ Methods
#### InitializeTile(Map, int, int, ITileProperties)
```csharp
protected virtual void InitializeTile(Map map, int x, int y, ITileProperties iProperties)
```

**Parameters** \
`map` [Map](../../Murder/Core/Map.html) \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`iProperties` [ITileProperties](../../Murder/Core/ITileProperties.html) \

#### Start(Context)
```csharp
public virtual void Start(Context context)
```

**Parameters** \
`context` [Context](../../Bang/Contexts/Context.html) \



⚡