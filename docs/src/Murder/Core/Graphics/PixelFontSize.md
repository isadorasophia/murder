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
#### Outline
```csharp
public bool Outline;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Size
```csharp
public float Size;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
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

#### WidthToNextLine(string, int)
```csharp
public float WidthToNextLine(string text, int start)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`start` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Get(int)
```csharp
public PixelFontCharacter Get(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[PixelFontCharacter](/Murder/Core/Graphics/PixelFontCharacter.html) \

#### AutoNewline(string, int)
```csharp
public string AutoNewline(string text, int width)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Measure(char)
```csharp
public Vector2 Measure(char text)
```

**Parameters** \
`text` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### Measure(string)
```csharp
public Vector2 Measure(string text)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### Draw(string, Batch2D, Vector2, Vector2, Color, float)
```csharp
public void Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color, float sort)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`justify` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(string, Batch2D, Vector2, Vector2, float, int, float, Color, T?, T?, int)
```csharp
public void Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, float scale, int visibleCharacters, float sort, Color color, T? strokeColor, T? shadowColor, int maxWidth)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`justify` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`scale` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`visibleCharacters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`strokeColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`shadowColor` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`maxWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Draw(string, Batch2D, Vector2, Color, float)
```csharp
public void Draw(string text, Batch2D spriteBatch, Vector2 position, Color color, float sort)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawCharacter(char, Batch2D, Vector2, Vector2, Color, float)
```csharp
public void DrawCharacter(char character, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color, float sort)
```

**Parameters** \
`character` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`justify` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡