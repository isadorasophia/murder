# CameraFollowComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct CameraFollowComponent : IComponent
```

Component used by the camera for tracking its target position.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public CameraFollowComponent()
```

```csharp
public CameraFollowComponent(Point targetPosition)
```

**Parameters** \
`targetPosition` [Point](../../Murder/Core/Geometry/Point.html) \

```csharp
public CameraFollowComponent(bool enabled, Entity secondaryTarget)
```

**Parameters** \
`enabled` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`secondaryTarget` [Entity](../../Bang/Entities/Entity.html) \

```csharp
public CameraFollowComponent(bool enabled, CameraStyle style)
```

**Parameters** \
`enabled` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`style` [CameraStyle](../../Murder/Components/CameraStyle.html) \

```csharp
public CameraFollowComponent(bool enabled)
```

**Parameters** \
`enabled` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### Enabled
```csharp
public readonly bool Enabled;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### SecondaryTarget
```csharp
public readonly Entity SecondaryTarget;
```

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \
#### Style
```csharp
public readonly CameraStyle Style;
```

Force to centralize the camera without a dead zone.

**Returns** \
[CameraStyle](../../Murder/Components/CameraStyle.html) \
#### TargetPosition
```csharp
public readonly T? TargetPosition;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \


⚡