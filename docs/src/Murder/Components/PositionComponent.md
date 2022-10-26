# PositionComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct PositionComponent : IParentRelativeComponent, IComponent, IEquatable<T>
```

Position component used to track entities positions within a grid.

**Implements:** _[IParentRelativeComponent](/Bang/Components/IParentRelativeComponent.html), [IComponent](/Bang/Components/IComponent.html), [IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public PositionComponent(Point p)
```

Create a new [PositionComponent](/Murder/Components/PositionComponent.html).

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
\

```csharp
public PositionComponent(Vector2 v)
```

Create a new [PositionComponent](/Murder/Components/PositionComponent.html).

**Parameters** \
`v` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\

```csharp
public PositionComponent(float x, float y, IComponent parent)
```

Create a new [PositionComponent](/Murder/Components/PositionComponent.html).

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`parent` [IComponent](/Bang/Components/IComponent.html) \

### ⭐ Properties
#### Cx
```csharp
public int Cx { get; }
```

This is the X grid coordinate. See [Grid](/Murder/Core/Grid.html) for more details on our grid specs.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Cy
```csharp
public int Cy { get; }
```

This is the Y grid coordinate. See [Grid](/Murder/Core/Grid.html) for more details on our grid specs.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### HasParent
```csharp
public virtual bool HasParent { get; }
```

Whether this position is tracking a parent entity.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Point
```csharp
public Point Point { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Pos
```csharp
public Vector2 Pos { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### X
```csharp
public readonly float X;
```

Relative X position of the component.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### XSnap
```csharp
public int XSnap { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Y
```csharp
public readonly float Y;
```

Relative Y position of the component.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### YSnap
```csharp
public int YSnap { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### GetGlobalPosition()
```csharp
public PositionComponent GetGlobalPosition()
```

Return the global position of the component within the world.

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### Equals(PositionComponent)
```csharp
public virtual bool Equals(PositionComponent other)
```

Compares two position components. This will take their parents into account.

**Parameters** \
`other` [PositionComponent](/Murder/Components/PositionComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(Object)
```csharp
public virtual bool Equals(Object obj)
```

**Parameters** \
`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetHashCode()
```csharp
public virtual int GetHashCode()
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### WithoutParent()
```csharp
public virtual IParentRelativeComponent WithoutParent()
```

Creates a copy of component with the relative coordinates without its parent.

**Returns** \
[IParentRelativeComponent](/Bang/Components/IParentRelativeComponent.html) \
\

#### OnParentModified(IComponent, Entity)
```csharp
public virtual void OnParentModified(IComponent parentComponent, Entity childEntity)
```

This tracks whenever a parent position has been modified.

**Parameters** \
`parentComponent` [IComponent](/Bang/Components/IComponent.html) \
\
`childEntity` [Entity](/Bang/Entities/Entity.html) \
\



⚡