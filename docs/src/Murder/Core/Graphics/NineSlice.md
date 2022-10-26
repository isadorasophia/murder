# NineSlice

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct NineSlice
```

### ⭐ Constructors
```csharp
public NineSlice(string texturePath, AtlasId atlasId, int sliceSize, Color color)
```

**Parameters** \
`texturePath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`atlasId` [AtlasId](/Murder/Data/AtlasId.html) \
`sliceSize` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

```csharp
public NineSlice(string texturePath, AtlasId atlasId, int sliceSize)
```

**Parameters** \
`texturePath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`atlasId` [AtlasId](/Murder/Data/AtlasId.html) \
`sliceSize` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Color
```csharp
public Color Color;
```

**Returns** \
[Color](/Murder/Core/Graphics/Color.html) \
#### SliceSize
```csharp
public readonly int SliceSize;
```

The size of the cut in pixels

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Draw(Batch2D, Rectangle, float)
```csharp
public void Draw(Batch2D spriteBatch, Rectangle drawArea, float layerDepth)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`drawArea` [Rectangle](https://docs.monogame.net/api/Microsoft.Xna.Framework.Rectangle.html) \
`layerDepth` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡