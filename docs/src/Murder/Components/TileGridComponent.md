# TileGridComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct TileGridComponent : IModifiableComponent, IComponent
```

This is a struct that points to a singleton class.
            Reactive systems won't be able to subscribe to this component.

**Implements:** _[IModifiableComponent](../../Bang/Components/IModifiableComponent.html), [IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public TileGridComponent()
```

```csharp
public TileGridComponent(Point origin, int width, int height)
```

**Parameters** \
`origin` [Point](../../Murder/Core/Geometry/Point.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

```csharp
public TileGridComponent(TileGrid grid)
```

**Parameters** \
`grid` [TileGrid](../../Murder/Core/TileGrid.html) \

```csharp
public TileGridComponent(int width, int height)
```

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Grid
```csharp
public readonly TileGrid Grid;
```

**Returns** \
[TileGrid](../../Murder/Core/TileGrid.html) \
#### Height
```csharp
public readonly int Height;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Origin
```csharp
public readonly Point Origin;
```

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \
#### Rectangle
```csharp
public IntRectangle Rectangle { get; }
```

**Returns** \
[IntRectangle](../../Murder/Core/Geometry/IntRectangle.html) \
#### Width
```csharp
public readonly int Width;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Subscribe(Action)
```csharp
public virtual void Subscribe(Action notification)
```

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \

#### Unsubscribe(Action)
```csharp
public virtual void Unsubscribe(Action notification)
```

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \



⚡