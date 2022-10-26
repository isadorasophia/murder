# SpriteAsset

**Namespace:** Murder.Assets.Graphics \
**Assembly:** Murder.dll

```csharp
public class SpriteAsset : GameAsset
```

**Implements:** _[GameAsset](/Murder/Assets/GameAsset.html)_

### ⭐ Constructors
```csharp
public SpriteAsset()
```

### ⭐ Properties
#### Animation
```csharp
public Animation Animation { get; }
```

**Returns** \
[Animation](/Murder/Core/Graphics/Animation.html) \
#### Atlas
```csharp
public AtlasId Atlas;
```

**Returns** \
[AtlasId](/Murder/Data/AtlasId.html) \
#### CanBeCreated
```csharp
public virtual bool CanBeCreated { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeDeleted
```csharp
public virtual bool CanBeDeleted { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeRenamed
```csharp
public virtual bool CanBeRenamed { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeSaved
```csharp
public virtual bool CanBeSaved { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CustomPath
```csharp
public virtual string CustomPath { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### EditorColor
```csharp
public virtual Vector4 EditorColor { get; }
```

**Returns** \
[Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \
#### EditorFolder
```csharp
public virtual string EditorFolder { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### FileChanged
```csharp
public bool FileChanged;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### FilePath
```csharp
public string FilePath { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### FirstFrame
```csharp
public string FirstFrame;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Frames
```csharp
public string Frames;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### FramesPerSecond
```csharp
public float FramesPerSecond;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Guid
```csharp
public Guid Guid { get; protected set; }
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### Icon
```csharp
public virtual char Icon { get; }
```

**Returns** \
[char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
#### Name
```csharp
public string Name { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### SaveLocation
```csharp
public virtual string SaveLocation { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### StoreInDatabase
```csharp
public virtual bool StoreInDatabase { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### TaggedForDeletion
```csharp
public bool TaggedForDeletion;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### Duplicate(string)
```csharp
public GameAsset Duplicate(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[GameAsset](/Murder/Assets/GameAsset.html) \

#### CreateFileList(IEnumerable<T>)
```csharp
public IEnumerable<T> CreateFileList(IEnumerable<T> frames)
```

**Parameters** \
`frames` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### CreateFrameList()
```csharp
public IEnumerable<T> CreateFrameList()
```

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### GetSimplifiedName()
```csharp
public string GetSimplifiedName()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Draw(Batch2D, Rectangle, Color, float)
```csharp
public void Draw(Batch2D spriteBatch, Rectangle destinationBox, Color _, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`destinationBox` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`_` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw(Batch2D, Vector2, float)
```csharp
public void Draw(Batch2D spriteBatch, Vector2 position, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### MakeGuid()
```csharp
public void MakeGuid()
```

#### RecalculateAnimation()
```csharp
public void RecalculateAnimation()
```



⚡