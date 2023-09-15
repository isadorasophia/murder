# CachedNineSlice

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct CachedNineSlice
```

### ⭐ Constructors
```csharp
public CachedNineSlice(NineSliceInfo info)
```

**Parameters** \
`info` [NineSliceInfo](../../../Murder/Core/Graphics/NineSliceInfo.html) \

```csharp
public CachedNineSlice(Guid SpriteAsset)
```

**Parameters** \
`SpriteAsset` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### _core
```csharp
public readonly Rectangle _core;
```

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
#### _image
```csharp
public readonly SpriteAsset _image;
```

**Returns** \
[SpriteAsset](../../../Murder/Assets/Graphics/SpriteAsset.html) \
### ⭐ Methods
#### Draw(Batch2D, Rectangle, DrawInfo, AnimationInfo)
```csharp
public void Draw(Batch2D batch, Rectangle target, DrawInfo drawInfo, AnimationInfo animationInfo)
```

**Parameters** \
`batch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`target` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`drawInfo` [DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \
`animationInfo` [AnimationInfo](../../../Murder/Core/Graphics/AnimationInfo.html) \

#### Draw(Batch2D, Rectangle, DrawInfo)
```csharp
public void Draw(Batch2D batch, Rectangle target, DrawInfo drawInfo)
```

**Parameters** \
`batch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`target` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`drawInfo` [DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \

#### DrawWithText(Batch2D, string, int, Color, T?, T?, Rectangle, float)
```csharp
public void DrawWithText(Batch2D batch, string text, int font, Color textColor, T? textOutlineColor, T? textShadowColor, Rectangle target, float sort)
```

**Parameters** \
`batch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`font` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`textColor` [Color](../../../Murder/Core/Graphics/Color.html) \
`textOutlineColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`textShadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`target` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡