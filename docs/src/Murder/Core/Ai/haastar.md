# HAAStar

**Namespace:** Murder.Core.Ai \
**Assembly:** Murder.dll

```csharp
public class HAAStar
```

### ⭐ Constructors
```csharp
public HAAStar(int width, int height)
```

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### CLUSTER_SIZE
```csharp
public static const int CLUSTER_SIZE;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### ClusterHeight
```csharp
public readonly int ClusterHeight;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### ClusterWidth
```csharp
public readonly int ClusterWidth;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### DebugNodes
```csharp
public ImmutableDictionary<TKey, TValue> DebugNodes;
```

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
### ⭐ Methods
#### Search(Map, Point, Point)
```csharp
public ImmutableDictionary<TKey, TValue> Search(Map map, Point initial, Point target)
```

**Parameters** \
`map` [Map](../../../Murder/Core/Map.html) \
`initial` [Point](../../../Murder/Core/Geometry/Point.html) \
`target` [Point](../../../Murder/Core/Geometry/Point.html) \

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

#### Refresh(Map)
```csharp
public void Refresh(Map map)
```

**Parameters** \
`map` [Map](../../../Murder/Core/Map.html) \



⚡