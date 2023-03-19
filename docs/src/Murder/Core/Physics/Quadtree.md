# Quadtree

**Namespace:** Murder.Core.Physics \
**Assembly:** Murder.dll

```csharp
public class Quadtree
```

### ⭐ Constructors
```csharp
public Quadtree(Rectangle mapBounds)
```

**Parameters** \
`mapBounds` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \

### ⭐ Properties
#### Collision
```csharp
public readonly QTNode<T> Collision;
```

**Returns** \
[QTNode\<T\>](/Murder/Core/Physics/QTNode-1.html) \
#### PushAway
```csharp
public readonly QTNode<T> PushAway;
```

**Returns** \
[QTNode\<T\>](/Murder/Core/Physics/QTNode-1.html) \
### ⭐ Methods
#### GetEntitiesAt(Rectangle, List`1&)
```csharp
public void GetEntitiesAt(Rectangle boundingBox, List`1& list)
```

**Parameters** \
`boundingBox` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`list` [List\<T\>&](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

#### UpdateQuadTree(IEnumerable<T>)
```csharp
public void UpdateQuadTree(IEnumerable<T> entities)
```

**Parameters** \
`entities` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \



⚡