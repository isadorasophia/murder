# QTNode\<T\>

**Namespace:** Murder.Core.Physics \
**Assembly:** Murder.dll

```csharp
public class QTNode<T>
```

### ⭐ Constructors
```csharp
public QTNode<T>(int level, Rectangle bounds)
```

**Parameters** \
`level` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`bounds` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \

### ⭐ Properties
#### Bounds
```csharp
public readonly Rectangle Bounds;
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \
#### Entities
```csharp
public readonly List<T> Entities;
```

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \
#### Level
```csharp
public readonly int Level;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Nodes
```csharp
public ImmutableArray<T> Nodes;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### Clear()
```csharp
public void Clear()
```

Recursivelly clears all entities of the node, but keeps the strtucture

#### DrawDebug(Batch2D)
```csharp
public void DrawDebug(Batch2D spriteBatch)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \

#### Insert(T, Rectangle)
```csharp
public void Insert(T entity, Rectangle boundingBox)
```

**Parameters** \
`entity` [T]() \
`boundingBox` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \

#### Reset()
```csharp
public void Reset()
```

Completelly resets the node removing anything inside

#### Retrieve(Rectangle, List`1&)
```csharp
public void Retrieve(Rectangle boundingBox, List`1& returnEntities)
```

**Parameters** \
`boundingBox` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`returnEntities` [List\<T\>&](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

#### Split()
```csharp
public void Split()
```



⚡