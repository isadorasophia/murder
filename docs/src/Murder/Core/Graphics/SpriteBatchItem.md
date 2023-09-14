# SpriteBatchItem

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class SpriteBatchItem
```

### ⭐ Constructors
```csharp
public SpriteBatchItem()
```

### ⭐ Properties
#### IndexData
```csharp
public Int32[] IndexData;
```

**Returns** \
[int[]](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Texture
```csharp
public Texture2D Texture;
```

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
#### VertexCount
```csharp
public int VertexCount;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### VertexData
```csharp
public VertexInfo[] VertexData;
```

**Returns** \
[VertexInfo[]](../../../Murder/Core/Graphics/VertexInfo.html) \
### ⭐ Methods
#### Set(Texture2D, Vector2, Vector2, T?, float, Vector2, ImageFlip, Color, Vector2, Vector3, float)
```csharp
public void Set(Texture2D texture, Vector2 position, Vector2 destinationSize, T? sourceRectangle, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 origin, Vector3 colorBlend, float layerDepth)
```

Sets a Texture to be drawn to the batch

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
\
`position` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
\
`destinationSize` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
\
`sourceRectangle` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
\
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`scale` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
\
`flip` [ImageFlip](../../../Murder/Core/Graphics/ImageFlip.html) \
\
`color` [Color](../../../Murder/Core/Graphics/Color.html) \
\
`origin` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
\
`colorBlend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
\
`layerDepth` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### SetPolygon(Texture2D, ReadOnlySpan<T>, DrawInfo)
```csharp
public void SetPolygon(Texture2D texture, ReadOnlySpan<T> vertices, DrawInfo drawInfo)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`vertices` [ReadOnlySpan\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1?view=net-7.0) \
`drawInfo` [DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \



⚡