# NineSlice

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public sealed struct NineSlice
```

### ⭐ Constructors
```csharp
public NineSlice(NineSliceInfo info)
```

**Parameters** \
`info` [NineSliceInfo](/Murder/Utilities/NineSliceInfo.html) \

### ⭐ Properties
#### Core
```csharp
public readonly Rectangle Core;
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \
#### Image
```csharp
public readonly AsepriteAsset Image;
```

**Returns** \
[AsepriteAsset](/Murder/Assets/Graphics/AsepriteAsset.html) \
### ⭐ Methods
#### Draw(Batch2D, Rectangle, float)
```csharp
public void Draw(Batch2D batch, Rectangle target, float sort)
```

**Parameters** \
`batch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`target` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawWithText(Batch2D, string, PixelFont, Color, T?, T?, Rectangle, float)
```csharp
public void DrawWithText(Batch2D batch, string text, PixelFont font, Color textColor, T? textStrokeColor, T? textShadowColor, Rectangle target, float sort)
```

**Parameters** \
`batch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`font` [PixelFont](/Murder/Core/Graphics/PixelFont.html) \
`textColor` [Color](/Murder/Core/Graphics/Color.html) \
`textStrokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`textShadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`target` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡