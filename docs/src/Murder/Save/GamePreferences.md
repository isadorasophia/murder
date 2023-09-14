# GamePreferences

**Namespace:** Murder.Save \
**Assembly:** Murder.dll

```csharp
public class GamePreferences
```

Tracks preferences of the current session. This is unique per run.
            Used to track the game settings that are not tied to any game run (for example, volume).

### ⭐ Constructors
```csharp
public GamePreferences()
```

### ⭐ Properties
#### _bloom
```csharp
protected bool _bloom;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### _downscale
```csharp
protected bool _downscale;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### _musicVolume
```csharp
protected float _musicVolume;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### _soundVolume
```csharp
protected float _soundVolume;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Bloom
```csharp
public bool Bloom { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Downscale
```csharp
public bool Downscale { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### MusicVolume
```csharp
public float MusicVolume { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### SoundVolume
```csharp
public float SoundVolume { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### SaveSettings()
```csharp
protected void SaveSettings()
```

#### ToggleBloomAndSave()
```csharp
public bool ToggleBloomAndSave()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ToggleDownscaleAndSave()
```csharp
public bool ToggleDownscaleAndSave()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### SetMusicVolume(float)
```csharp
public float SetMusicVolume(float value)
```

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### SetSoundVolume(float)
```csharp
public float SetSoundVolume(float value)
```

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### ToggleMusicVolumeAndSave()
```csharp
public float ToggleMusicVolumeAndSave()
```

This toggles the volume to the opposite of the current setting.
            Immediately serialize (and save) afterwards.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### ToggleSoundVolumeAndSave()
```csharp
public float ToggleSoundVolumeAndSave()
```

This toggles the volume to the opposite of the current setting.
            Immediately serialize (and save) afterwards.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### OnPreferencesChangedImpl()
```csharp
public virtual void OnPreferencesChangedImpl()
```

#### OnPreferencesChanged()
```csharp
public void OnPreferencesChanged()
```



⚡