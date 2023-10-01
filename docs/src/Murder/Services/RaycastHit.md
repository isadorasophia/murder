# RaycastHit

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
sealed struct RaycastHit
```

### ⭐ Constructors
```csharp
public RaycastHit()
```

```csharp
public RaycastHit(Entity entity, Vector2 point)
```

**Parameters** \
`entity` [Entity](../../Bang/Entities/Entity.html) \
`point` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

```csharp
public RaycastHit(Point tile, Vector2 point)
```

**Parameters** \
`tile` [Point](../../Murder/Core/Geometry/Point.html) \
`point` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

### ⭐ Properties
#### Entity
```csharp
public readonly Entity Entity;
```

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \
#### Point
```csharp
public readonly Vector2 Point;
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Tile
```csharp
public readonly Point Tile;
```

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \


⚡