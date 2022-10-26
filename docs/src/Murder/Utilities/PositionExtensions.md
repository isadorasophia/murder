# PositionExtensions

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public static class PositionExtensions
```

### ⭐ Methods
#### IsSameCell(PositionComponent, PositionComponent)
```csharp
public bool IsSameCell(PositionComponent this, PositionComponent other)
```

**Parameters** \
`this` [PositionComponent](/Murder/Components/PositionComponent.html) \
`other` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CellPoint(PositionComponent)
```csharp
public Point CellPoint(PositionComponent this)
```

**Parameters** \
`this` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \

#### FromCellToPointPosition(Point&)
```csharp
public Point FromCellToPointPosition(Point& point)
```

**Parameters** \
`point` [Point&](/Murder/Core/Geometry/Point.html) \

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \

#### ToCellPoint(PositionComponent)
```csharp
public Point ToCellPoint(PositionComponent position)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \

#### ToPoint(PositionComponent)
```csharp
public Point ToPoint(PositionComponent position)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \

#### Add(PositionComponent, Point)
```csharp
public PositionComponent Add(PositionComponent position, Point delta)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \
`delta` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### Add(PositionComponent, Vector2)
```csharp
public PositionComponent Add(PositionComponent position, Vector2 delta)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \
`delta` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### Add(PositionComponent, float, float)
```csharp
public PositionComponent Add(PositionComponent position, float dx, float dy)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \
`dx` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`dy` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### GetGlobalPosition(Entity)
```csharp
public PositionComponent GetGlobalPosition(Entity entity)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### ToPosition(Point&)
```csharp
public PositionComponent ToPosition(Point& position)
```

**Parameters** \
`position` [Point&](/Murder/Core/Geometry/Point.html) \

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### ToPosition(Vector2&)
```csharp
public PositionComponent ToPosition(Vector2& position)
```

**Parameters** \
`position` [Vector2&](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### ToPosition(Vector2&)
```csharp
public PositionComponent ToPosition(Vector2& position)
```

**Parameters** \
`position` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### AddToVector2(PositionComponent, Vector2)
```csharp
public Vector2 AddToVector2(PositionComponent position, Vector2 delta)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \
`delta` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### AddToVector2(PositionComponent, float, float)
```csharp
public Vector2 AddToVector2(PositionComponent position, float dx, float dy)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \
`dx` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`dy` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### FromCellToVector2CenterPosition(Point&)
```csharp
public Vector2 FromCellToVector2CenterPosition(Point& point)
```

**Parameters** \
`point` [Point&](/Murder/Core/Geometry/Point.html) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### FromCellToVector2Position(Point&)
```csharp
public Vector2 FromCellToVector2Position(Point& point)
```

**Parameters** \
`point` [Point&](/Murder/Core/Geometry/Point.html) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### ToSysVector2(PositionComponent)
```csharp
public Vector2 ToSysVector2(PositionComponent position)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### ToVector2(PositionComponent)
```csharp
public Vector2 ToVector2(PositionComponent position)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### SetGlobalPosition(Entity, PositionComponent)
```csharp
public void SetGlobalPosition(Entity entity, PositionComponent position)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \



⚡