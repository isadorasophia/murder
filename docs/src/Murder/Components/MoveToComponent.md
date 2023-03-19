# MoveToComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct MoveToComponent : IComponent
```

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public MoveToComponent(Vector2& target, float minDistance, float slowDownDistance)
```

**Parameters** \
`target` [Vector2&](/Murder/Core/Geometry/Vector2.html) \
`minDistance` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`slowDownDistance` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public MoveToComponent(Vector2& target)
```

**Parameters** \
`target` [Vector2&](/Murder/Core/Geometry/Vector2.html) \

### ⭐ Properties
#### MinDistance
```csharp
public readonly float MinDistance;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### SlowDownDistance
```csharp
public readonly float SlowDownDistance;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Target
```csharp
public readonly Vector2 Target;
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \


⚡