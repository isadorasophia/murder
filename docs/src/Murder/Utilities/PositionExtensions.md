# PositionExtensions

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public static class PositionExtensions
```

### ⭐ Methods
#### IsSameCell(IMurderTransformComponent, IMurderTransformComponent)
```csharp
public bool IsSameCell(IMurderTransformComponent this, IMurderTransformComponent other)
```

**Parameters** \
`this` [IMurderTransformComponent](../../Murder/Components/IMurderTransformComponent.html) \
`other` [IMurderTransformComponent](../../Murder/Components/IMurderTransformComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetGlobalTransform(Entity)
```csharp
public IMurderTransformComponent GetGlobalTransform(Entity entity)
```

**Parameters** \
`entity` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[IMurderTransformComponent](../../Murder/Components/IMurderTransformComponent.html) \

#### CellPoint(IMurderTransformComponent)
```csharp
public Point CellPoint(IMurderTransformComponent this)
```

**Parameters** \
`this` [IMurderTransformComponent](../../Murder/Components/IMurderTransformComponent.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### FromCellToPointPosition(Point&)
```csharp
public Point FromCellToPointPosition(Point& point)
```

**Parameters** \
`point` [Point&](../../Murder/Core/Geometry/Point.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### FromWorldToLowerBoundGridPosition(Point&)
```csharp
public Point FromWorldToLowerBoundGridPosition(Point& point)
```

**Parameters** \
`point` [Point&](../../Murder/Core/Geometry/Point.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### ToCellPoint(IMurderTransformComponent)
```csharp
public Point ToCellPoint(IMurderTransformComponent position)
```

**Parameters** \
`position` [IMurderTransformComponent](../../Murder/Components/IMurderTransformComponent.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### ToPoint(PositionComponent)
```csharp
public Point ToPoint(PositionComponent position)
```

**Parameters** \
`position` [PositionComponent](../../Murder/Components/PositionComponent.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### Add(PositionComponent, Point)
```csharp
public PositionComponent Add(PositionComponent position, Point delta)
```

**Parameters** \
`position` [PositionComponent](../../Murder/Components/PositionComponent.html) \
`delta` [Point](../../Murder/Core/Geometry/Point.html) \

**Returns** \
[PositionComponent](../../Murder/Components/PositionComponent.html) \

#### Add(PositionComponent, float, float)
```csharp
public PositionComponent Add(PositionComponent position, float dx, float dy)
```

**Parameters** \
`position` [PositionComponent](../../Murder/Components/PositionComponent.html) \
`dx` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`dy` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[PositionComponent](../../Murder/Components/PositionComponent.html) \

#### Add(PositionComponent, Vector2)
```csharp
public PositionComponent Add(PositionComponent position, Vector2 delta)
```

**Parameters** \
`position` [PositionComponent](../../Murder/Components/PositionComponent.html) \
`delta` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[PositionComponent](../../Murder/Components/PositionComponent.html) \

#### ToPosition(Point&)
```csharp
public PositionComponent ToPosition(Point& position)
```

**Parameters** \
`position` [Point&](../../Murder/Core/Geometry/Point.html) \

**Returns** \
[PositionComponent](../../Murder/Components/PositionComponent.html) \

#### ToPosition(Vector2&)
```csharp
public PositionComponent ToPosition(Vector2& position)
```

**Parameters** \
`position` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[PositionComponent](../../Murder/Components/PositionComponent.html) \

#### AddToVector2(IMurderTransformComponent, Vector2)
```csharp
public Vector2 AddToVector2(IMurderTransformComponent position, Vector2 delta)
```

**Parameters** \
`position` [IMurderTransformComponent](../../Murder/Components/IMurderTransformComponent.html) \
`delta` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### AddToVector2(PositionComponent, float, float)
```csharp
public Vector2 AddToVector2(PositionComponent position, float dx, float dy)
```

**Parameters** \
`position` [PositionComponent](../../Murder/Components/PositionComponent.html) \
`dx` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`dy` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### FromCellToVector2CenterPosition(Point&)
```csharp
public Vector2 FromCellToVector2CenterPosition(Point& point)
```

**Parameters** \
`point` [Point&](../../Murder/Core/Geometry/Point.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### FromCellToVector2Position(Point&)
```csharp
public Vector2 FromCellToVector2Position(Point& point)
```

**Parameters** \
`point` [Point&](../../Murder/Core/Geometry/Point.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### ToSysVector2(PositionComponent)
```csharp
public Vector2 ToSysVector2(PositionComponent position)
```

**Parameters** \
`position` [PositionComponent](../../Murder/Components/PositionComponent.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### ToVector2(IMurderTransformComponent)
```csharp
public Vector2 ToVector2(IMurderTransformComponent position)
```

**Parameters** \
`position` [IMurderTransformComponent](../../Murder/Components/IMurderTransformComponent.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### SetGlobalPosition(Entity, Vector2)
```csharp
public void SetGlobalPosition(Entity entity, Vector2 position)
```

**Parameters** \
`entity` [Entity](../../Bang/Entities/Entity.html) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### SetGlobalTransform(Entity, T)
```csharp
public void SetGlobalTransform(Entity entity, T transform)
```

**Parameters** \
`entity` [Entity](../../Bang/Entities/Entity.html) \
`transform` [T](../../) \

#### SetLocalPosition(Entity, Vector2)
```csharp
public void SetLocalPosition(Entity entity, Vector2 position)
```

**Parameters** \
`entity` [Entity](../../Bang/Entities/Entity.html) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \



⚡