# PositionRotationComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct PositionRotationComponent : IMurderTransformComponent, ITransformComponent, IParentRelativeComponent, IComponent, IEquatable<T>
```

Position component used to track entities positions within a grid.

**Implements:** _[IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html), [ITransformComponent](/Bang/Components/ITransformComponent.html), [IParentRelativeComponent](/Bang/Components/IParentRelativeComponent.html), [IComponent](/Bang/Components/IComponent.html), [IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public PositionRotationComponent(Point p, float angle)
```

Create a new [PositionRotationComponent](/Murder/Components/PositionRotationComponent.html).

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
\
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

```csharp
public PositionRotationComponent(Vector2 v, float angle)
```

Create a new [PositionRotationComponent](/Murder/Components/PositionRotationComponent.html).

**Parameters** \
`v` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

```csharp
public PositionRotationComponent(float x, float y, float angle, IMurderTransformComponent parent)
```

Create a new [PositionRotationComponent](/Murder/Components/PositionRotationComponent.html).

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`parent` [IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \
\

### ⭐ Properties
#### Angle
```csharp
public virtual float Angle { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### HasParent
```csharp
public virtual bool HasParent { get; }
```

Whether this position is tracking a parent entity.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Scale
```csharp
public virtual Vector2 Scale { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### X
```csharp
public virtual float X { get; }
```

Relative X position of the component.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Y
```csharp
public virtual float Y { get; }
```

Relative Y position of the component.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### Equals(PositionRotationComponent)
```csharp
public virtual bool Equals(PositionRotationComponent other)
```

Compares two position components. This will take their parents into account.

**Parameters** \
`other` [PositionRotationComponent](/Murder/Components/PositionRotationComponent.html) \

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

#### Add(IMurderTransformComponent)
```csharp
public virtual IMurderTransformComponent Add(IMurderTransformComponent r)
```

**Parameters** \
`r` [IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \

**Returns** \
[IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \

#### Add(Vector2)
```csharp
public virtual IMurderTransformComponent Add(Vector2 r)
```

**Parameters** \
`r` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \

#### GetGlobal()
```csharp
public virtual IMurderTransformComponent GetGlobal()
```

Return the global position of the component within the world.

**Returns** \
[IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \

#### Subtract(IMurderTransformComponent)
```csharp
public virtual IMurderTransformComponent Subtract(IMurderTransformComponent r)
```

**Parameters** \
`r` [IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \

**Returns** \
[IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \

#### Subtract(Vector2)
```csharp
public virtual IMurderTransformComponent Subtract(Vector2 r)
```

**Parameters** \
`r` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \

#### With(float, float)
```csharp
public virtual IMurderTransformComponent With(float x, float y)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[IMurderTransformComponent](/Murder/Components/IMurderTransformComponent.html) \

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