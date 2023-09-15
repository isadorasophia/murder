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
`mapBounds` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \

### ⭐ Properties
#### Collision
```csharp
public readonly QTNode<T> Collision;
```

**Returns** \
[QTNode\<T\>](../../../Murder/Core/Physics/QTNode-1.html) \
#### PushAway
```csharp
public readonly QTNode<T> PushAway;
```

**Returns** \
[QTNode\<T\>](../../../Murder/Core/Physics/QTNode-1.html) \
#### StaticRender
```csharp
public readonly QTNode<T> StaticRender;
```

**Returns** \
[QTNode\<T\>](../../../Murder/Core/Physics/QTNode-1.html) \
### ⭐ Methods
#### AddToCollisionQuadTree(IEnumerable<T>)
```csharp
public void AddToCollisionQuadTree(IEnumerable<T> entities)
```

**Parameters** \
`entities` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### AddToStaticRenderQuadTree(IEnumerable<T>)
```csharp
public void AddToStaticRenderQuadTree(IEnumerable<T> entities)
```

**Parameters** \
`entities` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### GetCollisionEntitiesAt(Rectangle, List<T>)
```csharp
public void GetCollisionEntitiesAt(Rectangle boundingBox, List<T> list)
```

**Parameters** \
`boundingBox` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`list` [List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

#### RemoveFromCollisionQuadTree(IEnumerable<T>)
```csharp
public void RemoveFromCollisionQuadTree(IEnumerable<T> entities)
```

**Parameters** \
`entities` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### RemoveFromStaticRenderQuadTree(IEnumerable<T>)
```csharp
public void RemoveFromStaticRenderQuadTree(IEnumerable<T> entities)
```

**Parameters** \
`entities` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### UpdateQuadTree(IEnumerable<T>)
```csharp
public void UpdateQuadTree(IEnumerable<T> entities)
```

Completelly clears and rebuilds the quad tree and pushAway quad tree using a given list of entities
            We should avoid this if possible

**Parameters** \
`entities` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \
\



⚡