# QuadtreeComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct QuadtreeComponent : IModifiableComponent, IComponent
```

**Implements:** _[IModifiableComponent](../../Bang/Components/IModifiableComponent.html), [IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public QuadtreeComponent(Rectangle size)
```

**Parameters** \
`size` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \

### ⭐ Properties
#### Quadtree
```csharp
public readonly Quadtree Quadtree;
```

**Returns** \
[Quadtree](../../Murder/Core/Physics/Quadtree.html) \
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