# AtlasTexture

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct AtlasTexture
```

An image coordinate inside an atlas

### ⭐ Constructors
```csharp
public AtlasTexture(string name, AtlasId atlasId, IntRectangle atlasRectangle, IntRectangle trimArea, Point originalSize, int atlasIndex, int atlasWidth, int atlasHeight)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`atlasId` [AtlasId](/Murder/Data/AtlasId.html) \
`atlasRectangle` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
`trimArea` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
`originalSize` [Point](/Murder/Core/Geometry/Point.html) \
`atlasIndex` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`atlasWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`atlasHeight` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Atlas
```csharp
public Texture2D Atlas { get; }
```

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
#### AtlasId
```csharp
public readonly AtlasId AtlasId;
```

**Returns** \
[AtlasId](/Murder/Data/AtlasId.html) \
#### AtlasIndex
```csharp
public readonly int AtlasIndex;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### AtlasSize
```csharp
public Point AtlasSize { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Empty
```csharp
public static AtlasTexture Empty;
```

**Returns** \
[AtlasTexture](/Murder/Core/Graphics/AtlasTexture.html) \
#### Height
```csharp
public int Height { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Name
```csharp
public readonly string Name;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### OriginalSize
```csharp
public Point OriginalSize { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Size
```csharp
public readonly Point Size;
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### SourceRectangle
```csharp
public readonly IntRectangle SourceRectangle;
```

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
#### TrimArea
```csharp
public readonly IntRectangle TrimArea;
```

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
#### UV
```csharp
public readonly Rectangle UV;
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \
#### Width
```csharp
public int Width { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Draw(Batch2D, Rectangle, Rectangle, Color, float, Vector3)
```csharp
public void Draw(Batch2D spriteBatch, Rectangle clip, Rectangle target, Color color, float depthLayer, Vector3 blend)
```

Draws a partial image stored inside an atlas to the spritebatch to a specific rect

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`clip` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`target` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`blend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \

#### Draw(Batch2D, Vector2, Rectangle, Color, float, Vector3)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, float depthLayer, Vector3 blend)
```

Draws a partial image stored inside an atlas to the spritebatch.

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
\
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`clip` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
\
`color` [Color](/Murder/Core/Graphics/Color.html) \
\
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`blend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
\

#### Draw(Batch2D, Vector2, Vector2, Vector2, float, ImageFlip, Color, Vector3, float)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, Vector2 scale, Vector2 origin, float rotation, ImageFlip flip, Color color, Vector3 blendStyle, float depthLayer)
```

Draws an image stored inside an atlas to the spritebatch.

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
\
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`scale` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`origin` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`flip` [ImageFlip](/Murder/Core/Graphics/ImageFlip.html) \
\
`color` [Color](/Murder/Core/Graphics/Color.html) \
\
`blendStyle` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
\
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\



⚡