# MenuOption

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public sealed struct MenuOption
```

### ⭐ Constructors
```csharp
public MenuOption()
```

```csharp
public MenuOption(bool selectable)
```

**Parameters** \
`selectable` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

```csharp
public MenuOption(string text, bool selectable)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`selectable` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### Enabled
```csharp
public readonly bool Enabled;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Length
```csharp
public int Length { get; }
```

Length of the text option.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### SoundOnClick
```csharp
public bool SoundOnClick;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Text
```csharp
public readonly string Text;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \


⚡