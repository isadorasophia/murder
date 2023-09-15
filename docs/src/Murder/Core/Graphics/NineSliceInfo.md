# NineSliceInfo

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct NineSliceInfo
```

### ⭐ Constructors
```csharp
public NineSliceInfo()
```

```csharp
public NineSliceInfo(Rectangle core, Guid image)
```

**Parameters** \
`core` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`image` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

```csharp
public NineSliceInfo(Guid image)
```

**Parameters** \
`image` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### Core
```csharp
public readonly Rectangle Core;
```

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
#### Empty
```csharp
public static NineSliceInfo Empty { get; }
```

**Returns** \
[NineSliceInfo](../../../Murder/Core/Graphics/NineSliceInfo.html) \
#### Image
```csharp
public readonly Guid Image;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
### ⭐ Methods
#### Cache()
```csharp
public CachedNineSlice Cache()
```

**Returns** \
[CachedNineSlice](../../../Murder/Core/Graphics/CachedNineSlice.html) \

#### Draw(Batch2D, Rectangle, DrawInfo, AnimationInfo)
```csharp
public void Draw(Batch2D batch, Rectangle target, DrawInfo info, AnimationInfo animationInfo)
```

**Parameters** \
`batch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`target` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`info` [DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \
`animationInfo` [AnimationInfo](../../../Murder/Core/Graphics/AnimationInfo.html) \

#### Draw(Batch2D, Rectangle, string, Color, float)
```csharp
public void Draw(Batch2D batch, Rectangle target, string animation, Color color, float sort)
```

**Parameters** \
`batch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`target` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`animation` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`color` [Color](../../../Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡