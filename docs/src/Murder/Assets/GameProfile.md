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
#### AssetResourcesPath
```csharp
public readonly string AssetResourcesPath;
```

Root path where our data .json files are stored.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### AtlasFolderName
```csharp
public readonly string AtlasFolderName;
```

Where our atlas .png and .json files are stored.
            Under: 
              packed/ -&gt; bin/resources/
                atlas/

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### BackColor
```csharp
public Color BackColor;
```

**Returns** \
[Color](/Murder/Core/Graphics/Color.html) \
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
public readonly string ContentAsepritePath;
```

Where our aseprite contents are stored.
            Under:
              packed/ -&gt; bin/resources/
                aseprite/

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ContentECSPath
```csharp
public readonly string ContentECSPath;
```

Where our ecs assets are stored.
            Under:
              resources/
                assets/
                  ecs/

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### EditorAssets
```csharp
public readonly EditorAssets EditorAssets;
```

**Returns** \
[EditorAssets](/Murder/Assets/EditorAssets.html) \
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
public readonly Exploration Exploration;
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
public readonly float FixedUpdateFactor;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### FontPath
```csharp
public readonly string FontPath;
```

Where our font contents are stored.
            Under:
              packed/ -&gt; bin/resources/
                fonts/

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Fullscreen
```csharp
public bool Fullscreen;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### GameHeight
```csharp
public readonly int GameHeight;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### GameScale
```csharp
public readonly int GameScale;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### GameWidth
```csharp
public readonly int GameWidth;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### GenericAssetsPath
```csharp
public readonly string GenericAssetsPath;
```

Where our generic assets are stored.
            Under:
              resources/
                assets/
                  data/

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Guid
```csharp
public Guid Guid { get; protected set; }
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### HiResPath
```csharp
public readonly string HiResPath;
```

Where our high resolution contents are stored.
            Under:
              packed/ -&gt; bin/resources
                shaders/

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Icon
```csharp
public virtual char Icon { get; }
```

**Returns** \
[char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
#### IsStoredInSaveData
```csharp
public virtual bool IsStoredInSaveData { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsVSyncEnabled
```csharp
public readonly bool IsVSyncEnabled;
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
public readonly float PushAwayInterval;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Rename
```csharp
public bool Rename { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### RenderDownscale
```csharp
public readonly int RenderDownscale;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### SaveLocation
```csharp
public virtual string SaveLocation { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ShadersPath
```csharp
public readonly string ShadersPath;
```

Where our aseprite contents are stored.
            Under:
              packed/ -&gt; bin/resources/
                shaders/

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ShowUiDebug
```csharp
public readonly bool ShowUiDebug;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### SoundsPath
```csharp
public readonly string SoundsPath;
```

Where our aseprite contents are stored.
            Under:
              packed/ -&gt; bin/resources/
                sounds/

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### StartingScene
```csharp
public readonly Guid StartingScene;
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
public readonly int TargetFps;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Theme
```csharp
public readonly Theme Theme;
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