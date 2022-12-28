# GameDataManager

**Namespace:** Murder.Data \
**Assembly:** Murder.dll

```csharp
public class GameDataManager : IDisposable
```

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public GameDataManager(IMurderGame game)
```

Creates a new game data manager.

**Parameters** \
`game` [IMurderGame](/Murder/IMurderGame.html) \
\

### ⭐ Properties
#### _allAssets
```csharp
protected readonly Dictionary<TKey, TValue> _allAssets;
```

Maps:
            [Guid] -&gt; [Asset]

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### _allSavedData
```csharp
protected readonly Dictionary<TKey, TValue> _allSavedData;
```

This is the collection of save data.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### _assetsBinDirectoryPath
```csharp
protected string _assetsBinDirectoryPath;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### _binResourcesDirectory
```csharp
protected string _binResourcesDirectory;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### _database
```csharp
protected readonly Dictionary<TKey, TValue> _database;
```

Maps:
            [Game asset type] -&gt; [Guid]

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### _gameProfile
```csharp
protected GameProfile _gameProfile;
```

**Returns** \
[GameProfile](/Murder/Assets/GameProfile.html) \
#### ActiveSaveData
```csharp
public SaveData ActiveSaveData { get; }
```

Active saved run in the game.

**Returns** \
[SaveData](/Murder/Assets/SaveData.html) \
#### AssetsBinDirectoryPath
```csharp
public string AssetsBinDirectoryPath { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### AvailableUniqueTextures
```csharp
public ImmutableArray<T> AvailableUniqueTextures;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### CachedUniqueTextures
```csharp
public readonly CacheDictionary<TKey, TValue> CachedUniqueTextures;
```

**Returns** \
[CacheDictionary\<TKey, TValue\>](/Murder/Utilities/CacheDictionary-2.html) \
#### CustomGameShader
```csharp
public Effect CustomGameShader;
```

Custom optional game shader, provided by [GameDataManager._game](/murder/data/gamedatamanager.html#_game).

**Returns** \
[Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
#### DitherTexture
```csharp
public Texture2D DitherTexture;
```

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
#### GameDirectory
```csharp
public virtual string GameDirectory { get; }
```

Directory used for saving custom data.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### GameProfile
```csharp
public GameProfile GameProfile { get; protected set; }
```

**Returns** \
[GameProfile](/Murder/Assets/GameProfile.html) \
#### GameProfileFileName
```csharp
public static const string GameProfileFileName;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### LargeFont
```csharp
public PixelFont LargeFont;
```

A larger, 12 pixel tall font TODO: Is this font broken??

**Returns** \
[PixelFont](/Murder/Core/Graphics/PixelFont.html) \
#### LoadedAtlasses
```csharp
public readonly Dictionary<TKey, TValue> LoadedAtlasses;
```

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### OtherEffects
```csharp
public virtual Effect[] OtherEffects { get; }
```

**Returns** \
[Effect[]](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
#### PackedBinDirectoryPath
```csharp
public string PackedBinDirectoryPath { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### PixelFont
```csharp
public PixelFont PixelFont;
```

A small, 7 pixel tall font

**Returns** \
[PixelFont](/Murder/Core/Graphics/PixelFont.html) \
#### Preferences
```csharp
public GamePreferences Preferences { get; }
```

**Returns** \
[GamePreferences](/Murder/Save/GamePreferences.html) \
#### SaveBasePath
```csharp
public static string SaveBasePath { get; }
```

Save directory path used when serializing user data.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ShaderBricks
```csharp
public Effect ShaderBricks;
```

A shader for applying colorgrades, "brick" mosaics and enforcing a palette.

**Returns** \
[Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
#### ShaderColorgrade
```csharp
public Effect ShaderColorgrade;
```

A shader for applying colorgrades.

**Returns** \
[Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
#### ShaderRelativePath
```csharp
protected readonly string ShaderRelativePath;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ShaderSimple
```csharp
public Effect ShaderSimple;
```

The cheapest and simplest shader. It also has a desaturation/saturation mode.

**Returns** \
[Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
#### ShaderSprite
```csharp
public Effect ShaderSprite;
```

Actually a fancy shader, has some sprite effect tools for us, like different color blending modes.

**Returns** \
[Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
#### Sounds
```csharp
public ImmutableArray<T> Sounds { get; private set; }
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### TestTexture
```csharp
public Texture2D TestTexture;
```

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
### ⭐ Methods
#### FetchAssetsAtPath(string, bool, bool, bool)
```csharp
protected IEnumerable<T> FetchAssetsAtPath(string fullPath, bool recursive, bool skipFailures, bool stopOnFailure)
```

**Parameters** \
`fullPath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`recursive` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`skipFailures` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`stopOnFailure` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### TryCompileShader(string, out Effect&)
```csharp
protected virtual bool TryCompileShader(string name, Effect& result)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`result` [Effect&](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CreateGameProfile()
```csharp
protected virtual GameProfile CreateGameProfile()
```

**Returns** \
[GameProfile](/Murder/Assets/GameProfile.html) \

#### CreateSaveData(string)
```csharp
protected virtual SaveData CreateSaveData(string name)
```

Creates an implementation of SaveData for the game.

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[SaveData](/Murder/Assets/SaveData.html) \

#### RemoveAsset(Type, Guid)
```csharp
protected virtual void RemoveAsset(Type t, Guid assetGuid)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
`assetGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

#### AddAssetForCurrentSave(GameAsset)
```csharp
public bool AddAssetForCurrentSave(GameAsset asset)
```

**Parameters** \
`asset` [GameAsset](/Murder/Assets/GameAsset.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CanLoadSaveData(T?)
```csharp
public bool CanLoadSaveData(T? guid)
```

**Parameters** \
`guid` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasAsset(Guid)
```csharp
public bool HasAsset(Guid id)
```

**Parameters** \
`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### LoadAllSaves()
```csharp
public bool LoadAllSaves()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### LoadSaveAsCurrentSave(T?)
```csharp
public bool LoadSaveAsCurrentSave(T? guid)
```

**Parameters** \
`guid` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### LoadSaveAtPath(string)
```csharp
public bool LoadSaveAtPath(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### LoadShader(string, Effect&, bool)
```csharp
public bool LoadShader(string name, Effect& effect, bool breakOnFail)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`effect` [Effect&](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
`breakOnFail` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveAssetForCurrentSave(Guid)
```csharp
public bool RemoveAssetForCurrentSave(Guid guid)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### SerializeSave()
```csharp
public bool SerializeSave()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGetDynamicAsset(out T&)
```csharp
public bool TryGetDynamicAsset(T& asset)
```

**Parameters** \
`asset` [T&]() \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetAsset(Guid)
```csharp
public GameAsset GetAsset(Guid id)
```

**Parameters** \
`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[GameAsset](/Murder/Assets/GameAsset.html) \

#### TryGetAsset(Guid)
```csharp
public GameAsset TryGetAsset(Guid id)
```

Get a generic asset with a <paramref name="id" />.

**Parameters** \
`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[GameAsset](/Murder/Assets/GameAsset.html) \

#### TryGetAssetForCurrentSave(Guid)
```csharp
public GameAsset TryGetAssetForCurrentSave(Guid guid)
```

Retrieve a dynamic asset within the current save data based on a guid.

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[GameAsset](/Murder/Assets/GameAsset.html) \

#### GetAllSaves()
```csharp
public IEnumerable<T> GetAllSaves()
```

List all the available saves within the game.

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### GetAllAssets()
```csharp
public ImmutableArray<T> GetAllAssets()
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### FilterAllAssets(Type[])
```csharp
public ImmutableDictionary<TKey, TValue> FilterAllAssets(Type[] types)
```

**Parameters** \
`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

#### FilterAllAssetsWithImplementation(Type[])
```csharp
public ImmutableDictionary<TKey, TValue> FilterAllAssetsWithImplementation(Type[] types)
```

Filter all the assets and any types that implement those types.
            Cautious: this may be slow or just imply extra allocations.

**Parameters** \
`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

#### FilterOutAssets(Type[])
```csharp
public ImmutableDictionary<TKey, TValue> FilterOutAssets(Type[] types)
```

Return all the assets except the ones in <paramref name="types" />.

**Parameters** \
`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

#### FindAllNamesForAsset(Type)
```csharp
public ImmutableHashSet<T> FindAllNamesForAsset(Type t)
```

Find all the assets names for an asset type <paramref name="t" />.

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
\

**Returns** \
[ImmutableHashSet\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableHashSet-1?view=net-7.0) \

#### GetPrefab(Guid)
```csharp
public PrefabAsset GetPrefab(Guid id)
```

**Parameters** \
`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[PrefabAsset](/Murder/Assets/PrefabAsset.html) \

#### ResetActiveSave()
```csharp
public SaveData ResetActiveSave()
```

This resets the active save data.

**Returns** \
[SaveData](/Murder/Assets/SaveData.html) \

#### TryGetActiveSaveData()
```csharp
public SaveData TryGetActiveSaveData()
```

Active saved run in the game.

**Returns** \
[SaveData](/Murder/Assets/SaveData.html) \

#### Sound(string)
```csharp
public SoundEffect Sound(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[SoundEffect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Audio.SoundEffect.html) \

#### GetAsset(Guid)
```csharp
public T GetAsset(Guid id)
```

**Parameters** \
`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[T]() \

#### GetDynamicAsset()
```csharp
public T GetDynamicAsset()
```

Retrieve a dynamic asset within the current save data.
            If no dynamic asset is found, it creates a new one to the save data.

**Returns** \
[T]() \

#### TryGetAsset(Guid)
```csharp
public T TryGetAsset(Guid id)
```

**Parameters** \
`id` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[T]() \

#### FetchTexture(string)
```csharp
public Texture2D FetchTexture(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

#### FetchAtlas(AtlasId)
```csharp
public TextureAtlas FetchAtlas(AtlasId atlas)
```

**Parameters** \
`atlas` [AtlasId](/Murder/Data/AtlasId.html) \

**Returns** \
[TextureAtlas](/Murder/Core/Graphics/TextureAtlas.html) \

#### TryFetchAtlas(AtlasId)
```csharp
public TextureAtlas TryFetchAtlas(AtlasId atlas)
```

**Parameters** \
`atlas` [AtlasId](/Murder/Data/AtlasId.html) \

**Returns** \
[TextureAtlas](/Murder/Core/Graphics/TextureAtlas.html) \

#### LoadSounds()
```csharp
public ValueTask LoadSounds()
```

This will load all the sounds to the game.

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### FetchSong(string)
```csharp
public ValueTask<TResult> FetchSong(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[ValueTask\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask-1?view=net-7.0) \

#### FetchSound(string)
```csharp
public ValueTask<TResult> FetchSound(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[ValueTask\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask-1?view=net-7.0) \

#### CreateSave(string)
```csharp
public virtual SaveData CreateSave(string name)
```

Create a new save data based on a name.

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[SaveData](/Murder/Assets/SaveData.html) \

#### DeleteAllSaves()
```csharp
public virtual void DeleteAllSaves()
```

#### Dispose()
```csharp
public virtual void Dispose()
```

#### Init(string)
```csharp
public virtual void Init(string resourcesBinPath)
```

**Parameters** \
`resourcesBinPath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### InitShaders()
```csharp
public virtual void InitShaders()
```

#### LoadContent()
```csharp
public virtual void LoadContent()
```

#### RefreshAtlas()
```csharp
public virtual void RefreshAtlas()
```

#### AddAsset(T)
```csharp
public void AddAsset(T asset)
```

**Parameters** \
`asset` [T]() \

#### DisposeAtlases()
```csharp
public void DisposeAtlases()
```

#### InitializeAssets()
```csharp
public void InitializeAssets()
```

#### LoadShaders(bool)
```csharp
public void LoadShaders(bool breakOnFail)
```

Override this to load all shaders present in the game.

**Parameters** \
`breakOnFail` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### QuickSave()
```csharp
public void QuickSave()
```

Quickly serialize our save assets.

#### RemoveAsset(Guid)
```csharp
public void RemoveAsset(Guid assetGuid)
```

**Parameters** \
`assetGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

#### RemoveAsset(T)
```csharp
public void RemoveAsset(T asset)
```

**Parameters** \
`asset` [T]() \

#### SaveWorld(MonoWorld)
```csharp
public void SaveWorld(MonoWorld world)
```

**Parameters** \
`world` [MonoWorld](/Murder/Core/MonoWorld.html) \

#### UnloadAllSaves()
```csharp
public void UnloadAllSaves()
```

Used to clear all saves files currently active.



⚡