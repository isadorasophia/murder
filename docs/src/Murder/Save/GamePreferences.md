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