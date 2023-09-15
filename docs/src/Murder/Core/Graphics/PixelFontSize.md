# PixelFontSize

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class PixelFontSize
```

### ⭐ Constructors
```csharp
public PixelFontSize()
```

### ⭐ Properties
#### BaseLine
```csharp
public float BaseLine;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Characters
```csharp
public Dictionary<TKey, TValue> Characters;
```

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### LineHeight
```csharp
public int LineHeight;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Textures
```csharp
public List<T> Textures;
```

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \
### ⭐ Methods
#### HeightOf(string)
```csharp
public float HeightOf(string text)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### WidthToNextLine(ReadOnlySpan<T>, int, bool)
```csharp
public float WidthToNextLine(ReadOnlySpan<T> text, int start, bool trimWhitespace)
```

**Parameters** \
`text` [ReadOnlySpan\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1?view=net-7.0) \
`start` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`trimWhitespace` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(string, Batch2D, Vector2, Vector2, Vector2, int, float, Color, T?, T?, int, bool)
```csharp
public Point Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Vector2 scale, int visibleCharacters, float sort, Color color, T? strokeColor, T? shadowColor, int maxWidth, bool debugBox)
```

Draw a text with pixel font. If <paramref name="maxWidth" /> is specified, this will automatically wrap the text.

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`spriteBatch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`justify` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`scale` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`visibleCharacters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](../../../Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`maxWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`debugBox` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \

#### DrawSimple(string, Batch2D, Vector2, Vector2, Vector2, float, Color, T?, T?, bool)
```csharp
public Point DrawSimple(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Vector2 scale, float sort, Color color, T? strokeColor, T? shadowColor, bool debugBox)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`spriteBatch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`justify` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`scale` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](../../../Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`debugBox` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \

#### AutoNewline(string, int)
```csharp
public string AutoNewline(string text, int width)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Measure(string)
```csharp
public Vector2 Measure(string text)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Vector2](../../../Murder/Core/Geometry/Vector2.html) \

#### Draw(string, Batch2D, Vector2, Vector2, Color, float)
```csharp
public void Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color, float sort)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`spriteBatch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`justify` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`color` [Color](../../../Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(string, Batch2D, Vector2, Color, float)
```csharp
public void Draw(string text, Batch2D spriteBatch, Vector2 position, Color color, float sort)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`spriteBatch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`color` [Color](../../../Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡