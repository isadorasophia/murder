# CarveComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct CarveComponent : IComponent
```

This is used for carve components within the map (that will not move a lot and 
            should be taken into account on pathfinding).
            This a tag used when caching information in [Map](../../Murder/Core/Map.html).

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public CarveComponent()
```

```csharp
public CarveComponent(bool blockVision, bool obstacle, bool clearPath, int weight)
```

**Parameters** \
`blockVision` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`obstacle` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`clearPath` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`weight` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### BlockVision
```csharp
public readonly bool BlockVision;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CarveClearPath
```csharp
public static CarveComponent CarveClearPath { get; }
```

**Returns** \
[CarveComponent](../../Murder/Components/CarveComponent.html) \
#### ClearPath
```csharp
public readonly bool ClearPath;
```

Whether this carve component will add a path if there was previously a collision in its area.
            For example, a bridge over a river.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Obstacle
```csharp
public readonly bool Obstacle;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Weight
```csharp
public readonly int Weight;
```

Weight of the component, if not an obstacle.
            Ignored if [CarveComponent.Obstacle](../../Murder/Components/CarveComponent.html#obstacle) is true.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### WithClearPath(bool)
```csharp
public CarveComponent WithClearPath(bool clearPath)
```

**Parameters** \
`clearPath` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[CarveComponent](../../Murder/Components/CarveComponent.html) \



⚡