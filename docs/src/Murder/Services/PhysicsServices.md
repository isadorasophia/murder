# PhysicsServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class PhysicsServices
```

### ⭐ Methods
#### CollidesAt(Map&, int, ColliderComponent, Vector2, IEnumerable<T>, out Int32&)
```csharp
public bool CollidesAt(Map& map, int ignoreId, ColliderComponent collider, Vector2 position, IEnumerable<T> others, Int32& hitId)
```

**Parameters** \
`map` [Map&](/Murder/Core/Map.html) \
`ignoreId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`collider` [ColliderComponent](/Murder/Components/ColliderComponent.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`others` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \
`hitId` [int&](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CollidesAt(Map&, int, ColliderComponent, Vector2, IEnumerable<T>)
```csharp
public bool CollidesAt(Map& map, int ignoreId, ColliderComponent collider, Vector2 position, IEnumerable<T> others)
```

**Parameters** \
`map` [Map&](/Murder/Core/Map.html) \
`ignoreId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`collider` [ColliderComponent](/Murder/Components/ColliderComponent.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`others` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CollidesAtTile(Map&, ColliderComponent, Vector2)
```csharp
public bool CollidesAtTile(Map& map, ColliderComponent collider, Vector2 position)
```

**Parameters** \
`map` [Map&](/Murder/Core/Map.html) \
`collider` [ColliderComponent](/Murder/Components/ColliderComponent.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CollidesWith(Entity, Entity)
```csharp
public bool CollidesWith(Entity entityA, Entity entityB)
```

**Parameters** \
`entityA` [Entity](/Bang/Entities/Entity.html) \
`entityB` [Entity](/Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CollidesWith(IShape, Point, IShape, Point)
```csharp
public bool CollidesWith(IShape shape1, Point position1, IShape shape2, Point position2)
```

**Parameters** \
`shape1` [IShape](/Murder/Core/Geometry/IShape.html) \
`position1` [Point](/Murder/Core/Geometry/Point.html) \
`shape2` [IShape](/Murder/Core/Geometry/IShape.html) \
`position2` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ContainsPoint(Entity, Point)
```csharp
public bool ContainsPoint(Entity entity, Point point)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`point` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RaycastTiles(World, PositionComponent, PositionComponent, GridCollisionType, out RaycastHit&)
```csharp
public bool RaycastTiles(World world, PositionComponent myPosition, PositionComponent otherPosition, GridCollisionType flags, RaycastHit& hit)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`myPosition` [PositionComponent](/Murder/Components/PositionComponent.html) \
`otherPosition` [PositionComponent](/Murder/Components/PositionComponent.html) \
`flags` [GridCollisionType](/Murder/Core/GridCollisionType.html) \
`hit` [RaycastHit&](/Murder/Services/RaycastHit.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetAllCollisionsAt(PositionComponent, ColliderComponent, int, IEnumerable<T>)
```csharp
public IEnumerable<T> GetAllCollisionsAt(PositionComponent position, ColliderComponent collider, int ignoreId, IEnumerable<T> others)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \
`collider` [ColliderComponent](/Murder/Components/ColliderComponent.html) \
`ignoreId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`others` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### Neighbours(PositionComponent, World)
```csharp
public IEnumerable<T> Neighbours(PositionComponent position, World world)
```

Get all the neighbours of a position within the world.
            This does not check for collision (yet)!

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \
`world` [World](/Bang/World.html) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### FilterPositionAndColliderEntities(World, bool, Type[])
```csharp
public ImmutableArray<T> FilterPositionAndColliderEntities(World world, bool solidOnly, Type[] requireComponents)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`solidOnly` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`requireComponents` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### FilterPositionAndColliderEntities(World, bool)
```csharp
public ImmutableArray<T> FilterPositionAndColliderEntities(World world, bool solidOnly)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`solidOnly` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### FilterPositionAndColliderEntities(World, Func<T, TResult>)
```csharp
public ImmutableArray<T> FilterPositionAndColliderEntities(World world, Func<T, TResult> filter)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`filter` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-2?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### FilterPositionAndColliderEntities(IEnumerable<T>, bool)
```csharp
public ImmutableArray<T> FilterPositionAndColliderEntities(IEnumerable<T> entities, bool solidOnly)
```

**Parameters** \
`entities` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \
`solidOnly` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetBoundingBox(ColliderComponent, Point)
```csharp
public IntRectangle GetBoundingBox(ColliderComponent collider, Point position)
```

**Parameters** \
`collider` [ColliderComponent](/Murder/Components/ColliderComponent.html) \
`position` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

#### GetCarveBoundingBox(ColliderComponent, Point)
```csharp
public IntRectangle GetCarveBoundingBox(ColliderComponent collider, Point position)
```

**Parameters** \
`collider` [ColliderComponent](/Murder/Components/ColliderComponent.html) \
`position` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

#### GetColliderBoundingBox(Entity)
```csharp
public IntRectangle GetColliderBoundingBox(Entity target)
```

Get bounding box of an entity that contains both [ColliderComponent](/Murder/Components/ColliderComponent.html)
            and [PositionComponent](/Murder/Components/PositionComponent.html).

**Parameters** \
`target` [Entity](/Bang/Entities/Entity.html) \

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

#### FindNextAvailablePosition(World, Entity, PositionComponent)
```csharp
public T? FindNextAvailablePosition(World world, Entity e, PositionComponent target)
```

Find an eligible position to place an entity <paramref name="e" /> in the world that does not collide
            with other entities and targets <paramref name="target" />.
            This will return immediate neighbours if <paramref name="target" /> is already occupied.

**Parameters** \
`world` [World](/Bang/World.html) \
`e` [Entity](/Bang/Entities/Entity.html) \
`target` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \



⚡