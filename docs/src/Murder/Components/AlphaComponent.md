# AlphaComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct AlphaComponent : IComponent
```

Set alpha of a component being displayed in the screen.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public AlphaComponent()
```

```csharp
public AlphaComponent(AlphaSources source, float amount)
```

**Parameters** \
`source` [AlphaSources](../../Murder/Components/AlphaSources.html) \
`amount` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public AlphaComponent(Single[] sources)
```

**Parameters** \
`sources` [float[]](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Alpha
```csharp
public float Alpha { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### Set(AlphaSources, float)
```csharp
public AlphaComponent Set(AlphaSources source, float amount)
```

**Parameters** \
`source` [AlphaSources](../../Murder/Components/AlphaSources.html) \
`amount` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[AlphaComponent](../../Murder/Components/AlphaComponent.html) \

#### Get(AlphaSources)
```csharp
public float Get(AlphaSources source)
```

**Parameters** \
`source` [AlphaSources](../../Murder/Components/AlphaSources.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡