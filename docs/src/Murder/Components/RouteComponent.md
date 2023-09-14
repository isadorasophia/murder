# RouteComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct RouteComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public RouteComponent(ImmutableDictionary<TKey, TValue> route, Point initial, Point target)
```

**Parameters** \
`route` [ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
`initial` [Point](../../Murder/Core/Geometry/Point.html) \
`target` [Point](../../Murder/Core/Geometry/Point.html) \

### ⭐ Properties
#### Initial
```csharp
public readonly Point Initial;
```

Initial position cell.

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \
#### Nodes
```csharp
public readonly ImmutableDictionary<TKey, TValue> Nodes;
```

Nodes path that the agent will make.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### Target
```csharp
public readonly Point Target;
```

Goal position cell.

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \


⚡