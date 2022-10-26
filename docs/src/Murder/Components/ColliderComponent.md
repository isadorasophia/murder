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
public ColliderComponent(ImmutableArray<T> shapes, bool solid, Color color)
```

**Parameters** \
`shapes` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`solid` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

### ⭐ Properties
#### DebugColor
```csharp
public readonly Color DebugColor;
```

**Returns** \
[Color](/Murder/Core/Graphics/Color.html) \
#### Shapes
```csharp
public readonly ImmutableArray<T> Shapes;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Solid
```csharp
public readonly bool Solid;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \


⚡