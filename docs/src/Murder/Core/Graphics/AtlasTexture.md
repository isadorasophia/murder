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
public Vector2 AtlasSize { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
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
public Vector2 OriginalSize { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
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
#### Draw(Batch2D, Rectangle, Rectangle, Color, float)
```csharp
public void Draw(Batch2D spriteBatch, Rectangle destination, Rectangle clip, Color color, float depthLayer)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`destination` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`clip` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(Batch2D, Rectangle, Color, float)
```csharp
public void Draw(Batch2D spriteBatch, Rectangle destination, Color color, float depthLayer)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`destination` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(Batch2D, Vector2, Rectangle, Color, float, Vector3)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, float depthLayer, Vector3 blend)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`clip` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`blend` [Vector3](/Murder/Core/Geometry/Vector3.html) \

#### Draw(Batch2D, Vector2, Rectangle, Color, float)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, Rectangle clip, Color color, float depthLayer)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`clip` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(Batch2D, Vector2, Vector2, float, Color, Vector2)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, Vector2 size, float rotation, Color color, Vector2 origin)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`size` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`origin` [Vector2](/Murder/Core/Geometry/Vector2.html) \

#### Draw(Batch2D, Vector2, Vector2, float, Color, float)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, Vector2 size, float rotation, Color color, float depthLayer)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`size` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(Batch2D, Vector2, float, Color, ImageFlip, float, Vector3)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, float rotation, Color color, ImageFlip flip, float depthLayer, Vector3 colorBlend)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`flip` [ImageFlip](/Murder/Core/Graphics/ImageFlip.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`colorBlend` [Vector3](/Murder/Core/Geometry/Vector3.html) \

#### Draw(Batch2D, Vector2, float, Color, ImageFlip, float)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, float rotation, Color color, ImageFlip flip, float depthLayer)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`flip` [ImageFlip](/Murder/Core/Graphics/ImageFlip.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(Batch2D, Vector2, float, Color, float, Vector3)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, float rotation, Color color, float depthLayer, Vector3 blendMode)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`blendMode` [Vector3](/Murder/Core/Geometry/Vector3.html) \

#### Draw(Batch2D, Vector2, float, Color, float)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, float rotation, Color color, float depthLayer)
```

Draws a sprite to the spritebatch.

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`depthLayer` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡