# LazyShape

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct LazyShape : IShape
```

**Implements:** _[IShape](/Murder/Core/Geometry/IShape.html)_

### ⭐ Constructors
```csharp
public LazyShape(float radius, Point offset)
```

**Parameters** \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`offset` [Point](/Murder/Core/Geometry/Point.html) \

### ⭐ Properties
#### Offset
```csharp
public readonly Point Offset;
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Radius
```csharp
public readonly float Radius;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### SQUARE_ROOT_OF_TWO
```csharp
public static const float SQUARE_ROOT_OF_TWO;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### Rectangle(Point)
```csharp
public Rectangle Rectangle(Point addPosition)
```

**Parameters** \
`addPosition` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \

#### GetBoundingBox()
```csharp
public virtual Rectangle GetBoundingBox()
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \



⚡