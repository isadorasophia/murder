# MoveToComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct MoveToComponent : IComponent
```

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public MoveToComponent(Vector2& target, float maxSpeed, float accel)
```

**Parameters** \
`target` [Vector2&](/Murder/Core/Geometry/Vector2.html) \
`maxSpeed` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`accel` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Accel
```csharp
public readonly float Accel;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### MaxSpeed
```csharp
public readonly float MaxSpeed;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
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