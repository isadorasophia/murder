# PixelFont

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class PixelFont
```

### ⭐ Constructors
```csharp
public PixelFont(FontAsset asset)
```

**Parameters** \
`asset` [FontAsset](../..//Murder/Assets/Graphics/FontAsset.html) \

### ⭐ Properties
#### Index
```csharp
public int Index;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### LineHeight
```csharp
public int LineHeight { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### GetLineWidth(ReadOnlySpan<T>)
```csharp
public float GetLineWidth(ReadOnlySpan<T> text)
```

**Parameters** \
`text` [ReadOnlySpan\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(Batch2D, string, Vector2, Vector2, Vector2, float, Color, T?, T?, int, int, bool)
```csharp
public Point Draw(Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, Vector2 scale, float sort, Color color, T? strokeColor, T? shadowColor, int maxWidth, int visibleCharacters, bool debugBox)
```

**Parameters** \
`spriteBatch` [Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`alignment` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`scale` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](../..//Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`maxWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`visibleCharacters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`debugBox` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### DrawSimple(Batch2D, string, Vector2, Vector2, Vector2, float, Color, T?, T?, bool)
```csharp
public Point DrawSimple(Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, Vector2 scale, float sort, Color color, T? strokeColor, T? shadowColor, bool debugBox)
```

**Parameters** \
`spriteBatch` [Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`alignment` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`scale` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](../..//Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`debugBox` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### Escape(string)
```csharp
public string Escape(string text)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡