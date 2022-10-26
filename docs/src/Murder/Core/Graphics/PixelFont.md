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
#### Sizes
```csharp
public List<T> Sizes;
```

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \
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

#### Get(float)
```csharp
public PixelFontSize Get(float size)
```

**Parameters** \
`size` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[PixelFontSize](/Murder/Core/Graphics/PixelFontSize.html) \

#### Draw(float, Batch2D, string, Vector2, Vector2, Color, T?, T?)
```csharp
public void Draw(float baseSize, Batch2D spriteBatch, string text, Vector2 position, Vector2 justify, Color color, T? strokeColor, T? shadowColor)
```

**Parameters** \
`baseSize` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`justify` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Draw(float, Batch2D, string, Vector2, Color, T?, T?)
```csharp
public void Draw(float baseSize, Batch2D spriteBatch, string text, Vector2 position, Color color, T? strokeColor, T? shadowColor)
```

**Parameters** \
`baseSize` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Draw(float, Batch2D, string, int, Vector2, Vector2, Color, T?, T?)
```csharp
public void Draw(float baseSize, Batch2D spriteBatch, string text, int visibleCharacters, Vector2 position, Vector2 justify, Color color, T? strokeColor, T? shadowColor)
```

**Parameters** \
`baseSize` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`visibleCharacters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`justify` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \



⚡