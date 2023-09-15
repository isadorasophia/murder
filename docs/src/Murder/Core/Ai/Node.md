# Node

**Namespace:** Murder.Core.Ai \
**Assembly:** Murder.dll

```csharp
class Node
```

### ⭐ Constructors
```csharp
public Node(Point p, Point c, int weight)
```

**Parameters** \
`p` [Point](../../../Murder/Core/Geometry/Point.html) \
`c` [Point](../../../Murder/Core/Geometry/Point.html) \
`weight` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Cluster
```csharp
public readonly Point Cluster;
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### Neighbours
```csharp
public readonly Dictionary<TKey, TValue> Neighbours;
```

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### P
```csharp
public readonly Point P;
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### Weight
```csharp
public readonly int Weight;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### X
```csharp
public int X { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Y
```csharp
public int Y { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### HasNeighbour(Point)
```csharp
public bool HasNeighbour(Point p)
```

**Parameters** \
`p` [Point](../../../Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### PathTo(Point)
```csharp
public ImmutableDictionary<TKey, TValue> PathTo(Point p)
```

**Parameters** \
`p` [Point](../../../Murder/Core/Geometry/Point.html) \

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

#### AddEdge(Point, ImmutableDictionary<TKey, TValue>, double)
```csharp
public void AddEdge(Point p, ImmutableDictionary<TKey, TValue> path, double cost)
```

**Parameters** \
`p` [Point](../../../Murder/Core/Geometry/Point.html) \
`path` [ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
`cost` [double](https://learn.microsoft.com/en-us/dotnet/api/System.Double?view=net-7.0) \

#### RemoveEdge(Point)
```csharp
public void RemoveEdge(Point p)
```

**Parameters** \
`p` [Point](../../../Murder/Core/Geometry/Point.html) \



⚡