# IShaderProvider

**Namespace:** Murder \
**Assembly:** Murder.dll

```csharp
public abstract IShaderProvider
```

A game that leverages murder and use custom shaders should implement this in their [IMurderGame](../Murder/IMurderGame.html).

### ⭐ Properties
#### Shaders
```csharp
public abstract virtual String[] Shaders { get; }
```

Names of custom shaders that will be provided.
            This is expected to be placed in ./<game_directory />/../resources

**Returns** \
[string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \


⚡