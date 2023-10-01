# MoveToPerfectComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct MoveToPerfectComponent : IComponent
```

This is a move to component that is not tied to any agent and
            that matches perfectly the target.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public MoveToPerfectComponent(Vector2& target, float duration, EaseKind ease)
```

**Parameters** \
`target` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`ease` [EaseKind](../../Murder/Utilities/EaseKind.html) \

### ⭐ Properties
#### Duration
```csharp
public readonly float Duration;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### EaseKind
```csharp
public readonly EaseKind EaseKind;
```

**Returns** \
[EaseKind](../../Murder/Utilities/EaseKind.html) \
#### StartPosition
```csharp
public readonly T? StartPosition;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### StartTime
```csharp
public readonly float StartTime;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Target
```csharp
public readonly Vector2 Target;
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
### ⭐ Methods
#### WithStartPosition(Vector2&)
```csharp
public MoveToPerfectComponent WithStartPosition(Vector2& startPosition)
```

**Parameters** \
`startPosition` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[MoveToPerfectComponent](../../Murder/Components/MoveToPerfectComponent.html) \



⚡