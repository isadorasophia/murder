# ColliderServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class ColliderServices
```

### ⭐ Methods
#### GetBoundingBox(Entity)
```csharp
public IntRectangle GetBoundingBox(Entity e)
```

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[IntRectangle](../../Murder/Core/Geometry/IntRectangle.html) \

#### ToGrid(IntRectangle)
```csharp
public IntRectangle ToGrid(IntRectangle rectangle)
```

**Parameters** \
`rectangle` [IntRectangle](../../Murder/Core/Geometry/IntRectangle.html) \

**Returns** \
[IntRectangle](../../Murder/Core/Geometry/IntRectangle.html) \

#### GetCollidersBoundingBox(Entity, bool)
```csharp
public IntRectangle[] GetCollidersBoundingBox(Entity e, bool gridCoordinates)
```

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`gridCoordinates` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[IntRectangle[]](../../Murder/Core/Geometry/IntRectangle.html) \

#### FindCenter(Entity)
```csharp
public Point FindCenter(Entity e)
```

Returns the center point of an entity with all its colliders.

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### SnapToGrid(Vector2)
```csharp
public Vector2 SnapToGrid(Vector2 positive)
```

**Parameters** \
`positive` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### SnapToRelativeGrid(Vector2, Vector2)
```csharp
public Vector2 SnapToRelativeGrid(Vector2 position, Vector2 origin)
```

**Parameters** \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`origin` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \



⚡