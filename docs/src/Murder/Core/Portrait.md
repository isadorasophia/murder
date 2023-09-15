# Portrait

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public sealed struct Portrait
```

### ⭐ Constructors
```csharp
public Portrait()
```

```csharp
public Portrait(Guid aseprite, string animationId)
```

**Parameters** \
`aseprite` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### AnimationId
```csharp
public readonly string AnimationId;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### HasImage
```csharp
public bool HasImage { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### HasValue
```csharp
public bool HasValue { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Sprite
```csharp
public readonly Guid Sprite;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
### ⭐ Methods
#### WithAnimationId(string)
```csharp
public Portrait WithAnimationId(string animationId)
```

**Parameters** \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Portrait](../../Murder/Core/Portrait.html) \



⚡