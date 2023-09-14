# ScaleComponent

**Namespace:** Murder.Components.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct ScaleComponent : IComponent, IEquatable<T>
```

**Implements:** _[IComponent](../../../Bang/Components/IComponent.html), [IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public ScaleComponent(Vector2 Scale)
```

**Parameters** \
`Scale` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \

### ⭐ Properties
#### Scale
```csharp
public Vector2 Scale { get; public set; }
```

**Returns** \
[Vector2](../../../Murder/Core/Geometry/Vector2.html) \
### ⭐ Methods
#### Equals(ScaleComponent)
```csharp
public virtual bool Equals(ScaleComponent other)
```

**Parameters** \
`other` [ScaleComponent](../../../Murder/Components/Graphics/ScaleComponent.html) \

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

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Deconstruct(out Vector2&)
```csharp
public void Deconstruct(Vector2& Scale)
```

**Parameters** \
`Scale` [Vector2&](../../../Murder/Core/Geometry/Vector2.html) \



⚡