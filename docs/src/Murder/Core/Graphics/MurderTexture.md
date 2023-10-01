# MurderTexture

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct MurderTexture
```

### ⭐ Constructors
```csharp
public MurderTexture(AtlasCoordinates AtlasCoordinates)
```

**Parameters** \
`AtlasCoordinates` [AtlasCoordinates](../../../Murder/Core/Graphics/AtlasCoordinates.html) \

```csharp
public MurderTexture(string texture)
```

**Parameters** \
`texture` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Methods
#### Draw(Batch2D, Vector2, Vector2, Rectangle, Color, ImageFlip, float, Vector3)
```csharp
public void Draw(Batch2D batch2D, Vector2 position, Vector2 scale, Rectangle clip, Color color, ImageFlip flip, float sort, Vector3 blend)
```

Draws a texture with a clipping area.

**Parameters** \
`batch2D` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`scale` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`clip` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`color` [Color](../../../Murder/Core/Graphics/Color.html) \
`flip` [ImageFlip](../../../Murder/Core/Graphics/ImageFlip.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`blend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \



⚡