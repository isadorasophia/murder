# ColliderComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct ColliderComponent : IComponent
```

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public ColliderComponent()
```

```csharp
public ColliderComponent(ImmutableArray<T> shapes, int layer, Color color)
```

**Parameters** \
`shapes` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`layer` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

### ⭐ Properties
#### DebugColor
```csharp
public readonly Color DebugColor;
```

**Returns** \
[Color](/Murder/Core/Graphics/Color.html) \
#### Layer
```csharp
public readonly int Layer;
```

Value of layer according to [CollisionLayersBase](/Murder/Core/Physics/CollisionLayersBase.html).

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Shapes
```csharp
public readonly ImmutableArray<T> Shapes;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### SetLayer(int)
```csharp
public ColliderComponent SetLayer(int layer)
```

Set layer according to [CollisionLayersBase](/Murder/Core/Physics/CollisionLayersBase.html).

**Parameters** \
`layer` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[ColliderComponent](/Murder/Components/ColliderComponent.html) \



⚡