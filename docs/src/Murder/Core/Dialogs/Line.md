# Line

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public sealed struct Line
```

### ⭐ Constructors
```csharp
public Line()
```

```csharp
public Line(Guid speaker, string text)
```

Create a line with a text. That won't be used as a timer.

**Parameters** \
`speaker` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

```csharp
public Line(string text)
```

Create a line with a text without any speaker.

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

```csharp
public Line(T? speaker, float delay)
```

**Parameters** \
`speaker` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`delay` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public Line(T? speaker, string portrait, string text, T? delay)
```

**Parameters** \
`speaker` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`portrait` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`delay` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

```csharp
public Line(T? speaker)
```

**Parameters** \
`speaker` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

### ⭐ Properties
#### Delay
```csharp
public readonly T? Delay;
```

Delay in seconds.

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### IsText
```csharp
public bool IsText { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Portrait
```csharp
public readonly string Portrait;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Speaker
```csharp
public readonly T? Speaker;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Text
```csharp
public readonly string Text;
```

If the caption has a text, this will be the information.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### WithDelay(float)
```csharp
public Line WithDelay(float delay)
```

**Parameters** \
`delay` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Line](../..//Murder/Core/Dialogs/Line.html) \

#### WithPortrait(string)
```csharp
public Line WithPortrait(string portrait)
```

**Parameters** \
`portrait` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Line](../..//Murder/Core/Dialogs/Line.html) \

#### WithSpeaker(Guid)
```csharp
public Line WithSpeaker(Guid speaker)
```

**Parameters** \
`speaker` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[Line](../..//Murder/Core/Dialogs/Line.html) \

#### WithText(string)
```csharp
public Line WithText(string text)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Line](../..//Murder/Core/Dialogs/Line.html) \



⚡