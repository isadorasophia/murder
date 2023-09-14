# IMurderTransformComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public abstract IMurderTransformComponent : ITransformComponent, IParentRelativeComponent, IComponent
```

This is an interface for transform components within the game.

**Implements:** _[ITransformComponent](../..//Bang/Components/ITransformComponent.html), [IParentRelativeComponent](../..//Bang/Components/IParentRelativeComponent.html), [IComponent](../..//Bang/Components/IComponent.html)_

### ⭐ Properties
#### Angle
```csharp
public abstract virtual float Angle { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Cx
```csharp
public virtual int Cx { get; }
```

This is the X grid coordinate. See [Grid](../..//Murder/Core/Grid.html) for more details on our grid specs.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Cy
```csharp
public virtual int Cy { get; }
```

This is the Y grid coordinate. See [Grid](../..//Murder/Core/Grid.html) for more details on our grid specs.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Point
```csharp
public virtual Point Point { get; }
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### Scale
```csharp
public abstract virtual Vector2 Scale { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### Vector2
```csharp
public virtual Vector2 Vector2 { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### X
```csharp
public abstract virtual float X { get; }
```

Relative X position of the component.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Y
```csharp
public abstract virtual float Y { get; }
```

Relative Y position of the component.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### Add(IMurderTransformComponent)
```csharp
public abstract IMurderTransformComponent Add(IMurderTransformComponent r)
```

**Parameters** \
`r` [IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### Add(Vector2)
```csharp
public abstract IMurderTransformComponent Add(Vector2 r)
```

**Parameters** \
`r` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### GetGlobal()
```csharp
public abstract IMurderTransformComponent GetGlobal()
```

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### Subtract(IMurderTransformComponent)
```csharp
public abstract IMurderTransformComponent Subtract(IMurderTransformComponent r)
```

**Parameters** \
`r` [IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### Subtract(Vector2)
```csharp
public abstract IMurderTransformComponent Subtract(Vector2 r)
```

**Parameters** \
`r` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### With(float, float)
```csharp
public abstract IMurderTransformComponent With(float x, float y)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### Add(Point)
```csharp
public virtual IMurderTransformComponent Add(Point r)
```

**Parameters** \
`r` [Point](../..//Murder/Core/Geometry/Point.html) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### Subtract(Point)
```csharp
public virtual IMurderTransformComponent Subtract(Point r)
```

**Parameters** \
`r` [Point](../..//Murder/Core/Geometry/Point.html) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### With(Point)
```csharp
public virtual IMurderTransformComponent With(Point p)
```

**Parameters** \
`p` [Point](../..//Murder/Core/Geometry/Point.html) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \

#### With(Vector2)
```csharp
public virtual IMurderTransformComponent With(Vector2 p)
```

**Parameters** \
`p` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[IMurderTransformComponent](../..//Murder/Components/IMurderTransformComponent.html) \



⚡