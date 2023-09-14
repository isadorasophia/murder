# TextureAtlas

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class TextureAtlas : IDisposable
```

A texture atlas, the texture2D can be loaded and unloaded from the GPU at any time
            We will keep the texture lists in memory all the time, though.

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public TextureAtlas(string name, AtlasId id)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`id` [AtlasId](../../../Murder/Data/AtlasId.html) \

### ⭐ Properties
#### _entries
```csharp
public Dictionary<TKey, TValue> _entries;
```

Used publically only for the json serializer

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### CountEntries
```csharp
public int CountEntries { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Id
```csharp
public readonly AtlasId Id;
```

**Returns** \
[AtlasId](../../../Murder/Data/AtlasId.html) \
#### Name
```csharp
public readonly string Name;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### Get(string)
```csharp
public AtlasCoordinates Get(string id)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[AtlasCoordinates](../../../Murder/Core/Graphics/AtlasCoordinates.html) \

#### Exist(string)
```csharp
public bool Exist(string id)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasId(string)
```csharp
public bool HasId(string id)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryCreateTexture(string, out Texture2D&)
```csharp
public bool TryCreateTexture(string id, Texture2D& texture)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`texture` [Texture2D&](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGet(string, out AtlasCoordinates&)
```csharp
public bool TryGet(string id, AtlasCoordinates& coord)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`coord` [AtlasCoordinates&](../../../Murder/Core/Graphics/AtlasCoordinates.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetAllEntries()
```csharp
public IEnumerable<T> GetAllEntries()
```

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### CreateTextureFromAtlas(AtlasCoordinates, SurfaceFormat, int)
```csharp
public Texture2D CreateTextureFromAtlas(AtlasCoordinates textureCoord, SurfaceFormat format, int scale)
```

This creates a new texture on the fly and should be *AVOIDED!*. Use `Get` instead.

**Parameters** \
`textureCoord` [AtlasCoordinates](../../../Murder/Core/Graphics/AtlasCoordinates.html) \
\
`format` [SurfaceFormat](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SurfaceFormat.html) \
\
`scale` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

#### CreateTextureFromAtlas(string)
```csharp
public Texture2D CreateTextureFromAtlas(string id)
```

This creates a new texture on the fly and should be *AVOIDED!*. Use `Get` instead.

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

#### Dispose()
```csharp
public virtual void Dispose()
```

#### LoadTextures()
```csharp
public void LoadTextures()
```

#### PopulateAtlas(IEnumerable<T>)
```csharp
public void PopulateAtlas(IEnumerable<T> entries)
```

**Parameters** \
`entries` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### UnloadTextures()
```csharp
public void UnloadTextures()
```



⚡