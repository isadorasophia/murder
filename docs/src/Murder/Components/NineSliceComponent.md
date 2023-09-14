# NineSliceComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct NineSliceComponent : IComponent
```

This component makes sure that any sprite will render as a 9-slice instead,
            as specified.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public NineSliceComponent()
```

### ⭐ Properties
#### Sprite
```csharp
public readonly Guid Sprite;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### Target
```csharp
public readonly Rectangle Target;
```

**Returns** \
[Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
#### TargetSpriteBatch
```csharp
public readonly TargetSpriteBatches TargetSpriteBatch;
```

**Returns** \
[TargetSpriteBatches](../../Murder/Core/Graphics/TargetSpriteBatches.html) \
#### YSortOffset
```csharp
public readonly int YSortOffset;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \


⚡