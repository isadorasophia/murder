# PixelFont

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class PixelFont
```

### ⭐ Constructors
```csharp
public PixelFont(string face)
```

**Parameters** \
`face` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### Face
```csharp
public string Face;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### LineHeight
```csharp
public int LineHeight { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### GetLineWidth(float, string)
```csharp
public float GetLineWidth(float size, string text)
```

**Parameters** \
`size` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### AddFontSize(XmlElement, AtlasId, bool)
```csharp
public PixelFontSize AddFontSize(XmlElement data, AtlasId atlasId, bool outline)
```

**Parameters** \
`data` [XmlElement](https://learn.microsoft.com/en-us/dotnet/api/System.Xml.XmlElement?view=net-7.0) \
`atlasId` [AtlasId](/Murder/Data/AtlasId.html) \
`outline` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[PixelFontSize](/Murder/Core/Graphics/PixelFontSize.html) \

#### Draw(Batch2D, string, float, Vector2, Vector2, float, Color, T?, T?)
```csharp
public void Draw(Batch2D spriteBatch, string text, float scale, Vector2 position, Vector2 alignment, float sort, Color color, T? strokeColor, T? shadowColor)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`scale` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`alignment` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Draw(Batch2D, string, float, Vector2, float, Color, T?, T?)
```csharp
public void Draw(Batch2D spriteBatch, string text, float scale, Vector2 position, float sort, Color color, T? strokeColor, T? shadowColor)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`scale` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Draw(Batch2D, string, float, int, Vector2, Vector2, float, Color, T?, T?)
```csharp
public void Draw(Batch2D spriteBatch, string text, float scale, int visibleCharacters, Vector2 position, Vector2 alignment, float sort, Color color, T? strokeColor, T? shadowColor)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`scale` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`visibleCharacters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`alignment` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \



⚡