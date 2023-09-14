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
`point` [Vector2](../../Murder/Core/Geometry/Vector2.html) \

```csharp
public RaycastHit(Point tile, Vector2 point)
```

**Parameters** \
`tile` [Point](../../Murder/Core/Geometry/Point.html) \
`point` [Vector2](../../Murder/Core/Geometry/Vector2.html) \

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
[Vector2](../../Murder/Core/Geometry/Vector2.html) \
#### Tile
```csharp
public readonly Point Tile;
```

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \


⚡