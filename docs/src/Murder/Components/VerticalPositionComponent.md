# VerticalPositionComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct VerticalPositionComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public VerticalPositionComponent()
```

```csharp
public VerticalPositionComponent(float z, float zVelocity, bool hasGravity)
```

**Parameters** \
`z` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`zVelocity` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`hasGravity` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### GRAVITY
```csharp
public static const float GRAVITY;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### HasGravity
```csharp
public readonly bool HasGravity;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Z
```csharp
public readonly float Z;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### ZVelocity
```csharp
public readonly float ZVelocity;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### UpdatePosition(float, float)
```csharp
public VerticalPositionComponent UpdatePosition(float deltaTime, float bounciness)
```

**Parameters** \
`deltaTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`bounciness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[VerticalPositionComponent](../../Murder/Components/VerticalPositionComponent.html) \



⚡