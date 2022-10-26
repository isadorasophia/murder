# CarveComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct CarveComponent : IComponent
```

This is used for carve components within the map (that will not move a lot and 
            should be taken into account on pathfinding).
            This a tag used when caching information in [Map](/Murder/Core/Map.html).

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public CarveComponent()
```

### ⭐ Properties
#### BlockVision
```csharp
public readonly bool BlockVision;
```

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
            Ignored if [CarveComponent.Obstacle](/murder/components/carvecomponent.html#obstacle) is true.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \


⚡