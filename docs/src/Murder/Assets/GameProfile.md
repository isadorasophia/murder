# GameProfile

**Namespace:** Murder.Assets \
**Assembly:** Murder.dll

```csharp
public class GameProfile : GameAsset
```

**Implements:** _[GameAsset](/Murder/Assets/GameAsset.html)_

### ⭐ Constructors
```csharp
public GameProfile()
```

### ⭐ Properties
#### AtlasFolderName
```csharp
public string AtlasFolderName;
```

Where our atlas .png and .json files are stored

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
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
#### ContentAsepritePath
```csharp
public string ContentAsepritePath;
```

Where our ecs assets are stored

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ContentDataPath
```csharp
public string ContentDataPath;
```

Where our generic assets are stored

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ContentECSPath
```csharp
public string ContentECSPath;
```

Where our ecs assets are stored

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Cursors
```csharp
public Cursors Cursors;
```

**Returns** \
[Cursors](/Murder/Assets/Cursors.html) \
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
#### Exploration
```csharp
public Exploration Exploration;
```

**Returns** \
[Exploration](/Murder/Assets/Exploration.html) \
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
#### FixedUpdateFactor
```csharp
public float FixedUpdateFactor;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Fullscreen
```csharp
public bool Fullscreen;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### GameAssetsContentPath
```csharp
public string GameAssetsContentPath;
```

Where our data .json files are stored

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### GameHeight
```csharp
public int GameHeight;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### GameScale
```csharp
public int GameScale;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### GameWidth
```csharp
public int GameWidth;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
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
#### IsVSyncEnabled
```csharp
public bool IsVSyncEnabled;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Name
```csharp
public string Name { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### PushAwayInterval
```csharp
public float PushAwayInterval;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### SaveLocation
```csharp
public virtual string SaveLocation { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ShowUiDebug
```csharp
public bool ShowUiDebug;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### StartingScene
```csharp
public Guid StartingScene;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
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
#### TargetFps
```csharp
public int TargetFps;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Theme
```csharp
public Theme Theme;
```

**Returns** \
[Theme](/Murder/Assets/Theme.html) \
### ⭐ Methods
#### Duplicate(string)
```csharp
public GameAsset Duplicate(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[GameAsset](/Murder/Assets/GameAsset.html) \

#### GetSimplifiedName()
```csharp
public string GetSimplifiedName()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### MakeGuid()
```csharp
public void MakeGuid()
```



⚡