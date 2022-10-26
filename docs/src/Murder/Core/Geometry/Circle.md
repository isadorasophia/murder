# Circle

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Circle
```

### ⭐ Constructors
```csharp
public Circle(float radius)
```

**Parameters** \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public Circle(float x, float y, float radius)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Radius
```csharp
public readonly float Radius;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### X
```csharp
public readonly float X;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Y
```csharp
public readonly float Y;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### Contains(Point)
```csharp
public bool Contains(Point point)
```

**Parameters** \
`point` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### AddPosition(PositionComponent)
```csharp
public Circle AddPosition(PositionComponent position)
```

**Parameters** \
`position` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[Circle](/Murder/Core/Geometry/Circle.html) \

#### AddPosition(Point)
```csharp
public Circle AddPosition(Point position)
```

**Parameters** \
`position` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[Circle](/Murder/Core/Geometry/Circle.html) \



⚡