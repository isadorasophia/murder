# SpriteBatchItem

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class SpriteBatchItem : IBatchItem
```

**Implements:** _[IBatchItem](/Murder/Core/Graphics/IBatchItem.html)_

### ⭐ Constructors
```csharp
public SpriteBatchItem()
```

### ⭐ Properties
#### IndexData
```csharp
public virtual Int32[] IndexData { get; private set; }
```

**Returns** \
[int[]](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Texture
```csharp
public virtual Texture2D Texture { get; private set; }
```

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
#### VertexData
```csharp
public virtual VertexInfo[] VertexData { get; private set; }
```

**Returns** \
[VertexInfo[]](/Murder/Core/Graphics/VertexInfo.html) \
### ⭐ Methods
#### Clear()
```csharp
public void Clear()
```

#### Set(Texture2D, Vector2, Vector2, T?, float, Vector2, ImageFlip, Color, Vector2, Vector3, float)
```csharp
public void Set(Texture2D texture, Vector2 position, Vector2 destinationSize, T? sourceRectangle, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 origin, Vector3 colorBlend, float layerDepth)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`destinationSize` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`sourceRectangle` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`scale` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`flip` [ImageFlip](/Murder/Core/Graphics/ImageFlip.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`origin` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`colorBlend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
`layerDepth` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Set(Texture2D, Vector2[], Vector2, T?, float, Vector2, ImageFlip, Color, Vector2, Vector3, float)
```csharp
public void Set(Texture2D texture, Vector2[] vertices, Vector2 position, T? sourceRectangle, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 origin, Vector3 colorBlend, float layerDepth)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`vertices` [Vector2[]](/Murder/Core/Geometry/Vector2.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`sourceRectangle` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`scale` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`flip` [ImageFlip](/Murder/Core/Graphics/ImageFlip.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`origin` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`colorBlend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
`layerDepth` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡