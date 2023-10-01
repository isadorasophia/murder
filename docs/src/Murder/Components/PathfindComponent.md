# PathfindComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct PathfindComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public PathfindComponent(Vector2& target, PathfindAlgorithmKind algorithm)
```

**Parameters** \
`target` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`algorithm` [PathfindAlgorithmKind](../../Murder/Core/Ai/PathfindAlgorithmKind.html) \

### ⭐ Properties
#### Algorithm
```csharp
public readonly PathfindAlgorithmKind Algorithm;
```

**Returns** \
[PathfindAlgorithmKind](../../Murder/Core/Ai/PathfindAlgorithmKind.html) \
#### Target
```csharp
public readonly Vector2 Target;
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \


⚡