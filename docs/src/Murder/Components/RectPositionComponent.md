# RectPositionComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct RectPositionComponent : IParentRelativeComponent, IComponent
```

**Implements:** _[IParentRelativeComponent](../../Bang/Components/IParentRelativeComponent.html), [IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public RectPositionComponent(float top, float left, float bottom, float right, Vector2 size, Vector2 origin, IComponent parent)
```

**Parameters** \
`top` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`left` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`bottom` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`right` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`size` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`origin` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`parent` [IComponent](../../Bang/Components/IComponent.html) \

### ⭐ Properties
#### HasParent
```csharp
public virtual bool HasParent { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Origin
```csharp
public readonly Vector2 Origin;
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Size
```csharp
public readonly Vector2 Size;
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
### ⭐ Methods
#### GetBox(Entity, Point, T?)
```csharp
public Rectangle GetBox(Entity entity, Point screenSize, T? referenceSize)
```

**Parameters** \
`entity` [Entity](../../Bang/Entities/Entity.html) \
`screenSize` [Point](../../Murder/Core/Geometry/Point.html) \
`referenceSize` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[Rectangle](../../Murder/Core/Geometry/Rectangle.html) \

#### AddPadding(RectPositionComponent)
```csharp
public RectPositionComponent AddPadding(RectPositionComponent b)
```

**Parameters** \
`b` [RectPositionComponent](../../Murder/Components/RectPositionComponent.html) \

**Returns** \
[RectPositionComponent](../../Murder/Components/RectPositionComponent.html) \

#### WithSize(Vector2)
```csharp
public RectPositionComponent WithSize(Vector2 size)
```

**Parameters** \
`size` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[RectPositionComponent](../../Murder/Components/RectPositionComponent.html) \

#### WithoutParent()
```csharp
public virtual IParentRelativeComponent WithoutParent()
```

**Returns** \
[IParentRelativeComponent](../../Bang/Components/IParentRelativeComponent.html) \

#### OnParentModified(IComponent, Entity)
```csharp
public virtual void OnParentModified(IComponent parentComponent, Entity childEntity)
```

**Parameters** \
`parentComponent` [IComponent](../../Bang/Components/IComponent.html) \
`childEntity` [Entity](../../Bang/Entities/Entity.html) \



⚡